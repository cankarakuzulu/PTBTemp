/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using System.Collections.Generic;

namespace nopact.Commons.Extensions
{
	public static class ListExtensions {

		public static T Pop<T>(this List<T> list) {

			if (list.Count > 0) {

				T item = list[0];
				list.Remove(item);

				return item;

			}
			else {

				return default(T);

			}

		}

		public static string HashData<T>(this List<T> list) {

			System.Text.StringBuilder sb = new System.Text.StringBuilder(list.Count);

			for (int i = 0; i < list.Count; i++) {

				sb.Append(list[i].ToString());

			}

			return sb.ToString();

		}

	}
}
