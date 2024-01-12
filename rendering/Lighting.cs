using System;
using System.Collections.Generic;
using Godot;

public partial class Lighting : Node2D
{

    private List<DirectionalLight2D> lights { get; set; }

    private CanvasModulate dimmer;

    private float raveAmount;
    private bool isRaving;

    private int isDimmed;
    private float dimAmount;

    private bool isMoody;
    private float moodyAmount;

    private PointLight2D moodLight;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        dimmer = GetNode<CanvasModulate>("Dimmer");
        moodLight = GetNode<PointLight2D>("MoodLight");
        lights = new() {
            GetNode<DirectionalLight2D>("DirectionalLight2D1"),
            GetNode<DirectionalLight2D>("DirectionalLight2D2"),
            GetNode<DirectionalLight2D>("DirectionalLight2D3"),
            GetNode<DirectionalLight2D>("DirectionalLight2D4"),
        };
    }

    public void Dim()
    {
        isDimmed++;
    }

    public void Undim()
    {
        isDimmed = Math.Max(isDimmed - 1, 0);
    }

    public void Rave()
    {
        if (isRaving)
        {
            return;
        }
        Dim();
        isRaving = true;
    }

    public void Unrave()
    {
        Undim();
        isRaving = false;
    }

    public void Moody()
    {
        if (isMoody)
        {
            return;
        }
        Dim();
        isMoody = true;
    }

    public void Unmoody()
    {
        Undim();
        isMoody = false;
    }

    public void Effect(string name)
    {
        if (name == "rave")
        {
            Rave();
            Timer t = new()
            {
                WaitTime = 20,
                OneShot = true
            };
            t.Timeout += () =>
            {
                Unrave();
                t.QueueFree();
            };
            AddChild(t);
            t.Start();
        }
    }

    Color ColorLerp(Color[] colours, float t)
    {

        t = Mathf.Clamp(t, 0, 1);

        float delta = 1f / (colours.Length - 1);
        int startIndex = (int)(t / delta);

        if (startIndex == colours.Length - 1)
        {
            return colours[^1];
        }

        float localT = (t % delta) / delta;

        return (colours[startIndex] * (1f - localT)) +
                (colours[startIndex + 1] * localT);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        raveAmount = isRaving ? Mathf.Clamp(raveAmount + (float)delta, 0f, 1f) : Mathf.Clamp(raveAmount - (float)delta, 0f, 1f);

        dimAmount = isDimmed > 0 ? Mathf.Clamp(dimAmount + (float)delta, 0f, 1f) : Mathf.Clamp(dimAmount - (float)delta, 0f, 1f);
        moodyAmount = isMoody ? Mathf.Clamp(moodyAmount + (float)delta, 0f, 1f) : Mathf.Clamp(moodyAmount - (float)delta, 0f, 1f);

        dimmer.Color = ColorLerp(new[] { Color.Color8(255, 255, 255), Color.Color8(80, 80, 80) }, dimAmount);
        moodLight.Energy = moodyAmount * 3.5f;

        var t = Time.GetTicksMsec() / 1000f;
        for (var i = 0; i < lights.Count; i++)
        {
            var l = lights[i];
            float f = 4f + (i * 1.5f);
            float phase = -i * 100;
            float amp = 5f;

            float e = Mathf.Sin((t * f) + phase);

            l.Energy = ((e * (amp / 2f)) + (amp / 2f)) * raveAmount;
        }
    }
}
