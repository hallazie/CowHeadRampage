using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public Dictionary<string, Sprite> characterPortraitDict = new Dictionary<string, Sprite>();
    public List<Sprite> characterPortraitList = new List<Sprite>();
    public bool activate;

    public GameObject portrait;
    public GameObject messageBox;
    public GameObject messageText;
    public GameObject target;

    public Vector2 offset = new Vector2(-6, -2);

    private Queue<DialogueItem> dialogueQueue = new Queue<DialogueItem>();

    private static DialogueManager _instance;
    public static DialogueManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        foreach (Sprite sprite in characterPortraitList)
        {
            characterPortraitDict.Add(sprite.name, sprite);
        }
    }

    void Start()
    {
        /*
         一次完整的对话过程：
            1. DialogueManager <===     activate (show E)            ==== target
            2. DialogueManager ====     trigger (press E)            ===> target
            3. DialogueManager <===     inject dialogue info         ==== target
            4. DialogueManager ====     finish play (change state)   ===> target
         */
    }

    void Update()
    {
        transform.position = (Vector2)Camera.main.transform.position + offset;
        if (activate)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextDialogue();
            }
        }
        else
        {
            
        }
    }

    public void ShowPortrait(string portraitName)
    {
        if (!characterPortraitDict.ContainsKey(portraitName))
            return;
        portrait.GetComponent<SpriteRenderer>().sprite = characterPortraitDict[portraitName];
    }

    public void DisplayInjectedDialogueQueue()
    {
        /*
         播放存储在实际Dialogue对象中的对话序列（由 <Portrait, Message> 组成）。
        具体序列存储在资源池中，由target对象根据自身状态fetch出
         */
        activate = true;
        messageBox.SetActive(true);
        DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (dialogueQueue.Count == 0) {
            messageBox.SetActive(false);
            portrait.GetComponent<SpriteRenderer>().enabled = false;
            target.SendMessage("StopConversation");  
            return;
        }
        DialogueItem dialogue = dialogueQueue.Dequeue();
        messageText.GetComponent<Text>().text = dialogue.message;
        ShowPortrait(dialogue.portrait);
    }

    public void StartDialogue(GameObject target)
    {
        // dialogueQueue = dialogueItems;
        if (!activate)
        {
            activate = true;
            this.target = target;
            // change to send by parameter
            dialogueQueue = DialogueInventory.GetDialogueData();
            messageBox.SetActive(true);
            portrait.GetComponent<SpriteRenderer>().enabled = true;
            GameManager.instance.cowHead.StartConversation();
            DisplayInjectedDialogueQueue();
        }
    }

    public void EndDialogue()
    {
        activate = false;
        GameManager.instance.cowHead.StopConversation();
    }
}
