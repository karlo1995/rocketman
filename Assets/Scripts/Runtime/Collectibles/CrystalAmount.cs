using DG.Tweening;
using UnityEngine;

public class CrystalAmount : MonoBehaviour
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
            item.SetActive(false, true);
        });
    }
}
