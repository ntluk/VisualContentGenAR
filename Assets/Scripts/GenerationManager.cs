using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meta.XR.BuildingBlocks;
using UnityEngine;
using UnityEngine.UI;

public class GenerationManager : MonoBehaviour
{
    private float xCoordObjectInImage;
    private float yCoordObjectInImage;
    private float lastXCoordObjectInImage;
    private float lastYCoordObjectInImage;
    private bool xCoordObjectInImageUpdated = false;
    private bool yCoordObjectInImageUpdated = false;
    
    private GenerationProcessor genProcess;
    //private TextToObject txt2obj;
    //private TextToImage txt2img;
    //private AnimateImage animImg;
    private ObjectLoader objLoad;
    
    private string objectGenerating;
    
    void Awake()
    {
        genProcess = GetComponent<GenerationProcessor>();
        objLoad = GetComponent<ObjectLoader>();
    }
    
    private void OnEnable()
    {
       // anchorSystem.OnAnchorsLoadCompleted.AddListener(GetAnchors);
    }

    public void OnHover(string obj)
    {
        objectGenerating = obj;
        Debug.LogWarning(obj);
    }
    public void setXcoordImageObject(float x)
    {
        if (!Mathf.Approximately(x, xCoordObjectInImage))
        {
            xCoordObjectInImage = x;
            xCoordObjectInImageUpdated = true;
            //objectGenerating = obj;
            Gen3DModelFromImgCoords();
        }
        Debug.LogWarning("x");
    }

    public void setYcoordImageObject(float y)
    {
        if (!Mathf.Approximately(y, yCoordObjectInImage))
        {
            yCoordObjectInImage = y;
            yCoordObjectInImageUpdated = true;
            //objectGenerating = obj;
            Gen3DModelFromImgCoords();
        }
    }

    private void Gen3DModelFromImgCoords()
    {
        if (xCoordObjectInImageUpdated && yCoordObjectInImageUpdated)
        {
            if (!Mathf.Approximately(xCoordObjectInImage, lastXCoordObjectInImage) || !Mathf.Approximately(yCoordObjectInImage, lastYCoordObjectInImage))
            {
                lastXCoordObjectInImage = xCoordObjectInImage;
                lastYCoordObjectInImage = yCoordObjectInImage;
                xCoordObjectInImageUpdated = false;
                yCoordObjectInImageUpdated = false;

                genProcess.ImageToObject(xCoordObjectInImage, yCoordObjectInImage);
                Debug.LogWarning("img2obj");
                
                //SpawnObjectPreview();
                ShowObjectPreview();
                StartCoroutine(LoadObjectUntexturedFirst(objectGenerating));
            }
        }
    }
    
    public void TranscriptPromptToObject(string prompt)
    {
        genProcess.VoiceToMesh(prompt);
        Debug.LogWarning("transcript2obj");
    }
    
    public void TranscriptPromptToImage(string prompt)
    {
        genProcess.VoiceToImage(prompt);
        Debug.LogWarning("txt2img");
    }
    
    public void AnimatePainting(string imgPath)
    {
        genProcess.AnimateImage(imgPath);
        Debug.LogWarning("animImg");
    }

    private void ShowObjectPreview()
    {
        GameObject.Find(objectGenerating).GetComponentInChildren<MeshRenderer>().enabled = true;
    }
    
    private IEnumerator LoadObjectUntexturedFirst(string obj)
    {
        yield return StartCoroutine(LoadObjectUntextured(obj)); 
        yield return StartCoroutine(LoadObject()); 
    }
    public IEnumerator LoadObject()
    {
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        
        string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D";
        FileInfo fileLatestGlb = null;

        // wait until one file exists
        yield return new WaitUntil(() => 
            new DirectoryInfo(path).GetFiles("*.glb").Length > 0
        );

        // pick the latest
        fileLatestGlb = new DirectoryInfo(path).GetFiles("*.glb")
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        Debug.Log($"New file appeared! Loading {fileLatestGlb.Name}");

        // wait until the file is unlocked
        yield return new WaitUntil(() => !IsFileLocked(fileLatestGlb));
        yield return new WaitForSeconds(0.1f);

        objLoad.Load3DObject();
        Debug.Log("obj loaded");

        yield return new WaitForSeconds(1f);
        // empty folder again
        try
        {
            File.Delete(fileLatestGlb.FullName);
            Debug.Log($"Deleted file: {fileLatestGlb.Name}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to delete file: {e.Message}");
        }
    }

    public IEnumerator LoadObjectUntextured(string obj)
    {
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();

        string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D";
        FileInfo fileLatestGlb = null;

        // wait until one file exists
        yield return new WaitUntil(() =>
            new DirectoryInfo(path).GetFiles("*.glb").Length > 0
        );

        // pick the latest
        fileLatestGlb = new DirectoryInfo(path).GetFiles("*.glb")
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        Debug.Log($"New file appeared! Loading {fileLatestGlb.Name}");

        // wait until the file is unlocked
        yield return new WaitUntil(() => !IsFileLocked(fileLatestGlb));
        yield return new WaitForSeconds(0.1f);

        objLoad.Load3DObjectUntextured(obj);
        Debug.Log("obj loaded");

        yield return new WaitForSeconds(1f);
        // empty folder again
        try
        {
            File.Delete(fileLatestGlb.FullName);
            Debug.Log($"Deleted file: {fileLatestGlb.Name}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to delete file: {e.Message}");
        }
    }
    
    public IEnumerator loadImage(GameObject image, Sprite SpriteMain, Sprite Ergebnis)
    {
        FileInfo fileLatestPng = new DirectoryInfo("C:/Projekte/ComfyUI_windows_portable/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".jpg").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestPng = new DirectoryInfo("D:/Projects/AAGEPlus/ComfyUI_windows_portable/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".png").OrderByDescending(f => f.LastWriteTime).First();

        UnityEngine.Debug.Log($" New file appeared! Loading {fileLatestPng.Name}");

        yield return new WaitUntil(() => !IsFileLocked(fileLatestPng));
        yield return new WaitForSeconds(0.1f);

        UnityEngine.Debug.Log($"Finished loading image.");

        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = texLoadImageSecure(fileLatestPng.FullName, mat.mainTexture as Texture2D);
        SpriteMain = Sprite.Create(mat.mainTexture as Texture2D, new Rect(0.0f, 0.0f, mat.mainTexture.width, mat.mainTexture.height), new Vector2(0.5f, 0.5f), 100.0f);


        Ergebnis = SpriteMain;
        image.GetComponent<Image>().sprite = Ergebnis;
    }
    
    public IEnumerator loadVideo()
    {
        FileInfo fileLatestPng = new DirectoryInfo("C:/Projekte/ComfyUI_windows_portable/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".jpg").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestPng = new DirectoryInfo("D:/Projects/AAGEPlus/ComfyUI_windows_portable/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".png").OrderByDescending(f => f.LastWriteTime).First();

        UnityEngine.Debug.Log($" New file appeared! Loading {fileLatestPng.Name}");

        yield return new WaitUntil(() => !IsFileLocked(fileLatestPng));
        yield return new WaitForSeconds(0.1f);

        UnityEngine.Debug.Log($"Finished loading image.");

        
    }
    
    public static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            return true;
        }

        return false;
    }
    
    public static Texture2D texLoadImageSecure(string _strFilePath, Texture2D _texDefault)
    {
        if (!File.Exists(_strFilePath))
        {
            Debug.Log($"Could not load image {_strFilePath}. File does not exist.");
            return _texDefault;
        }

        Texture2D texture = new Texture2D(1, 1);
        try
        {
            byte[] bytes = File.ReadAllBytes(_strFilePath);
            texture.LoadImage(bytes);
            return texture;
        }
        catch
        {
            Debug.Log($"Could not load image {_strFilePath}. Crashed.");
            return _texDefault;
        }
    }
}