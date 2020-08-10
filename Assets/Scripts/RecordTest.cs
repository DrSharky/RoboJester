using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordTest : MonoBehaviour
{
    const string pluginName = "com.example.recordhelper.RecordHelper";
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;
    public AudioSource playback;

    public static AndroidJavaClass PluginClass
    {
        get
        {
            if(_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(pluginName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartRecording()
    {
        Debug.Log("Started Recording.....");
        Debug.Log("Started Recording.....");
        Debug.Log("Started Recording.....");
        Debug.Log("Started Recording.....");
        Debug.Log("Started Recording.....");
        Debug.Log("Started Recording.....");
#if UNITY_ANDROID
        PluginInstance.Call("startRecording");
#endif
    }
    string fileName;

    public void StopRecording()
    {
        Debug.Log("Stopped Recording.....");
        Debug.Log("Stopped Recording.....");
        Debug.Log("Stopped Recording.....");
        Debug.Log("Stopped Recording.....");
        Debug.Log("Stopped Recording.....");
        Debug.Log("Stopped Recording.....");

#if UNITY_ANDROID
        fileName = PluginInstance.Call<string>("stopRecording");
#endif
        Debug.Log($"Audio file name: {fileName}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
