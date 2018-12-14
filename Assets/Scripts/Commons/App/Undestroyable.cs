/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using UnityEngine;

namespace nopact.Commons.App
{
	public class Undestroyable : MonoBehaviour {

		private static Undestroyable instance;

		void Awake() {
		
			if (instance == null)
			{
				DontDestroyOnLoad(gameObject);
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(gameObject);	
			}
		
		}
	
	}
}
