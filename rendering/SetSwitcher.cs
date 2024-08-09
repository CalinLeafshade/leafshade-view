using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Leafshade;

public partial class SetSwitcher : Node2D
{

    [Export]
    public string ActiveSet { get; set; }

    public SpriteSet GetActiveSet()
    {
        return GetSetByName(ActiveSet);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        foreach (SpriteSet set in GetSets())
        {
            set.TransitionAmount = set.SetName == ActiveSet
                ? Mathf.Min(1, set.TransitionAmount + (float)delta)
                : Mathf.Max(0, set.TransitionAmount - (float)delta);
        }
    }

    public IEnumerable<SpriteSet> GetSets()
    {
        return GetChildren().OfType<SpriteSet>();
    }

    private SpriteSet GetSetByName(string name)
    {
        return GetChildren().OfType<SpriteSet>().FirstOrDefault(x => x.SetName == name);
    }

   
}
