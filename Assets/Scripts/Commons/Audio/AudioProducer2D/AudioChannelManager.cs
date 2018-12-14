/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using nopact.Commons.Domain.Enum;
using nopact.Commons.Utility;
using nopact.Commons.Utility.Debug;
using nopact.Commons.Utility.Timer;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace nopact.Commons.Audio
{
    public class AudioChannelManager
    {

        public float SfxVolume
        {

            get { return sfxVolume; }
            set { sfxVolume = value; }

        }

        private AudioSource[ ] bgmChannels;
        private AudioSource[ ] sfxChannels;
        private Action[ ] onCompleteRegistry;
        private float[ ] onDelayedCompleteTimes;
        private Action onFadeComplete;

        private AudioSource fadingChannel;
        private CountdownTimer fadeTimer;
        private string expectedFadeTimerId;

        private float sfxVolume = 1.0f;

        private float[ ] inherentSfxClipVolumes;

        public AudioChannelManager( Transform root, int sfxChannelCount, int bgmChannelCount, float fadeDuration )
        {


            sfxChannels = new AudioSource[ sfxChannelCount ];
            for ( int sfxChannelIndex = 0; sfxChannelIndex < sfxChannelCount; sfxChannelIndex++ )
            {

                GameObject channelBody = new GameObject();
                channelBody.name = "SfxChan_" + sfxChannelIndex;
                channelBody.transform.parent = root;

                sfxChannels[ sfxChannelIndex ] = channelBody.AddComponent<AudioSource>() as AudioSource;
                sfxChannels[ sfxChannelIndex ].gameObject.SetActive( false );
                sfxChannels[ sfxChannelIndex ].playOnAwake = false;

            }

            inherentSfxClipVolumes = new float[ sfxChannelCount ];

            bgmChannels = new AudioSource[ bgmChannelCount ];
            for ( int bgmChannelIndex = 0; bgmChannelIndex < bgmChannelCount; bgmChannelIndex++ )
            {

                GameObject channelBody = new GameObject();
                channelBody.name = "BgmChan_" + bgmChannelIndex;
                channelBody.transform.parent = root;

                bgmChannels[ bgmChannelIndex ] = channelBody.AddComponent<AudioSource>() as AudioSource;
                bgmChannels[ bgmChannelIndex ].gameObject.SetActive( false );
                bgmChannels[ bgmChannelIndex ].playOnAwake = false;

                fadeTimer = new CountdownTimer( NopactUtility.GenerateId(), fadeDuration, CountdownScope.Menu, true, ( s ) => OnFadeTimerComplete( s ), ( t, p ) => OnFadeTimerPercentage( t, p ) );

            }

            onCompleteRegistry = new Action[ sfxChannels.Length + bgmChannelCount ];

            onDelayedCompleteTimes = new float[ sfxChannels.Length ];

        }
        
        // these methods have same logical issues and are old. Pending refactoring
        public void PlaySfxDelayed( float delay, AudioClip clip, float volume, Action onComplete = null )
        {

            if ( delay<= 0 )
            {
                PlaySfx( clip, volume, onComplete );
                return;
            }

            if ( clip == null )
            {

                DebugWrapper.LogWarning( "[AudioSystem] Play Sfx - Request parameters clip is null" );
                return;

            }

            int chanNr;
            AudioSource channel = GetFreeChannel( false, out chanNr );

            if ( channel == null ) return;

            onCompleteRegistry[ chanNr ] = onComplete;

            onDelayedCompleteTimes[ chanNr ] = ( float ) AudioSettings.dspTime + delay + clip.length;

            channel.volume = sfxVolume * volume;
            inherentSfxClipVolumes[ chanNr ] = volume;
            channel.clip = clip;
            channel.loop = false;
            channel.PlayDelayed( delay );

        }

        public void PlaySfx( AudioClip clip, float volume, Action onComplete = null )
        {

            if ( clip == null )
            {

                DebugWrapper.LogWarning( "[AudioSystem] Play Sfx - Request parameters clip is null" );
                return;

            }

            int chanNr;
            AudioSource channel = GetFreeChannel( false, out chanNr );

            if ( channel == null ) return;

            onCompleteRegistry[ chanNr ] = onComplete;

            channel.volume = sfxVolume * volume;
            inherentSfxClipVolumes[ chanNr ] = volume;
            channel.clip = clip;
            channel.loop = false;
            channel.Play();

        }

        public void FadeActiveBGM( Action onComplete )
        {

            this.onFadeComplete = onComplete;

            fadingChannel = GetActiveBGMChannel();

            if ( fadingChannel == null )
            {

                this.onFadeComplete();
                this.onFadeComplete = null;
                return;

            }

            fadeTimer.Obsolete = false;
            fadeTimer.Remaining = fadeTimer.Duration;
            expectedFadeTimerId = TimerUtility.Instance.RegisterTimer( fadeTimer );

        }

        public void SetSfxVolume( float volume )
        {

            sfxVolume = volume;

        }

        private void OnFadeTimerComplete( string id )
        {

            if ( id != expectedFadeTimerId ) return;
            if ( fadingChannel == null ) return;

            fadingChannel.Stop();
            fadingChannel.gameObject.SetActive( false );

            if ( onFadeComplete != null )
            {

                onFadeComplete();

            }


        }

        private void OnFadeTimerPercentage( string id, float percentage )
        {

            if ( id != expectedFadeTimerId ) return;
            if ( fadingChannel == null ) return;

            fadingChannel.volume = 1.0f - percentage;

        }

        public void PlaySfxLoop( AudioClip clip, float inherentVolume )
        {
            if ( clip == null )
            {

                DebugWrapper.LogWarning( "[AudioSystem] PlayLoop - Request parameters clip is null" );
                return;

            }

            int chanNr;
            AudioSource channel = GetFreeChannel( false, out chanNr );

            if ( channel == null ) return;

            channel.volume = sfxVolume * inherentVolume;
            inherentSfxClipVolumes[ chanNr ] = inherentVolume;
            channel.clip = clip;
            channel.loop = true;
            channel.Play();

        }

        public void StopAllSfxLoops()
        {

            AudioSource[ ] channelList = sfxChannels;

            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( channelList[ chanIndex ].gameObject.activeSelf && channelList[ chanIndex ].isPlaying && channelList[ chanIndex ].loop )
                {

                    channelList[ chanIndex ].loop = false;
                    channelList[ chanIndex ].Stop();

                }

            }


        }
        public void StopAllSfx()
        {
            AudioSource[ ] channelList = sfxChannels;

            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( channelList[ chanIndex ].gameObject.activeSelf )
                {

                    channelList[ chanIndex ].Stop();

                }

            }
        }

        public int StopSfxLoop( AudioClip clip )
        {            
            if ( clip == null )
            {

                DebugWrapper.LogWarning( "[AudioSystem] StopLoop - Request parameters clip is null" );
                return -2;

            }

            AudioSource sfxChannel = GetFirstPlaying( clip );

            if ( sfxChannel == null )
            {

                DebugWrapper.LogWarning ( "[AudioSystem] Cannot Stop Loop as it is inactive. Audio Clip Name:"+clip.name);
                return -1;
            }
            
            sfxChannel.Stop();
            sfxChannel.clip = null;
            sfxChannel.loop = false;

            return 0;

        }

        public void PlayBgm( AudioClip clip, Action onComplete = null )
        {

            int chanNr;
            AudioSource channel = GetFreeChannel( true, out chanNr );

            if ( channel == null ) return;

            onCompleteRegistry[ sfxChannels.Length + chanNr ] = onComplete;

            channel.clip = clip;
            channel.loop = false;
            channel.Play();

        }

        public AudioSource GetActiveBGMChannel()
        {

            AudioSource[ ] channelList = bgmChannels;

            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( channelList[ chanIndex ].isPlaying && channelList[ chanIndex ].gameObject.activeSelf == true )
                {

                    return channelList[ chanIndex ];

                }

            }

            return null;

        }

        public void Update()
        {
            HandleSfxVolumeLevels();

            DisableUnusedChannels( false );
            DisableUnusedChannels( true );

        }

        private void HandleSfxVolumeLevels()
        {

            for ( int channelIndex = 0; channelIndex < sfxChannels.Length; channelIndex++ )
            {

                sfxChannels[ channelIndex ].volume = inherentSfxClipVolumes[ channelIndex ] * sfxVolume;

            }

        }

        private AudioSource GetFirstPlaying( AudioClip clip )
        {

            AudioSource[ ] channelList = sfxChannels;
            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( channelList[ chanIndex ].gameObject.activeInHierarchy && channelList[ chanIndex ].isPlaying == true && channelList[ chanIndex ].clip == clip )
                {

                    return channelList[ chanIndex ];

                }

            }

            return null;

        }


        private AudioSource GetFreeChannel( bool isBgm, out int chanNr )
        {

            AudioSource[ ] channelList = isBgm ? bgmChannels : sfxChannels;

            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( !channelList[ chanIndex ].gameObject.activeSelf )
                {

                    channelList[ chanIndex ].volume = 1.0f;
                    channelList[ chanIndex ].gameObject.SetActive( true );
                    chanNr = chanIndex;
                    return channelList[ chanIndex ];

                }

            }

            string channelType = isBgm ? "BGM" : "SFX";

            DebugWrapper.LogError( "[AudioSystem] No free channels. Sound is being dropped for " + channelType + " increase channel count!" );
            chanNr = -1;
            return null;

        }

        private void DisableUnusedChannels( bool isBgm )
        {

            AudioSource[ ] channelList = isBgm ? bgmChannels : sfxChannels;

            for ( int chanIndex = channelList.Length - 1; chanIndex > -1; chanIndex-- )
            {

                if ( !channelList[ chanIndex ].isPlaying && channelList[ chanIndex ].gameObject.activeSelf == true )
                {

                    channelList[ chanIndex ].gameObject.SetActive( false );

                    if ( isBgm )
                    {

                        if ( onCompleteRegistry[ chanIndex + sfxChannels.Length ] != null )
                        {

                            onCompleteRegistry[ chanIndex + sfxChannels.Length ]();
                            onCompleteRegistry[ chanIndex + sfxChannels.Length ] = null;

                        }

                    }
                    else
                    {

                        if ( AudioSettings.dspTime <= onDelayedCompleteTimes[ chanIndex ] )
                        {

                            continue;

                        }
                        else
                        {

                            onDelayedCompleteTimes[ chanIndex ] = 0.0f;

                        }

                        if ( onCompleteRegistry[ chanIndex ] != null )
                        {

                            onCompleteRegistry[ chanIndex ]();
                            onCompleteRegistry[ chanIndex ] = null;

                        }

                    }

                }

            }

        }
    }



}
