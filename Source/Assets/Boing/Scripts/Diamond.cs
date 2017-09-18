using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Advertisements;

#if UNITY_ANDROID
using Firebase;
using Firebase.Analytics;
#endif

//переименовать/вынести
public class Diamond : MonoBehaviour
{
	public GameObject BtnWheel;
	public Text txtGems;
	bool isWheelRunning = false;
	public GameObject LockedImage;
	public float speed;
	int WheelGems = 0, oldGems = 0;

	void OnEnable ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("Wheel_Of_Fortune_Screen", this.name);
		#endif
		BtnWheel.GetComponent<Button>().enabled = true;
		BtnWheel.GetComponent<ConstantForce>().enabled = false;
		isWheelRunning = false;
		BtnWheel.GetComponent<RectTransform>().rotation = Quaternion.Euler (Vector3.zero);
		oldGems = Util.GetDiamond();
		txtGems.text = oldGems.ToString ();
		LockedImage.SetActive (true);
	}

	void FixedUpdate ()
	{
		if (isWheelRunning) {
			if (BtnWheel.GetComponent<ConstantForce>().torque.z > 0) {
				BtnWheel.GetComponent<ConstantForce>().torque = new Vector3 (0, 0, BtnWheel.GetComponent<ConstantForce> ().torque.z - speed);
			} else {
				StartCoroutine ("UpdateGems");
			}
		}
	}

	public void PressWheelButtn ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("Wheel_Of_Fortune_Button", this.name);
		#endif
		BtnWheel.GetComponent<Button>().enabled = false;
		BtnWheel.GetComponent<ConstantForce>().enabled = true;
		BtnWheel.GetComponent<ConstantForce>().torque = new Vector3 (0, 0, UnityEngine.Random.Range (700, 1400));
		isWheelRunning = true;
	}

	void OnTriggerEnter (Collider other)
	{
		Debug.Log (other.gameObject.transform.parent.name);
		WheelGems = int.Parse (other.gameObject.transform.parent.name);
	}

	IEnumerator UpdateGems ()
	{
        //разобраться с паузами
		isWheelRunning = false;
		BtnWheel.GetComponent<ConstantForce> ().torque = Vector3.zero;

		yield return new WaitForSeconds (2f);

		for (int i = 1; i <= WheelGems; i++) {
			txtGems.text = (oldGems + i).ToString ();
			yield return new WaitForSeconds (0.5f);
		}
		oldGems = int.Parse (txtGems.text);

		yield return new WaitForSeconds (0.1f);

		Util.SetDiamond (oldGems);

		yield return new WaitForSeconds (0.1f);

		GameObject.Find ("_GameOverPanel").GetComponent<GameOver> ().UpdateScore ();

		LockedImage.SetActive (true);

		StopCoroutine ("UpdateGems");

	}

	public void ShowAds ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("Wheel_Of_Fortune_VideoAds_Button", this.name);
		#endif
		if (Advertisement.IsReady ("rewardedVideo")) {
			Debug.Log ("Ads");
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		}
	}


	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			LockedImage.SetActive (false);
			if (WheelGems == 0) {
				WheelGems = 2;
				StartCoroutine ("UpdateGems");
			}
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			break;
		}
	}
}
