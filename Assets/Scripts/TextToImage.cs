using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class TextToImage : MonoBehaviour
{
    public Process process;
    public StreamWriter streamWriter;
    private Thread thread;

    private List<string> liLines = new List<string>();
    private List<string> liErrors = new List<string>();


    public void Start()
    {
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };


        process.Start();
        process.BeginOutputReadLine();

        streamWriter = process.StandardInput;
        if (streamWriter.BaseStream.CanWrite)
        {
            //RunCI();
        }
    }

    public void Run()
    {
        //streamWriter.WriteLine($"cd D:/Projects/AAGEPlus/repo/Python");
        streamWriter.WriteLine($"cd C:/Projekte/AIArtExtendedPlus/Python");
        
        streamWriter.WriteLine($"python txt2img_workflowUDO.py");
        UnityEngine.Debug.Log("Writing: " + $"queueing txt2img");
        
            
    }
}