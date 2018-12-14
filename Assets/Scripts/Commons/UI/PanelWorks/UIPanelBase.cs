using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.UI.PanelWorks
{

    public abstract class UIPanelBase :MonoBehaviour
    {

        [SerializeField] protected UIElement[ ] elements;        
        protected PanelStates state;
        protected bool areControlsLocked = true;
        protected GameObject uGameObject;

        #region PublicAPI

        public void Close( int customAnimationIndex = -1 )
        {            
            switch ( state )
            {
                case PanelStates.Shown:
                    PlayClose( isHurried:false, customAnimationIndex:customAnimationIndex );
                    break;
                case PanelStates.Closing:
                    PlayClose( isHurried: true, customAnimationIndex: customAnimationIndex );
                    break;
                case PanelStates.Opening:
                    Stop();
                    PlayClose( isHurried: false, customAnimationIndex: customAnimationIndex );
                    break;
                case PanelStates.Inactive:
                    break;
            }
        }

        public void Open( int customAnimationIndex = -1 )
        {
            switch ( state )
            {
                case PanelStates.Inactive:
                    Activate();
                    PlayOpen( isHurried: false, customAnimationIndex: customAnimationIndex );
                    break;
                case PanelStates.Opening:
                    PlayOpen( isHurried: true, customAnimationIndex: customAnimationIndex );
                    break;
                case PanelStates.Closing:
                    Stop();
                    PlayOpen( isHurried: false, customAnimationIndex: customAnimationIndex );
                    break;
                case PanelStates.Shown:
                    break;
            }
        }

        public void ShowAll()
        {
            UpdateElementStatus( isHidden: false );
        }

        public void HideAll()
        {
            UpdateElementStatus( isHidden: true );
        }

        public abstract void Setup<T>( T config ) where T : UIPanelParameter;

        #endregion

        #region VirtualMethods

        protected virtual void Opened( )
        {
            state = PanelStates.Shown;
            LockControls( false );
        }

        protected virtual void Closed( )
        {
            state = PanelStates.Inactive;            
            Deactivate();
        }       

        protected virtual void Closing()
        {

        }

        protected virtual void LockControls ( bool isLocked )
        {
            areControlsLocked = isLocked;
        }

        protected virtual void UpdateElementStatus( bool isHidden )
        {
            
        }

        #endregion

        #region AbstractMethods        
        protected abstract void Activate();
        protected abstract void Deactivate();
        protected abstract void PlayOpen( bool isHurried, int customAnimationIndex = -1 );
        protected abstract void PlayClose( bool isHurried, int customAnimationIndex = -1 );
        protected abstract void Stop();
        #endregion

    }

}

