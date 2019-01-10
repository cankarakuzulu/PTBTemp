using System.Collections;
using System.Collections.Generic;
using nopact.Game.Utilities.Spline;
using nopact.ShapeCutter.PlaySession.Track;
using ShapeCutter.PlaySession.Track;
using UnityEditor;
using UnityEngine;

public class SplineThicknessDisplayWindow : EditorWindow
{

    public static SplineThicknessDisplayWindow window;

    private const int VERT_COUNT = 400;
    private TrackMaster master;
    private BezierSpline track, perfect;
    private DifficultySplineHolder[] dsh;
    private DifficultySplineHolder.SplineType splineType;
    private float trackWidth, perfectWidth;
    
    [MenuItem( "no-pact/ShapeCutter/Show Editor Helper" )]
    public static void ShowWindow()
    {
        EditorWindow currentWindow = EditorWindow.GetWindow(typeof(SplineThicknessDisplayWindow));
        
        currentWindow.titleContent = new GUIContent("Spline thickness Displayer");
        currentWindow.minSize = currentWindow.maxSize =  new Vector2( 280, 500 );
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    private void OnGUI()
    {
        DrawEditor();
    }

    void DrawEditor()
    {
        EditorGUILayout.BeginVertical("box");
        master = EditorGUILayout.ObjectField("Track Master:", master, typeof(TrackMaster), true, GUILayout.Width(250)) as TrackMaster;
        splineType = (DifficultySplineHolder.SplineType) EditorGUILayout.EnumPopup(splineType);
        
        if (master != null)
        {
            SerializedObject so = new SerializedObject(master);

            trackWidth = so.FindProperty("size").floatValue;
            perfectWidth = so.FindProperty("perfectTrackSize").floatValue;

            SerializedProperty difficultiesSP = so.FindProperty("difficulties");
            if (difficultiesSP != null && difficultiesSP.isArray && dsh ==  null)
            {
                for (int dIndex = 0; dIndex < difficultiesSP.arraySize; dIndex++)
                {
                    SerializedProperty se = difficultiesSP.GetArrayElementAtIndex(dIndex);
                    var typeIndex = se.FindPropertyRelative("type").intValue;
                    if (typeIndex == (int) splineType)
                    {
                        track = se.FindPropertyRelative("path").objectReferenceValue as BezierSpline;
                        perfect = se.FindPropertyRelative("perfect").objectReferenceValue as BezierSpline;
                        break;
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    
    private void OnSceneGUI(SceneView sceneview)
    {
        if (perfect != null && track != null)
        {
            var sum = 0.0f;
            var delta = 1.0f / VERT_COUNT;

            for (int vertIndex = 0; vertIndex < VERT_COUNT; vertIndex++)
            {
                DrawSplineWidth(sum, track, trackWidth, Color.gray);
                sum += delta;
            }
            
            DrawSplineBody( perfect, perfectWidth, Color.red);
            
        
        }
    }

    private void DrawSplineWidth(float sum, BezierSpline bezierSpline, float width, Color color)
    {
        var trackPoint =bezierSpline.GetPoint(sum);
        trackPoint.y = 0;
        var trackNormal = Vector3.Cross(bezierSpline.GetDirection(sum), Vector3.up).normalized;
        trackNormal.y = 0;
        var p1 = trackPoint + trackNormal * width;
        var p2 = trackPoint - trackNormal * width;
        
        Handles.color = color;
        Handles.DrawLine( p1, p2);
    }
    
    private void DrawSplineBody( BezierSpline bezierSpline, float width, Color color)
    {
        Vector3 pp1 = Vector3.up, pp2 = Vector3.up;
        var sum = 0.0f;
        var delta = 1.0f / VERT_COUNT;

        for (int vertIndex = 0; vertIndex < VERT_COUNT; vertIndex++)
        {
            var trackPoint = bezierSpline.GetPoint(sum);
            trackPoint.y = 0;
            var trackNormal = Vector3.Cross(bezierSpline.GetDirection(sum), Vector3.up).normalized;
            trackNormal.y = 0;
            var p1 = trackPoint + trackNormal * width;
            var p2 = trackPoint - trackNormal * width;
            sum += delta;
            if (vertIndex > 0)
            {
                Handles.color = color;
                Handles.DrawAAConvexPolygon( p1, p2, pp2, pp1 );        
            }
            pp1 = p1;
            pp2 = p2;
        }

        
        
    }
}
