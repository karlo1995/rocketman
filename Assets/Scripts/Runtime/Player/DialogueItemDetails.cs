using System.Collections.Generic;
using UnityEngine;

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
    public List<DialogueHolder> DialogueHolders = new();
}

[System.Serializable]
public class DialogueHolder
{
    [TextArea(4, 10)] public string DialogueText;
    public List<CharacterAnimationPair> CharacterAnimations = new();
}

[System.Serializable]
public class CharacterAnimationPair
{
    public SpineAnimationCharacters Character;
    public string SpineAnimationName;
}

public enum SpineAnimationCharacters
{
     Zoe, Leo, Ava, Ethan, Double_Zoe, Double_Leo, Double_Ava, Double_Ethan, Echelon
}