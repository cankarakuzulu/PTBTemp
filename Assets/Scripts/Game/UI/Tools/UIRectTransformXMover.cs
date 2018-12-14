using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Game.UI.Tools
{
	public class UIRectTransformXMover : MonoBehaviour {

		[SerializeField] protected float iphoneXOffset = 2.5f;
		[SerializeField] protected float iphoneXRatioLimit = 1.8f;
		private RectTransform xForm;

		void Awake()
		{
			xForm = GetComponent<RectTransform>();
		}
	
		void Start ()
		{
			var ratio = Screen.height / Screen.width;
			if (ratio >=iphoneXRatioLimit)
			{
				xForm.anchoredPosition -= Vector2.up * iphoneXOffset;
			}
		}
	}
}
