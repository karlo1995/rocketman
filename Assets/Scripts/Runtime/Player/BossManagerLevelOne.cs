using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class BossManagerLevelOne : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation bossAnimation;
    [SerializeField] private Rigidbody2D bossRb2D;
    [SerializeField] private Transform bossPos;
    [SerializeField] private Transform playerDeathTrigger; // this is not a permanent player death indicator for Pos Y here this is for temporary since its on testing
    [SerializeField] private GameObject[] crystalPlatform;
    [SerializeField] private GameObject[] RedCrystalCollected; // Array to keep track of collected red crystals

    // Animation variables
    private string airTime = "Airtime";
    private string head180 = "Head 180";
    private string headDownToUpLeft = "Head down to up_left";
    private string headDownToUpRight = "Head down to up_right";
    private string idle = "Idle";
    private string jump = "Jump";
    private string landing = "Landing";
    private string walk = "walk";

    private bool playerDeath = false;

    private PlatformDetectionWhatPlayerPlatformNumber platformDetection;
    private int currentPlatformIndex = -1;
    private int redCrystalTotalCollectIndex = 0;

    // Adjustable Y-axis offset for landing
    [SerializeField] private float landingYOffset = 1.0f;

    void Start()
    {
        // the coroutine to begin boss behavior after 7 seconds this is a default/temporary
        StartCoroutine(StartBossBehaviorAfterDelay());

        // Find the PlatformDetectionWhatPlayerPlatformNumber script
        platformDetection = FindObjectOfType<PlatformDetectionWhatPlayerPlatformNumber>();

        if (platformDetection != null)
        {
            currentPlatformIndex = platformDetection.GetCurrentPlayerPlatformNumber() - 1;
            Debug.Log("Current Boss Platform Number: " + (currentPlatformIndex + 1));
        }
        else
        {
            Debug.LogError("PlatformDetectionWhatPlayerPlatformNumber script not found!");
        }

        Debug.Log("Red Crystal Collected Count: " + RedCrystalCollected.Length);
        redCrystalTotalCollectIndex = RedCrystalCollected.Length;
    }

    private void Update()
    {
        if (playerDeathTrigger.position.y < -11.0f)
        {
            Debug.Log("Death player from boss");
        }

        if(RedCrystalCollected == null || RedCrystalCollected.Length == 0)
        {
            Debug.Log("All red crystall collected proceed to success");
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            // Detect the crystal platform the boss collided with
            for (int i = 0; i < crystalPlatform.Length; i++)
            {
                if (col.gameObject == crystalPlatform[i])
                {
                    Debug.Log($"Collided with crystal platform number {i + 1}!");
                    currentPlatformIndex = i;
                    break;
                }
            }

            // Check if the collided object has the Boss_1_Red_Crystals_Tag component and destroy it
            if (col.gameObject.TryGetComponent(out Boss_1_Red_Crystals_Tag _))
            {
                var crystal = col.gameObject.GetComponent<Boss_1_Red_Crystals_Tag>();
                crystal.DestroyRedCrystal();
            }
        }
    }

    public void PlayAnimationForBoss(string animationName, bool isLoop)
    {
        if (bossAnimation.AnimationName != animationName)
        {
            bossAnimation.loop = isLoop;
            bossAnimation.AnimationName = animationName;
        }
    }

    IEnumerator BossBehavior()
    {
        while (true)
        {
            yield return StartCoroutine(BossIdleTransition());

            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, crystalPlatform.Length);
            } while (randomIndex == currentPlatformIndex);

            Transform targetPlatform = crystalPlatform[randomIndex].transform;

            PlayAnimationForBoss(jump, false);

            yield return new WaitForSeconds(0.5f);

            Vector3 targetPosition = targetPlatform.position + new Vector3(0, landingYOffset, 0);

            bossRb2D.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(1f);

            currentPlatformIndex = randomIndex;

            PlayAnimationForBoss(landing, false);

            yield return new WaitForSeconds(1f);
        }
    }

    // Boss idle animation transition
    IEnumerator BossIdleTransition()
    {
        PlayAnimationForBoss(idle, false);
        yield return new WaitForSeconds(1.5f);
        PlayAnimationForBoss(head180, false);
        yield return new WaitForSeconds(3f);
        PlayAnimationForBoss(idle, true);
    }

    IEnumerator StartBossBehaviorAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        StartCoroutine(BossBehavior());
    }
}
