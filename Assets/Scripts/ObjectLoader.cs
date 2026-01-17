using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    private void Start()
    {
        //GameObject object3D = new GameObject("GameObject3", typeof(Rigidbody), typeof(BoxCollider));
        //object3D.transform.position = new Vector3(5, 10, 0);
        //var gltf = object3D.AddComponent<GLTFast.GltfAsset>();
        //gltf.Url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";

        
    }

     public void Load3DObject()
    {
        //GameObject object3D2 = new GameObject("GameObject3", typeof(Rigidbody), typeof(BoxCollider));
        GameObject object3D2 = new GameObject("GameObject3");
        object3D2.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        object3D2.transform.Rotate(0.0f, 0.0f, 0.0f, Space.Self);
        var gltf2 = object3D2.AddComponent<GLTFast.GltfAsset>();
        gltf2.Url = "file://D:\\Comfy\\ComfyUI_h2_1\\ComfyUI\\output\\Hy21_Mesh.glb";
    }
}
