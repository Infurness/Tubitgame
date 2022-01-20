
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class XcodeConfigPostBuild {
#if UNITY_IOS
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        var targetGuid = proj.GetUnityMainTargetGuid();
        proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/GoogleService-Info.plist", "GoogleService-Info.plist"));
        proj.WriteToFile(projPath);
        string plistPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
           
        // Get root
        PlistElementDict rootDict = plist.root;
           
        // background location useage key (new in iOS 8)
        rootDict.SetString("NSUserTrackingUsageDescription", "Collect Analytics Data");
           
        // background modes
        
           
        // Write to file
        File.WriteAllText(plistPath, plist.WriteToString());
    }

#endif
    
}

