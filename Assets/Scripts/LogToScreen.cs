using UnityEngine;
using System.Collections;

public class LogToScreen : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue ();

    void Start ()
    {
        Debug.Log ("Log1");
        Debug.Log ("Log2");
        Debug.Log ("Log3");
        Debug.Log ("Log4");
        InvokeRepeating ("UnLog", 0, 3);
        Invoke ("StartRecievingMessages", 2);
    }

    void StartRecievingMessages ()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable ()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog (string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue (newString);
        if (type == LogType.Log)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue (newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI ()
    {
        GUIStyle headStyle = new GUIStyle ();
        headStyle.fontSize = 30;
        GUILayout.Label (myLog, headStyle);
    }

    void UnLog ()
    {
        myLogQueue.Dequeue ();
    }
}