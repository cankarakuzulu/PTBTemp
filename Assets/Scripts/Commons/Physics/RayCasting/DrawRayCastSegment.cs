using nopact.Commons.Utility.LayerMask;
using UnityEngine;

namespace nopact.Commons.Physics.RayCasting
{
	public class DrawRayCastSegment : MonoBehaviour {

		public Transform rayEnd;
		public bool detected;
		public float rayHitDistance = 0.0f;
	
		public GameObject objectInRange;
		private Vector3 forward;
	
		private LayerMaskUtils.LayerMaskPack maskPack;
	
		[Range (0.0f, 150.0f)]
		public float range;
	
		// Use this for initialization
		void Start () {
	
			maskPack = new LayerMaskUtils.LayerMaskPack ();
		
			for ( int i = 0; i < maskPack.layerInteraction.Length; i++ ) {
			
				maskPack.layerInteraction [i] = false;
			
			}
		
			maskPack.layerInteraction[12] = true;

		
		}
	
		// Update is called once per frame
		void FixedUpdate () {
		
			rayEnd.localPosition = new Vector3 (0, 0, range);
		
			forward = transform.TransformDirection ( Vector3.forward );
		
			RaycastHit raycastHit;
			if ( UnityEngine.Physics.Raycast ( transform.position, forward, out raycastHit,  rayEnd.localPosition.z, maskPack.Value () ) ) {
			
				rayHitDistance = raycastHit.distance;
				objectInRange = raycastHit.collider.gameObject;
			
				detected = true;
			
			}
			else {
			
				detected = false;
			
			}
	
		}
	
		void OnDrawGizmos () {
		
			if( rayEnd != null ) {
			
				if( detected ) {
		

					Gizmos.color = Color.red;
					Gizmos.DrawRay (transform.position, forward * rayEnd.localPosition.z );

				}
				else {
			

					Gizmos.color = Color.green;
					Gizmos.DrawRay (transform.position, forward * rayEnd.localPosition.z );
				
				}
			}
		
		}

	}
}
