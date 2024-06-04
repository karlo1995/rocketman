using DG.Tweening;
using Runtime.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Collectibles
{
    public class FuelAmount : MonoBehaviour
    {
        [SerializeField] private float additionalFuelAmount;
        public float AdditionalFuelAmount => additionalFuelAmount;
        
        public void CollidedToPlayer()
        {
            gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 0.3f).OnComplete(() =>
            {
                var item = gameObject.GetComponent<CollectibleItem>();
                var data = item.Data;
            
                LevelManager.Instance.AddCollectibleData(data);
                gameObject.SetActive(false);
            });
        }
    }
}
