using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.IO;

public class GetBuildHash : MonoBehaviour
{
    private void Awake()
    {
      
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private TMP_Text buildHashText;
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR //Do this if outside editor
        TextAsset mytxtData = Resources.Load<TextAsset> ("buildHash");
        string buildHash = mytxtData.text;
        buildHashText.text = buildHash;
#else //If inside editor, so hash can also be seen 
        buildHashText.text = GetHash();
#endif
    }

#if UNITY_EDITOR
    private string GetHash () //Get hash using shell
    {
#if UNITY_EDITOR_WIN
        string shellCmd = "cmd.exe";
        string shellCmdArg = "/c";
#elif UNITY_EDITOR_OSX
	    string shellCmd = "bash";
        string shellCmdArg = "-c";
#endif

        string cmdArguments = shellCmdArg + " \"" + "git rev-parse --short HEAD" + "\"";

        var procStartInfo = new System.Diagnostics.ProcessStartInfo (shellCmd, cmdArguments);
        procStartInfo.RedirectStandardOutput = true;
        procStartInfo.UseShellExecute = false;
        procStartInfo.CreateNoWindow = true;

        System.Diagnostics.Process proc = new System.Diagnostics.Process ();
        proc.StartInfo = procStartInfo;
        proc.Start ();
        //proc.WaitForExit (maxWaitTimeInSec * 1000); //Should we set a wait time? I dont see it necessary since this will only execute in the editor
        string result = proc.StandardOutput.ReadToEnd ();

        return result;
    }
#endif
}
