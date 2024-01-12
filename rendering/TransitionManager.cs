using System;
using Godot;

public partial class TransitionManager : Node2D
{

    [Export]
    public PackedScene PooferScene { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnTransition(string setName)
    {
        var poofer = PooferScene.Instantiate<Poofer>();
        poofer.SetName = setName;
        AddChild(poofer);
    }
}
