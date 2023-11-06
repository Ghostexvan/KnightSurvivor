using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class PythonScriptCall : MonoBehaviour
{
    //[Tooltip("Specify your Python version. EX: If your Python version is 3.9.6, then you'll type ")]
    //public string pythonVersion = "39";

    /// NOTE: WE NEED TO REWORK THIS WHEN OUR UI IS FINISHED/FINALIZED - This will probably be the method we'll be using instead of finding Python using PATH (since every computer is different)
    //[Tooltip("Specify your Python Path")]
    //public static string pythonPath = "";

    [Tooltip("Specify the path to the included Python executable app. This will be a const since this app may or may not be put in. Do have the path lead right to the .exe file.")]
    // public const string pythonAppPath = "E:\\Pycharm\\NLN\\hand-gesture-recognition-mediapipe-main\\dist\\app_v2.exe";
    //public const string pythonAppPath = "E:/Pycharm/NLN/hand-gesture-recognition-mediapipe-main/dist/app_v2/app_v2.exe";
    public string pythonAppPath = "E:\\Pycharm\\NLN\\hand-gesture-recognition-mediapipe-main\\dist\\app_v2\\app_v2.exe";

    private static Process appProcess;
    private bool isActive = false;      // Initial value will be false

    void Awake()
    {
        //pythonPath = FindPythonExecutable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //print(FindPythonExecutable());
        //RunPythonScript();
        isActive = GetComponent<UDPControllable>().isActive;

        if (isActive)
        {
            StartCoroutine(RunOnStart());
            //Time.timeScale = 1f;
        }
        else
        {
            UnityEngine.Debug.LogWarning("UDP is disabled! App will not be started");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Find Python path by using PATH environment
    //string FindPythonExecutable()
    //{
    //    var entries = Environment.GetEnvironmentVariable("path").Split(';');
    //    string python_location = null;

    //    foreach (string entry in entries)
    //    {
    //        if (entry.ToLower().Contains("python"))
    //        {
    //            var breadcrumbs = entry.Split('\\');
    //            foreach (string breadcrumb in breadcrumbs)
    //            {
    //                if (breadcrumb.ToLower().Contains("python"))
    //                {
    //                    python_location += breadcrumb + '\\';
    //                    break;
    //                }
    //                python_location += breadcrumb + '\\';
    //            }
    //            break;
    //        }
    //    }

    //    if (python_location == null)
    //        return null;
    //    return python_location + '\\' + "Python" + pythonVersion + '\\' + "python.exe";
    //}

    //private static void RunPythonScript()
    //{
    //    Process process;

    //    try
    //    {
    //        ProcessStartInfo start = new ProcessStartInfo();
    //        start.FileName = @pythonPath;
    //        start.Arguments = @"""E:\\Pycharm\\NLN\\hand-gesture-recognition-mediapipe-main\\app_v2.py""";
    //        start.UseShellExecute = false;
    //        start.RedirectStandardOutput = true;
    //        //start.WindowStyle = ProcessWindowStyle.Hidden;
    //        //start.CreateNoWindow = true;

    //        process = Process.Start(start);
    //        using (StreamReader reader = process.StandardOutput)
    //        {
    //            string result = reader.ReadToEnd();
    //            Console.Write(result);
    //        }

    //        Application.quitting += () =>
    //        {
    //            if (!process.HasExited)
    //            {
    //                process.Kill();
    //            }
    //        };
    //    }
    //    catch (Exception err)
    //    {
    //        print("Python Script Error: " + err);
    //    }


    //}

    //[RuntimeInitializeOnLoadMethod]
    // Cái này chạy somg song với các hàm khác (pretty much hàm nào cũng vậy tbh)
    // Sở dĩ hàm tạo Map chạy tuần tự được là vì ta đặt mỗi cái là yield return StartCoroutine(<hàm>), với mỗi <hàm> yield return null
    IEnumerator RunOnStart()
    {
        print(Application.dataPath);
        //Time.timeScale = 0f;

        ProcessStartInfo appInfo = new ProcessStartInfo();
        appInfo.FileName = pythonAppPath;
        //appInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //appInfo.CreateNoWindow = true;

        appProcess = Process.Start(appInfo);

        // Event được gọi  khi người dùng thoát game
        //Application.quitting += () =>
        //{
        //    if (!appProcess.HasExited)
        //    {
        //        appProcess.Kill();
        //    }
        //};
        //yield return new WaitForSeconds(20);

        yield return null;
    }

    // Tắt Camera app khi thoát scene
    private void OnDestroy()
    {
        if (appProcess != null)
        {
            if (!appProcess.HasExited)
            {
                appProcess.Kill();
            }

        }
    }

    private void OnApplicationQuit()
    {
        if (appProcess != null)
        {
            if (!appProcess.HasExited)
            {
                appProcess.Kill();
            }
        }
    }
}
