using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Video;
using UnityEngine.Networking;

public class Stream_Image : MonoBehaviour
{
    public RawImage frame; 
    public Text message;

    private string sourceURL;
    private string sourceURL1;
    private string sourceURL2;
    private Texture2D texture;
    private Stream stream;
    private string mode = "snapshots";       // snapshots or stream

    private void OnEnable()
    {
        string Name = PlayerPrefs.GetString("name","SpiderBot0");
        string Ip = PlayerPrefs.GetString("ip","10.0.0.236");
        string Port = PlayerPrefs.GetString("port","5000");
        string streaming = PlayerPrefs.GetString("streaming","N");

        sourceURL = "http://" + Ip + ":" + Port + "/video_feed";
        sourceURL1 = "http://" + Ip + ":" + Port + "/video_feed2";
        sourceURL2 = "http://" + Ip + ":" + Port + "/";
        
        if (streaming == "N"){
            mode = "snapshots";
        }else{
            mode = "stream";
        }

        //Testing Internet COnnection
        if (CheckConnection(sourceURL2) == false){
            message.text = "Error to connect:\r " + sourceURL2;
            mode = "Error";
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {        
        texture = new Texture2D(2, 2);
        StartCoroutine(GetFrame());
    }

    IEnumerator GetFrame()
    {
        if (mode == "snapshots"){
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
        }else if (mode == "stream"){
            // create HTTP request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
            //Optional (if authorization is Digest)
            //req.Credentials = new NetworkCredential("", "");
            // get response
            WebResponse resp = req.GetResponse();
            // get response stream
            stream = resp.GetResponseStream();
            Byte[] JpegData = new Byte[200000];

            while (true)
            {
                int bytesToRead = FindLength(stream);
                //print(bytesToRead);
                if (bytesToRead == -1)
                {
                    print("End of stream");
                    yield break;
                }

                int leftToRead = bytesToRead;

                while (leftToRead > 0)
                {
                    leftToRead -= stream.Read(JpegData, bytesToRead - leftToRead, leftToRead);
                    yield return null;
                }

                MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);

                texture.LoadImage(ms.GetBuffer());
                frame.texture = texture;
                stream.ReadByte(); // CR after bytes
                stream.ReadByte(); // LF after bytes
            }
        }//else if (mode == "stream"){

        //}
    }

    int FindLength(Stream stream)
    {
        int b;
        string line = "";
        int result = -1;
        bool atEOL = false;
        while ((b = stream.ReadByte()) != -1)
        {
            if (b == 10) continue; // ignore LF char
            if (b == 13)
            { // CR
                if (atEOL)
                {  // two blank lines means end of header
                    stream.ReadByte(); // eat last LF
                    return result;
                }
                if (line.StartsWith("Content-Length:"))
                {
                    result = Convert.ToInt32(line.Substring("Content-Length:".Length).Trim());
                    //print(result);
                }
                else
                {
                    line = "";
                }
                atEOL = true;
            }
            else
            {
                atEOL = false;
                line += (char)b;
                //print(Convert.ToString(line));
            }
        }
        return -1;
    }
    
    bool CheckConnection(string URL)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Timeout = 1000;
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

