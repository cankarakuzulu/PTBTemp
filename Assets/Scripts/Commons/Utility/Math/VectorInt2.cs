/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using UnityEngine;

namespace nopact.Commons.Utility.Math
{
	[System.Serializable]
	public struct VectorInt2 {

		public int x, y;
	
		public VectorInt2 (int x, int y) {
		
			this.x = x;
			this.y = y;
		
		}
	
		public static int Manhattan(VectorInt2 v1, VectorInt2 v2) {
		
			return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y) ;
		
		}
	
		public static bool operator ==(VectorInt2 v1, VectorInt2 v2) {
		
			return v1.x == v2.x && v1.y == v2.y ;
		
		}
	
		public static bool operator !=(VectorInt2 v1, VectorInt2 v2) {
		
			return !(v1.x == v2.x && v1.y == v2.y );	
		
		}
	
		public static VectorInt2 operator +(VectorInt2 v1, VectorInt2 v2) {
		
			return new VectorInt2(v1.x + v2.x, v1.y + v2.y );	
		
		}
	
		public static VectorInt2 operator -(VectorInt2 v1, VectorInt2 v2) {
		
			return new VectorInt2(v1.x - v2.x, v1.y - v2.y);	
		
		}
	
		public override string ToString() {
		
			return "(" + x + ", " + y + ")";		
		
		}
	
		public override bool Equals(object o) {
		
			if (this.GetType() != o.GetType()) {
			
				return false;
			
			}
			else {
			
				VectorInt2 v = (VectorInt2) o;
			
				return this.x == v.x && this.y == v.y ;
			
			}	
		
		}
	
		public override int GetHashCode() {
		
			int h = this.x ^ this.y ;
		
			return h.GetHashCode();
		
		}
	
	}
}
