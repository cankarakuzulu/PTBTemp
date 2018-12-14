using UnityEngine;
using UnityEditor;
using nopact.Commons.UI.PanelWorks;
using System.Collections.Generic;

namespace nopact.Editor.Game.UI
{
    
    [ CustomEditor( typeof( GameUI ) )]
    public class GameUIEditor : UnityEditor.Editor
    {

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            GameUI gui = target as GameUI;
            if(gui!= null )
            {
                var so = new SerializedObject( target );
                gui.SortPanels();                
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty( target );
            }
        }

        public override void OnInspectorGUI()
        {
            GameUI gui = target as GameUI;
            if ( gui == null )
            {
                return;
            }
            SerializedObject so = new SerializedObject( target );
            SerializedProperty sp = so.FindProperty( "panels" );

            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            DrawDict( sp );
            if ( EditorGUI.EndChangeCheck() )
            {

                EditorUtility.SetDirty( target );
                so.ApplyModifiedProperties();
             
            }
            EditorGUILayout.EndVertical();

        }

        private void DrawDict( SerializedProperty serializedArray )
        {

            List<string> names = new List<string>();

            EditorGUILayout.BeginVertical( "box" );
            EditorGUILayout.LabelField( new GUIContent( "Panels:" ) );
            if ( serializedArray.arraySize > 0 )
            {
                int delIndex = -1;
                for ( int arrayIndex = 0; arrayIndex < serializedArray.arraySize; arrayIndex++ )
                {

                    var arrayElement = serializedArray.FindPropertyRelative( string.Format( "Array.data[{0}]", arrayIndex.ToString() ) );                    
                    delIndex = Mathf.Max( delIndex, DrawDictElement( arrayElement, arrayIndex, names ));

                }

                if ( delIndex >= 0 )
                {

                    serializedArray.DeleteArrayElementAtIndex( delIndex );

                }
            }
            
            if ( GUILayout.Button("Add") )
            {
                serializedArray.InsertArrayElementAtIndex( 0 );                
            }

            EditorGUILayout.EndVertical();

            
        }

        private int DrawDictElement( SerializedProperty sp, int index, List<string> names )
        {
            int delIndex = -1;
            Color tempColor = GUI.color;
            SerializedProperty spName = sp.FindPropertyRelative( "name" );
            SerializedProperty spPanel = sp.FindPropertyRelative( "panel" );
            if ( names.Contains(spName.stringValue )) {
                GUI.color = Color.yellow;
            }
            else
            {
                names.Add( spName.stringValue );
            }

            GUILayout.BeginVertical( "box" );           

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( new GUIContent( "Key:" ), GUILayout.Width( 50 ) );
            GUI.SetNextControlName( string.Format( "Panel:{0}", index.ToString() ) );
            spName.stringValue = EditorGUILayout.TextField( spName.stringValue );
            if( GUILayout.Button( "X", EditorStyles.miniButtonRight, GUILayout.Width(30 ) ))
            {
                delIndex = index;
            }
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName( string.Format( "Panel:{0}", index.ToString() ) );
            EditorGUILayout.PropertyField( spPanel );
            if ( EditorGUI.EndChangeCheck())
            {

                UIPanelBase panelValue = spPanel.objectReferenceValue as UIPanelBase;

                if ( panelValue != null )
                {
                    spName.stringValue = panelValue.name;
                }               

            }
            GUILayout.EndVertical();
            GUI.color = tempColor;
            return delIndex;

        }
    }


}
