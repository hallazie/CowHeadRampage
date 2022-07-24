using System.Collections;
using System.Collections.Generic;

public class DialogueInventory
{
    public static Queue<DialogueItem> GetDialogueData()
    {
        Queue<DialogueItem> dialogueItems = new Queue<DialogueItem>();
        dialogueItems.Enqueue(new DialogueItem("wolfhead-portrait-0", "瞅啥瞅"));
        dialogueItems.Enqueue(new DialogueItem("cowhead-portrait-0", "瞅你咋地"));
        dialogueItems.Enqueue(new DialogueItem("wolfhead-portrait-0", "瞅啥瞅？！"));
        dialogueItems.Enqueue(new DialogueItem("cowhead-portrait-0", "瞅你咋地！？"));
        return dialogueItems;
    }
}
