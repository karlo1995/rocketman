using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Levels.Platform_Scripts
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private Transform launchPosition;
        [SerializeField] private GameObject[] platformTriggers;

        public Transform SpawnPosition => spawnPosition;
        public Transform LaunchPosition => launchPosition;

        [SerializeField] private Transform midLandingPosition;
        [SerializeField] private Transform midLandingWalkingPosition;

        private SpriteRenderer spriteRenderer;
        private MeshRenderer meshRenderer;
        
        public int levelPlatform;
        private int spawnedPlatformIndex;
        public int SpawnedPlatformIndex => spawnedPlatformIndex;

        private bool isSamePlatformCollided;
        public bool willTriggerCameraMove;

        private bool isActive;
        public bool IsActive;

        private void Awake()
        {
            if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                this.spriteRenderer = spriteRenderer;
            }
            else if (gameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                this.meshRenderer = meshRenderer;
            }
            
            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(false);
            }
        }

        public void InitPlatform(Vector2 platformPosition, int levelPlatform, int spawnedPlatformIndex, bool willTriggerCameraMove)
        {
            this.levelPlatform = levelPlatform;
            this.spawnedPlatformIndex = spawnedPlatformIndex;
            this.willTriggerCameraMove = willTriggerCameraMove;

            transform.DOMove(platformPosition, 0f).OnComplete(SpawnPlatform).SetUpdate(true);

            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(true);
            }
        }

        public void RemovePlatform()
        {
            isActive = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(0f, 0.3f).OnComplete(() => { gameObject.SetActive(false); }).SetUpdate(true);
            }
            else if(meshRenderer != null)
            {
                gameObject.GetComponent<MeshRenderer>().materials[0].DOFade(1.2f, 0.3f).OnComplete(() => { gameObject.SetActive(false); }).SetUpdate(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void SpawnPlatform()
        {
            isActive = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(1f, 0.3f).OnComplete(() => { gameObject.SetActive(true); }).SetUpdate(true);
            }
            else if(meshRenderer != null)
            {
                gameObject.GetComponent<MeshRenderer>().materials[0].DOFade(1f, 0.3f).OnComplete(() => { gameObject.SetActive(true); }).SetUpdate(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void CollisionEnterBehaviour(bool isSamePlatformCollided)
        {
            this.isSamePlatformCollided = isSamePlatformCollided;

            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(false);
            }

            if (!PlayerDeathController.Instance.IsDropped)
            {
                var needToWalk = CheckIfNeedToWalkToMid();

                if (!needToWalk)
                {
                    SpawnNextPlatform();
                }
            }
            else
            {
                PlayerDeathController.Instance.ResetDropStatus();

                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                PlayerDragController.Instance.SetCanDrag();
            }
        }

        public void CollisionExitBehaviour()
        {
            PlayerDragController.Instance.AddPlatformControllerToCheck(this);
            ResetLevelPlatform();
        }

        public void OpenAnimationTriggers()
        {
            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(true);
            }
        }

        private bool CheckIfNeedToWalkToMid()
        {
            var distance = Vector2.Distance(midLandingPosition.position, PlayerWalkController.Instance.PivotPoint.position);
            if (distance > 0.3f)
            {
                PlayerWalkController.Instance.MoveTowardMid(midLandingWalkingPosition, distance, SpawnNextPlatform);
                return true;
            }

            return false;
        }

        private void ResetLevelPlatform()
        {
            LevelManager.Instance.SetPlatformToRemove(this);
        }

        private void SpawnNextPlatform()
        {
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            StartCoroutine(StartSpawningPlatformCountdown());

            PlayerDragController.Instance.SetCanDrag();
        }

        private IEnumerator StartSpawningPlatformCountdown()
        {
            if (!isSamePlatformCollided)
            {
                yield return new WaitForSeconds(1f);
                LevelManager.Instance.SpawnNextPlatform(willTriggerCameraMove);
            }
        }
    }
}