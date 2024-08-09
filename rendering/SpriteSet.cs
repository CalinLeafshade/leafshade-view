using System;
using System.Collections.Generic;
using Godot;

namespace Leafshade;

public partial class SpriteSet : Node2D
{

	public enum SpriteState
	{
		Idle,
		Talk,
		Blink,
		TalkBlink
	}

	[Signal]
	public delegate void SpriteStateChangedEventHandler(SpriteState newState);

	[Export]
	public string SetName { get; set; }

	[Export]
	public AttachmentManager AttachmentManager {get; set;}

	[Export]
	public float MaximumOpacity {get; set;} = 1f;

	[Export]
	public Texture2D Idle { get; set; }
	[Export]
	public Texture2D Talk { get; set; }
	[Export]
	public Texture2D Blink { get; set; }
	[Export]
	public Texture2D TalkBlink { get; set; }

	private SpriteState state;

	[Export]
	public SpriteState State { 
		get {
			return state;
		}
		set {
			var old = state;
			state = value;

			if (value != old) {
				EmitSignal(SignalName.SpriteStateChanged, (int)value);
			}

		}
	}

	[Export]
	public float TransitionAmount { get; set; }

	[Export]
	public Sprite2D Sprite { get; set; }

	[Export]
	public Polygon2D Polygon {get; set;}

	

	private Texture2D SpriteForState(SpriteState state)
	{
		return state switch
		{
			SpriteState.Idle => Idle,
			SpriteState.Talk => Talk,
			SpriteState.Blink => Blink,
			SpriteState.TalkBlink => TalkBlink,
			_ => throw new ArgumentOutOfRangeException(nameof(state))
		};
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try {
			Sprite = GetNode<Sprite2D>("%Sprite");
		}
		catch {}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Sprite != null) {
			Sprite.Texture = SpriteForState(State);
		}
		
		if (Polygon != null) {
			Polygon.Texture = SpriteForState(State);
		}

		Modulate = new Color(1, 1, 1, TransitionAmount);
	}

	public void EnableAttachment(string name) {
		AttachmentManager?.EnableAttachment(name);
	}

	public void DisableAttachment(string name) {
		AttachmentManager?.DisableAttachment(name);
	}

	internal IEnumerable<string> GetAvailableAttachments()
	{
		return AttachmentManager?.GetAllAttachmentNames() ?? Array.Empty<string>();
	}

}
