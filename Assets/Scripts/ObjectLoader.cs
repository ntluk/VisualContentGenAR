using System;
using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    [SerializeField]
    private GenerationManager genManager;

    [SerializeField] 
    private GameObject leftHand;

    public void Load3DObjectUntextured(string obj)
    {
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

        var gr = go.AddComponent<Grabbable>();
        gr.InjectOptionalRigidbody(rb);
        
        var hg =go.AddComponent<HandGrabInteractable>();
        hg.InjectOptionalPointableElement(gr);
        hg.InjectRigidbody(rb);
        hg.Enable();
        
        var gi =go.AddComponent<GrabInteractable>();
        gi.InjectOptionalPointableElement(gr);
        gi.InjectRigidbody(rb);
        gi.Enable();
    }
    
    public void Load3DObject()
    {
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
    }

    public void Load3DMesh()
    {
        GameObject object3D2 = new GameObject("Mesh");
        object3D2.transform.position = leftHand.transform.position;
        object3D2.transform.position += Vector3.up * 0.1f;
        object3D2.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        Vector3 meshEuler = object3D2.transform.eulerAngles;
        meshEuler.y = 180f;
        object3D2.transform.eulerAngles = meshEuler;
        
        var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
        gltf2.Url = "file://D://Comfy//ComfyUI_h2_1//ComfyUI//output//mesh//ComfyUI_00001_.glb";
        gltf2.Load(gltf2.Url);

        StartCoroutine(MakeGrabbable(object3D2));
    }
}
