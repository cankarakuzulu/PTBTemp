using UnityEngine;

namespace nopact.Commons.Utility.LayerMask
{
	public abstract class LayerMaskUtils  {

		public class LayerMaskPack {
	
			public bool [] layerInteraction;
	
			public void Invert () {
			
				for ( int i = 0; i < 32; i ++ ) {
				
					layerInteraction [ i ] = !layerInteraction [ i ];
				
				}
			
			}
		
			public int Value () {
			
				int layerMaskInt = 0;
		
				for ( int i = 0; i<32; i ++ ) {
				
					if ( layerInteraction [ i ] ) {
				
						layerMaskInt += (int) Mathf.Pow ( 2.0f, i );
				
					}
			
				}
		
				return layerMaskInt;
			
			}

			public void SetLayerMask ( string layerName, bool value ) {

				layerInteraction [ UnityEngine.LayerMask.NameToLayer ( layerName ) ] = value;

			}
		
			public LayerMaskPack () {
		
				layerInteraction = new bool [32];
			
			}
	
		}
	
	
	}
}

