using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Control_Spider : MonoBehaviour
{
    public Joystick joystick_Right;
    public Joystick joystick_Left;
    private float waitTime = 5.0f;
    private float timer = 0.0f;
    //private string sourceURL = "http://192.168.1.117:5000/mov";
    private string sourceURL;
    
    private string mode = "";
    private string data_before = "";
    
    private void OnEnable()
    {
        string Name = PlayerPrefs.GetString("name","Spider");
        string Ip = PlayerPrefs.GetString("ip");
        string Port = PlayerPrefs.GetString("port");

        sourceURL = "http://" + Ip + ":" + Port + "/mov";
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
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                timer = timer - waitTime;

                if (joystick_Right.Vertical* 10.0f >1)
                {
                    mode = "F";
                }else if(joystick_Right.Vertical* 10.0f < -1)
                {
                    mode = "B";
                }else
                {
                    mode = "S";
                }
                string data = "?mode=D&j1_v="+ mode + "&j1_h=20";

                if (data != data_before){
                    data_before = data;
                    UnityWebRequest www = UnityWebRequest.Post(sourceURL+data,"");
                    yield return www.SendWebRequest();
                }else{
                    yield return "No data";
                }
            }

            //if (Input.GetKeyDown(KeyCode.Escape))
            //    SceneManager.LoadScene("Menu");
        }
    }
}