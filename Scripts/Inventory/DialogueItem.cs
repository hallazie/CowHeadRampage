using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItem
{
    public string portrait;
    public string message;

    public DialogueItem(string portrait, string message)
    {
        this.portrait = portrait;
        this.message = message;
    }
}
