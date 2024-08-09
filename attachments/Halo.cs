using Godot;
using Leafshade;
using System;

public partial class Halo : AttachmentItem
{

	private Sprite2D sprite;
	private Vector2 initialPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<Sprite2D>("Sprite2D");
		initialPosition = sprite.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float y = Mathf.Sin(Time.GetTicksMsec() * 0.0008f) * 12f;
		sprite.Position = initialPosition + new Vector2(0, y);
		base._Process(delta);
	}
}
