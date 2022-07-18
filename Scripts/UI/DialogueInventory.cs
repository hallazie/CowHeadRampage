using System.Collections;
using System.Collections.Generic;

public class DialogueInventory
{
    public static Queue<DialogueItem> GetDialogueData()
    {
        Queue<DialogueItem> dialogueItems = new Queue<DialogueItem>();
        dialogueItems.Enqueue(new DialogueItem("wolfhead-2", "瞅啥瞅"));
        dialogueItems.Enqueue(new DialogueItem("cowhead-0", "瞅你咋地"));
        dialogueItems.Enqueue(new DialogueItem("pighead-0", "？？"));
        dialogueItems.Enqueue(new DialogueItem("horsehead-0", "日你仙人"));
        return dialogueItems;
    }
}
