using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANDROID
using Firebase;
using Firebase.Analytics;
#endif
#if ANDROID

public class FirebaseManager : MonoBehaviour
{


	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	void OnEnable ()
	{
		dependencyStatus = FirebaseApp.CheckDependencies ();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync ().ContinueWith (task => {
				dependencyStatus = FirebaseApp.CheckDependencies ();
				if (dependencyStatus == DependencyStatus.Available) {
					InitializeFirebase ();
				} else {
					Debug.LogError (
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase ();
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Handle initialization of the necessary firebase modules:
	void InitializeFirebase ()
	{
		Debug.Log ("Enabling data collection.");
		FirebaseAnalytics.SetAnalyticsCollectionEnabled (true);

		Debug.Log ("Set user properties.");
		// Set the user's sign up method.
		FirebaseAnalytics.SetUserProperty (
			FirebaseAnalytics.UserPropertySignUpMethod,
			"Google");
		// Set the user ID.
		FirebaseAnalytics.SetUserId ("colorhop_user_");
	}

	public void SetCurrentScreen (string screenName)
	{
		FirebaseAnalytics.SetCurrentScreen (screenName, this.name);
	}
		
}
#endif