using UnityEngine;

namespace Runtime.IAP
{
    [CreateAssetMenu(fileName = "New IAP Data", menuName = "Scriptable Object / IAP Data")]
    public class IAPData : ScriptableObject
    {
        [SerializeField] private string productId;
        [SerializeField] private string productName;
        [SerializeField] private string productDetails;
        [SerializeField] private float productPrice;
        [SerializeField] private RewardType rewardType;

        public string ProductId => productId;
        public string ProductName => productName;
        public string ProductDetails => productDetails;
        public float ProductPrice => productPrice;
        public RewardType RewardType => rewardType;
    }

    public enum RewardType
    {
        AdRemove,
        Lives,
        
    }
}
