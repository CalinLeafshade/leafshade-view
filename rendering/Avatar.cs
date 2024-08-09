using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Leafshade.SpriteSet;

namespace Leafshade;

public partial class Avatar : Node2D
{

	[Export]
	public AttachmentManager Attachments { get; set; }

	private Dictionary<string, DateTime> attachmentActivations = new Dictionary<string, DateTime>();

	private bool isTalking;
	private bool isBlinking;
	private RandomNumberGenerator rng = new();

	private readonly Spring bounceSpring = new();
	private ulong lastBounce;
	private ulong lastSpoke;
	private ulong lastBlink;
	private double blinkTimer;

	private Timer nudeTimer;

	[Signal]
	public delegate void CostumeChangedEventHandler(string newCostume);

	[Export]
	public float BounceStrength { get; set; } = 1f;

	[Export]
	public int BounceCooldown { get; set; }


	public ulong TimeSinceSpeaking => Time.GetTicksMsec() - lastSpoke;

	public ulong TimeSinceLastBounce => Time.GetTicksMsec() - lastBounce;

	public ulong TimeSinceBlinking => Time.GetTicksMsec() - lastBlink;

	public SpriteState CurrentState => IsTalking && isBlinking ? SpriteState.TalkBlink :
		IsTalking ? SpriteState.Talk :
		isBlinking ? SpriteState.Blink :
		SpriteState.Idle;

	private SetSwitcher Sprite;

	private string oldSet;

	public string CurrentSprite => Sprite.ActiveSet;

	public bool IsTalking
	{
		get => isTalking; set
		{
			GD.Print(value);
			isTalking = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nudeTimer = new Timer
		{
			WaitTime = 15
		};
		AddChild(nudeTimer);
		nudeTimer.Timeout += () =>
		{
			GD.Print(oldSet);
			GD.Print(Sprite.ActiveSet);
			if (Sprite.ActiveSet == "nude")
			{
				SetCostume(oldSet);
			}
		};
		Sprite = GetNode<SetSwitcher>("%SetSwitcher");
	}

	public IEnumerable<string> GetAvailableCostumes()
	{
		return Sprite.GetSets().Select(x => x.SetName);
	}

	public IEnumerable<string> GetAvailableAttachments()
	{
		return Sprite.GetSets().SelectMany(x => x.GetAvailableAttachments()).Distinct();
	}

	private void UpdateAttachments()
	{
		var set = Sprite.GetActiveSet();
		foreach (var kvp in attachmentActivations.OrderBy(x => x.Value))
		{
			var dif = (DateTime.Now - kvp.Value).TotalSeconds;

			if (dif > 120)
			{
				set.DisableAttachment(kvp.Key);
			}
			else
			{
				set.EnableAttachment(kvp.Key);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bounceSpring.Update(delta);
		float time = Time.GetTicksMsec() / 1000f;

		float sineAmplitude = 20f;

		Vector2 sineOffset = new(Mathf.Sin(time * 0.3f) * 10f, (Mathf.Sin(time * 0.3f) * sineAmplitude) + sineAmplitude);
		Vector2 bounceOffset = new(0, bounceSpring.Value);
		Vector2 bounceMargin = new(0, 30f);

		Position = sineOffset + bounceOffset + bounceMargin;
		float stretch = bounceSpring.Value * -0.002f;
		Scale = new(1 - stretch, 1 + stretch);

		SpriteSet set = Sprite.GetActiveSet();

		if (set != null)
		{
			set.State = CurrentState;
		}

		if (TimeSinceBlinking > 100)
		{
			isBlinking = false;
		}

		blinkTimer -= delta;
		if (blinkTimer <= 0)
		{
			Blink();
		}

		Modulate = new Color(1f, 1f, 1f, set.MaximumOpacity);

		UpdateAttachments();

	}

	public void EnableAttachment(string name)
	{

		attachmentActivations[name] = DateTime.Now;

	}

	public void SetCostume(string name)
	{
		if (name == Sprite.ActiveSet)
		{
			return;
		}

		oldSet = Sprite.ActiveSet;
		Sprite.ActiveSet = name;

		if (name == "nude")
		{
			nudeTimer.Start();
		}

		EmitSignal(SignalName.CostumeChanged, name);

	}

	public void Blink()
	{
		blinkTimer = rng.RandfRange(3f, 8f);
		lastBlink = Time.GetTicksMsec();
		isBlinking = true;
	}

	public void Bounce(float strength)
	{
		if (TimeSinceSpeaking > (ulong)BounceCooldown)
		{
			bounceSpring.Velocity = -strength * BounceStrength;
			lastBounce = Time.GetTicksMsec();
		}
	}

	public void StartTalking(float strength)
	{
		GD.Print("Talk");
		IsTalking = true;
		Bounce(strength);
		lastSpoke = Time.GetTicksMsec();
	}

	public void StopTalking()
	{
		IsTalking = false;
	}
}
