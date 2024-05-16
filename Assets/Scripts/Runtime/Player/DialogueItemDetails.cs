using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueItemDetails : ScriptableObject
{
    public List<DialogueItem> DialogueItems = new();

    public DialogueItem GetDialogueItemById(string p_id)
    {
        foreach (var item in DialogueItems)
        {
            if (p_id.Equals(item.DialogueId))
            {
                return item;
            }
        }

        return null;
    }
}

[System.Serializable]
public class DialogueItem
{
    public string DialogueId;
    public List<SpineAnimationCharacters> SpineAnimationCharacters = new(); // Character names "zoe, dr leo, dr ava, zoe"
    public List<string> SpineAnimationNames = new(); // Animation names "Facial_Expressions 
    [TextArea(4, 10)] public List<string> Dialogue = new();
}

public enum SpineAnimationCharacters
{
    Zoe, Leo, Ava, Ethan
}

