using System;
using Godot;

namespace Leafshade;

public partial class MainScene : Node2D
{

	private Avatar avatar;
	private TransitionManager poofer;
	private Weather weather;

	private Lighting lighting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Root.TransparentBg = true;
		GetTree().Root.Transparent = true;
		avatar = GetNode<Avatar>("%Avatar");
		poofer = GetNode<TransitionManager>("%TransitionManager");
		weather = GetNode<Weather>("%Weather");
		lighting = GetNode<Lighting>("Lighting");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var bc = new Vector2(GetWindow().Size.X / 2, GetWindow().Size.Y);
		Position = bc;

	}

	public override void _Input(InputEvent @event)
	{

		if (@event is InputEventKey key)
		{
			if (key.Keycode == Key.Key1)
			{
				avatar.SetCostume("leafshade");
			}
			else if (key.Keycode == Key.Key2)
			{
				avatar.SetCostume("vampire");
			}
			else if (key.Keycode == Key.Key3)
			{
				avatar.SetCostume("neon");
			}
			else if (key.Keycode == Key.Key4)
			{
				avatar.SetCostume("catboy");
			}
			else if (key.Keycode == Key.Key5)
			{
				avatar.SetCostume("cowboy");
			}
			else if (key.Keycode == Key.Key0)
			{
				avatar.EnableAttachment("glasses");
			}
			else if (key.Keycode == Key.Key9)
			{
				avatar.EnableAttachment("partyhat");
			}
			else if (key.Keycode == Key.Key8)
			{
				lighting.Rave();
			}

		}
	}




}
