using System.IO;

using UnityEngine;

using UnityEditor;

using UnityEditor.Callbacks;

using UnityEditor.iOS.Xcode;



#if UNITY_IOS || UNITY_TVOS

// Create specific aliases for iOS.Xcode imports.

// Unity Editor on macOS can report a conflict with other plugins

using PlistDocument = UnityEditor.iOS.Xcode.PlistDocument;

using PlistElementDict = UnityEditor.iOS.Xcode.PlistElementDict;

#endif



public class UpdateXcodeBuildSystemPostProcessor : MonoBehaviour

{

    [PostProcessBuild(0)]

    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)

    {

        if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)

        {

            UpdateXcodeBuildSystem(path);

        }

    }



    private static void UpdateXcodeBuildSystem(string projectPath)

    {
        // Get the path to the Xcode project

        string pbxProjectPath = PBXProject.GetPBXProjectPath(projectPath);

        var pbxProject = new PBXProject();



        // Open the Xcode project

        pbxProject.ReadFromFile(pbxProjectPath);



        // Get the UnityFramework target GUID

        string unityFrameworkTargetGuid =

            pbxProject.GetUnityFrameworkTargetGuid();


        pbxProject.SetBuildProperty(unityFrameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "No");
        var mainTargetPath = pbxProject.GetUnityMainTargetGuid();
        pbxProject.SetBuildProperty(mainTargetPath, "Always embed Swift Standard libraries", "Yes");
        
        pbxProject.SetBuildProperty(mainTargetPath,  "Embedded Content Contains Swift Code", "No");
       






    }

}