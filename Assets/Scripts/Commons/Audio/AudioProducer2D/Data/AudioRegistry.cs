/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using nopact.Commons.Audio;

using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

[Serializable]
public class AudioRegistry
{

	public Dictionary<string, AudioEntry> data;
    
	public AudioRegistry()
	{

		Initialize();

	}

	public void Save()
	{

        if ( File.Exists ( Application.dataPath + AudioParameters.dataFilePath ) )
        {

            File.Delete(Application.dataPath + AudioParameters.dataFilePath);

        }

        string dDirectory = Application.dataPath + AudioParameters.dataFilePath.Remove(AudioParameters.dataFilePath.IndexOf(AudioParameters.dataFileName));
        
        if ( !Directory.Exists ( dDirectory + "Data\"") )
        {

            Directory.CreateDirectory(dDirectory + "Data");

        }        
        		
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.dataPath + AudioParameters.dataFilePath, FileMode.OpenOrCreate);
		
		bf.Serialize(file, data);
		file.Close();

	}

	public Dictionary<string, AudioEntry> GetLoadLevel(int loadLevel)
	{

	Dictionary<string, AudioEntry> loadRegistry = new Dictionary<string, AudioEntry>();

		if ( data == null )  return null;


		foreach ( KeyValuePair<string,AudioEntry> kvp in data ) {

			if ( kvp.Value.loadLevel == loadLevel ) {

				loadRegistry.Add ( kvp.Key, kvp.Value );

			}

		}

	return loadRegistry;

	}

	public void Reset()
	{
		data = new Dictionary<string, AudioEntry>();
		Save();
	}

	private void Initialize()
	{
		Load();
	}

	public void Kill () {
        	
		data = null;

	}

    public void Load()
    {
		TextAsset asset = ((TextAsset) Resources.Load ( AudioParameters.dataFileName ));

		byte[] serializedContent = null;

		BinaryFormatter bf = new BinaryFormatter();

		if ( asset != null ) {
			
			serializedContent = asset.bytes;
						
		}

		if ( serializedContent != null ) {

			Stream stream = new MemoryStream ( serializedContent );

			data = (Dictionary<string, AudioEntry>) bf.Deserialize ( stream );

		}

		if ( data == null ) {

				data = new Dictionary<string,AudioEntry>();
				Save ();
					
		}       

    }

}


[Serializable]
public class AudioEntry
{

	public string path;
	public int loadLevel;
	public float inherentVolume = 1.0f;
	public AudioType type;
	public AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;

	public AudioEntry() 
	{
	}

	public AudioEntry(string path, AudioType type, int loadLevel, float volume)
	{

		this.path = path;
		this.type = type;
		this.loadLevel = loadLevel;
		this.inherentVolume = volume;

	}
    
}

public enum AudioType
{

    SFX,
    MUSIC

}


