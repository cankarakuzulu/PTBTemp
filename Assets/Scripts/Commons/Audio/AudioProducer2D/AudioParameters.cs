/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using UnityEngine;

namespace nopact.Commons.Audio
{

    public static class AudioParameters
    {

        public static readonly string dataFileName = "Data/AudioRegistry";
        public static readonly string liveFileName = "AudioParameters";

        public static readonly string dataFilePath = System.IO.Path.DirectorySeparatorChar + "Resources/" + dataFileName + ".bytes";
        public static readonly string liveFilePath = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + liveFileName + ".json";

    }

    [System.Serializable]
    public class LiveAudioParameters
    {
        //defaults
        public int sfxChannelCount = 24;
        public int musicChannelCount = 1;
        public float sfxVolumeMultiplier = 1.0f;
        public float musicVolumeMultiplier = 1.0f;

        public float bgmFadeTime = 1.0f;

        public string[ ] lastBGMPlayed = { "", "" };

    }

}
