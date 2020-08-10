using UnityEngine;
using UnityEngine.UI;
using System;
using static AndroidBridgeUtils;

public class AndroidBridge : MonoBehaviour, IAndroidBridge
{
    [SerializeField] private Button startListeningBtn = null;
    //[SerializeField] private Toggle continuousListeningTgle = null;
    //[SerializeField] private Text resultsTxt = null;
    public static Action<string> speechOutput;
    public static Action speechInput;


    void Start()
    {
        this.gameObject.name = ANDROIDBRIDGE_GO_NAME;
        startListeningBtn.onClick.AddListener(StartListening);
        //continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
    }

    private void StartListening()
    {
        AndroidRunnableCall("StartListening");
        speechInput.Invoke();
    }

    private void SetContinuousListening(bool isContinuous)
    {
        AndroidCall("SetContinuousListening", isContinuous);
    }

    public void OnResult(string recognizedResult)
    {
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);

        //resultsTxt.text = "";
        //for (int i = 0; i < result.Length; i++)
        //{
        //    resultsTxt.text += result[i] + '\n';
        //}
        speechOutput.Invoke(result[0]);
    }
}
