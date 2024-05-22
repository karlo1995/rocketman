using System.Collections.Generic;
using Script.Misc;
using UnityEngine;
using Spine.Unity;
using TMPro;

public class DisplayDialogue : Singleton<DisplayDialogue>
{
    [SerializeField] private DialogueItemDetails dialogueItemDetails;
    [SerializeField] private TMP_Text dialogueTxt;
    [SerializeField] private SkeletonAnimation zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation avaExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_avaExpresionSpineAnimation;
    
    public GameObject dialogueContainer;
    private int dialogueIncrement = 0; // can be public parameter in the future

    private Dictionary<SpineAnimationCharacters, SkeletonAnimation> characterAnimations;
    
    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        InitializeCharacterAnimations();
    }

    public void Open()
    {
        isOpen = true;
        DisplayDialogueById("stage 1 scene 1");
    }

    private void InitializeCharacterAnimations()
    {
        characterAnimations = new Dictionary<SpineAnimationCharacters, SkeletonAnimation>
        {
            { SpineAnimationCharacters.Zoe, zoeExpresionSpineAnimation },
            { SpineAnimationCharacters.Ethan, ethanExpresionSpineAnimation },
            { SpineAnimationCharacters.Leo, leoExpresionSpineAnimation },
            { SpineAnimationCharacters.Ava, avaExpresionSpineAnimation },
            { SpineAnimationCharacters.Double_Zoe, double_zoeExpresionSpineAnimation },
            { SpineAnimationCharacters.Double_Ethan, double_ethanExpresionSpineAnimation },
            { SpineAnimationCharacters.Double_Leo, double_leoExpresionSpineAnimation },
            { SpineAnimationCharacters.Double_Ava, double_avaExpresionSpineAnimation }
        };
        
        dialogueContainer.SetActive(false);
    }

    private void DisplayDialogueById(string p_id)
    {
        dialogueContainer.SetActive(true);
        var dialogueItem = dialogueItemDetails.GetDialogueItemById(p_id);

        if (dialogueItem != null)
        {
            if (dialogueIncrement >= 0 && dialogueIncrement < dialogueItem.DialogueHolders.Count)
            {
                var dialogueHolder = dialogueItem.DialogueHolders[dialogueIncrement];
                dialogueTxt.text = dialogueHolder.DialogueText;

                foreach (var character in characterAnimations.Values)
                {
                    character.gameObject.SetActive(false);
                }

                foreach (var characterAnimation in dialogueHolder.CharacterAnimations)
                {
                    if (characterAnimations.TryGetValue(characterAnimation.Character, out var skeletonAnimation))
                    {
                        skeletonAnimation.gameObject.SetActive(true);
                        PlayAnimation(skeletonAnimation, "Facial_Expressions/" + characterAnimation.SpineAnimationName, true);
                    }
                    else
                    {
                        Debug.LogWarning($"No SkeletonAnimation found for character: {characterAnimation.Character}");
                    }
                }
            }
            else
            {
                dialogueContainer.SetActive(false);
                dialogueTxt.text = "";
                dialogueIncrement = 0;
                
                isOpen = false;
            }
        }
        else
        {
            Debug.LogWarning($"No dialogue item found for ID: {p_id}");
        }
    }

    private void PlayAnimation(SkeletonAnimation skeletonAnimation, string animationName, bool isLoop)
    {
        var animation = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);
        if (animation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, isLoop);
        }
        else
        {
            Debug.LogWarning($"Animation not found: {animationName}");
        }
    }

    public void NextDialogueBtn()
    {
        dialogueIncrement++;
        DisplayDialogueById("stage 1 scene 1"); // the string character inside will replace in a future parameter
    }
}
