/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using nopact.Commons.Utility.Debug;

namespace nopact.Commons.Audio
{
    public class AudioProducer2D : MonoBehaviour
    {

        public bool IsHandled
        {

            get { return isHandled; }
            set
            {

                isHandled = value;

            }

        }
        private bool isHandled = false;


        public bool IsDebug
        {

            get { return isDebug; }
            set
            {

                isDebug = value;
            }

        }

        public LiveAudioParameters LiveParameters
        {
            get
            {

                if ( dataManager != null )
                {

                    return dataManager.LiveParameters;

                }

                return null;

            }

        }


        private bool isDebug = false;


        private AudioDataManager dataManager;
        private AudioChannelManager channelManager;

        private bool isInitialized = false;

        private string pendingBgmId;
        private Action pendingBgmOnCompleteAction;


        public void SfxMute( bool isOn )
        {

            float soundLevel = isOn ? 1.0f : 0.0f;
            channelManager.SetSfxVolume( soundLevel );

        }

        public void RequestLiveParameterUpdate( LiveAudioParameters liveParameters )
        {

            dataManager.RequestLiveParameterUpdate( liveParameters );

        }

        public void StopAllSfxLoops()
        {

            if ( dataManager == null || channelManager == null ) return;

            channelManager.StopAllSfxLoops();

        }
        public void StopAllSfx()
        {
            if ( dataManager == null || channelManager == null ) return;

            channelManager.StopAllSfx();
        }
        public void EndSfxLoop( string id )
        {
            
            if ( dataManager == null || channelManager == null ) return;

            float volume;
            AudioClip clip = dataManager.GetClip( id, out volume );

            if ( clip == null )
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat( "[AudioProducer] Audio Clip \"{0}\" is not found in the database. (maybe you forgot to include?) ", id );

                DebugWrapper.LogError( sb.ToString() );
                return;
            }

            int returnCode = channelManager.StopSfxLoop( clip );

            if ( returnCode > -1 ) return;

            if ( isDebug ) DebugWrapper.LogWarning( "[AudioSystem] Cannot Stop Loop as it is inactive. Audio Clip Name:" + clip.name );

        }

        public void StartSfxLoop( string obj )
        {

            if ( dataManager == null || channelManager == null ) return;

            float volume;
            AudioClip clip = dataManager.GetClip( obj, out volume );

            if ( clip == null )
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat( "[AudioProducer] Audio Clip \"{0}\" is not found in the database. (maybe you forgot to include?) ", obj );

                DebugWrapper.LogError( sb.ToString() );
                return;
            }

            channelManager.PlaySfxLoop( clip, volume );

        }


        public void Initialize()
        {
            gameObject.transform.parent = UnityEngine.Camera.main.transform;
            if ( dataManager == null )
            {
                dataManager = new AudioDataManager();
            }
            LiveAudioParameters parameters = dataManager.LiveParameters;
            channelManager = new AudioChannelManager( transform, parameters.sfxChannelCount, parameters.musicChannelCount, parameters.bgmFadeTime );

            isInitialized = true;

        }


        void Update()
        {

            if ( isInitialized )
            {

                if ( channelManager != null )
                {

                    channelManager.Update();

                }

            }

        }

        public void PlaySfx ( string id, float volume = 1.0f, float delay = 0.0f )
        {
            if ( dataManager == null || channelManager == null ) return;

            float masterVolume;
            AudioClip clip = dataManager.GetClip( id, out masterVolume );

            if ( clip == null )
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat( "[AudioProducer] Audio Clip \"{0}\" is not found in the database. (maybe you forgot to include?) ", id );

                DebugWrapper.LogError( sb.ToString() );
                return;
            }
            channelManager.PlaySfxDelayed( delay, clip, volume );
        }


        public void PlayBGM( string id, Action onComplete )
        {

            pendingBgmId = id;
            pendingBgmOnCompleteAction = onComplete;

            channelManager.FadeActiveBGM( () => OnActiveBGMFaded() );

        }


        private void OnActiveBGMFaded()
        {

            float volume;
            AudioClip clip = dataManager.GetClip( pendingBgmId, out volume );

            if ( clip == null ) return;

            channelManager.PlayBgm( clip, pendingBgmOnCompleteAction );

        }

    }

}

