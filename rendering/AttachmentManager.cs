using System;
using System.Linq;
using Godot;

namespace Leafshade;

public partial class AttachmentManager : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void EnableAttachment(string name)
    {

        var allAttachments = this.AllChildren().OfType<AttachmentItem>();

        var item = allAttachments.FirstOrDefault(x => x.AttachmentName == name);

        if (item != null)
        {
            item.Activate();
            item.GetSiblings().OfType<AttachmentItem>().ToList().ForEach(x => x.Deactivate());
        }

    }
}
