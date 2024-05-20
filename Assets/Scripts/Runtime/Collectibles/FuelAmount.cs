using DG.Tweening;
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
            gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 0.3f);
        }
    }
}
