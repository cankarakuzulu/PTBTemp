using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace nopact.Commons.Animations.StateMachineUtilities
{
    public class InvokeOnStateChange :StateMachineBehaviour
    {

        public static event System.Action<string> OnEnter, OnExit;
        [SerializeField] private string id = string.Empty;
        [SerializeField] private bool triggerOnEnter = false, triggerOnExit = false;
        
        override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            if ( OnEnter != null && triggerOnEnter )
            {
                OnEnter( id );
            }
        }
        
        override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            if ( OnExit != null && triggerOnExit )
            {
                OnExit( id );
            }
        }       
    }
}
