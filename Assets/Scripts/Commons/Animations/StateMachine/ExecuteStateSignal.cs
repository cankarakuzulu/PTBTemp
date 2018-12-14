using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Animations.StateMachineUtilities
{
    public class ExecuteStateSignal: MonoBehaviour {

        [SerializeField] private string id;
        [SerializeField] private UnityEngine.Events.UnityEvent OnEnterState, OnExitState;

        private void OnEnable()
        {
            InvokeOnStateChange.OnEnter += InvokeOnStateChange_OnEnter;
            InvokeOnStateChange.OnExit += InvokeOnStateChange_OnExit;
        }

        private void OnDisable()
        {
            InvokeOnStateChange.OnEnter -= InvokeOnStateChange_OnEnter;
            InvokeOnStateChange.OnExit -= InvokeOnStateChange_OnExit;
        }

        private void InvokeOnStateChange_OnExit( string id )
        {
            if ( id == this.id )
            {
                if ( OnExitState != null )
                {
                    OnExitState.Invoke();
                }
            }
        }

        private void InvokeOnStateChange_OnEnter( string id )
        {        
            if ( id == this.id )
            {
                if ( OnEnterState != null  )
                {
                    OnEnterState.Invoke();
                }        
            }
        }    
    }
}
