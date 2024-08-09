using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Leafshade;

public partial class Listener : Node
{

    private const float MIN_DB = 60;
    private AudioEffectSpectrumAnalyzerInstance spec;
    private AudioEffectRecord record;

    private Control ui;
    private HSlider thresholdSlider;
    private ProgressBar valueBar;
    private MenuButton inputMenu;
    private Timer restartTimer;
    private AudioStreamPlayer player;

    [Signal]
    public delegate void StartedTalkingEventHandler(float strength);

    [Signal]
    public delegate void StoppedTalkingEventHandler();

    [Export]
    public float Threshold { get; set; } = 0.1f;

    [Export]
    public ulong TalkingHold { get; set; } = 100;

    [Export]
    public Avatar Avatar {get; set;}

    public bool IsTalking { get; private set; }

    private ulong startedTalking;
    private ulong stoppedTalking;

    private List<float> energies = new();

    private async Task Restart()
    {
        player?.QueueFree();
        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        CallDeferred("Start");
    }

    private void Start()
    {

        player = new AudioStreamPlayer
        {
            Stream = new AudioStreamMicrophone(),
            Bus = "Record"
        };

        AddChild(player);

        player.Play();
    }

    public override void _Ready()
    {
        spec = AudioServer.GetBusEffectInstance(1, 1) as AudioEffectSpectrumAnalyzerInstance;
        record = AudioServer.GetBusEffect(1, 0) as AudioEffectRecord;

        restartTimer = GetNode<Timer>("%RestartTimer");

        restartTimer.Timeout += async () =>
        {
            GD.Print("Restarting Audio");
            await Restart();
        };

        Button restartButton = GetNode<Button>("%RestartButton");

        restartButton.Pressed += async () =>
        {
            await Restart();
        };

        thresholdSlider = GetNode<HSlider>("%ThresholdSlider");
        valueBar = GetNode<ProgressBar>("%ValueDisplay");
        inputMenu = GetNode<MenuButton>("%InputMenu");

        thresholdSlider.Value = Threshold * 100;
        thresholdSlider.ValueChanged += (double val) => Threshold = (float)val / 100f;
        ui = GetNode<Control>("UI");

        string[] inputs = AudioServer.GetInputDeviceList();

        foreach (string i in inputs)
        {
            inputMenu.GetPopup().AddItem(i);
        }

        inputMenu.GetPopup().IdPressed += (long index) =>
        {
            string item = inputMenu.GetPopup().GetItemText((int)index);
            AudioServer.InputDevice = item;
            GD.Print(item);
        };

        inputMenu.Text = AudioServer.InputDevice;

        CallDeferred("Start");

        if (Avatar != null) {
            foreach(var set in Avatar.GetAvailableCostumes()) {
                var b = new Button();
                b.Text = set;
                b.Pressed += () => Avatar.SetCostume(set);
                GetNode("%CostumeButtons").AddChild(b);
            }

            foreach(var set in Avatar.GetAvailableAttachments()) {
                var b = new Button();
                b.Text = set;
                b.Pressed += () => Avatar.EnableAttachment(set);
                GetNode("%CostumeButtons").AddChild(b);
            }
        }

    }

    private void Talk(float str)
    {
        if ((Time.GetTicksMsec() - stoppedTalking) > TalkingHold && !IsTalking)
        {
            IsTalking = true;
            startedTalking = Time.GetTicksMsec();
            _ = EmitSignal(SignalName.StartedTalking, str);
        }
    }

    private void Untalk()
    {
        if ((Time.GetTicksMsec() - startedTalking) > TalkingHold && IsTalking)
        {
            IsTalking = false;
            stoppedTalking = Time.GetTicksMsec();
            _ = EmitSignal(SignalName.StoppedTalking);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        ui.Visible = GetWindow().HasFocus();
        inputMenu.Text = AudioServer.InputDevice;
        Vector2 mag = spec.GetMagnitudeForFrequencyRange(200, 7000);
        float len = mag.Length();

        float e = Mathf.Clamp((MIN_DB + Mathf.LinearToDb(len)) / MIN_DB, 0, 1);

        energies.Insert(0, e);

        int avgFrameCount = (int)Engine.GetFramesPerSecond() / 12;

        energies = energies.Take(avgFrameCount).ToList();

        float energy = energies.DefaultIfEmpty(0).Average();

        valueBar.Value = energy * 100;

        if (energy > Threshold)
        {
            Talk(energy);
        }
        else
        {
            Untalk();
        }

    }
}
