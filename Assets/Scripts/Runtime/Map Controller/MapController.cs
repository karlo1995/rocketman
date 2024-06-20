using DG.Tweening;
using Runtime.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Map_Controller
{
    public class MapController : MonoBehaviour
    {
        public static MapController Instance;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private MapItem[] _mapItems;
        [SerializeField] private Transform _mapIcon;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _canvasGroup.DOFade(0f, 0f);
        }

        public void OpenMap()
        {
            Debug.Log("1");
            if (true)//LevelManager.Instance.IsLastStage())
            {
                Debug.Log("2");

                SceneManager.LoadScene(4);
            }
            else if (LevelManager.Instance.IsNextStageBoss())
            {
                Debug.Log("3");

                var stageLevel = PlayerDataManager.Instance.CurrentStageLevel - 1;
                _mapItems[stageLevel].MoveRocketMan(_mapIcon, () =>
                {
                    SceneManager.LoadScene(3);
                });
            }
            else
            {
                Debug.Log("4");

                Debug.Log("Next Level");
                
                var stageLevel = PlayerDataManager.Instance.CurrentStageLevel - 1;
                _mapItems[stageLevel].MoveRocketMan(_mapIcon, DoneMapAnimation);
            }

            _canvasGroup.DOFade(1f, 0f);
        }

        private void DoneMapAnimation()
        {
            SceneController.Instance.ChangeScene(1);
        }
    }
}