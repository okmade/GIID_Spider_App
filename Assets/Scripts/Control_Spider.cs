using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Control_Spider : MonoBehaviour
{
    public Joystick joystick_Right;
    public Joystick joystick_Left;
    private float waitTime = 5.0f;
    private float timer = 0.0f;
    private string sourceURL = "http://192.168.1.107:5000/mov";
    
    private string mode = "";
    private string data_before = "";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PostData());
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
        
                /*if(www.isNetworkError || www.isHttpError) {
                    Debug.Log(www.error);
                }
                else {
                    Debug.Log("Form upload complete!");
                }*/
            }
        }
    }
}