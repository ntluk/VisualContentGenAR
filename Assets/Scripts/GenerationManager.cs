using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meta.XR.BuildingBlocks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;

public class GenerationManager : MonoBehaviour
{
    private float xCoordObjectInImage;
    private float yCoordObjectInImage;
    private float lastXCoordObjectInImage;
    private float lastYCoordObjectInImage;
    private bool xCoordObjectInImageUpdated = false;
    private bool yCoordObjectInImageUpdated = false;
    
    private GenerationProcessor genProcess;
    private ObjectLoader objLoad;
    public RoomManager room;
    
    private string objectGenerating;
    public Material genMat;

    private bool loading = false;
    
    void Awake()
    {
        genProcess = GetComponent<GenerationProcessor>();
        objLoad = GetComponent<ObjectLoader>();
        room = GetComponent<RoomManager>();
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
                string imgPath = "";

                if (objectGenerating.Equals("Preview_Peach") || objectGenerating.Equals("Preview_DarkGrape") || objectGenerating.Equals("Preview_Grape") || objectGenerating.Equals("Preview_Fly"))
                    //imgPath = "D:\\Projects\\VisualContentGenAR\\Assets\\Textures\\Still-Life-of-Fruit-Emilie-Preyer-oil-painting.jpeg";
                    imgPath = "C:\\Projekte\\VisualContentGenAR\\Assets\\Textures\\Still-Life-of-Fruit-Emilie-Preyer-oil-painting.jpeg";
                else
                    //imgPath = "D:\\Projects\\VisualContentGenAR\\Assets\\Textures\\PoM.png";
                    imgPath = "C:\\Projekte\\VisualContentGenAR\\Assets\\Textures\\PoM.png";
                    
                genProcess.ImageToObject(xCoordObjectInImage, yCoordObjectInImage, imgPath);
                Debug.LogWarning("img2obj");
                
                //SpawnObjectPreview();
                ShowObjectPreview();
                if (!loading)
                    StartCoroutine(LoadObjectUntexturedFirst(objectGenerating));
            }
        }
    }
    
    public void TranscriptPromptToObject(string prompt)
    {
        genProcess.VoiceToMesh(prompt);
        Debug.LogWarning("transcript2obj");
        StartCoroutine(LoadMesh());
    }
    
    public void TranscriptPromptToImage(string prompt)
    {
        GameObject image = null;
        image = room.virtualPainting;
        
        foreach (Transform child in image.transform)
        {
            child.gameObject.SetActive(false);
            if (child.name == "Frame")
                child.gameObject.SetActive(true);
        }
        
        image.GetComponent<Renderer>().material = genMat;
        
        genProcess.VoiceToImage(prompt);
        Debug.LogWarning("txt2img");
        Debug.LogWarning(GameObject.Find("VirtualImage"));
        StartCoroutine(LoadImage(GameObject.Find("VirtualImage")));
    }
    
    public void AnimatePainting(string imgPath, int type)
    {
        GameObject image = null;
        Renderer renderer;
        Texture tex;
        
        if (type == 0)
        {
            //imgPath = "D:\\Projects\\VisualContentGenAR\\Assets\\Textures\\1_kI_cbCh6HYSMUqfFAHtK1Q.jpg";
            //imgPath = "C:\\Projekte\\VisualContentGenAR\\Assets\\Textures\\1_kI_cbCh6HYSMUqfFAHtK1Q.jpg";
            image = room.defaultPainting;
        }
        else if (type == 1)
        {
            //imgPath = "D:\\Projects\\VisualContentGenAR\\Assets\\Textures\\Still-Life-of-Fruit-Emilie-Preyer-oil-painting.jpeg";
            //imgPath = "C:\\Projekte\\VisualContentGenAR\\Assets\\Textures\\Still-Life-of-Fruit-Emilie-Preyer-oil-painting.jpeg";
            image = room.virtualPainting;

        }
        
        foreach (Transform child in image.transform)
        {
            child.gameObject.SetActive(false);
            if (child.name == "Frame")
                child.gameObject.SetActive(true);
        }
        
        renderer = image.GetComponent<Renderer>();
        tex = renderer.material.mainTexture;
        imgPath = Path.GetFullPath(AssetDatabase.GetAssetPath(tex));
        Debug.LogWarning(imgPath);
        
        renderer.material = genMat;
        
        genProcess.AnimateImage(imgPath);
        Debug.LogWarning("animImg");
        
        StartCoroutine(LoadVideo(image));
    }

    private void ShowObjectPreview()
    {
        GameObject.Find(objectGenerating).GetComponentInChildren<MeshRenderer>().enabled = true;
    }
    
    private IEnumerator LoadObjectUntexturedFirst(string obj)
    {
        loading = true;
        yield return StartCoroutine(LoadObjectUntextured(obj)); 
        yield return StartCoroutine(LoadObject()); 
    }
    
    public IEnumerator LoadObject()
    {
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();

        //string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D";
        string path = "C:/ComfyUI_h2_1/ComfyUI/output/3D";
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

        GameObject object3D2 = GameObject.Find("TexObject");
        Transform model = object3D2.transform.Find("world");
        if (model == null)
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

        loading = false;
    }

    public IEnumerator LoadObjectUntextured(string obj)
    {
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();

        //string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D";
        string path = "C:/ComfyUI_h2_1/ComfyUI/output/3D";
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
    
    public IEnumerator LoadMesh()
    {
        loading = true;
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();

        //string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output/mesh";
        string path = "C:/ComfyUI_h2_1/ComfyUI/output/mesh";
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

        objLoad.Load3DMesh();
        Debug.Log("mesh loaded");

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
        loading = false;
    }
    
    public IEnumerator LoadImage(GameObject image)
    {
        loading = true;
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        
        //string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output";
        string path = "C:/ComfyUI_h2_1/ComfyUI/output/Images";
        FileInfo fileLatestPng;

        // wait until one file exists
        yield return new WaitUntil(() =>
            new DirectoryInfo(path).GetFiles("*.png").Length > 0
        );

        // pick the latest
        fileLatestPng = new DirectoryInfo(path)
            .GetFiles("*.png")
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        Debug.Log($"New file appeared! Loading {fileLatestPng.Name}");

        // wait until file is unlocked
        yield return new WaitUntil(() => !IsFileLocked(fileLatestPng));
        yield return new WaitForSeconds(0.1f);
        
        // copy file into project folder
        string copy = Path.Combine(
            Application.persistentDataPath,
            fileLatestPng.Name
        );

        File.Copy(fileLatestPng.FullName, copy, true);
        
        if (image.GetComponent<VideoPlayer>())
            Destroy(image.GetComponent<VideoPlayer>());
        
        Texture2D texture = LoadTextureFromFile(copy);
        if(!image.GetComponent<MeshRenderer>())
            Debug.LogWarning("no MR");
        MeshRenderer mr = image.GetComponent<MeshRenderer>();
        
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetTexture("_BaseMap", texture);
        mr.material = mat;

        Debug.Log("img loaded");

        yield return new WaitForSeconds(1f);
        // empty folder again
        try
        {
            File.Delete(fileLatestPng.FullName);
            Debug.Log($"Deleted file: {fileLatestPng.Name}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to delete file: {e.Message}");
        }

        loading = false;
    }
    
    public IEnumerator LoadVideo(GameObject image)
    {
        loading = true;
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        //FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output/3D").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        
        //string path = "D:/Comfy/ComfyUI_h2_1/ComfyUI/output";
        string path = "C:/ComfyUI_h2_1/ComfyUI/output";
        FileInfo fileLatestMp4;

        // wait until one file exists
        yield return new WaitUntil(() =>
            new DirectoryInfo(path).GetFiles("*.mp4").Length > 0
        );

        // pick the latest 
        fileLatestMp4 = new DirectoryInfo(path)
            .GetFiles("*.mp4")
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        Debug.Log($"New file appeared! Loading {fileLatestMp4.Name}");

        // wait until file is unlocked
        yield return new WaitUntil(() => !IsFileLocked(fileLatestMp4));
        yield return new WaitForSeconds(0.1f);
        
        // copy file into project folder
        string copy = Path.Combine(
            Application.persistentDataPath,
            fileLatestMp4.Name
        );
        
        File.Copy(fileLatestMp4.FullName, copy, true);

        // add VideoPlayer
        VideoPlayer videoPlayer = image.AddComponent<VideoPlayer>();
        
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = copy;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

        // render to material
        Material renderMat = new Material(
            Shader.Find("Universal Render Pipeline/Lit")
        );
        MeshRenderer mr = image.GetComponent<MeshRenderer>();
        
        if (mr != null)
        {
            mr.material = renderMat;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = mr;
            videoPlayer.targetMaterialProperty = "_BaseMap"; 
        }
        else
        {
            Debug.LogWarning("No MR");
        }

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();
        Debug.Log("Video playing");

        yield return new WaitForSeconds(1f);
        // empty folder again
        try
        {
            File.Delete(fileLatestMp4.FullName);
            Debug.Log($"Deleted file: {fileLatestMp4.Name}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to delete file: {e.Message}");
        }
        loading = false;
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
    
    Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] data = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(data);
        tex.Apply();
        return tex;
    }

}