using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace nopact.Commons.Persistence
{
    public class Persistence<T> :IPersistence<T> where T : class, IPersistentData, new()
    {
        private string version;
        public void Save()
        {
            string defaultDataPath = string.Concat( new string[ ] { Application.persistentDataPath, Path.DirectorySeparatorChar.ToString(), "playerData.dat" } );
            BinaryFormatter bf = new BinaryFormatter();
            Data.SetVersion( version );
            FileStream file = File.Open( defaultDataPath, FileMode.OpenOrCreate );            
            bf.Serialize( file, Data );
            file.Close();
            Data.DataUpdate();
        }

        public void Reset()
        {
            Data = new T();
            Data.DataInit();
            Data.DataUpdate();
            Save();
        }

        public void Initialize( string version )
        {
            this.version = version;
            Load();
        }

        private void Load()
        {
            string defaultDataPath = string.Concat( new string[ ] { Application.persistentDataPath, Path.DirectorySeparatorChar.ToString(), "playerData.dat" } );
            if ( File.Exists( defaultDataPath ))
            {
                FileStream file = File.Open(defaultDataPath, FileMode.Open);
             
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Data = ( T ) bf.Deserialize( file );
                }
                catch ( SerializationException e )
                {
                    ApplicationException applicationException = new ApplicationException( string.Format( "[Persistance] Cannot load persistance file. Err:{0}", e.Message ) );
                    throw ( applicationException );                        
                }
                catch ( IOException ioE )
                {
                    ApplicationException applicationException = new ApplicationException( string.Format( "[Persistance] Cannot access file Err:{0}", ioE.Message ) );
                    throw ( applicationException );
                }
                
                file.Close();
                               
            }
            else
            {
                Data = new T();
                Data.DataInit();                
            }
            Data.DataUpdate();
        }

        public T Data
        {
            get;
            private set;
        }
    }
}
