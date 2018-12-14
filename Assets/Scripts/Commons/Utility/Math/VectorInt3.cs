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
	public struct VectorInt3 {

		public int x, y, z;
	
		public VectorInt3 (int x, int y, int z) {
		
			this.x = x;
			this.y = y;
			this.z = z;
		
		}
	
		public static int Manhattan(VectorInt3 v1, VectorInt3 v2) {
		
			return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y) + Mathf.Abs(v1.z - v2.z);
		
		}
	
		public static bool operator ==(VectorInt3 v1, VectorInt3 v2) {
		
			return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
		
		}
	
		public static bool operator !=(VectorInt3 v1, VectorInt3 v2) {
		
			return !(v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);	
		
		}
	
		public static VectorInt3 operator +(VectorInt3 v1, VectorInt3 v2) {
		
			return new VectorInt3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);	
		
		}
	
		public static VectorInt3 operator -(VectorInt3 v1, VectorInt3 v2) {
		
			return new VectorInt3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);	
		
		}
	
		public override string ToString() {
		
			return "(" + x + ", " + y + ", " + z + ")";		
		
		}
	
		public override bool Equals(object o) {
		
			if (this.GetType() != o.GetType()) {
			
				return false;
			
			}
			else {
			
				VectorInt3 v = (VectorInt3) o;
			
				return this.x == v.x && this.y == v.y && this.z == v.z;
			
			}	
		
		}
	
		public override int GetHashCode() {
		
			int h = this.x ^ this.y ^ this.z;
		
			return h.GetHashCode();
		
		}
	
	}
}
