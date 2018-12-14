/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using nopact.Commons.Utility.Singleton;
using UnityEngine;

namespace nopact.Commons.Player
{
    public class PlayerData<T> : GenericSingleton<PlayerData<T>> where T : class, IPlayerSavedData, new()
    {
        private T data;

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerData.dat", FileMode.OpenOrCreate);

            bf.Serialize(file, data);
            file.Close();
        }

        public void Reset()
        {
            data = new T();
            Save();
        }

        protected override void Initialize()
        {
            Load();
        }

        private void Load()
        {
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerData.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerData.dat", FileMode.Open);

                data = (T) bf.Deserialize(file);

                file.Close();
            }
            else
            {
                data = new T();
                data.Initialize();
            }
        }

        public T SavedData
        {
            get
            {
                return this.data;
            }
        }
    }

    public interface IPlayerSavedData
    {
        void Initialize();
    }

    [Serializable]
    public class SimplePlayerSavedData : IPlayerSavedData
    {
        public int highestScore;
        public int xp;

        public void Initialize()
        {

        }
    }
}