using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;

public class NPPostProcessor  {

    [PostProcessBuild]
    public static void OnPostprocessBuild( BuildTarget buildTarget, string path )
    {
        if ( buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS )
        {
            string projPath = PBXProject.GetPBXProjectPath( path );

            PBXProject proj = new PBXProject();
            proj.ReadFromString( File.ReadAllText( projPath ) );

            string targetName = PBXProject.GetUnityTargetName();
            string target = proj.TargetGuidByName( targetName );

            proj.AddFileToBuild( target, proj.AddFile( "usr/lib/libsqlite3.dylib", "Frameworks/libsqlite3.dylib", PBXSourceTree.Sdk ) );
            proj.AddFileToBuild( target, proj.AddFile( "usr/lib/libz.dylib", "Frameworks/libz.dylib", PBXSourceTree.Sdk ) );
            proj.AddFileToBuild( target, proj.AddFile( "usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk ) );
            proj.AddFileToBuild( target, proj.AddFile( "Frameworks/UserNotifications.framework", "Frameworks/UserNotifications.framework", PBXSourceTree.Sdk ) );

            File.WriteAllText( projPath, proj.WriteToString() );
        }
    }
}
    #endif
