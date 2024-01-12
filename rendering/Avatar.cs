using System;
using Godot;
using static Leafshade.SpriteSet;

namespace Leafshade;

public partial class Avatar : Node2D
{

	[Export]
	public AttachmentManager Attachments { get; set; }

	private bool isTalking;
	private bool isBlinking;
	private RandomNumberGenerator rng = new();

	private readonly Spring bounceSpring = new();
	private ulong lastBounce;
	private ulong lastSpoke;
	private ulong lastBlink;
	private double blinkTimer;

	[Signal]
	public delegate void CostumeChangedEventHandler(string newCostume);

	[Export]
	public float BounceStrength { get; set; } = 1f;

	[Export]
	public int BounceCooldown { get; set; }


	public ulong TimeSinceSpeaking => Time.GetTicksMsec() - lastSpoke;

	public ulong TimeSinceLastBounce => Time.GetTicksMsec() - lastBounce;

	public ulong TimeSinceBlinking => Time.GetTicksMsec() - lastBlink;

	public SpriteState CurrentState => isTalking && isBlinking ? SpriteState.TalkBlink :
		isTalking ? SpriteState.Talk :
		isBlinking ? SpriteState.Blink :
		SpriteState.Idle;

	private SetSwitcher Sprite;

	public string CurrentSprite => Sprite.ActiveSet;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<SetSwitcher>("%SetSwitcher");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bounceSpring.Update(delta);
		float time = Time.GetTicksMsec() / 1000f;

		float sineAmplitude = 20f;

		Vector2 sineOffset = new(Mathf.Sin(time * 0.2f) * 10f, (Mathf.Sin(time * 0.3f) * sineAmplitude) + sineAmplitude);
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

	}

	public void EnableAttachment(string name)
	{
		Attachments.EnableAttachment(name);
	}

	public void SetCostume(string name)
	{
		string oldSet = Sprite.ActiveSet;
		Sprite.ActiveSet = name;

		if (oldSet != name)
		{
			EmitSignal(SignalName.CostumeChanged, name);
		}
	}

	public void Blink()
	{
		blinkTimer = rng.RandfRange(1f, 4f);
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
		isTalking = true;
		Bounce(strength);
		lastSpoke = Time.GetTicksMsec();
	}

	public void StopTalking()
	{
		isTalking = false;
	}
}
