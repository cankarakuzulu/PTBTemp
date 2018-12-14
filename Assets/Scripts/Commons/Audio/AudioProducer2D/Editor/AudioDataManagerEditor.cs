/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using nopact.Commons.Utility.Debug;
using System.Linq;

public class AudioDataManagerEditor : EditorWindow
{

    public enum FilterAudioType
    {

        ALL,
        SFX,
        MUSIC

    }

    private enum FilterStringFieldType { 
    
        ID,
        PATH

    }
        
	private AudioEntry currentAudioEntry;
	private string displayId = "";
	private Vector2 mainScroll;
	private AudioRegistry registry;


    private FilterStringFieldType filterStringFieldType;
    private string filterString = "";
    private FilterAudioType filterAudioType;
    private int filterLoadLevel = -1;

    private EditorWindow wizardWindow;
    private EditorWindow mixerWindow;

	[MenuItem ("no-pact/Audio/Audio Registry Editor")]
	public static void  ShowWindow () {

		EditorWindow currentWindow = EditorWindow.GetWindow(typeof(AudioDataManagerEditor));

        currentWindow.titleContent = new GUIContent ("Audio Registry Editor");
        currentWindow.minSize = new Vector2(289, 505);
        currentWindow.maxSize = new Vector2(289, 505);

	}

	void OnEnable () {

		registry = new AudioRegistry();
        registry.data = ( registry.data.OrderBy( ( k ) => k.Key ) ).ToDictionary( (t)=>t.Key,(k)=>k.Value ) ;

    }
	
	void OnGUI () {

		DrawEditor ();

	}

	void DrawEditor () {

        EditorGUILayout.BeginVertical("box", GUILayout.Width(210), GUILayout.Height ( 500 ));
		EditorGUILayout.BeginHorizontal ("box");
		if( GUILayout.Button ("Save") ) {

            EditorUtility.DisplayDialog("Operation Successiful","Saved Changes", "Ok");
			registry.Save();
            AssetDatabase.Refresh();

		};

		if( GUILayout.Button ("Revert") ) {

            if (EditorUtility.DisplayDialog("Warning!", "This operation will revert the audio database\nto its last saved state. Are you sure?", "Yes", "No")) registry.Load();

		};

		if( GUILayout.Button ("Purge") ) {
			
            if ( EditorUtility.DisplayDialog ( "Warning!","You are about to DELETE the ENTIRE sound database.\nAre you sure this is what you want?", "Yes", "No" ) ) registry.Reset ();
            AssetDatabase.Refresh();

		};

        if (GUILayout.Button("Mixer"))
        {

            if (mixerWindow == null)
            {

                CreateNewMixerWindow();

            }

        }

        if (GUILayout.Button("Wizard"))
        {

            if (wizardWindow == null)
            {

                CreateNewWizardWindow();

            }
                        
        }

		EditorGUILayout.EndHorizontal();

		DrawEntryBoard ( registry.data );
				
		EditorGUILayout.BeginHorizontal ();
		
		EditorGUILayout.Space();
		
		if( GUILayout.Button ("Add/Update") ) {

			if ( registry.data.ContainsKey ( displayId ) ) {

				registry.data.Remove ( displayId );

			}

			registry.data.Add ( displayId, currentAudioEntry );
            registry.data = ( registry.data.OrderBy( ( k ) => k.Key ) ).ToDictionary( ( t ) => t.Key, ( k ) => k.Value );

        };
		
		EditorGUILayout.Space();
		
		if ( GUILayout.Button ("Remove Selected") ) {

			if ( registry.data.ContainsKey ( displayId ) ) {
				
				registry.data.Remove ( displayId );
				
			}

		}
		
		EditorGUILayout.Space ();
		
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        Dictionary<string, AudioEntry> filteredDictionary = ApplyFilter(registry.data);
        
		EditorGUILayout.BeginVertical ("ShurikenEffectBg");
		mainScroll = EditorGUILayout.BeginScrollView ( mainScroll );
		
		DrawEntryList ( filteredDictionary );

		EditorGUILayout.EndScrollView ();
		EditorGUILayout.EndVertical();

        DrawListFilter();
				
		EditorGUILayout.EndVertical();
        
    }

    void DrawListFilter()
    {

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Filter:"), GUILayout.Width(35));
        filterString = EditorGUILayout.TextField(filterString, GUILayout.Width(90));
        filterStringFieldType = (FilterStringFieldType)EditorGUILayout.EnumPopup(filterStringFieldType, GUILayout.Width(40));

        filterLoadLevel = EditorGUILayout.IntPopup(filterLoadLevel, new string[] { "All", "0", "1", "2", "3", "4", "5" }, new int[] { -1, 0, 1, 2, 3, 4, 5 }, GUILayout.Width(40));
        filterAudioType = (FilterAudioType)EditorGUILayout.EnumPopup(filterAudioType, GUILayout.Width(40));

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();

    }

	void OnDisable () {

		if ( wizardWindow != null ) wizardWindow.Close();
        if (mixerWindow != null) mixerWindow.Close();

        AudioDataManagerMixer.registry = null;
		AudioDataManagerWizardWindow.registry = null;

		registry.Kill();
		registry = null;

	}


    void CreateNewWizardWindow()
    {

        wizardWindow = EditorWindow.GetWindow(typeof(AudioDataManagerWizardWindow));
        wizardWindow.minSize = new Vector2(289, 505);
        wizardWindow.maxSize = new Vector2(289, 505);
        wizardWindow.titleContent = new GUIContent("Audio Wizard");
        AudioDataManagerWizardWindow.registry = registry;

    }

    void CreateNewMixerWindow()
    {

        mixerWindow = EditorWindow.GetWindow(typeof(AudioDataManagerMixer));
        mixerWindow.minSize = new Vector2(1280, 500);
        mixerWindow.maxSize = new Vector2(1280, 500);
        mixerWindow.titleContent = new GUIContent("Audio Mixer");
        AudioDataManagerMixer.registry = registry;

    }

    Dictionary<string, AudioEntry> ApplyFilter(Dictionary<string, AudioEntry> dictionary)
    {

        Dictionary<string, AudioEntry> resultingDictionary = new Dictionary<string,AudioEntry>();

        foreach (KeyValuePair<string, AudioEntry> kvp in dictionary)
        {

            switch (filterAudioType)
            {

                case FilterAudioType.MUSIC :

                    if (kvp.Value.type == AudioType.SFX)
                    {

                        continue;

                    }
                    
                    break;

                case FilterAudioType.SFX :

                    if (kvp.Value.type == AudioType.MUSIC)
                    {

                        continue;

                    }

                    break;

            }

            if (filterLoadLevel != -1 && filterLoadLevel != kvp.Value.loadLevel)
            {

                continue;

            }

            string nameFilterSource = (filterStringFieldType == FilterStringFieldType.ID) ? kvp.Key : kvp.Value.path;

            nameFilterSource = nameFilterSource.ToLowerInvariant();

            string lowerFilterKey = filterString.ToLowerInvariant();

            if (!nameFilterSource.Contains(lowerFilterKey) && lowerFilterKey != "" )
            {

                continue;

            }

            resultingDictionary.Add(kvp.Key, kvp.Value);

        }
        
        return resultingDictionary;
    }


	void DrawEntryList ( Dictionary<string, AudioEntry> dictionary ) {

        
		if ( dictionary == null ) return;
        
		foreach ( KeyValuePair<string, AudioEntry> pair in dictionary ) {
						
			EditorGUILayout.BeginHorizontal ( "box" );

			Color oldColor = GUI.color;

			if ( pair.Key == displayId ) {

				GUI.color = Color.yellow;

			}

			if ( GUILayout.Button ( pair.Value.type.ToString() + " : " + pair.Key ) ) {

				displayId = pair.Key;
                		TrySelectAsset(pair.Value.path);
				currentAudioEntry  = pair.Value;

			}

			GUI.color = oldColor;
			EditorGUILayout.EndHorizontal ();

		}

	}

    void TrySelectAsset(string path)
    {

	string[] splited = path.Split ('/');
	path = splited [splited.Length-1];

	string[] assets = AssetDatabase.FindAssets(path);

	if ( assets == null || assets.Length < 1 ) return;
	    
	string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

	AudioClip ac = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
	        
	Selection.activeObject = ac;
	   

    }

   

	void DrawEntryBoard ( Dictionary<string, AudioEntry> dictionary ) {

		if (dictionary == null ) return;

		if ( displayId != "" && currentAudioEntry == null ) {
					
			if( dictionary.TryGetValue ( displayId, out currentAudioEntry ) ) {



			}

		}

		if ( currentAudioEntry ==  null ) {
			
			currentAudioEntry = new AudioEntry( );
						
		}

		EditorGUILayout.BeginVertical ("box", GUILayout.Width ( 200 ) );
		
		string key = displayId, path= currentAudioEntry.path;
		int loadLevel = currentAudioEntry.loadLevel;


		AudioType type = currentAudioEntry.type;

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField ( new GUIContent("ID:") , GUILayout.Width ( 50 ) );
		key = EditorGUILayout.TextField ( key );

		EditorGUILayout.EndHorizontal();

		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField(new GUIContent("Path:"), GUILayout.Width(50));
		
		Color oldColor = GUI.color;
		GUI.color = Color.yellow;
		EditorGUILayout.LabelField(path);
		
		GUI.color = oldColor;
		
		EditorGUILayout.EndHorizontal();
		if ( GUILayout.Button ( "Get Asset Path") ) {

			Object selectedObj = Selection.activeObject;

			if ( selectedObj != null ) {

				AudioClip audioClip = (AudioClip) selectedObj;

				if ( audioClip != null ) {

					path = AssetDatabase.GetAssetPath ( audioClip.GetInstanceID() );
                    DebugWrapper.Log(path);
					string[] pathSplitted = path.Split ('/');

					bool startAdding = false;
					
					string fileName = "";

					for ( int splitedIndex = 0; splitedIndex < pathSplitted.Length; splitedIndex ++ ) {

						if ( startAdding ) {

							fileName += pathSplitted [ splitedIndex ] + "/";

						}

						if ( pathSplitted [ splitedIndex ] == "Resources" ) {

							startAdding = true;

						}

					}

					string[] fileNameSplited = fileName.Split('.');
					path = fileNameSplited[0];

                    DebugWrapper.Log(path);

				}

			}

		}

		float inherentVolume = currentAudioEntry.inherentVolume;

		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField ( new GUIContent("Volume:"), GUILayout.Width ( 50 ) );
		inherentVolume = EditorGUILayout.Slider ( inherentVolume, 0.0f,1.0f );
		
		EditorGUILayout.EndHorizontal();

		currentAudioEntry.inherentVolume = inherentVolume;


		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField ( new GUIContent("Load Level:"),  GUILayout.Width ( 60 ) );
		loadLevel = EditorGUILayout.IntField ( loadLevel ,  GUILayout.Width ( 15 ) );

		AudioClipLoadType aclt = currentAudioEntry.loadType;

		EditorGUILayout.Space();

		EditorGUILayout.LabelField ( new GUIContent("Type:"),  GUILayout.Width ( 30 ) );

		aclt = (AudioClipLoadType) EditorGUILayout.EnumPopup ( aclt, GUILayout.Width ( 140 ) );

		currentAudioEntry.loadType = aclt;

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField ( new GUIContent("Type:") );

		type = (AudioType) EditorGUILayout.EnumPopup ( type,  GUILayout.Width (50) );
		
		EditorGUILayout.EndHorizontal();

		
		EditorGUILayout.EndVertical ();

		displayId = key;
		currentAudioEntry.path = path;
		currentAudioEntry.loadLevel = loadLevel;
		currentAudioEntry.type = type;

	}

}

