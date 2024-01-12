using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;

namespace Leafshade;

public partial class AttachmentItem : Node2D
{

    protected Avatar Avatar { get; private set; }

    private bool Active { get; set; }

    [Export]
    public int DefaultLength {get; set; } = 120;

    [Export]
    public string AttachmentName { get; set; }

    [Export]
    public Sprite2D FallbackSprite { get; set; }

    [Export]
    public Godot.Collections.Dictionary<string, NodePath> Sprites { get; set; }

    private Sprite2D ActiveSprite => Sprites.ContainsKey(Avatar.CurrentSprite) ? GetNode<Sprite2D>(Sprites[Avatar.CurrentSprite]) : FallbackSprite;
    private Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Avatar = GetParent().GetParent().GetParent<Avatar>();
        timer = new Timer();
        timer.Timeout += () =>
        {
            Deactivate();
        };
        AddChild(timer);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        var da = Active ? delta : -delta;

        Modulate = new Color(1, 1, 1, (float)Mathf.Clamp(Modulate.A + da, 0f, 1f));

        if (ActiveSprite != null)
        {
            ActiveSprite.Modulate = new Color(1, 1, 1, (float)Mathf.Clamp(ActiveSprite.Modulate.A + delta, 0f, 1f));
        }

        GetChildren()
            .OfType<Sprite2D>()
            .Where(x => x != ActiveSprite)
            .ToList()
            .ForEach(x => x.Modulate = new Color(1, 1, 1, (float)Mathf.Clamp(x.Modulate.A - delta, 0f, 1f)));
    }

    public void Activate(int length = -1)
    {
        if (length == -1) {
            length = DefaultLength;
        }
        Active = true;
        timer.WaitTime = Math.Max(timer.TimeLeft, length);
        timer.Start();
    }

    public void Deactivate()
    {
        Active = false;
        timer.Stop();
    }

}
