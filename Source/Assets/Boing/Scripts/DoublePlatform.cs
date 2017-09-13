using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePlatform : MonoBehaviour
{
	GameManager gameManager;
	Platform[] gameobjects;
	// Use this for initialization
	void OnEnable ()
	{
		this.gameManager = FindObjectOfType<GameManager> ();
		StartCoroutine (CoroutDestroyIfNotVisible ());
		gameobjects = this.GetComponentsInChildren<Platform> ();
		gameobjects [0].isDouble = true;
		gameobjects [1].isDouble = true;
	}

	IEnumerator CoroutDestroyIfNotVisible ()
	{
		yield return new WaitForSeconds (1f);

		while (true) {
	
			if (gameManager.playerParent.transform.localPosition.z - this.transform.localPosition.z > 5) {
				DODestroy ();
				break;
			}
			yield return new WaitForSeconds (0.3f);
		}
	}

	void DODestroy ()
	{
		StopAllCoroutines ();

		gameManager.SpawnPlatform (gameobjects [0].gameObject.transform, gameobjects [1].gameObject.transform, true);

		StartCoroutine (CoroutDestroyIfNotVisible ());
	}
}
