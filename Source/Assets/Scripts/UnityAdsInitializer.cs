using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
using System;
using ChartboostSDK;

public class UnityAdsInitializer : MonoBehaviour
{
	[SerializeField]
	private string
		androidGameId = "1482913",
		iosGameId = "1482914";

	private bool hasInterstitial = false;
	private bool autocache = true;
	private bool showInterstitial = true;
	private int frameCount = 0;


	BannerView bannerView;
	InterstitialAd interstitial;
	bool isBannerLoaded = false;

	[SerializeField]
	private bool testMode;

	void OnEnable ()
	{
		SetupDelegates ();
	}

	void Start ()
	{

		Debug.Log ("Is Initialized: " + Chartboost.isInitialized ());
//
//		if (!Chartboost.isInitialized ()) {
//			Chartboost.Create ();
//		}
		Chartboost.setAutoCacheAds (autocache);

		string gameId = null;

		//Unity Ads

		#if UNITY_ANDROID
		gameId = androidGameId;
		#elif UNITY_IOS
		gameId = iosGameId;
		#endif

		if (Advertisement.isSupported && !Advertisement.isInitialized) {
			Advertisement.Initialize (gameId, testMode);
		}

		ShowInterstitial ();

		RequestBannerView ();
	}

	void SetupDelegates ()
	{
		// Listen to all impression-related events
		Chartboost.didInitialize += didInitialize;
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
	
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
		#endif
	}

	void OnDisable ()
	{
		// Listen to all impression-related events
		Chartboost.didInitialize -= didInitialize;
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;

		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
		#endif
	}

	private Vector2 beginFinger;
	// finger
	private float deltaFingerY;
	// finger
	private Vector2 beginPanel;
	// scrollpanel
	private Vector2 latestPanel;
	// scrollpanel

	void Update ()
	{
//		UpdateScrolling ();

		frameCount++;
		if (frameCount > 30) {
			// update these periodically and not every frame
			hasInterstitial = Chartboost.hasInterstitial (CBLocation.Default);
			frameCount = 0;
		}
	}



	public void RequestBannerView ()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-6779975111022031/1713852566";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-6779975111022031/4778999604";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		BannerView bannerView = new BannerView (adUnitId, AdSize.Banner, AdPosition.Top);

		// Called when an ad request has successfully loaded.
		bannerView.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
	
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the banner with the request.
		bannerView.LoadAd (request);
	}



	public void removeBanner ()
	{
		if (isBannerLoaded)
			bannerView.Hide ();
	}



	public void HandleOnAdLoaded (object sender, EventArgs args)
	{
		isBannerLoaded = true;
		Debug.Log ("OnAdLoaded event received.");
		// Handle the ad loaded event.
	}

	public void HandleOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		isBannerLoaded = false;
		Debug.Log ("Interstitial Failed to load: " + args.Message);
		// Handle the ad failed to load event.
	}

	public void ShowBanner ()
	{
		if (PlayerPrefs.GetString ("Ads") == "false")
			bannerView.Show ();
	}

	public void ShowInterstitial ()
	{
		Debug.Log ("IsInitialized :" + Chartboost.isInitialized ());
		Chartboost.showInterstitial (CBLocation.Default);
		Chartboost.cacheInterstitial (CBLocation.Default);
	}

	void didInitialize (bool status)
	{
		Debug.Log (string.Format ("didInitialize: {0}", status));
	}

	void didFailToLoadInterstitial (CBLocation location, CBImpressionError error)
	{
		Debug.Log (string.Format ("didFailToLoadInterstitial: {0} at location {1}", error, location));
	}

	void didDismissInterstitial (CBLocation location)
	{
		Debug.Log ("didDismissInterstitial: " + location);
	}

	void didCloseInterstitial (CBLocation location)
	{
		Debug.Log ("didCloseInterstitial: " + location);
	}

	void didClickInterstitial (CBLocation location)
	{
		Debug.Log ("didClickInterstitial: " + location);
	}

	void didCacheInterstitial (CBLocation location)
	{
		Debug.Log ("didCacheInterstitial: " + location);
	}

	bool shouldDisplayInterstitial (CBLocation location)
	{
		// return true if you want to allow the interstitial to be displayed
		Debug.Log ("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
		return showInterstitial;
	}

	void didDisplayInterstitial (CBLocation location)
	{
		Debug.Log ("didDisplayInterstitial: " + location);
	}

	#if UNITY_IPHONE
	void didCompleteAppStoreSheetFlow ()
	{
		Debug.Log ("didCompleteAppStoreSheetFlow");
	}

	//		void TrackIAP() {
	//		// The iOS receipt data from Unibill is already base64 encoded.
	//		// Others store kit plugins may be different.
	//		// This is a sample sandbox receipt.
	//		string sampleReceipt = @"ewoJInNpZ25hdHVyZSIgPSAiQXBNVUJDODZBbHpOaWtWNVl0clpBTWlKUWJLOEVk
	//		ZVhrNjNrV0JBWHpsQzhkWEd1anE0N1puSVlLb0ZFMW9OL0ZTOGNYbEZmcDlZWHQ5
	//		aU1CZEwyNTBsUlJtaU5HYnloaXRyeVlWQVFvcmkzMlc5YVIwVDhML2FZVkJkZlcr
	//		T3kvUXlQWkVtb05LeGhudDJXTlNVRG9VaFo4Wis0cFA3MHBlNWtVUWxiZElWaEFB
	//		QURWekNDQTFNd2dnSTdvQU1DQVFJQ0NHVVVrVTNaV0FTMU1BMEdDU3FHU0liM0RR
	//		RUJCUVVBTUg4eEN6QUpCZ05WQkFZVEFsVlRNUk13RVFZRFZRUUtEQXBCY0hCc1pT
	//		QkpibU11TVNZd0pBWURWUVFMREIxQmNIQnNaU0JEWlhKMGFXWnBZMkYwYVc5dUlF
	//		RjFkR2h2Y21sMGVURXpNREVHQTFVRUF3d3FRWEJ3YkdVZ2FWUjFibVZ6SUZOMGIz
	//		SmxJRU5sY25ScFptbGpZWFJwYjI0Z1FYVjBhRzl5YVhSNU1CNFhEVEE1TURZeE5U
	//		SXlNRFUxTmxvWERURTBNRFl4TkRJeU1EVTFObG93WkRFak1DRUdBMVVFQXd3YVVI
	//		VnlZMmhoYzJWU1pXTmxhWEIwUTJWeWRHbG1hV05oZEdVeEd6QVpCZ05WQkFzTUVr
	//		RndjR3hsSUdsVWRXNWxjeUJUZEc5eVpURVRNQkVHQTFVRUNnd0tRWEJ3YkdVZ1NX
	//		NWpMakVMTUFrR0ExVUVCaE1DVlZNd2daOHdEUVlKS29aSWh2Y05BUUVCQlFBRGdZ
	//		MEFNSUdKQW9HQkFNclJqRjJjdDRJclNkaVRDaGFJMGc4cHd2L2NtSHM4cC9Sd1Yv
	//		cnQvOTFYS1ZoTmw0WElCaW1LalFRTmZnSHNEczZ5anUrK0RyS0pFN3VLc3BoTWRk
	//		S1lmRkU1ckdYc0FkQkVqQndSSXhleFRldngzSExFRkdBdDFtb0t4NTA5ZGh4dGlJ
	//		ZERnSnYyWWFWczQ5QjB1SnZOZHk2U01xTk5MSHNETHpEUzlvWkhBZ01CQUFHamNq
	//		QndNQXdHQTFVZEV3RUIvd1FDTUFBd0h3WURWUjBqQkJnd0ZvQVVOaDNvNHAyQzBn
	//		RVl0VEpyRHRkREM1RllRem93RGdZRFZSMFBBUUgvQkFRREFnZUFNQjBHQTFVZERn
	//		UVdCQlNwZzRQeUdVakZQaEpYQ0JUTXphTittVjhrOVRBUUJnb3Foa2lHOTJOa0Jn
	//		VUJCQUlGQURBTkJna3Foa2lHOXcwQkFRVUZBQU9DQVFFQUVhU2JQanRtTjRDL0lC
	//		M1FFcEszMlJ4YWNDRFhkVlhBZVZSZVM1RmFaeGMrdDg4cFFQOTNCaUF4dmRXLzNl
	//		VFNNR1k1RmJlQVlMM2V0cVA1Z204d3JGb2pYMGlreVZSU3RRKy9BUTBLRWp0cUIw
	//		N2tMczlRVWU4Y3pSOFVHZmRNMUV1bVYvVWd2RGQ0TndOWXhMUU1nNFdUUWZna1FR
	//		Vnk4R1had1ZIZ2JFL1VDNlk3MDUzcEdYQms1MU5QTTN3b3hoZDNnU1JMdlhqK2xv
	//		SHNTdGNURXFlOXBCRHBtRzUrc2s0dHcrR0szR01lRU41LytlMVFUOW5wL0tsMW5q
	//		K2FCdzdDMHhzeTBiRm5hQWQxY1NTNnhkb3J5L0NVdk02Z3RLc21uT09kcVRlc2Jw
	//		MGJzOHNuNldxczBDOWRnY3hSSHVPTVoydG04bnBMVW03YXJnT1N6UT09IjsKCSJw
	//		dXJjaGFzZS1pbmZvIiA9ICJld29KSW05eWFXZHBibUZzTFhCMWNtTm9ZWE5sTFdS
	//		aGRHVXRjSE4wSWlBOUlDSXlNREV5TFRBMExUTXdJREE0T2pBMU9qVTFJRUZ0WlhK
	//		cFkyRXZURzl6WDBGdVoyVnNaWE1pT3dvSkltOXlhV2RwYm1Gc0xYUnlZVzV6WVdO
	//		MGFXOXVMV2xrSWlBOUlDSXhNREF3TURBd01EUTJNVGM0T0RFM0lqc0tDU0ppZG5K
	//		eklpQTlJQ0l5TURFeU1EUXlOeUk3Q2draWRISmhibk5oWTNScGIyNHRhV1FpSUQw
	//		Z0lqRXdNREF3TURBd05EWXhOemc0TVRjaU93b0pJbkYxWVc1MGFYUjVJaUE5SUNJ
	//		eElqc0tDU0p2Y21sbmFXNWhiQzF3ZFhKamFHRnpaUzFrWVhSbExXMXpJaUE5SUNJ
	//		eE16TTFOems0TXpVMU9EWTRJanNLQ1NKd2NtOWtkV04wTFdsa0lpQTlJQ0pqYjIw
	//		dWJXbHVaRzF2WW1Gd2NDNWtiM2R1Ykc5aFpDSTdDZ2tpYVhSbGJTMXBaQ0lnUFNB
	//		aU5USXhNVEk1T0RFeUlqc0tDU0ppYVdRaUlEMGdJbU52YlM1dGFXNWtiVzlpWVhC
	//		d0xrMXBibVJOYjJJaU93b0pJbkIxY21Ob1lYTmxMV1JoZEdVdGJYTWlJRDBnSWpF
	//		ek16VTNPVGd6TlRVNE5qZ2lPd29KSW5CMWNtTm9ZWE5sTFdSaGRHVWlJRDBnSWpJ
	//		d01USXRNRFF0TXpBZ01UVTZNRFU2TlRVZ1JYUmpMMGROVkNJN0Nna2ljSFZ5WTJo
	//		aGMyVXRaR0YwWlMxd2MzUWlJRDBnSWpJd01USXRNRFF0TXpBZ01EZzZNRFU2TlRV
	//		Z1FXMWxjbWxqWVM5TWIzTmZRVzVuWld4bGN5STdDZ2tpYjNKcFoybHVZV3d0Y0hW
	//		eVkyaGhjMlV0WkdGMFpTSWdQU0FpTWpBeE1pMHdOQzB6TUNBeE5Ub3dOVG8xTlNC
	//		RmRHTXZSMDFVSWpzS2ZRPT0iOwoJImVudmlyb25tZW50IiA9ICJTYW5kYm94IjsK
	//		CSJwb2QiID0gIjEwMCI7Cgkic2lnbmluZy1zdGF0dXMiID0gIjAiOwp9";
	//
	//		// Demonstrate Base64 encoding. Not necessary for the data above
	//		// If the receipt was not base64 encoded, send encodedText not sampleReceipt
	//		//byte[] bytesToEncode = Encoding.UTF8.GetBytes(sampleReceipt);
	//		//string encodedText = Convert.ToBase64String(bytesToEncode);
	//
	//		// Send the receipt for track an In App Purchase Event
	//		Chartboost.trackInAppAppleStorePurchaseEvent(sampleReceipt,
	//		"sample product title", "sample product description", "1.99", "USD", "sample product identifier" );
	//		//byte[] decodedText = Convert.FromBase64String(sampleReceipt);
	//		//Debug.Log("Decoded: " + System.Text.Encoding.UTF8.GetString(decodedText));
	//		//Debug.Log("Encoded: " + encodedText);
	//		}
		


	//#elif UNITY_ANDROID
	//	void TrackIAP ()
	//	{
	//		Debug.Log ("TrackIAP");
	//		// title, description, price, currency, productID, purchaseData, purchaseSignature
	//		// This data should be sent after handling the results from the google store.
	//		// This is fake data and doesn't represent a real or even imaginary purchase
	//		Chartboost.trackInAppGooglePlayPurchaseEvent ("SampleItem", "TestPurchase", "0.99", "USD", "ProductID", "PurchaseData", "PurchaseSignature");
	//
	//	}
	//	#else
	//		void TrackIAP() {
	//		Debug.Log("TrackIAP on unsupported platform");
	//		}
	#endif
}