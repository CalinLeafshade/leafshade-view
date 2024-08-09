using System;
using System.Collections.Generic;
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

    public bool HasAttachment(string name) {
        return this.AllChildren().OfType<AttachmentItem>().Any(x => x.AttachmentName == name);
    }

    public void EnableAttachment(string name)
    {

        var allAttachments = this.AllChildren().OfType<AttachmentItem>();

        var item = allAttachments.FirstOrDefault(x => x.AttachmentName == name);

        if (item != null)
        {
            item.Active = true;
            item.GetSiblings().OfType<AttachmentItem>().ToList().ForEach(x => x.Active = false);
        }

    }

    internal void DisableIncompatibleAttachments(string name)
    {
         var allAttachments = this.AllChildren().OfType<AttachmentItem>();

        var item = allAttachments.FirstOrDefault(x => x.AttachmentName == name);

        if (item != null)
        {
            item.GetSiblings().OfType<AttachmentItem>().ToList().ForEach(x => x.Active = false);
        }
    }

    internal void DisableAttachment(string name)
    {
         var allAttachments = this.AllChildren().OfType<AttachmentItem>();

        var item = allAttachments.FirstOrDefault(x => x.AttachmentName == name);

        if (item != null) {
            item.Active = false;
        }
    }

    internal IEnumerable<string> GetAllAttachmentNames()
    {
        return this.AllChildren().OfType<AttachmentItem>().Select(x => x.AttachmentName);
    }

}
