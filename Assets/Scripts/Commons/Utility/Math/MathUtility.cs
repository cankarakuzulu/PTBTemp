using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Utility.Math
{
	public class MathUtility {
	
		private static System.Random random = new System.Random();
	
		public static float Clamp(float f, float min, float max) {
		
			if (f <= min) {
			
				return min;
			
			}
			else if (f >= max) {
			
				return max;
			
			}
			else {
			
				return f;
			
			}
		
		}

		public static float PingPong(float start, float finish, float t, float normalizationFactor){
		
			if(finish == start){
			
				return finish;
			
			}
			else{
			
				return start + Mathf.Sign(finish - start) * Mathf.PingPong(t * normalizationFactor, Mathf.Abs(finish - start)); 	
			
			}
											
		}
	
		public static float Max(IList<float> list){
		
			float max = list[0];
		
			foreach (float x in list){
			
				if(x > max){
				
					max = x;
				
				}
			
			}
		
			return max;
			
		}
	
		public static float Percentage01(float val, float max) {
		
			return (Mathf.Clamp(val, 0, max)) / max;
		
		}
		
	
		public static float TruncateFloat( float baseFloat, int decimals ){
		
			float returnFloat;
			float modifier;
			modifier = IntPower (10.0f, decimals);
			returnFloat = baseFloat * (modifier);
			returnFloat = ( Mathf.RoundToInt (returnFloat) );
			returnFloat = returnFloat / (modifier);
			return returnFloat;
		
		}
	
		public static float TruncateFloat( float baseFloat ){
		
			return TruncateFloat ( baseFloat, 2);
		
		}
	
		public static float IntPower(float number, int power){
		
			float result=1;
			int i = 0;
		
			if ( power == 0 ){
			
				return 1.0f;
			
			}
			else{
			
				if( power > 0){
				
					for ( i = 0;  i<power;  i++ ){
					
						result *= number;
					
					}
				}
				else{
				
					power = -power;
				
					for ( i = 0;  i<power;  i++ ){
					
						result *= 1/number;
					
					}
				}
			}
		
			return result;
		}
	
		public static int NextRandom(int min, int max) {
		
			return random.Next(min, max);
		
		}
	
		public static float NextRandomZeroOne(int mapToOne) {
		
			return ((float)random.Next(0, mapToOne + 1)) / mapToOne;	
		
		}
	
		public static float NextRandomMinusPlusOne(int mapToOne) {
		
			int doubleMap = 2 * mapToOne;
		
			return ((float)(random.Next(0, (doubleMap) + 1)) / mapToOne) - 1;
		
		}
	
	}
}
