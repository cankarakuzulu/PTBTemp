/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using nopact.Commons.Utility.Debug;

namespace nopact.Commons.Audio
{
    public class AudioDataManager
    {

        public LiveAudioParameters LiveParameters
        {

            get
            {

                if ( liveParameters == null )
                {
                    LoadLiveParameters();
                }
                return liveParameters;

            }

        }

        private Dictionary<string, AudioClipData> liveClipData;
        private AudioRegistry registry;
        private LiveAudioParameters liveParameters;

        public AudioDataManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            registry = new AudioRegistry();
            LoadClipsLoadLevel( 0 );

        }

        private void LoadLiveParameters()
        {

            if ( !File.Exists( AudioParameters.liveFilePath ) )
            {

                SaveLiveParameters();

            }

            string jsonReadout = "";

            using ( TextReader reader = File.OpenText( AudioParameters.liveFilePath ) )
            {

                jsonReadout = reader.ReadLine();

            }

            liveParameters = JsonReader.Deserialize<LiveAudioParameters>( jsonReadout );
            if ( liveParameters == null )
            {

                liveParameters = new LiveAudioParameters();

            }

        }

        public void SaveLiveParameters()
        {

            if ( liveParameters == null ) liveParameters = new LiveAudioParameters();

            string jsonFile = JsonWriter.Serialize( liveParameters );

            using ( TextWriter writer = File.CreateText( AudioParameters.liveFilePath ) )
            {

                writer.WriteLine( jsonFile );

            }

        }

        public void RequestLiveParameterUpdate( LiveAudioParameters parameters )
        {

            if ( parameters != null )
            {

                this.liveParameters = parameters;

            }

            SaveLiveParameters();
            LoadLiveParameters();

        }


        public void LoadClipsLoadLevel( int audioLevel )
        {

            if ( liveClipData == null )
            {

                liveClipData = new Dictionary<string, AudioClipData>();

            }

            List<AudioClipData> loadQueue = new List<AudioClipData>();

            if ( liveClipData != null )
            {

                foreach ( KeyValuePair<string, AudioClipData> kvp in liveClipData )
                {

                    if ( kvp.Value.loadLevel == 0 || kvp.Value.loadLevel == audioLevel )
                    {

                        kvp.Value.id = kvp.Key;
                        loadQueue.Add( kvp.Value );

                    }

                }

            }

            if ( loadQueue.Count > 0 && audioLevel == 0 )
            {

                DebugWrapper.LogWarning( "[AudioDataManager] You are loading LoadingLevel(0) AudioClips more then once. Check your design!\nInterrupted load process..." );
                return;
            }

            Dictionary<string, AudioEntry> loadLevel = registry.GetLoadLevel( audioLevel );

            foreach ( KeyValuePair<string, AudioEntry> kvp in loadLevel )
            {

                AudioClipData audioClipData = new AudioClipData( kvp.Value );
                audioClipData.id = kvp.Key;

                loadQueue.Add( audioClipData );

            }

            LoadResources( loadQueue );

        }

        private void LoadResources( List<AudioClipData> loadQueue )
        {

            if ( loadQueue == null ) return;

            for ( int loadIndex = loadQueue.Count - 1; loadIndex >= 0; loadIndex-- )
            {

                //DebugWrapper.Log ( loadQueue[loadIndex].path );

                loadQueue[ loadIndex ].clip = Resources.Load<AudioClip>( loadQueue[ loadIndex ].path );
                // Doesn't work. Because unity(!)
                //loadQueue [loadIndex].clip.loadType = loadQueue [loadIndex ].loadType;

                if ( loadQueue[ loadIndex ].clip != null )
                {

                    liveClipData.Add( loadQueue[ loadIndex ].id, loadQueue[ loadIndex ] );

                }

            }

        }

        public AudioClip GetClip( string id, out float inherentVolume )
        {

            inherentVolume = 1.0f;

            AudioClipData outClipData;

            if ( liveClipData.TryGetValue( id, out outClipData ) )
            {

                inherentVolume = outClipData.inherentVolume;

                return outClipData.clip;

            }
            else
            {

                DebugWrapper.LogError( "[AudioSystem] Sound id:" + id + " is invalid or you are trying to play it in a wrong load level" );

            }

            return null;

        }

        public void Kill()
        {

        }

    }

    public class AudioClipData :AudioEntry
    {

        public string id;
        public AudioClip clip;

        public AudioClipData()
        {

        }

        public AudioClipData( AudioEntry audioEntry )
        {

            this.loadLevel = audioEntry.loadLevel;
            this.path = audioEntry.path;
            this.type = audioEntry.type;

            this.loadType = audioEntry.loadType;

            this.inherentVolume = audioEntry.inherentVolume;

        }

    }


}
