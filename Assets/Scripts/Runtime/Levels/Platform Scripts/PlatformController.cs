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
        
        private bool isSamePlatformCollided;
        private bool isActive;

        private TestPlatformData currentTestPlatformData;
        
        private bool isCollapsingPlatform;
        public bool IsCollapsingPlatform => isCollapsingPlatform;

        private CollapsingPlatform collapsingPlatform;
        public CollapsingPlatform CollapsingPlatform => collapsingPlatform;

        private BoxCollider2D boxCollider2D;
        
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
            
            //check if platform is collapsing platform
            if (gameObject.TryGetComponent(out CollapsingPlatform collapsingPlatform))
            {
                isCollapsingPlatform = true;
                this.collapsingPlatform = collapsingPlatform;
            }

            if (gameObject.TryGetComponent(out BoxCollider2D boxCollider2D))
            {
                this.boxCollider2D = boxCollider2D;
            }

            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(false);
            }
        }

        public void InitPlatform(TestPlatformData currentTestPlatformData)
        {
            this.currentTestPlatformData = currentTestPlatformData;
            boxCollider2D.enabled = true;
            
            transform.DOMove(currentTestPlatformData.PlatformPosition, 0f).OnComplete(SpawnPlatform).SetUpdate(true);
            
            foreach (var trigger in platformTriggers)
            {
                trigger.SetActive(true);
            }
        }

        public string GetCameraIdToUse()
        {
            return currentTestPlatformData.CameraIdToUse;
        }

        public Vector2 GetSpawnPosition()
        {
            return currentTestPlatformData.GetSpawnPosition();
        }
        
        public Vector2 GetLaunchPosition()
        {
            return currentTestPlatformData.GetLaunchPosition();
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
                gameObject.SetActive(true);
                spriteRenderer.DOFade(1f, 0.3f).OnComplete(() => { gameObject.SetActive(true); }).SetUpdate(true);
            }
            else if(meshRenderer != null)
            {
                gameObject.SetActive(true);
                gameObject.GetComponent<MeshRenderer>().materials[0].DOFade(1f, 0.3f).OnComplete(() =>
                {   
                    //check if platform is collapsing platform
                    if (isCollapsingPlatform)
                    {
                        if (gameObject.TryGetComponent(out CollapsingPlatform collapsingPlatform))
                        {
                            collapsingPlatform.ResetCollapsingPlatform();
                        }
                    }
                }).SetUpdate(true);
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

            if (currentTestPlatformData.ShouldRemoveColliderUponExit)
            {
                boxCollider2D.enabled = false;
            }
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

        private void SpawnNextPlatform()
        {
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

            if (isCollapsingPlatform)
            {
                collapsingPlatform.CallCollapsingFunction();
            }
            
            StartCoroutine(StartSpawningPlatformCountdown());

            PlayerDragController.Instance.SetCanDrag();
        }

        private IEnumerator StartSpawningPlatformCountdown()
        {
            if (!isSamePlatformCollided)
            {
                yield return new WaitForSeconds(0.3f);
                LevelManager.Instance.SpawnNextPlatform(this);
            }
        }

        public void ResetPlayer()
        {
            if (currentTestPlatformData.ShouldRemoveColliderUponExit)
            {
                boxCollider2D.enabled = true;
            }
        }
    }
}