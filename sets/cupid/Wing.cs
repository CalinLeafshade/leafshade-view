using Godot;
using System;

public partial class Wing : Node2D
{

	[Export]
	public Bone2D TopBone {get; set;}

	[Export]
	public Bone2D SecondBone {get; set;}

	[Export]
	public float StrengthOne {get;set;} = 1f;

	[Export]
	public float StrengthTwo {get;set;} = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (TopBone != null) {
			TopBone.Rotation = Mathf.Sin(Time.GetTicksMsec() * 0.001f) * (0.1f * StrengthOne) - 0.1f;
		}
		if (SecondBone != null) {
			SecondBone.Rotation = Mathf.Sin(Time.GetTicksMsec() * 0.001f) * (0.05f * StrengthTwo) - 0.2f;
		}
	}
}
