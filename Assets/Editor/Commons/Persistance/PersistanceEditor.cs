using UnityEditor;
using UnityEngine;
using System.IO;
namespace nopact.Grindin.Editor.Commons.Persistance
{
    public class PersistanceEditor
    {
        [MenuItem("no-pact/Data/Delete Player Data")]
        public static void DeletePlayerData()
        {
            string defaultDataPath = string.Concat( new string[ ] { Application.persistentDataPath, Path.DirectorySeparatorChar.ToString(), "playerData.dat" } );

            if (File.Exists(defaultDataPath))
            {
                File.Delete(defaultDataPath);
            }
        }
    }
}