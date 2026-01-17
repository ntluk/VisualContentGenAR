using System.Collections;
using System.IO;
using System.Linq;
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
    
    private ImageToObject img2obj;
    private TextToObject txt2obj;
    private TextToImage txt2img;
    private AnimateImage animImg;
    private ObjectLoader objLoad;

    void Awake()
    {
        img2obj = GetComponent<ImageToObject>();
        objLoad = GetComponent<ObjectLoader>();
    }

    public void setXcoordImageObject(float x)
    {
        if (!Mathf.Approximately(x, xCoordObjectInImage))
        {
            xCoordObjectInImage = x;
            xCoordObjectInImageUpdated = true;
            Gen3DModelFromImgCoords();
        }
    }

    public void setYcoordImageObject(float y)
    {
        if (!Mathf.Approximately(y, yCoordObjectInImage))
        {
            yCoordObjectInImage = y;
            yCoordObjectInImageUpdated = true;
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

                img2obj.Run(xCoordObjectInImage, yCoordObjectInImage);
                Debug.LogWarning("img2obj");
                
                StartCoroutine(LoadObject());
            }
        }
    }
    
    public void TranscriptPromptToObject(string prompt)
    {
        //txt2obj.Run(prompt);
        Debug.LogWarning("txt2obj");
    }
    
    public void TranscriptPromptToImage(string prompt)
    {
        //txt2img.Run(prompt);
        Debug.LogWarning("txt2img");
    }
    
    public void AnimatePainting(int id)
    {
        //animImg.Run(id);
        Debug.LogWarning("animImg");
    }
    
    public IEnumerator LoadObject()
    {
        //FileInfo fileLatestGlb = new DirectoryInfo("C:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();
        FileInfo fileLatestGlb = new DirectoryInfo("D:/Comfy/ComfyUI_h2_1/ComfyUI/output").GetFiles().Where(x => Path.GetExtension(x.Name) == ".glb").OrderByDescending(f => f.LastWriteTime).First();

        Debug.Log($" New file appeared! Loading {fileLatestGlb.Name}");

        yield return new WaitUntil(() => !IsFileLocked(fileLatestGlb));
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"Finished generating obj.");
        
        objLoad.Load3DObject();
        Debug.Log("obj loaded");
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