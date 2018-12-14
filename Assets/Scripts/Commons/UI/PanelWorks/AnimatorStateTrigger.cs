using UnityEngine;
using System.Collections;
namespace nopact.Commons.UI.PanelWorks
{
    public class AnimatorStateTrigger :StateMachineBehaviour
    {

        public static event System.Action<int, string> OnEnter, OnExit;        
        [SerializeField] private string triggerID = string.Empty;
        [SerializeField] private bool triggerOnEnter = false, triggerOnExit = false;
        
        override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
                        
            if ( OnEnter != null && triggerOnEnter )
            {
                OnEnter( animator.GetInstanceID(), triggerID );
            }

        }
                
        override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {

            if ( OnExit != null && triggerOnExit )
            {

                OnExit( animator.GetInstanceID(), triggerID );

            }

        }
    }
}
