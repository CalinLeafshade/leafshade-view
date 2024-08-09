using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class PolygonWiggler : Bone2D
{

	[Export]
	public float Speed {get; set;} = 1.5f;

	[Export]
	public float Offset {get; set;}

	[Export]
	public float Amplitude {get; set;} = 4f;
	List<Bone2D> bones;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		
	}

	public void BuildTree() {
		bones = new();
		Bone2D bone = this;
		
		while (bone != null) {
			bones.Add(bone);
			bone = bone.GetChildren().OfType<Bone2D>().FirstOrDefault();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (bones == null) {
			BuildTree();
		}

		float t = Time.GetTicksMsec() / 1000f;

		for(var i = 0; i < bones.Count; i++) {
			float r = Mathf.Sin(t * Speed + (i * 0.5f) + Offset) * (2f + i * Amplitude);
			float r2 = Mathf.Cos(t * Speed * 1.5f + (i * 0.5f) + Offset) * (2f + i * Amplitude);
			bones[i].RotationDegrees = (r + r2) / 2f;
		}
	}
}
