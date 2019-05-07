using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;



public class StreamVideo : MonoBehaviour {

    public Texture2D texture;
	public bool streamOnStart = true;
    bool streaming = false;
    private Stream stream;
    public RawImage frame; 
    private string sourceURL = "http://192.168.1.109:5000/video_feed";
    //private string sourceURL = "http://14.200.98.103:60001/cgi-bin/snapshot.cgi?chn=0&u=admin&p=&q=0&1555220480";

    void Start() {
		if(streamOnStart)
			StartStream();	
    }

    // public void Snapshot ()
	// {
	// 	var request = new HTTP.Request ("GET", sourceURL);
	// 	request.Send ((obj) => {
	// 		texture.LoadImage (obj.Bytes);
	// 	});
    // }
    IEnumerator StreamSnapshots() {
		while(streaming) {
            // UnityWebRequest wr = new UnityWebRequest(sourceURL);
            // DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            // print("Here1");
            // wr.downloadHandler = texDl;
            // yield return wr.SendWebRequest();
            // print("Here2");
            // if(!(wr.isNetworkError || wr.isHttpError)) {
            //     print("Here3");
            //     texture = texDl.texture;
            //     frame.texture = texture;
            // }

			// var request = new HTTP.Request ("GET", sourceURL);
			// request.Send ();
			// while(!request.isDone) yield return null;
			// if(request.response.status == 200) {
			// 	texture.LoadImage (request.response.Bytes);
            //  frame.texture = texture;
			// }

            UnityWebRequest www = UnityWebRequest.Get(sourceURL);
            print("Here1");
            yield return www.SendWebRequest();
            print("Here2");
            // texture = (((DownloadHandlerTexture)www.downloadHandler).texture);
            // print("Here3");
            // frame.texture = texture;
            if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            }
            else {
                // Show results as text
                Debug.Log(www.downloadHandler.data);
    
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }

		}
	}
	
	public void StartStream() {
		if(!streaming) {
			streaming = true;
			StartCoroutine(StreamSnapshots());
		}
    }

    public void StopStream() {
		streaming = false;
    }

}
/* 
    public RawImage frame; 

    private string sourceURL = "http://192.168.1.109:5000/video_feed";
    private Texture2D texture;
    private Stream stream;

    public void Start()
    {
        texture = new Texture2D(2, 2);
        // create HTTP request
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
        //Optional (if authorization is Digest)
        //req.Credentials = new NetworkCredential("", "");
        // get response
        WebResponse resp = req.GetResponse();
        // get response stream
        stream = resp.GetResponseStream();
        StartCoroutine(GetFrame());
    }

    IEnumerator GetFrame()
    {
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
} */