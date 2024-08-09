using Godot;
using Leafshade;
using System;

public partial class EyeGlow : Sprite2D
{

	[Export]
	public SpriteSet SpriteSet {get; set;}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var isBlinking = SpriteSet?.State == SpriteSet.SpriteState.Blink || SpriteSet?.State == SpriteSet.SpriteState.TalkBlink;
		Visible = !isBlinking;

		var a = Mathf.Sin(Time.GetTicksMsec() * 0.002f) * 0.5f + 1f;
		Modulate = new Color(1f,1f,1f,a);
	}
}
