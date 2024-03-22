using UnityEngine;

// Ref: https://discussions.unity.com/t/is-there-any-way-to-view-the-console-in-a-build/22279/6
public class ConsoleToGUI : MonoBehaviour
{
    public bool doShow = true;

    string myLog = "*begin log";
    string filename = "";
    int kChars = 700;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() 
    { 
        Application.logMessageReceived += Log; 
    }

    void OnDisable() 
    { 
        Application.logMessageReceived -= Log; 
    }

    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.Space)) 
        { 
            doShow = !doShow; 
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) 
        {
            myLog = "";
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        // for onscreen...
        myLog = myLog + "" + logString + "\n============\n";
        if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }

        // for the file ...
        if (filename == "")
        {
            string d = System.Environment.GetFolderPath(
               System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
            System.IO.Directory.CreateDirectory(d);
            string r = Random.Range(1000, 9999).ToString();
            filename = d + "/log-" + r + ".txt";
        }
        try
        {
            System.IO.File.AppendAllText(filename, logString + ""); 
        }
        catch { }
    }

    void OnGUI()
    {
        if (!doShow) 
        { 
            return; 
        }

        /*
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero, Quaternion.identity,
            new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f)
        );
        */

        GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
    }
}