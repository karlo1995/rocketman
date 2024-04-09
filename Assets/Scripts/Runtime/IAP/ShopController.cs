using System;
using System.Collections.Generic;
using Runtime.Manager;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace Runtime.IAP
{
    public class ShopController : MonoBehaviour, IDetailedStoreListener
    {
        public static ShopController Instance;
        [SerializeField] private int _iapCount;
        [SerializeField] private ShopItem[] _shopItems;

        private List<IAPData> _iapData = new();
        IStoreController _storeController; // The Unity Purchasing system.

        //Your products IDs. They should match the ids of your products in your store.
        public string goldProductId = "com.mycompany.mygame.gold1";
        public string diamondProductId = "com.mycompany.mygame.diamond1";

        public Text GoldCountText;
        public Text DiamondCountText;

        int m_GoldCount;
        int m_DiamondCount;

        private void Awake()
        {
            Instance = this;

            for (var i = 0; i < _iapCount; i++)
            {
                var iapDataName = "IAP Data/IAP Data " + i;
                var iapData = Resources.Load<IAPData>(iapDataName);
                _iapData.Add(iapData);
            }

        }

        private void Start()
        {
            InitializePurchasing();
            UpdateShopItems();
        }

        private void UpdateShopItems()
        {
            for (var i = 0; i < _iapData.Count; i++)
            {
                _shopItems[i].Init(_iapData[i]);
            }
        }

        private void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
           // builder.AddProduct(goldProductId, ProductType.Consumable);
           // builder.AddProduct(diamondProductId, ProductType.Consumable);

            foreach (var iap in _iapData)
            {
                builder.AddProduct(iap.ProductId, ProductType.Consumable);
            }
            
            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyIAP(string id)
        {
            _storeController.InitiatePurchase(id);
        }
        
        // public void BuyGold()
        // {
        //     _storeController.InitiatePurchase(goldProductId);
        // }
        //
        // public void BuyDiamond()
        // {
        //     _storeController.InitiatePurchase(diamondProductId);
        // }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            _storeController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            Debug.Log("Successfully Purchased IAP: " + product.definition.id);

            //Add the purchased product to the players inventory
            if (product.definition.id == goldProductId)
            {
                AddGold();
            }
            else if (product.definition.id == diamondProductId)
            {
                AddDiamond();
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        private void ProcessPurchaseSuccessful(string id)
        {
            var iap = GetIAPDataById(id);
            
            switch (iap.RewardType)
            {
                case RewardType.AdRemove:
                    PlayerDataManager.Instance.RemoveAds();
                    break;
                case RewardType.Lives:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IAPData GetIAPDataById(string id)
        {
            foreach (var iap in _iapData)
            {
                if (iap.ProductId.Equals(id))
                {
                    return iap;
                }
            }

            Debug.Log("Wrong IAP ID!");
            return null;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        void AddGold()
        {
            m_GoldCount++;
            UpdateUI();
        }

        void AddDiamond()
        {
            m_DiamondCount++;
            UpdateUI();
        }

        void UpdateUI()
        {
            GoldCountText.text = $"Your Gold: {m_GoldCount}";
            DiamondCountText.text = $"Your Diamonds: {m_DiamondCount}";
        }
    }
}
