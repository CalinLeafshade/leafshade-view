using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;

namespace Leafshade;

public partial class AttachmentItem : Node2D
{

    public bool Active { get; set; }

    [Export]
    public string AttachmentName { get; set; }

  
    public override void _Ready()
    {
     
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        var da = Active ? delta : -delta;

        Modulate = new Color(1, 1, 1, (float)Mathf.Clamp(Modulate.A + da, 0f, 1f));

       
    }

    public void Activate(int length = -1)
    {
      
        Active = true;
       
    }

    public void Deactivate()
    {
        Active = false;
      
    }

}
