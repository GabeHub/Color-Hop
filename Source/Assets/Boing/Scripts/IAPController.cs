using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

#if UNITY_ANDROID
using Firebase;
using Firebase.Analytics;
#endif

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class IAPController : MonoBehaviour,IStoreListener
{
	private static IStoreController m_StoreController;
	// The Unity Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;
	// The store-specific Purchasing subsystems.

	//public GameObject screenHandler;
	public static string kProductIDComboLevels1_25 = "com.colorhop.combopack1";
	public static string kProductIDComboLevels25_50 = "com.colorhop.combopack2";
	public static string kProductIDLevelKey = "com.colorhop.levelskey";

	public static string kProductIDAds = "com.colorhop.ads";
	public GameObject purchasePopUp, btnComboLevel25;
	string NonConsumableProductID = "";
	string ProductIAPID = "";

	public void StartIAPInit ()
	{
		// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null) {
			// Begin to configure our connection to Purchasing
			InitializePurchasing ();
		}
		btnComboLevel25.SetActive (true);

		btnComboLevel25.GetComponent<Button> ().onClick.RemoveAllListeners ();
		if (PlayerPrefs.GetString ("levelCombo0_24") == "false") {
			btnComboLevel25.GetComponent<Button> ().onClick.AddListener (() => buyLevels ("levelCombo0_24"));
		} else if (PlayerPrefs.GetString ("levelCombo25_49") == "false" && PlayerPrefs.GetString ("levelCombo0_24") == "true") {
			btnComboLevel25.GetComponent<Button> ().onClick.AddListener (() => buyLevels ("levelCombo25_49"));
		} else if (PlayerPrefs.GetString ("levelCombo25_49") == "true") {
			btnComboLevel25.SetActive (false);
		}
	}

	public void InitializePurchasing ()
	{
		// If we have already connected to Purchasing ...
		if (IsInitialized ()) {
			// ... we are done here.
			return;
		}

		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

		// Add a product to sell / restore by way of its identifier, associating the general identifier
		// with its store-specific identifiers.
		builder.AddProduct (kProductIDComboLevels1_25, ProductType.NonConsumable);
		builder.AddProduct (kProductIDComboLevels25_50, ProductType.NonConsumable);
		builder.AddProduct (kProductIDLevelKey, ProductType.Consumable);
		builder.AddProduct (kProductIDAds, ProductType.NonConsumable);


//		foreach (KeyValuePair<int,string > pair in GetComponent<UIManager> ().kProductIDLevels) {
		//			builder.AddProduct (pair.Value, ProductType.NonConsumable);
//			Debug.Log (pair.Key + " " + pair.Value);
//		}
			
		

		UnityPurchasing.Initialize (this, builder);

	}

	private bool IsInitialized ()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void buyLevels (string ProductID)
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("IAP_Button", this.name);
		#endif
		
		ProductIAPID = ProductID;
		if (ProductIAPID == "levelCombo0_24") {
			NonConsumableProductID = kProductIDComboLevels1_25;
			BuyProductID (kProductIDComboLevels1_25);
		} else if (ProductIAPID == "levelCombo25_49") {
			NonConsumableProductID = kProductIDComboLevels25_50;
			BuyProductID (kProductIDComboLevels25_50);
		} else {
			NonConsumableProductID = kProductIDLevelKey;
			BuyProductID (NonConsumableProductID);
//			if (GetComponent<UIManager> ().kProductIDLevels.ContainsKey (int.Parse (ProductID))) {
//				string value = GetComponent<UIManager> ().kProductIDLevels [int.Parse (ProductID)];
//				Debug.Log (value);
//				NonConsumableProductID = value;
//				BuyProductID (NonConsumableProductID);
//			}

		} 
	}

	public void buyAdsFree ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("AdsFree_IAP_Button", this.name);
		#endif
		ProductIAPID = "Ads";
		NonConsumableProductID = kProductIDAds;
		BuyProductID (kProductIDAds);
	}


	public void BuyProductID (string ProductId)
	{
		// If Purchasing has been initialized ...
		if (IsInitialized ()) {
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = m_StoreController.products.WithID (ProductId);

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if (product != null && product.availableToPurchase) {
				//purchasePopUp.SetActive (true);
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				m_StoreController.InitiatePurchase (product);
			}
			// Otherwise ...
			else {
				purchasePopUp.SetActive (false);
				// ... report the product look-up failure situation  
				Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
		// Otherwise ...
		else {
			purchasePopUp.SetActive (false);
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			Debug.Log ("BuyProductID FAIL. Not initialized.");
		}
	}

	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases ()
	{
		// If Purchasing has not yet been set up ...
		if (!IsInitialized ()) {
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log ("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer ||
		    Application.platform == RuntimePlatform.OSXPlayer) {
			// ... begin restoring purchases
			Debug.Log ("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions> ();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions ((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log ("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		// Otherwise ...
		else {
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	// --- IStoreListener
	//

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		Debug.Log ("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed (InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		if (String.Equals (args.purchasedProduct.definition.id, NonConsumableProductID, StringComparison.Ordinal)) {
			Debug.Log (string.Format ("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			purchasePopUp.SetActive (false);
			Debug.Log (ProductIAPID.ToString ());
			if (ProductIAPID.IsNormalized ()) {
				GetComponent<UIManager> ().ds.ChangeLevelStatus (int.Parse (ProductIAPID), 0);
				GetComponent<UIManager> ().GetLevelsFromDB ();
			} else {
				PlayerPrefs.SetString (ProductIAPID, "true");
			}
			
			if (PlayerPrefs.GetString ("levelCombo0_24") == "true") {
				btnComboLevel25.SetActive (false);
				for (int LevelID = 1; LevelID < 25; LevelID++)
					GetComponent<UIManager> ().ds.ChangeLevelStatus (int.Parse (ProductIAPID), 0);
				GetComponent<UIManager> ().GetLevelsFromDB ();
			}

			if (PlayerPrefs.GetString ("levelCombo25_49") == "true") {
				btnComboLevel25.SetActive (false);
				for (int LevelID = 25; LevelID < 50; LevelID++)
					GetComponent<UIManager> ().ds.ChangeLevelStatus (int.Parse (ProductIAPID), 0);
				GetComponent<UIManager> ().GetLevelsFromDB ();
			}

			if (PlayerPrefs.GetString ("Ads") == "true") {
				//adsFree.SetActive (false);
			}
		} else {
			Debug.Log (string.Format ("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
			PlayerPrefs.SetString (ProductIAPID, "false");
		}

		//screenHandler.GetComponent<ScreenHandler> ().checkIAPLock ();

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.
		Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

		if (PlayerPrefs.GetString ("levelCombo25") == "false") {
			btnComboLevel25.SetActive (false);
		}
	}
}
