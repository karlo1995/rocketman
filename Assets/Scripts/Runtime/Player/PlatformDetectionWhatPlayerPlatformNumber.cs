using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class PlatformDetectionWhatPlayerPlatformNumber : MonoBehaviour
{

    [SerializeField] private GameObject[] crystalPlatform;
    [SerializeField] private GameObject playerFeetCollider;
    [SerializeField] private Rigidbody2D playerRb2D;
    [SerializeField] private SkeletonAnimation bossAnimation;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject onStartCameraTransition ,mainCameraObj;

    private int currentBossPlatformNumber;
    private int currentPlatformIndex = -1;

    private float landingYOffset = 1;
    private float failThreshold = -11f;

    private void Start()
    {
        StartCoroutine(CameraOnStartTransition());
        StartCoroutine(startUpPlayerPos());
    }

    private void Update()
    {
        if (playerTransform.transform.position.y < failThreshold)
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            // detect which platform the player collided with
            for (int i = 0; i < crystalPlatform.Length; i++)
            {
                if (col.gameObject == crystalPlatform[i])
                {
                    Debug.Log($"Player collided with crystal platform number {i + 1}!");
                    currentBossPlatformNumber = i + 1;
                    Debug.Log("Player platform number: " + currentBossPlatformNumber);
                    break;
                }
            }

            // if the collided object has the Boss_1_Red_Crystals_Tag component
            if (col.gameObject.TryGetComponent<Boss_1_Red_Crystals_Tag>(out var crystal))
            {
                // destroy the crystal object
                Debug.Log("working");
                Destroy(crystal.gameObject);
            }

            if (col.gameObject.TryGetComponent<BossManagerLevelOne>(out var bossAttri))
            {
                // destroy the crystal object
                Debug.Log("boss collided");
                StartCoroutine(FeetOnAndOffForDeathIndications());
            }
        }
    }

    // get the current platform platform number
    public int GetCurrentPlayerPlatformNumber()
    {
        return currentBossPlatformNumber;
    }

    IEnumerator FeetOnAndOffForDeathIndications()
    {
        playerFeetCollider.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        playerFeetCollider.SetActive(true);

    }

    IEnumerator startUpPlayerPos()
    {

        Transform targetPlatform = crystalPlatform[4].transform;

        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

        Vector3 targetPosition = targetPlatform.position + new Vector3(0, landingYOffset, 0);

        playerRb2D.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(1.5f);
        PlayAnimationForPlayer("Main_Animations/idle", false);
    }

    IEnumerator RespawnPlayer()
    {

        Debug.Log("respawn");
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, crystalPlatform.Length);
            } while (randomIndex == currentPlatformIndex);

            Transform targetPlatform = crystalPlatform[randomIndex].transform;

            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
            PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

            yield return new WaitForSeconds(0.5f);

            Vector3 targetPosition = targetPlatform.position + new Vector3(0, landingYOffset, 0);

            playerRb2D.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(1f);

            currentPlatformIndex = randomIndex;
            PlayAnimationForPlayer("Main_Animations/idle", false);

    }

    // in start of boss, we add like a camera transition for the given suggestion by sir john
    private IEnumerator CameraOnStartTransition()
    {

        yield return new WaitForSeconds(5f);
        mainCameraObj.SetActive(false);
        onStartCameraTransition.SetActive(true);

        yield return new WaitForSeconds(7f);
        mainCameraObj.SetActive(true);
        onStartCameraTransition.SetActive(false);
    }

    public void PlayAnimationForPlayer(string animationName, bool isLoop)
    {
        if (bossAnimation.AnimationName != animationName)
        {
            bossAnimation.loop = isLoop;
            bossAnimation.AnimationName = animationName;
        }
    }
}
