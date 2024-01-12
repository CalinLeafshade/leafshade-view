using System;
using Godot;

public partial class Weather : Node2D
{

	GpuParticles2D snow;
	GpuParticles2D rain;
	[Export]
	public Lighting lighting { get; set; }
	Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		snow = GetNode<GpuParticles2D>("%Snow");
		rain = GetNode<GpuParticles2D>("%Rain");
		timer = GetNode<Timer>("%Timer");
		timer.Timeout += () =>
		{
			StopAll();
			lighting.Unmoody();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StopAll()
	{
		snow.Emitting = false;
		rain.Emitting = false;
	}

	public void Snow()
	{
		StopAll();
		snow.Emitting = true;
		lighting.Moody();
		timer.Start();
	}

	public void Rain()
	{
		StopAll();
		rain.Emitting = true;
		lighting.Moody();
		timer.Start();
	}

	public void Start(string type)
	{
		if (type == "snow")
		{
			Snow();
		}
		else if (type == "rain")
		{
			Rain();
		}
	}
}
