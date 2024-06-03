using System.Collections.Generic;
using System.Collections;
using Script.Misc;
using UnityEngine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;

public class DisplayDialogue : Singleton<DisplayDialogue>
{
    [SerializeField] private DialogueItemDetails dialogueItemDetails;
    [SerializeField] private TMP_Text dialogueTxt;
    [SerializeField] private SkeletonAnimation echelonExpresionSpineAnimation; //as for min-time ill use ethan as ECHELON as placeholder
    [SerializeField] private SkeletonAnimation zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation avaExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_zoeExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_ethanExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_leoExpresionSpineAnimation;
    [SerializeField] private SkeletonAnimation double_avaExpresionSpineAnimation;


    //sprites
    [SerializeField] private Sprite zoeSprite, ethanSprite, leoSprite, avaSprite, echelonSprite, ava_leoSprite, zoe_ethanSprite;
    [SerializeField] private Image spriteOneNamePlaceHolder, spriteTwoNamePlaceHolder;
    public GameObject dialogueContainer;
    public Button prevBtn;
    private int dialogueIncrement = 0; // can be public parameter in the future

    private Dictionary<SpineAnimationCharacters, SkeletonAnimation> characterAnimations;
    private Dictionary<SpineAnimationCharacters, Sprite> characterSprites;

    private bool isOpen;
    public bool IsOpen => isOpen;

    public Animator animator;
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

    void Start()
    {
        animator.SetBool("IsOpen", true);
        
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
        dialogueContainer.SetActive(true);
        var dialogueItem = dialogueItemDetails.GetDialogueItemById(p_id);

        if (dialogueItem != null)
        {
          
            if (dialogueIncrement >= 0 && dialogueIncrement < dialogueItem.DialogueHolders.Count)
            {
                var dialogueHolder = dialogueItem.DialogueHolders[dialogueIncrement];

                StartCoroutine(TypeWriter(dialogueTxt.text = dialogueHolder.DialogueText));
                
                // Deactivate all character animations
                foreach (var character in characterAnimations.Values)
                {
                    character.gameObject.SetActive(false);
                }

                // Track active characters
                var activeCharacters = new List<SpineAnimationCharacters>();

                // Activate the current characters' animations and update sprite
                foreach (var characterAnimation in dialogueHolder.CharacterAnimations)
                {
                    Debug.Log($"Character: {characterAnimation.Character}"); // Debug log for active characters

                  
                    if (characterAnimations.TryGetValue(characterAnimation.Character, out var skeletonAnimation))
                    {
                        skeletonAnimation.gameObject.SetActive(true);

                        // Try the first animation name, if not found, try the second one
                        if (!PlayAnimation(skeletonAnimation, "Facial_Expressions/" + characterAnimation.SpineAnimationName, true))
                        {
                            PlayAnimation(skeletonAnimation, "expressions/" + characterAnimation.SpineAnimationName, true);
                        }

                        activeCharacters.Add(characterAnimation.Character);
                    }
                    else
                    {
                        Debug.LogWarning($"No SkeletonAnimation found for character: {characterAnimation.Character}");
                    }
                }

                // Update the sprite based on active characters
                UpdateSpritePlaceholder(activeCharacters);
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
            //changing placeholder for sprite names
            spriteTwoNamePlaceHolder.gameObject.SetActive(true);
            spriteTwoNamePlaceHolder.sprite = ava_leoSprite;
        }
        else if (activeCharacters.Contains(SpineAnimationCharacters.Zoe) && activeCharacters.Contains(SpineAnimationCharacters.Double_Ethan))
        {
            spriteOneNamePlaceHolder.gameObject.SetActive(false);
            //changing placeholder for sprite names
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

    private bool PlayAnimation(SkeletonAnimation skeletonAnimation, string animationName, bool isLoop)
    {
        var animation = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);
        if (animation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, isLoop);
            return true;
        }
        else
        {
            Debug.LogWarning($"Animation not found: {animationName}");
            return false;
        }
    }

    public void NextDialogueBtn()
    {
        dialogueIncrement++;
        if(dialogueIncrement > 0)
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
        else if(dialogueIncrement > 0)
        {
            prevBtn.gameObject.SetActive(true);
        }
        DisplayDialogueById("stage 1 scene 1");
    }

    


}
