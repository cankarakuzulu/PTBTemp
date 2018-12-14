/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;
using nopact.Commons.Utility.Math;

namespace nopact.Commons.Utility
{
	public class NopactUtility
	{
		public static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static DateTime epoch = new DateTime(2012, 9, 5, 0, 0, 0);
	
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static string GenerateId()
		{
			TimeSpan ts = DateTime.Now - epoch;
		
			StringBuilder sb = new StringBuilder(2);
			sb.Append(MathUtility.NextRandom(0, 10000)).Append(ts.TotalMilliseconds);

			return sb.ToString();	
		}

		public static T ParseEnum<T>(string value)
		{
			return (T) Enum.Parse(typeof( T ), value, true);
		}

		public static UnityEngine.LayerMask ComputeLayerMask(string[] layers)
		{
			int mask = 0;
		
			foreach (string layer in layers)
			{
				mask = mask | (1 << UnityEngine.LayerMask.NameToLayer(layer));	
			}
		
			return mask;
		}
	
	}
}
