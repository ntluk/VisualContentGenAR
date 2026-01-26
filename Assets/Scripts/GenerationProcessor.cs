using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class GenerationProcessor : MonoBehaviour
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

    public void ImageToObject(float x, float y)
    {
        streamWriter.WriteLine($"D:");
        streamWriter.WriteLine($"cd D:\\Projects\\VisualContentGenAR\\Python");
        //streamWriter.WriteLine($"cd C:\\Projekte\\NiklasTluk\\VisualContentGenAR\\Pythonn");
        
        //streamWriter.WriteLine($"python segmentation_workflow.py --x=1260.0 --y=600.0");
        streamWriter.WriteLine($"python segmentation_workflow.py --x=" + x + " --y=" + y);
        UnityEngine.Debug.Log("Writing: " + $"queueing img2obj at (" + x + ", " + y + ")");
        
            
    }
    public void VoiceToMesh(string p)
    {
        streamWriter.WriteLine($"D:");
        streamWriter.WriteLine($"cd D:\\Projects\\VisualContentGenAR\\Python");
        //streamWriter.WriteLine($"cd C:\\Projekte\\NiklasTluk\\VisualContentGenAR\\Pythonn");
        
        //streamWriter.WriteLine($"python segmentation_workflow.py --x=1260.0 --y=600.0");
        streamWriter.WriteLine($"python genObjFast.py --p=" + p);
        UnityEngine.Debug.Log("Writing: " + $"queueing img2obj with prompt:" + p);
        
            
    }
}