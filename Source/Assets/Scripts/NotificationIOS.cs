using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationIOS : MonoBehaviour
{
	#if UNITY_IOS
	void RegisterForNotif ()
	{
		UnityEngine.iOS.NotificationServices.RegisterForNotifications (UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
	}

	void ScheduleNotification ()
	{
		UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification ();

		notif.fireDate = DateTime.Now.AddHours (24);

		notif.alertBody = "Color Hop contest \nThanks for playing Color Hop.\nPlay Color Hop every day of this week, get a minimum of 50 score and be added to the monthly giveaway of an iPad. Score 50... Share your highest score on Instagram with #ColorHop to stand a chance of winning an iPad";

		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification (notif);

	}

	void OnApplicationPause (bool isPause)
	{
		if (isPause) { // App going to background
			// cancel all notifications first.

			UnityEngine.iOS.NotificationServices.ClearLocalNotifications ();

			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications ();

			ScheduleNotification ();


		}
	}
	#endif
}
