/*
 *
 *  * Copyright (c) 2013-2014 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using nopact.Commons.Domain.Enum;
using nopact.Commons.Utility.Singleton;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

namespace nopact.Commons.Player
{
    public class PlayerPreferences<T> : GenericSingleton<PlayerPreferences<T>> where T : class, IPlayerPreferenceData, new()
    {
        private const string PLAYER_PREFERENCES = "player_preferences";

        private T data;

        protected override void Initialize()
        {
            data = GetPlayerPreferences();

            if (data == null)
            {
                data = new T();
                data.Initialize();
            }
        }

        public T GetPlayerPreferences()
        {
            return GetJsonValue(PLAYER_PREFERENCES);
        }

        private T GetJsonValue(string key)
        {
            string result = PlayerPrefs.GetString(key, null);

            if (result != null)
            {
                return JsonReader.Deserialize<T>(result);
            }
            else
            {
                return default(T);
            }

        }

        public void Save()
        {
            PlayerPrefs.SetString(PLAYER_PREFERENCES, JsonWriter.Serialize(data));
            PlayerPrefs.Save();
        }

        public T Data
        {
            get
            {
                return this.data;
            }
            set
            {
                data = value;
            }
        }
    }

    public interface IPlayerPreferenceData
    {
        void Initialize();
    }

    public class SimplePlayerPreferencesData : IPlayerPreferenceData
    {
        private bool soundOn;
        private float soundVolume;
        private bool musicOn;
        private float musicVolume;
        private GameLanguage lang;
        private bool batterySavingMode;

        public SimplePlayerPreferencesData()
        {
        }

        public void Initialize()
        {
            this.soundOn = true;
            this.soundVolume = 1f;
            this.musicOn = true;
            this.musicVolume = 1f;

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Turkish:

                    this.lang = GameLanguage.Turkish;
                    break;

                default:

                    this.lang = GameLanguage.English;
                    break;
            }
        }

        [JsonName("soundOn")]
        public bool SoundOn
        {
            get
            {
                return this.soundOn;
            }
            set
            {
                soundOn = value;
            }
        }

        [JsonName("soundVolume")]
        public float SoundVolume
        {
            get
            {
                return this.soundVolume;
            }
            set
            {
                soundVolume = value;
            }
        }

        [JsonName("musicOn")]
        public bool MusicOn
        {
            get
            {
                return this.musicOn;
            }
            set
            {
                musicOn = value;
            }
        }

        [JsonName("musicVolume")]
        public float MusicVolume
        {
            get
            {
                return this.musicVolume;
            }
            set
            {
                musicVolume = value;
            }
        }

        [JsonName("lang")]
        public GameLanguage Lang
        {
            get
            {
                return this.lang;
            }
            set
            {
                lang = value;
            }
        }

        [JsonName("batterySavingMode")]
        public bool BatterySavingMode
        {
            get
            {
                return this.batterySavingMode;
            }
            set
            {
                batterySavingMode = value;
            }
        }
    }
}
