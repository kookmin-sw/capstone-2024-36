using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;

public class MultiplayerBuildAndRun
{
    #region Window
    [MenuItem("Jobs/Multiplayer/Build/1 Players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    [MenuItem("Jobs/Multiplayer/Build/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Jobs/Multiplayer/Build/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Jobs/Multiplayer/Build/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        string buildPath1 = 
            System.IO.Directory.GetCurrentDirectory() +
            "/Builds/Win64/" + GetProjectName() +
            "1/";

        ProcessStartInfo startInfo = new ProcessStartInfo();
        List<string> scenePaths = new List<string>();
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (s.enabled)
                scenePaths.Add(s.path);
        }

        for (int i = 1; i <= playerCount; i++)
        {
            if (i == 1)
            {
                // disable auto run player

                // GetScenePaths()
                BuildPipeline.BuildPlayer(scenePaths.ToArray(),
                "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.None);
            }
            else
            {
                string i_buildPath =
                    System.IO.Directory.GetCurrentDirectory() +
                    "/Builds/Win64/" + GetProjectName() + 
                    i.ToString() + "/";

                CopyDirectory(buildPath1, i_buildPath, true);

                // i_buildPath += GetProjectName() + ".exe";

                // startInfo.FileName = i_buildPath;
                // Process.Start(startInfo);
            }
        }

        // Current Directory를 지정하기 위해 AutoRunPlayer를 사용하는 대신
        // 아래 함수로 실행시킨다. 
        // 세이브 파일 충돌 방지
        RunWin64(playerCount);
    }

    [MenuItem("Jobs/Multiplayer/Run/1 Players")]
    static void PerformWin64Run1()
    {
        RunWin64(1);
    }

    [MenuItem("Jobs/Multiplayer/Run/2 Players")]
    static void PerformWin64Run2()
    {
        RunWin64(2);
    }

    [MenuItem("Jobs/Multiplayer/Run/3 Players")]
    static void PerformWin64Run3()
    {
        RunWin64(3);
    }

    [MenuItem("Jobs/Multiplayer/Run/4 Players")]
    static void PerformWin64Run4()
    {
        RunWin64(4);
    }

    static void RunWin64(int playerCount)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        for(int i = 1; i <= playerCount ; i++)
        {
            try
            {
                string path =
                    System.IO.Directory.GetCurrentDirectory() +
                    "/Builds/Win64/" + GetProjectName() +
                    i.ToString() + "/";

                UnityEngine.Debug.Log(path);

                startInfo.WorkingDirectory = path;
                startInfo.FileName = path + GetProjectName() + ".exe"; ;
                Process process = Process.Start(startInfo);
            } 
            catch (Win32Exception)
            {
                UnityEngine.Debug.LogError($"build {i} player first then you can run it");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.GetType().ToString() + ":" + e.Message);
            }
        }
    }

    #endregion

    public static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}
