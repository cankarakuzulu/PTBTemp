using UnityEngine;
using UnityEngine.Assertions;

namespace nopact.Commons.Utility
{
    public class GameUtility : MonoBehaviour
    {        
        [SerializeField] private bool assertionsRaiseException;
        
        private void Awake()
        {            
            Assert.raiseExceptions = assertionsRaiseException;            
        }
    }
}
