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
	public class AppInfo : ScriptableObject
	{
		public string appId;
		public string version;
		public string serverUrl;
		public string secret;
		public string csk;
	}
}
