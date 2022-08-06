using System.Collections;
using System.Collections.Generic;

public class DialogueInventory
{
    public static Queue<DialogueItem> GetDialogueData()
    {
        Queue<DialogueItem> dialogueItems = new Queue<DialogueItem>();
        // dialogueItems.Enqueue(new DialogueItem("none", ""));
        dialogueItems.Enqueue(new DialogueItem("wolfhead-portrait-0-ws", "瞅啥瞅"));
        dialogueItems.Enqueue(new DialogueItem("cowhead-portrait-0-ws", "瞅你咋地"));
        dialogueItems.Enqueue(new DialogueItem("wolfhead-portrait-0-ws", "瞅啥瞅？！"));
        dialogueItems.Enqueue(new DialogueItem("horsehead-portrait-1-ws", "瞅你咋地！？"));
        return dialogueItems;
    }
}
