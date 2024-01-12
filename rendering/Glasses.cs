using Godot;
using Leafshade;
using System;

public partial class Glasses : AttachmentItem
{

	Sprite2D partial;
	Sprite2D full;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		partial = GetNode<Sprite2D>("PartialGlasses");
		full = GetNode<Sprite2D>("FullGlasses");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		bool isFull = Avatar.CurrentSprite == "neon";

		var d = isFull ? -delta : delta;

		partial.Modulate = new Color(1f,1f,1f, Mathf.Clamp(partial.Modulate.A + (float)d, 0f, 1f));
		full.Modulate = new Color(1f,1f,1f, Mathf.Clamp(full.Modulate.A - (float)d, 0f, 1f));

		
	}
}
