using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using System.IO;

class SetBuildHashValue : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild (BuildReport report) //This code will be execute before building the proyect
    {
        #if UNITY_EDITOR_WIN
            string shellCmd = "cmd.exe";
            string shellCmdArg = "/c";
        #elif UNITY_EDITOR_OSX
			string shellCmd = "bash";
            string shellCmdArg = "-c";
        #endif

        string cmdArguments = shellCmdArg + " \"" + "git rev-parse --short HEAD" + "\"";
        Debug.Log ("GitTool.Exec: Attempting to execute command: " + (shellCmd + " " + cmdArguments));

        var procStartInfo = new System.Diagnostics.ProcessStartInfo (shellCmd, cmdArguments);
        procStartInfo.RedirectStandardOutput = true;
        procStartInfo.UseShellExecute = false;
        procStartInfo.CreateNoWindow = true;

        System.Diagnostics.Process proc = new System.Diagnostics.Process ();
        proc.StartInfo = procStartInfo;
        proc.Start ();
        //proc.WaitForExit (maxWaitTimeInSec * 1000);
        string result = proc.StandardOutput.ReadToEnd ();

        string path = Application.dataPath + "/Resources/buildHash.txt"; //Save hash in resources folder, so it can be accessed inside the apk
        Debug.Log (path);
        File.WriteAllText(path,result);      
    }
}

