using System;
using Godot;

public partial class Poofer : Node2D
{

    private GpuParticles2D particles;
    private Timer endTimer;

    [Export]
    public string SetName { get; set; }

    [Export]
    public Godot.Collections.Dictionary<string, Gradient> Colors { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        particles = GetNode<GpuParticles2D>("%Particles");
        endTimer = GetNode<Timer>("%EndTimer");
        endTimer.Timeout += () => QueueFree();
        Poof(SetName);
    }

    public void Poof(string setName)
    {
        GD.Print("Poof");
        var mat = particles.ProcessMaterial as ParticleProcessMaterial;

        if (Colors.ContainsKey(setName))
        {
            var ramp = mat.ColorRamp as GradientTexture1D;
            ramp.Gradient = Colors[setName];
        }
        particles.Emitting = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
