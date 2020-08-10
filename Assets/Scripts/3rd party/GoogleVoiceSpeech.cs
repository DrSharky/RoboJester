﻿//	Copyright (c) 2016 steele of lowkeysoft.com
//        http://lowkeysoft.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
// Acquired from https://github.com/steelejay/LowkeySpeech
//
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;

[RequireComponent (typeof (AudioSource))]

public class GoogleVoiceSpeech : MonoBehaviour
{
	struct ClipData
	{
		public int samples;
	}

	public static Action<string> command;
	public static Action<AudioClip> playback;
	public string apiKey = "AIzaSyDbHxpv3AXZvggZsxmT8PdOuq2J7sFv4xo";

	//A handle to the attached AudioSource
	public AudioSource goAudioSource;
	private bool micConnected = false;
	private int minFreq;
	private int maxFreq;
	private string Response, filePath;
	private const int HEADER_SIZE = 44;

	void Start ()
	{
		//Check if there is at least one microphone connected
		if(Microphone.devices.Length <= 0)
		{
				//Throw a warning message at the console if there isn't
				Debug.LogWarning("Microphone not connected!");
		}
		else //At least one microphone is present
		{
			//Set 'micConnected' to true
			micConnected = true;

			//Get the default microphone recording capabilities
			Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

			//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
			if(minFreq == 0 && maxFreq == 0)
			{
				//...meaning 44100 Hz can be used as the recording sampling rate
				maxFreq = 44100;
			}

			//Get the attached AudioSource component
			//goAudioSource = GetComponent<AudioSource>();
		}
	}

	public void OnButtonPress() 
	{
		//If there is a microphone
		if (micConnected)
		{
			//If the audio from any microphone isn't being recorded
			if(!Microphone.IsRecording(null))
			{

#if UNITY_ANDROID && !UNITY_EDITOR
				goAudioSource = GameObject.FindWithTag("AAA").GetComponent<AudioSource>();
#else
				goAudioSource = gameObject.AddComponent<AudioSource>();
#endif


				//Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource
				goAudioSource.clip = Microphone.Start(Microphone.devices[0], true, 7, maxFreq); //Currently set for a 7 second clip
				
			}
			else //Recording is in progress
			{
				//Case the 'Stop and Play' button gets pressed
				float filenameRand = UnityEngine.Random.Range (0.0f, 10.0f);
				string filename = "testing" + filenameRand;
				Microphone.End(null); //Stop the audio recording
				Debug.Log( "Recording Stopped");

				if (!filename.ToLower().EndsWith(".wav"))
				{
					filename += ".wav";
				}
					
				filePath = Path.Combine("testing/", filename);
				filePath = Path.Combine(Application.persistentDataPath, filePath);
				Debug.Log("Created filepath string: " + filePath);

				// Make sure directory exists if user is saving to sub dir.
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				SavWav.Save (filePath, goAudioSource.clip); //Save a temporary Wav File
				Debug.Log( "Saving @ " + filePath);
				string apiURL = "https://speech.googleapis.com/v1p1beta1/speech:recognize?key=AIzaSyDbHxpv3AXZvggZsxmT8PdOuq2J7sFv4xo";

				Debug.Log( "Uploading " + filePath);
				HttpUploadFile (apiURL, filePath, goAudioSource.clip);				
			}
		}
		else // No microphone
		{
			//Print a red "Microphone not connected!" message at the center of the screen

		}
	}

	public async void HttpUploadFile(string url, string file, AudioClip clip)
	{
		Debug.Log(string.Format("Uploading {0} to {1}", file, url));
		HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
		//wr.ContentType = "audio/l16; rate=16000";
		wr.Method = "POST";
		byte[] buffer = new byte[clip.samples * clip.channels];
		FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
		fs.Read(buffer, 0, buffer.Length);
		string base64 = Convert.ToBase64String(buffer);
		fs.Close();

		using (var streamWriter = new StreamWriter(wr.GetRequestStream()))
		{
			string json = 
				"{" +
					"\"config\":" +
					"{" +
						"\"encoding\":\"LINEAR16\"," +
						"\"sampleRateHertz\":\"16000\"," +
						"\"languageCode\":\"en-us\"" +
					"}," +
					"\"audio\":" +
					"{" +
						"\"content\":" +
						"\""+base64+"\""+
					"}" +
				"}";
			streamWriter.Write(json);
		}

		//var httpResponse = (HttpWebResponse)wr.GetResponse();

		using(WebResponse response = await wr.GetResponseAsync())
		{
			using (var streamReader = new StreamReader(response.GetResponseStream()))
			{
				Response = string.Format("{0}", streamReader.ReadToEnd());
			}

			Debug.Log("Response String: " + Response);

			var jsonresponse = SimpleJSON.JSON.Parse(Response);

			if (jsonresponse != null)
			{
				string resultString = jsonresponse["results"][0].ToString();
				var jsonResults = SimpleJSON.JSON.Parse(resultString);

				if (jsonResults != null)
				{

					string transcripts = jsonResults["alternatives"][0]["transcript"].ToString().Replace("\"", "");

					Debug.Log("transcript string: " + transcripts);

					if (Enum.IsDefined(typeof(VoiceCommands), transcripts))
					{
						command.Invoke(transcripts);
					}
				}

				//command.Invoke(transcripts);
				
				//TextBox.text = transcripts;
			}

			playback.Invoke(goAudioSource.clip);

			File.Delete(filePath); //Delete the Temporary Wav file
		}

		


		//using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
		//{
		//	//var result = streamReader.ReadToEnd();
		//	return string.Format("{0}", streamReader.ReadToEnd());
		//}

		//wr.KeepAlive = true;
		//wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

		//Stream rs = wr.GetRequestStream();
		//FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
		////byte[] buffer = new byte[4096];
		//int bytesRead = 0;
		//while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
		//{
		//	rs.Write(buffer, 0, bytesRead);
		//}
		//fileStream.Close();
		//rs.Close();

		//WebResponse wresp = null;
		//try
		//{
		//	wresp = wr.GetResponse();
		//	Stream stream2 = wresp.GetResponseStream();
		//	StreamReader reader2 = new StreamReader(stream2);

		//	Response = string.Format("{0}", reader2.ReadToEnd());
		//	//Debug.Log("HTTP RESPONSE" +responseString);
		//	//return responseString;

		//}
		//catch(Exception ex)
		//{
		//	Debug.Log(string.Format("Error uploading file error: {0}", ex));
		//	if(wresp != null)
		//	{
		//		wresp.Close();
		//		wresp = null;
		//		//return "Error";
		//	}
		//}
		//finally
		//{
		//	wr = null;
		//}
		//return "empty";
	}
}
		