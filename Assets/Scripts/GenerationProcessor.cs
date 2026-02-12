using UnityEngine;
using System.Diagnostics;

public class GenerationProcessor : MonoBehaviour
{
    private void RunComfy(string args)
    {
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = args,
            WorkingDirectory = @"D:\Projects\VisualContentGenAR\Python",
            //WorkingDirectory = @"C:\Projekte\VisualContentGenAR\Python",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        
        process.OutputDataReceived += (s, e) => {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.Log(e.Data);
        };

        process.ErrorDataReceived += (s, e) => {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.LogError(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    public void ImageToObject(float x, float y, string i)
    {
        RunComfy($"segmentation_workflow.py --x={x} --y={y} --i=\"{i}\"");
        UnityEngine.Debug.Log($"queueing img2obj at ({x}, {y})");
    }
    public void VoiceToMesh(string p)
    {
        RunComfy($"genObjFast.py --p=\"{p}\"");
        UnityEngine.Debug.Log($"queueing txt2obj with prompt: {p}");
    }
    
    public void VoiceToImage(string p)
    {
        RunComfy($"genImg.py --p=\"{p}\"");
        UnityEngine.Debug.Log($"queueing txt2img with prompt: {p}");
    }
    
    public void AnimateImage(string i)
    {
        RunComfy($"animImg.py --i=\"{i}\"");
        UnityEngine.Debug.Log("queueing img2vid");
    }
}