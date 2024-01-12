using System;
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

	[Export]
	public string SetName { get; set; }

	[Export]
	public Texture2D Idle { get; set; }
	[Export]
	public Texture2D Talk { get; set; }
	[Export]
	public Texture2D Blink { get; set; }
	[Export]
	public Texture2D TalkBlink { get; set; }

	[Export]
	public SpriteState State { get; set; }

	[Export]
	public float TransitionAmount { get; set; }

	private Sprite2D Sprite { get; set; }

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
		Sprite = GetNode<Sprite2D>("%Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Sprite.Texture = SpriteForState(State);
		Sprite.Modulate = new Color(1, 1, 1, TransitionAmount);
	}
}
