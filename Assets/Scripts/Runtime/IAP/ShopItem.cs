using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.IAP
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _shopName;
        [SerializeField] private TextMeshProUGUI _shopDetails;
        [SerializeField] private TextMeshProUGUI _shopPrice;
        [SerializeField] private Button _shopBuyButton;

        private IAPData iapData;

        public void Init(IAPData iapData)
        {
            this.iapData = iapData;
            
            _shopName.text = iapData.ProductName;
            _shopDetails.text = iapData.ProductDetails;
            _shopPrice.text = iapData.ProductPrice + "$";
            
            _shopBuyButton.onClick.AddListener(OnClickBuy);
        }

        private void OnClickBuy()
        {
            ShopController.Instance.BuyIAP(iapData.ProductId);
        }
    }
}
