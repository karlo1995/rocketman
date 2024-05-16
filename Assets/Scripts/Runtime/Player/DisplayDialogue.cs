using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Script.Misc;

public class DisplayDialogue : Singleton<DisplayDialogue>
{
    [SerializeField] private DialogueItemDetails dialogueItemDetails;
    [SerializeField] private Text dialogueTxt;
    [SerializeField] private SkeletonAnimation zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation avaExpresionSpineAnimation;
    public GameObject dialogueContainer;
    private int dialogueIncrement = 0; // can be public parameter in the future

    private Dictionary<SpineAnimationCharacters, SkeletonAnimation> characterAnimations;

    void Awake()
    {
        InitializeCharacterAnimations(); // the string character inside will replace in a future parameter
    }

    void Start()
    {
        DisplayDialogueById("stage_1_scene_1");
    }

    public void DisplayDialogueById(string p_id)
    {
        dialogueContainer.SetActive(true);
        DialogueItem dialogueItem = dialogueItemDetails.GetDialogueItemById(p_id);

        if (dialogueItem != null)
        {
            if (dialogueIncrement >= 0 && dialogueIncrement < dialogueItem.Dialogue.Count)
            {
                dialogueTxt.text = dialogueItem.Dialogue[dialogueIncrement];
                
                if (dialogueIncrement < dialogueItem.SpineAnimationNames.Count && dialogueIncrement < dialogueItem.SpineAnimationCharacters.Count)
                {
                    var characterName = dialogueItem.SpineAnimationCharacters[dialogueIncrement];
                    string animationName = dialogueItem.SpineAnimationNames[dialogueIncrement];

                    Debug.Log($"Displaying dialogue for character: {characterName}, animation: {animationName}");

                    foreach (var character in characterAnimations.Keys)
                    {
                        characterAnimations[character].gameObject.SetActive(false);
                    }

                    if (characterAnimations.TryGetValue(characterName, out var skeletonAnimation))
                    {
                        skeletonAnimation.gameObject.SetActive(true);
                        PlayAnimation(skeletonAnimation, "Facial_Expressions/" + animationName, true);
                    }
                    else
                    {
                        skeletonAnimation.gameObject.SetActive(true);
                        PlayAnimation(skeletonAnimation, "Facial_Expressions/", true);
                        //Debug.LogWarning($"No SkeletonAnimation found for character: {characterName}");
                    }
                }
            }
            else
            {
                dialogueContainer.SetActive(false);
                dialogueTxt.text = "";
                dialogueIncrement = 0;
            }
        }
       
    }

    public void PlayAnimation(SkeletonAnimation skeletonAnimation, string animationName, bool isLoop)
    {
        if (skeletonAnimation.Skeleton.Data.FindAnimation(animationName) != null)
        {
            if (skeletonAnimation.AnimationState.GetCurrent(0) == null || skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name != animationName)
            {
                skeletonAnimation.state.SetAnimation(0, animationName, isLoop);
            }
        }
        else
        {
            Debug.Log($"Animation not found: {animationName}");
        }
    }

    private void InitializeCharacterAnimations()
    {
        characterAnimations = new Dictionary<SpineAnimationCharacters, SkeletonAnimation>
        {
            { SpineAnimationCharacters.Zoe, zoeExpresionSpineAnimation },
            { SpineAnimationCharacters.Ethan, ethanExpresionSpineAnimation },
            { SpineAnimationCharacters.Leo, leoExpresionSpineAnimation },
            { SpineAnimationCharacters.Ava, avaExpresionSpineAnimation }
        };
    }

    public void NextDialogueBtn()
    {
        dialogueIncrement++;
        DisplayDialogueById("stage_1_scene_1"); // the string character inside will replace in a future parameter
    }
}
