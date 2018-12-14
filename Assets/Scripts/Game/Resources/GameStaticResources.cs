using System;
using nopact.Commons.SceneDirection.Resources;
using UnityEngine;

namespace nopact.Game.Resources
{
	public class GameStaticResources : MonoBehaviour, IResourceProvider {

		public event Action OnInitializationComplete;
		[SerializeField] protected ResourcePair[ ] resources;
              
		public T Get<T>( string name, out ResourcePair.PoolOptions options) where T : UnityEngine.Object
		{
			var obj = FindResource(0, resources.Length, name, out options).Object;
			return obj as T;
		}

		public GameObject Get(string name, out ResourcePair.PoolOptions options)
		{
			return FindResource(0, resources.Length, name, out options).Object as GameObject;
		}

		public void Initialize()
		{
			Array.Sort( resources, ResourcePair.Comparer );
			if(OnInitializationComplete !=null)
			{
				OnInitializationComplete();
			}
		}

		private ResourcePair FindResource( int startIndex, int stopIndexExclusive, string name, out ResourcePair.PoolOptions options )
		{
			if ( startIndex == stopIndexExclusive )
			{
				options = new ResourcePair.PoolOptions();
				// unable to find.
				return null;
			}

			int mid = ( stopIndexExclusive + startIndex ) / 2;
			var middleElement = resources[ mid ];
			int compareResult = name.CompareTo( middleElement.Name );

			if ( compareResult == 0 )
			{
				options = middleElement.PoolParams;
				return middleElement;
			}

			if ( compareResult < 0 )
			{
				return FindResource( startIndex, mid, name , out options);
			}

			return FindResource( mid + 1, stopIndexExclusive, name , out options);
		}
	}
	

}
