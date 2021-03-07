using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;

public class Stream_Image : MonoBehaviour
{
    public RawImage frame; 
    public Text message;

    private string sourceURL;
    private string sourceURL1;
    private Texture2D texture;
    private Stream stream;
    private string mode = "FLASK";       // FLASK, MJPG, TCP, ERROR
    private string Name;
    private string Ip;
    private string Port;
    private string VideoSource;
    private bool stop = false;

    private void OnEnable()
    {
        Name = PlayerPrefs.GetString("Name","SpiderBot");
        Ip = PlayerPrefs.GetString("Ip","10.0.0.247");
        Port = PlayerPrefs.GetString("Port","8080");
        VideoSource = PlayerPrefs.GetString("VideoSource","FLASK");

        string videomsg = "";
        if (VideoSource == "FLASK"){
            sourceURL1 = "http://" + Ip + ":" + Port + "/video_snapshoots";
            videomsg = "Flask-TCP";
        }else if (VideoSource == "MJPG"){
            sourceURL1 = "http://" + Ip + ":8080/?action=snapshot";
            videomsg = "mjpg-streamer";
        }else if (VideoSource == "TCP"){
            sourceURL1 = Ip;
            videomsg = "Sockets-TCP";
        }
        mode = VideoSource;
        sourceURL = "http://" + Ip + ":" + Port + "/change_video?video_source=" + videomsg;
        
        //Testing Internet COnnection
        if (CheckConnection(sourceURL) == false){
            message.text = "Error to connect:\r " + sourceURL;
            mode = "ERROR";
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        //Application.runInBackground = true;
        texture = new Texture2D(640, 480);
        StartCoroutine(GetFrame());
    }

    IEnumerator GetFrame()
    {
        if ((mode == "FLASK") || (mode == "MJPG")){
            while (true){
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(sourceURL1)){
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError){
                        Debug.Log(uwr.error);
                    }else{
                        // Get downloaded asset bundle
                        texture = DownloadHandlerTexture.GetContent(uwr);
                        frame.texture = texture;
                    }
                }
            }
        }else if (mode == "TCP"){
            TcpClient client;
            NetworkStream stream;
            byte[] image = null;
            bool m_NetworkRunning = true;
            bool isImage = false;
            while (true)
            {
                try{
                    client = new TcpClient();
                    client.Connect(sourceURL1, 8081);           
                    stream = client.GetStream();
                    //Debug.Log("***** Client Connected to the server *****");
                    if (m_NetworkRunning && client.Connected && stream.CanRead)
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        byte[] data0 = reader.ReadBytes(8);
                        int length = Int32.Parse(Encoding.Default.GetString(data0));
                        image = reader.ReadBytes(length);
                        isImage = true;
                    }else{
                        isImage = false;
                    }
                    client.Close();
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                    isImage = false;
                }
                if (isImage == true)
                {
                    //Debug.Log($"received-image byte: {image.Length}");
                    //texture = new Texture2D(640, 480);
                    texture.LoadImage(image);      
                    frame.texture = texture;
                    isImage = false;
                    //Debug.Log("**** Image byte loaded... **** ");    
                }
                yield return new WaitForSeconds((float)(0.01));
            }
        }
    }

    bool CheckConnection(string URL)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Timeout = 1500;
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    
            if (response.StatusCode == HttpStatusCode.OK) return true;
            else return false;
        }
        catch
        {
            return false;
        }
    }

}

