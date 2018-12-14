/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL.. Use is subject to license terms.
 *
 */

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AudioDataManagerWizardWindow : EditorWindow {
    
    public static AudioRegistry registry;
    
    private string displayId;
    private AudioEntry currentAudioEntry;
    private Vector2 mainScroll;

    private List<string> pathList;

    public static void ShowWindow()
    {

        EditorWindow currentWindow = EditorWindow.GetWindow(typeof(AudioDataManagerWizardWindow));

        currentWindow.titleContent = new GUIContent("Audio Registry Add Wizard");
        currentWindow.minSize = new Vector2(289, 505);
        currentWindow.maxSize = new Vector2(289, 505);

    }

    void OnEnable()
    {

        AssetDatabase.Refresh();


    }

    void OnDisable()
    {

        AssetDatabase.Refresh();
        AudioDataManagerWizardWindow.registry = null;

    }

    void GetSelectedAssetPaths()
    {

        if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length < 1) return;

        pathList = new List<string>(); 

        string[] assetGUIDS = Selection.assetGUIDs;

        foreach ( string guid in assetGUIDS ) {

            string path = AssetDatabase.GUIDToAssetPath ( guid );

            if ( AssetDatabase.LoadAssetAtPath<AudioClip> ( path ) != null ) {
			
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
              		pathList.Add(path);

            }

        }
        
    }

    void DrawEditor()
    {

        EditorGUILayout.BeginVertical("box", GUILayout.Width(280), GUILayout.Height(500));
      
        if (pathList != null && pathList.Count > 0)
        {
            DrawEntryBoard( pathList );

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Next"))
            {

                if ( AudioDataManagerWizardWindow.registry != null && AudioDataManagerWizardWindow.registry.data != null &&  (AudioDataManagerWizardWindow.registry.data.ContainsKey(displayId) || (displayId == "" )) )
                {

                    EditorUtility.DisplayDialog("Invalid Key", "The ID must be unique and not string.Empty", "Ok");
                    return;

                }
                else
                {

                    AudioDataManagerWizardWindow.registry.data.Add(displayId, currentAudioEntry);
                    displayId = "";
                    currentAudioEntry = null;
                    pathList.RemoveAt(0);

                }

            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
      
        EditorGUILayout.BeginVertical("ShurikenEffectBg");
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        DrawEntryQueue();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();

    }

    void DrawEntryQueue()
    {

        if (pathList != null && pathList.Count > 0)
        {

            for ( int pathListIndex = 0; pathListIndex < pathList.Count; pathListIndex ++ )
            {

                string path = pathList[pathListIndex];

                Color oldColor = GUI.color;

                if (pathListIndex == 0)
                {
                    EditorGUILayout.LabelField(new GUIContent("Current:"));
                    GUI.color = Color.cyan;
                
                }
                else if (pathListIndex == 1)
                {

                    EditorGUILayout.LabelField(new GUIContent("Next in Queue:"));

                }

                EditorGUILayout.BeginHorizontal("box");

                string reductedPath = path.Split('/')[path.Split('/').Length - 1];

                
                EditorGUILayout.LabelField(new GUIContent(reductedPath));


                EditorGUILayout.EndHorizontal();
                GUI.color = oldColor;

                if (pathListIndex == 0)
                {

                    EditorGUILayout.Space();
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.Space();

                }



            }

        }
        else
        {

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Get Paths From Selected\nAudio Clips", GUILayout.Width(200), GUILayout.Height(50))) {

                GetSelectedAssetPaths();
            
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

        }

    }

    void DrawEntryBoard( List<string> pathList )
    {

        if (currentAudioEntry == null)
        {

            currentAudioEntry = new AudioEntry();
            displayId = "";

        }

        EditorGUILayout.BeginVertical("box", GUILayout.Width(200));

        string key = displayId, path = pathList[0];
        int loadLevel = currentAudioEntry.loadLevel;
        AudioType type = currentAudioEntry.type;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("ID:"), GUILayout.Width(50));
        key = EditorGUILayout.TextField(key);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Path:"), GUILayout.Width(50));

        Color oldColor = GUI.color;
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField(path);

        GUI.color = oldColor;

        EditorGUILayout.EndHorizontal();

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

        EditorGUILayout.LabelField(new GUIContent("Type:"));

        type = (AudioType)EditorGUILayout.EnumPopup(type, GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        displayId = key;
        currentAudioEntry.path = path;
        currentAudioEntry.loadLevel = loadLevel;
        currentAudioEntry.type = type;

    }

    void OnGUI()
    {

        DrawEditor();
        
    }

}
