
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

namespace AppAdvisory.Boing
{
	public class SmoothCamera2D : MonoBehaviour {

		float speed = 5f;
		private Vector3 velocity = Vector3.one * 1;
		public Transform target;
		public float y;
		public float z;

		Camera cam;

		void Awake()
		{
			cam = GetComponent<Camera>();

			y = transform.position.y;

			z = transform.position.z;
		}

		// Update is called once per frame
		void Update () 
		{
			if (target)
			{
				Vector3 point = cam.WorldToViewportPoint(target.position);
				Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));

				Vector3 destination = transform.position + delta;
//				Vector3 pos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
				Vector3 pos = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);

				float posX = pos.x;

				if(posX < -1f)
					posX = -1f;

				if(posX < +1f)
					posX = +1f;

				transform.position = new Vector3(pos.x, y, target.position.z + z) ;
			}

		}
	}
}