using UnityEngine;

public class ConnectedMics : MonoBehaviour
{
    void Start()
    {
        string[] mics = Microphone.devices;
        
        Debug.Log("Connected Mics:");
        foreach (string mic in mics)
        {
            Debug.Log(mic);
        }
    }
}