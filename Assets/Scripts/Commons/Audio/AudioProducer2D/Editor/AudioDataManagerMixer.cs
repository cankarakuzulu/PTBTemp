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

public class AudioDataManagerMixer : EditorWindow
{

    public static AudioRegistry registry;

    private string displayId;
    private AudioEntry currentAudioEntry;
    private Vector2 mainScroll;

    private string filterString ="";
    private AudioDataManagerEditor.FilterAudioType filterAudioType;

    private List<string> holdList;

    public static void ShowWindow()
    {

        EditorWindow currentWindow = EditorWindow.GetWindow(typeof(AudioDataManagerMixer));

        currentWindow.titleContent = new GUIContent("Audio Registry Sound Mixer");
        currentWindow.minSize = new Vector2(1280, 500);
        currentWindow.maxSize = new Vector2(1280, 500);

    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {
        registry = null;
    }

    public void OnGUI()
    {

        if (registry == null) return;

        Dictionary<string, AudioEntry> data = registry.data;
               
        DrawMixer( ApplyFilter ( data )  );

    }

    private float ConvertToDb(float ratio)
    {

        float decibels = Mathf.Log10(ratio) * 10;

        return decibels;

    }

    private float ConvertFromDb(float decibels)
    {

        float ratio = Mathf.Pow(10, (decibels / 10));
        return ratio;

    }

    

    private void DrawMixer(Dictionary<string, AudioEntry> data)
    {
        
        EditorGUILayout.BeginHorizontal("box", GUILayout.Width(400), GUILayout.Height(25));
        
        EditorGUILayout.LabelField(new GUIContent("Filter:"), GUILayout.Width(35));
        filterString = EditorGUILayout.TextField(filterString, GUILayout.Width(90));
        EditorGUILayout.LabelField(new GUIContent("Audio Type:"), GUILayout.Width(85));
        filterAudioType = (AudioDataManagerEditor.FilterAudioType)EditorGUILayout.EnumPopup(filterAudioType, GUILayout.Width(40));
        
        EditorGUILayout.EndHorizontal();        
        EditorGUILayout.BeginVertical("ShurikenEffectBg");
        
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        EditorGUILayout.BeginHorizontal();

        foreach (KeyValuePair<string,AudioEntry> kvp in data)
        {

            EditorGUILayout.BeginVertical("ChannelStripBg", GUILayout.Width ( 85) );
            EditorGUILayout.SelectableLabel( kvp.Key , GUILayout.Width ( 80 ) );
                      
            EditorGUILayout.LabelField(ConvertToDb(kvp.Value.inherentVolume).ToString("F") +" dB", GUILayout.Width ( 75));

            EditorGUILayout.BeginHorizontal("ShurikenEffectBg", GUILayout.Height ( 310 ));

            EditorGUILayout.BeginHorizontal("box");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            char[] nameChArray = kvp.Key.ToCharArray();

            for ( int charIndex = 0; charIndex < nameChArray.Length; charIndex ++ ) {

                sb.Append ( nameChArray[charIndex]).Append('\n');

            }
            
            EditorGUILayout.LabelField(sb.ToString(), GUILayout.Width(15), GUILayout.Height(290));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal("ShurikenEffectBg",GUILayout.Height ( 300) );

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            Color oldcolor = GUI.color;
            GUI.color = Color.yellow;
            kvp.Value.inherentVolume = ConvertFromDb( GUILayout.VerticalSlider(ConvertToDb( kvp.Value.inherentVolume ), 0f, -25.0f, GUILayout.Width(10), GUILayout.Height(285)) );
            GUI.color = oldcolor;
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal ();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(new GUIContent("Pan"), GUILayout.Width ( 25 ) );

            EditorGUILayout.Knob ( new Vector2(25,25), 0.0f, -1.0f, 1.0f,"",Color.black, Color.yellow, true );

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            Color oldCol = GUI.color;

            if (holdList != null && holdList.Contains(kvp.Key))
            {

                GUI.color = Color.yellow;

            }

            if (GUILayout.Button("Hold")) {

                if (holdList == null)
                {

                    holdList = new List<string>();
                    
                }

                if (holdList.Contains(kvp.Key))
                {

                    holdList.Remove(kvp.Key);

                }
                else
                {

                    holdList.Add(kvp.Key);

                }
            
            }

            GUI.color = oldCol;

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    Dictionary<string, AudioEntry> ApplyFilter(Dictionary<string, AudioEntry> dictionary)
    {

        Dictionary<string, AudioEntry> resultingDictionary = new Dictionary<string, AudioEntry>();

        foreach (KeyValuePair<string, AudioEntry> kvp in dictionary)
        {
            if (holdList == null || !holdList.Contains(kvp.Key))
            {

                switch (filterAudioType)
                {

                    case AudioDataManagerEditor.FilterAudioType.MUSIC:

                        if (kvp.Value.type == AudioType.SFX)
                        {

                            continue;

                        }

                        break;

                    case AudioDataManagerEditor.FilterAudioType.SFX:

                        if (kvp.Value.type == AudioType.MUSIC)
                        {

                            continue;

                        }

                        break;

                }



                string nameFilterSource = kvp.Key;

                nameFilterSource = nameFilterSource.ToLowerInvariant();

                string lowerFilterKey = filterString.ToLowerInvariant();

                if (!nameFilterSource.Contains(lowerFilterKey) && lowerFilterKey != "")
                {

                    continue;

                }
            }

            resultingDictionary.Add(kvp.Key, kvp.Value);

        }

        return resultingDictionary;
    }



}
