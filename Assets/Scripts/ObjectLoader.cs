using System;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    [SerializeField]
    private GenerationManager genManager;
    private void Start()
    {
        //GameObject object3D = new GameObject("GameObject3", typeof(Rigidbody), typeof(BoxCollider));
        //object3D.transform.position = new Vector3(5, 10, 0);
        //var gltf = object3D.AddComponent<GLTFast.GltfAsset>();
        //gltf.Url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";

        
    }

    public void Load3DObjectUntextured(string obj)
    {
        foreach (var anchor in genManager.anchorList)
        {
            Guid id = anchor.Uuid;
            GameObject prefab = genManager.mappings[id];

            if (string.Equals(prefab.name, obj))
            {
                GameObject object3D2 = new GameObject("GenObject");
                object3D2.transform.position = anchor.transform.position;
                object3D2.transform.rotation = anchor.transform.rotation;
                var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
                gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh_00001_.glb";
            }
        }
    }
    
    public void Load3DObject(string obj)
    {
        foreach (var anchor in genManager.anchorList)
        {
            Guid id = anchor.Uuid;
            GameObject prefab = genManager.mappings[id];

            if (string.Equals(prefab.name, obj))
            {
                GameObject object3D2 = new GameObject("GenObject");
                object3D2.transform.position = anchor.transform.position;
                object3D2.transform.rotation = anchor.transform.rotation;
                var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
                gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh.glb";
            }
        }
    }
}
