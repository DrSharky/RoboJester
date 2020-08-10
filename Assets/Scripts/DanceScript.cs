using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceScript : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        //AndroidBridge.speechOutput += ExecuteCommand;
        GoogleVoiceSpeech.command += ExecuteCommand;
    }

    void ExecuteCommand(string command)
    {
        if (command == "dance")
            Dance();
    }

    public void Dance()
    {
        anim.Play("Dance");
    }
}
