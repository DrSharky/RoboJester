using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatSuite.Devices;
using NatSuite.Examples.Components;

public class Mock : MonoBehaviour
{
    //[SerializeField] private AudioSource microSource;
    private AudioSource playbackSource;
    private WaitForSeconds playbackDelay = new WaitForSeconds(1.0f);
    [SerializeField] bool debug = false;

    IAudioDevice device;
    ClipRecorder recorder;

    private void Awake()
    {
#if !UNITY_EDITOR
        if(debug)
            gameObject.SetActive(false);
#endif
    }

    void Start()
    {
        //AndroidBridge.speechInput += Listen;
        //AndroidBridge.speechOutput += Playback;
        GoogleVoiceSpeech.playback += Playback;

        MediaDeviceQuery deviceQuery = new MediaDeviceQuery(MediaDeviceQuery.Criteria.AudioDevice);
        device = deviceQuery.currentDevice as AudioDevice;
        Debug.Log($"Device: {device}");
    }

    void Playback(AudioClip clip)
    {
        if(playbackSource == null)
            playbackSource = GetComponent<AudioSource>();
        StartCoroutine(Play(clip));
    }
    public IEnumerator Play(AudioClip clip)
    {
        Debug.Log("Play called.");
        playbackSource.clip = clip;
        yield return null;
        Debug.Log("Playback started");
        playbackSource.Play();
    }
}
