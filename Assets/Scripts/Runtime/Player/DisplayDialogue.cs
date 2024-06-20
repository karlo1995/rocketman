using System.Collections;
using System.Collections.Generic;
using Script.Misc;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDialogue : Singleton<DisplayDialogue>
{
    [SerializeField] private DialogueItemDetails dialogueItemDetails;
    [SerializeField] private TMP_Text dialogueTxt;
    [SerializeField] private SkeletonAnimation echelonExpresionSpineAnimation; // as for min-time ill use ethan as ECHELON as placeholder
    [SerializeField] private SkeletonAnimation zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation avaExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_avaExpresionSpineAnimation;

    // sprites
    [SerializeField] private Sprite zoeSprite, ethanSprite, leoSprite, avaSprite, echelonSprite, ava_leoSprite, zoe_ethanSprite;
    [SerializeField] private Image spriteOneNamePlaceHolder, spriteTwoNamePlaceHolder;
    public GameObject dialogueContainer;
    public Button prevBtn, nextBtn, closeBtn;

    private int dialogueIncrement = 0; // can be public parameter in the future

    private Dictionary<SpineAnimationCharacters, SkeletonAnimation> characterAnimations;
    private Dictionary<SpineAnimationCharacters, Sprite> characterSprites;

    private bool isOpen;
    public bool IsOpen => isOpen;

    private float typingSpeed = 0.03f;
  
    private void Awake()
    {
        InitializeCharacterAnimations();
        InitializeCharacterSprites();

        if (dialogueIncrement <= 0)
        {
            prevBtn.gameObject.SetActive(false);
            dialogueIncrement = 0;
        }
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
            { SpineAnimationCharacters.Echelon, echelonExpresionSpineAnimation },
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

    private void InitializeCharacterSprites()
    {
        characterSprites = new Dictionary<SpineAnimationCharacters, Sprite>
        {
            { SpineAnimationCharacters.Echelon, echelonSprite },
            { SpineAnimationCharacters.Zoe, zoeSprite },
            { SpineAnimationCharacters.Ethan, ethanSprite },
            { SpineAnimationCharacters.Leo, leoSprite },
            { SpineAnimationCharacters.Ava, avaSprite },
        };
    }

    private void DisplayDialogueById(string p_id)
    {
        StopAllCoroutines();
        dialogueContainer.SetActive(true);
        var dialogueItem = dialogueItemDetails.GetDialogueItemById(p_id);
        dialogueTxt.fontSize = 46;
        if (dialogueItem != null)
        {
            if (dialogueIncrement >= 0 && dialogueIncrement < dialogueItem.DialogueHolders.Count)
            {
                var dialogueHolder = dialogueItem.DialogueHolders[dialogueIncrement];
                StartCoroutine(TypeWriter(dialogueTxt.text = dialogueHolder.DialogueText));

                // off all the animation spines
                foreach (var character in characterAnimations.Values)
                {
                    character.gameObject.SetActive(false);
                }

                // it will track the current character using
                var activeCharacters = new List<SpineAnimationCharacters>();

                // set on the current characters' animations and update sprite
                foreach (var characterAnimation in dialogueHolder.CharacterAnimations)
                {
                    Debug.Log($"Character: {characterAnimation.Character}"); // Debug log for active characters

                    if (characterAnimations.TryGetValue(characterAnimation.Character, out var skeletonAnimation))
                    {
                        skeletonAnimation.gameObject.SetActive(true);

                        // play the neutral/default/ animation first, then the specified animation in a loop
                        PlayAnimationSequence(skeletonAnimation, characterAnimation.SpineAnimationName);
                      
                        activeCharacters.Add(characterAnimation.Character);
                    }
                    else
                    {
                        Debug.LogWarning($"No SkeletonAnimation found for character: {characterAnimation.Character}");
                    }
                }

                // change the sprite based on active characters
                UpdateSpritePlaceholder(activeCharacters);

                // update button if need to open the X/close btn
                UpdateButtonVisibility(dialogueItem.DialogueHolders.Count);
            }
            else
            {
                dialogueContainer.SetActive(false);
                dialogueTxt.text = "";
                dialogueIncrement = 0;

                isOpen = false;

                LevelManager.Instance.SpawnPlayerAtTheStartOfTheGame();
            }
        }
        else
        {
            Debug.LogWarning($"No dialogue item found for ID: {p_id}");
        }
    }

    private IEnumerator TypeWriter(string line)
    {
        dialogueTxt.text = "";
        yield return new WaitForSeconds(.5f);
        foreach (char letter in line.ToCharArray())
        {
            dialogueTxt.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    private void UpdateSpritePlaceholder(List<SpineAnimationCharacters> activeCharacters)
    {
        if (activeCharacters.Contains(SpineAnimationCharacters.Double_Leo) && activeCharacters.Contains(SpineAnimationCharacters.Ava))
        {
            spriteOneNamePlaceHolder.gameObject.SetActive(false);
            // changing placeholder for sprite names
            spriteTwoNamePlaceHolder.gameObject.SetActive(true);
            spriteTwoNamePlaceHolder.sprite = ava_leoSprite;
        }
        else if (activeCharacters.Contains(SpineAnimationCharacters.Zoe) && activeCharacters.Contains(SpineAnimationCharacters.Double_Ethan))
        {
            spriteOneNamePlaceHolder.gameObject.SetActive(false);
            // changing placeholder for sprite names
            spriteTwoNamePlaceHolder.gameObject.SetActive(true);
            spriteTwoNamePlaceHolder.sprite = zoe_ethanSprite;
        }
        else if (activeCharacters.Count == 1)
        {
            if (characterSprites.TryGetValue(activeCharacters[0], out var sprite))
            {
                spriteTwoNamePlaceHolder.gameObject.SetActive(false);
                spriteOneNamePlaceHolder.gameObject.SetActive(true);
                spriteOneNamePlaceHolder.sprite = sprite;
            }
        }
        else
        {
            // default is null
            spriteOneNamePlaceHolder.sprite = null;
            spriteTwoNamePlaceHolder.sprite = null;
        }
    }

    private void PlayAnimationSequence(SkeletonAnimation skeletonAnimation, string animationName)
    {
        var animationNames = new List<string>
        {
            "Facial_Expressions/Neutral_Default",
            "Facial_Expressions/Neutral",
            "expressions/default",
            "Facial_Expressions/" + animationName,
            "expressions/" + animationName
        };

        StartCoroutine(PlayAnimationsInSequence(skeletonAnimation, animationNames));
    }

    private IEnumerator PlayAnimationsInSequence(SkeletonAnimation skeletonAnimation, List<string> animationNames)
    {
        int index = 0;
        while (true)
        {
            var animationName = animationNames[index];
            if (TryPlayAnimation(skeletonAnimation, animationName, false))
            {
                yield return new WaitForSeconds(skeletonAnimation.AnimationState.GetCurrent(0).Animation.Duration);
            }
            index = (index + 1) % animationNames.Count;
        }
    }

    private bool TryPlayAnimation(SkeletonAnimation skeletonAnimation, string animationName, bool isLoop)
    {
        var animation = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);
        if (animation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, isLoop);
            return true;
        }
        return false;
    }

    public void NextDialogueBtn()
    {
        dialogueIncrement++;
        if (dialogueIncrement > 0)
        {
            prevBtn.gameObject.SetActive(true);
        }
        DisplayDialogueById("stage 1 scene 1");
    }

    public void PrevDialogueBtn()
    {
        dialogueIncrement--;
        if (dialogueIncrement <= 0)
        {
            prevBtn.gameObject.SetActive(false);
            dialogueIncrement = 0;
        }
        else if (dialogueIncrement > 0)
        {
            prevBtn.gameObject.SetActive(true);
        }
        DisplayDialogueById("stage 1 scene 1");
    }

    private void UpdateButtonVisibility(int dialogueHolderCount)
    {
        if (dialogueIncrement >= dialogueHolderCount - 1)
        {
            nextBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(false);
        }
    }
}
