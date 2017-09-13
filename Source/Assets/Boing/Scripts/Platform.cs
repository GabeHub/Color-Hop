
/***********************************************************************************************************
	 * Produced by App Advisory - http://app-advisory.com													   *
	 * Facebook: https://facebook.com/appadvisory															   *
	 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
	 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
	 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
	 ***********************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//myFix
using DG.Tweening;

#if AADOTWEEN
using DG.Tweening;
#endif


public class Platform : MonoBehaviour
{
	GameManager gameManager;

	public Transform jumpEffect;

	public Transform cube;

	public GameObject diamond;

	public GameObject alphabet;

	public bool isDouble = false;

	Platform t, t1;

	int lenEndlessWorld = 0, lenLevelWord = 0;

	void OnEnable ()
	{
		this.gameManager = FindObjectOfType<GameManager> ();
		jumpEffect.transform.localScale = Vector3.zero;
		jumpEffect.gameObject.SetActive (false);
		diamond.SetActive (false);
		alphabet.SetActive (false);

	}

	void Start ()
	{
		StartCoroutine (CoroutDestroyIfNotVisible ());
//		if (this.transform.parent) {
//			if (this.transform.parent.tag == "PlatformDouble") {
		if (isDouble == true) {
			t = this.transform.parent.GetComponentsInChildren<Platform> () [0];
			t1 = this.transform.parent.GetComponentsInChildren<Platform> () [1];
		}
		//}
	}

	public void OnPlayerBounce ()
	{
		jumpEffect.transform.localScale = Vector3.zero;
		jumpEffect.gameObject.SetActive (true);

        //myFix
        jumpEffect.DOScale (new Vector3 (3f, 0.3f, 3f), 0.2f).SetLoops (2, LoopType.Yoyo);

		#if AADOTWEEN
		jumpEffect.DOScale (new Vector3 (3f, 0.3f, 3f), 0.2f).SetLoops (2, LoopType.Yoyo);
		#endif

		if (diamond.activeInHierarchy) {
			gameManager.Add1Diamond ();
			diamond.SetActive (false);
		}

		if (alphabet.activeInHierarchy) {
			gameManager.Add1Alphabet (alphabet.GetComponent<TextMesh> ().text);
			alphabet.SetActive (false);
		}
	}

	IEnumerator CoroutDestroyIfNotVisible ()
	{
		if (gameManager.point > 0) {
			if (!isDouble)
				transform.position = new Vector3 (transform.position.x, 0f, transform.position.z);
			else
				transform.position = new Vector3 (transform.position.x, 0f, transform.position.z);

            //myFix
			transform.DOMoveY (0, 0.2f);

			#if AADOTWEEN
			transform.DOMoveY (0, 0.2f);
			#endif
		}

		if (GameData.CurrentGameMode == 0) {
			if (gameManager.point > 10 && Util.GetRandomNumber (0f, 100f) < 10f) {
				if (!alphabet.activeInHierarchy)
					CallGems ();
			}
				
			if (gameManager.point > 15 && Util.GetRandomNumber (0f, 100f) < 10f) {
				if (!diamond.activeInHierarchy) {
					if (GameData.FilledEndlessWord != GameData.EndlessWord)
						CallAlphabets (GameData.EndlessWord, lenEndlessWorld);
				}
			}

		} else {
			if (gameManager.point > 8 && Util.GetRandomNumber (0f, 100f) < 30f) {
				if (!alphabet.activeInHierarchy)
					CallGems ();
			}

			if (gameManager.point > 10/*% 4 == 0*/ && Util.GetRandomNumber(0f, 100f) < 30f) {
                if (!diamond.activeInHierarchy)
                {
                    if (GameData.LevelWord.Length != GameData.FilledLevelWord.Length)
                    {
                        CallAlphabets(GameData.LevelWord, lenLevelWord);
                    }
                }
			}
		}

		yield return new WaitForSeconds (1f);

		while (true) {
			lenEndlessWorld = GameData.FilledEndlessWord.Length;
			lenLevelWord = GameData.FilledLevelWord.Length;

			if (GameData.CurrentGameMode == 1) {
				if (alphabet.activeSelf && GameData.FilledLevelWord.Contains (alphabet.GetComponent<TextMesh> ().text))
					alphabet.SetActive (false);
			} else {
				if (alphabet.activeSelf && GameData.FilledEndlessWord.Contains (alphabet.GetComponent<TextMesh> ().text))
					alphabet.SetActive (false);
			}

			if (gameManager.playerParent.transform.localPosition.z - this.transform.localPosition.z > 5) {
				DODestroy ();
				break;
			}
			

			yield return new WaitForSeconds (0.3f);
		}
	}

	void CallGems ()
	{
		if (isDouble) {
            if (!t.gameObject.transform.Find("Diamond").gameObject.activeInHierarchy && !t1.gameObject.transform.Find("Diamond").gameObject.activeInHierarchy)
            {
                int index = UnityEngine.Random.Range(0, 2);
                if (index == 0)
                {
                    if (!t.gameObject.transform.Find("Alphabet").gameObject.activeInHierarchy)
                    {
                        t.gameObject.transform.Find("Diamond").gameObject.SetActive(true);
                        t1.gameObject.transform.Find("Diamond").gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!t1.gameObject.transform.Find("Alphabet").gameObject.activeInHierarchy)
                    {
                        t.gameObject.transform.Find("Diamond").gameObject.SetActive(false);
                        t1.gameObject.transform.Find("Diamond").gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            diamond.SetActive(true);
        }
    }

	void CallAlphabets (string alphabetString, int CharIndex)
	{
		Debug.Log ("@@@ :" + GameData.FilledLevelWord);
	
		if (isDouble)
        {
            if (!t.gameObject.transform.Find("Alphabet").gameObject.activeInHierarchy && !t1.gameObject.transform.Find("Alphabet").gameObject.activeInHierarchy)
            {
                int index = UnityEngine.Random.Range(0, 2);
                if (index == 0)
                {
                    if (!t.gameObject.transform.Find("Diamond").gameObject.activeInHierarchy)
                    {
                        t.gameObject.transform.Find("Alphabet").gameObject.SetActive(true);
                        t1.gameObject.transform.Find("Alphabet").gameObject.SetActive(false);
                        if (t.gameObject.transform.Find("Alphabet").gameObject.GetComponent<TextMesh>())
                            t.gameObject.transform.Find("Alphabet").gameObject.GetComponent<TextMesh>().text = alphabetString.ToCharArray()[CharIndex].ToString();
                    }
                }
                else
                {
                    if (!t1.gameObject.transform.Find("Diamond").gameObject.activeInHierarchy)
                    {
                        t.gameObject.transform.Find("Alphabet").gameObject.SetActive(false);
                        t1.gameObject.transform.Find("Alphabet").gameObject.SetActive(true);
                        if (t1.gameObject.transform.Find("Alphabet").gameObject.GetComponent<TextMesh>())
                            t1.gameObject.transform.Find("Alphabet").gameObject.GetComponent<TextMesh>().text = alphabetString.ToCharArray()[CharIndex].ToString();
                    }
                }
            }

        } else {
//			if (alphabet.GetComponent<TextMesh> ())
			Debug.Log ("!!! :" +	alphabetString + " :" + CharIndex.ToString ());

			alphabet.GetComponent<TextMesh> ().text = alphabetString.ToCharArray () [CharIndex].ToString ();

		}

	}

	void DODestroy ()
	{
		StopAllCoroutines ();
		if (!isDouble) {
			gameManager.SpawnPlatform (this.transform, null, false);
		}
		//else {
//			gameManager.SpawnPlatform (t.gameObject.transform, t1.gameObject.transform, true);
//		}
		StartCoroutine (CoroutDestroyIfNotVisible ());
	}
}
