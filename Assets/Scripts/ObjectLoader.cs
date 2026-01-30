using System;
using System.Collections;
using Oculus.Interaction;
using Unity.XR.CoreUtils;
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
        // find preview object in scene and save transform
        // load model ans set transform
        // delete preview object
        GameObject preview = GameObject.Find(obj);
        
        GameObject object3D2 = new GameObject("UntexObject");
        object3D2.transform.position = preview.transform.position;
        //object3D2.transform.rotation = preview.transform.rotation;
        Vector3 objEuler = object3D2.transform.eulerAngles;
        Vector3 previewEuler = preview.transform.eulerAngles;
        
        objEuler.x = previewEuler.x;
        objEuler.y = 270f;
        objEuler.z = previewEuler.z;

        object3D2.transform.eulerAngles = objEuler;

        object3D2.transform.localScale = preview.transform.localScale;
        var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
        gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh_00001_.glb";
        gltf2.Load(gltf2.Url);
        Destroy(preview);

        StartCoroutine(MakeGrabbable(object3D2));


        /*foreach (var anchor in genManager.anchorList)
        {
            Guid id = anchor.Uuid;
            GameObject prefab = genManager.mappings[id];

            if (string.Equals(prefab.name, obj))
            {
                GameObject object3D2 = new GameObject("UntexObject");
                object3D2.transform.position = anchor.transform.position;
                object3D2.transform.rotation = anchor.transform.rotation;
                var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
                gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh_00001_.glb";
            }
        }*/
    }

    private IEnumerator MakeGrabbable(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        var mr = obj.GetComponentInChildren<MeshRenderer>(true);
        GameObject go = mr.gameObject;

        var bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        go.AddComponent<Grabbable>();
        
    }
    
    public void Load3DObject()
    {
        // find untextured object in scene and save transform
        // load model ans set transform
        // delete untexured object
        GameObject untextured = GameObject.Find("UntexObject");
        
        GameObject object3D2 = new GameObject("TexObject");
        object3D2.transform.position = untextured.transform.position;
        //object3D2.transform.rotation = untextured.transform.rotation;
        Vector3 objEuler = object3D2.transform.eulerAngles;
        Vector3 previewEuler = untextured.transform.eulerAngles;
        
        objEuler.x = previewEuler.x;
        objEuler.y = 270f;
        objEuler.z = previewEuler.z;

        object3D2.transform.eulerAngles = objEuler;
        object3D2.transform.localScale = untextured.transform.localScale;
        var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
        gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh.glb";
        gltf2.Load(gltf2.Url);
        Destroy(untextured);
        
        StartCoroutine(MakeGrabbable(object3D2));
        
        /*object3D2.transform.Find("world/tmpoh_bewpp.ply").gameObject.AddComponent<BoxCollider>();
        object3D2.transform.Find("world/tmpoh_bewpp.ply").gameObject.AddComponent<Rigidbody>();
        object3D2.transform.Find("world/tmpoh_bewpp.ply").gameObject.AddComponent<Grabbable>();
        object3D2.transform.Find("world/tmpoh_bewpp.ply").gameObject.GetComponent<Rigidbody>().useGravity = false;
        object3D2.transform.Find("world/tmpoh_bewpp.ply").gameObject.GetComponent<Rigidbody>().isKinematic = true;


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
        }*/
    }

    public void Load3DMesh(string obj)
    {
        //ref hand transform
        
        
        GameObject preview = GameObject.Find(obj);

        GameObject object3D2 = new GameObject("Mesh");
        object3D2.transform.position = preview.transform.position;
        object3D2.transform.rotation = preview.transform.rotation;
        object3D2.transform.localScale = preview.transform.localScale;
        var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
        gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//3D//Hy21_Mesh_00001_.glb";
        gltf2.Load(gltf2.Url);
        Destroy(preview);

        StartCoroutine(MakeGrabbable(object3D2));
    }
}
