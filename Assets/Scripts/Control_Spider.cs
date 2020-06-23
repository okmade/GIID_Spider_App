using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Control_Spider : MonoBehaviour
{
    public Joystick joystick_Right;
    public Joystick joystick_Left;
    public Slider Speed;
    public Slider BodyHigh;
    public ToggleController A_D;
    public Text message;
    private float waitTime = 100000.0f;
    private float timer = 0.0f;
    private string sourceURL;
    private string sourceURL1;
    
    private string data_before = "";
    
    private void OnEnable()
    {
        string Name = PlayerPrefs.GetString("name","SpiderBot0");
        string Ip = PlayerPrefs.GetString("ip","10.0.0.236");
        string Port = PlayerPrefs.GetString("port","5000");

        sourceURL = "http://" + Ip + ":" + Port + "/data";
        sourceURL1 = "http://" + Ip + ":" + Port + "/";

        if (CheckConnection(sourceURL) == false){
            joystick_Right.gameObject.SetActive(false);
            joystick_Left.gameObject.SetActive(false);
            Speed.gameObject.SetActive(false);
            BodyHigh.gameObject.SetActive(false);
            A_D.gameObject.SetActive(false);
        }else{
            joystick_Right.gameObject.SetActive(true);
            joystick_Left.gameObject.SetActive(true);
            Speed.gameObject.SetActive(true);
            BodyHigh.gameObject.SetActive(true);
            A_D.gameObject.SetActive(true);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(PostData());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    IEnumerator PostData()
    {
        while (true){
            //timer += Time.deltaTime;
            //if (timer >= waitTime)
            //{
            //    timer = timer - waitTime;

                float moveDistanceValue = Mathf.Sqrt((joystick_Left.Horizontal * joystick_Left.Horizontal) + (joystick_Left.Vertical * joystick_Left.Vertical));
                float moveAngleValue = (Mathf.Atan2(joystick_Left.Horizontal, joystick_Left.Vertical) * Mathf.Rad2Deg);
                
                string data = "";
                
                data = "?mode=Full&moveDistance=" + Math.Round((moveDistanceValue * 30),2) + 
                       "&moveAngle=" + Math.Round(moveAngleValue,2) + 
                       "&timeScale=" + Math.Round(((2.8 - (Speed.value * 2.7)) + 0.3),2) + 
                       "&turnAngle=" + Math.Round((joystick_Right.Horizontal * 15),2) + 
                       "&offSetZ=" + Math.Round((((BodyHigh.value) * 80) + 20),2);

                if (A_D.isOn)
                {
                    data = data + "&offSetX=0" + 
                                  "&offSetY=0" + 
                                  "&roll=0" + 
                                  "&pitch=20" + 
                                  "&yaw=0";
                }else{
                    data = data + "&offSetX=0" + 
                                  "&offSetY=0" + 
                                  "&roll=0" + 
                                  "&pitch=0" + 
                                  "&yaw=0";
                }
                print("Data: " + data);
                message.text = data;

                if (data != data_before){
                    data_before = data;
                    //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceURL+data);
                    //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //yield return response; 
                    UnityWebRequest www = UnityWebRequest.Post(sourceURL+data,"");
                    yield return www.SendWebRequest();
                }
                yield return new WaitForSeconds((float)(0.01));
            //}
        }
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