using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

using NativeWebSocket;

[System.Serializable]
public class DeepgramResponse
{
    public int[] channel_index;
    public bool is_final;
    public Channel channel;
}

[System.Serializable]
public class Channel
{
    public Alternative[] alternatives;
}

[System.Serializable]
public class Alternative
{
    public string transcript;
}

public class DeepgramInstance : MonoBehaviour
{
    WebSocket websocket;

    public Ball _ball;

    async void Start()
    {
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Token INSERT_YOUR_API_KEY" }
        };
        websocket = new WebSocket("wss://api.deepgram.com/v1/listen?encoding=linear16&sample_rate=" + AudioSettings.outputSampleRate.ToString(), headers);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to Deepgram!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage: " + message);
            
            DeepgramResponse deepgramResponse = new DeepgramResponse();
            object boxedDeepgramResponse = deepgramResponse;
            EditorJsonUtility.FromJsonOverwrite(message, boxedDeepgramResponse);
            deepgramResponse = (DeepgramResponse) boxedDeepgramResponse;
            if (deepgramResponse.is_final)
            {
                var transcript = deepgramResponse.channel.alternatives[0].transcript;
                Debug.Log(transcript);
                int leftCount = new Regex(Regex.Escape("left")).Matches(transcript).Count;
                int rightCount = new Regex(Regex.Escape("right")).Matches(transcript).Count;
                int upCount = new Regex(Regex.Escape("up")).Matches(transcript).Count;
                int downCount = new Regex(Regex.Escape("down")).Matches(transcript).Count;
                for (int i = 0; i < leftCount; i++)
                {
                    _ball.PushLeft();
                }
                for (int i = 0; i < rightCount; i++)
                {
                    _ball.PushRight();
                }
                for (int i = 0; i < upCount; i++)
                {
                    _ball.PushUp();
                }
                for (int i = 0; i < downCount; i++)
                {
                    _ball.PushDown();
                }
            }
        };

        await websocket.Connect();
    }
    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
    #endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void ProcessAudio(byte[] audio)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(audio);
        }
    }
}

