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
    public Joystick joystick_Cam;
    public Slider Speed;
    public Slider BodyHigh;
    public ToggleController A_D;
    public Text message;
    public Animator anim_gait;
    public GameObject gait_panel;
    public Text gait_text;
    private string sourceURL;
    private string sourceURL1;
    private bool isData = false;
    private bool isConnected = false;
    private float OffSetAccelY = 0;
    private float gaitValue = 0;
    private float[] oldData = {30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10};
    private float[] newData = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private float[] maxData = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private string[] postStrings = {"&moveDistance=",
                                    "&moveAngle=",
                                    "&turnAngle=",
                                    "&timeScale=",
                                    "&offSetZ=",
                                    "&offSetX=",
                                    "&offSetY=",
                                    "&roll=", 
                                    "&pitch=", 
                                    "&yaw=",
                                    "&homeDistance=",
                                    "&tilt=",
                                    "&pan=",
                                    "&gait="};

    private void OnEnable()
    {
        string Name = PlayerPrefs.GetString("Name","SpiderBot0");
        string Ip = PlayerPrefs.GetString("Ip","10.0.0.247");
        string Port = PlayerPrefs.GetString("Port","5000");

        sourceURL = "http://" + Ip + ":" + Port + "/test";
        sourceURL1 = "http://" + Ip + ":" + Port + "/data";
        StartCoroutine(CheckConnection());
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
            isData = false;
            string data = "?mode=Full";

            // moveDistanceValue
            float temp = 0;
            float moveDistanceValue = Mathf.Sqrt((joystick_Left.Horizontal * joystick_Left.Horizontal) + (joystick_Left.Vertical * joystick_Left.Vertical));
            if (joystick_Left.Vertical  >= 0){temp = 1;}else{temp = -1;}
            newData[0] = (float)(Math.Round((moveDistanceValue * temp * maxData[0]),2));

            // moveAngleValue 
            float moveAngleValue = (Mathf.Atan2(joystick_Left.Horizontal, joystick_Left.Vertical) * Mathf.Rad2Deg);
            newData[1] = (float)Math.Round(moveAngleValue,2);

            // turnAngleValue
            temp = 0;
            float temp1 = 0;
            if (newData[0] != 0){temp = (maxData[2]/2);temp1 = -1;}else{temp = maxData[2];temp1 = 1;}
            newData[2]= (float)Math.Round((joystick_Right.Horizontal * temp * temp1),2);

            // timeScaleValue
            newData[3] = (float) Math.Round((((maxData[3] - 0.1) - (Speed.value * (maxData[3] - 0.1))) + 0.1),2);
            PlayerPrefs.SetFloat("valueTimeScale",Speed.value);

            // offSetZValue
            newData[4] = (float) Math.Round((((BodyHigh.value) * maxData[4]) + 30),2);
            PlayerPrefs.SetFloat("valueOffSetZ",BodyHigh.value);

            // offSetXValue, offSetYValue, rollValue, pitchValue, yawValue
            if (A_D.isOn)
            {
                newData[5] = 0; // offSetXValue Not Implemented Yet
                newData[6] = 0; // offSetYValue Not Implemented Yet
                newData[7] = (float) Math.Round((Input.acceleration.x * maxData[7] * -1),2);
                newData[8] = (float) Math.Round(((Input.acceleration.y - OffSetAccelY) * maxData[8]),2);
                newData[9] = 0; // yawValue Not Implemented Yet
            }else{
                OffSetAccelY = Input.acceleration.y;
                newData[5] = 0;
                newData[6] = 0;
                newData[7] = 0;
                newData[8] = 0;
                newData[9] = 0;
            }

            // homeDistanceValue Not Implemented Yet
            newData[10] = 71;

            // tiltValue
            newData[11] = (float) Math.Round((joystick_Cam.Horizontal * maxData[11]),1);

            // panValue
            newData[12] = (float) Math.Round((joystick_Cam.Vertical  * maxData[12]),1);

            // gait
            newData[13] = (float) Math.Round(gaitValue,0);

            for (int i = 0; i < 14; i++){
                if ((i == 11) || (i == 12)) {
                    if (newData[i] != 0){
                        data += postStrings[i] + newData[i];
                        isData = true;
                    }
                }else{
                    if (newData[i] != oldData[i]){
                        data += postStrings[i] + newData[i];
                        oldData[i] = newData[i];
                        isData = true;
                    }
                }
            }

            //print("Data: " + data);
            message.text = data;

            if (isData == true){
                UnityWebRequest www = UnityWebRequest.Post(sourceURL1+data,"");
                yield return www.SendWebRequest();
            }
            yield return new WaitForSeconds((float)(0.1));
        }
    }

    public void animationGait(bool inOut)
    {   if (inOut == true)
        {
            anim_gait.Play("Opened");
        }else{
            anim_gait.Play("Closed");
        }
        
    }

    public void setGait(float value)
    {
        gaitValue = value;
        gait_text.text = "GAIT " + gaitValue + " SELECTED";
        PlayerPrefs.SetFloat("gait", gaitValue);
    }
    IEnumerator CheckConnection()
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(sourceURL)){
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError){
                isConnected = false;
            }else{
                isConnected =  true;
            }
        }
        
        if (isConnected == false){
            joystick_Right.gameObject.SetActive(false);
            joystick_Left.gameObject.SetActive(false);
            joystick_Cam.gameObject.SetActive(false);
            Speed.gameObject.SetActive(false);
            BodyHigh.gameObject.SetActive(false);
            A_D.gameObject.SetActive(false);
            gait_panel.SetActive(false);
        }else{
            joystick_Right.gameObject.SetActive(true);
            joystick_Left.gameObject.SetActive(true);
            joystick_Cam.gameObject.SetActive(true);
            Speed.gameObject.SetActive(true);
            BodyHigh.gameObject.SetActive(true);
            A_D.gameObject.SetActive(true);
            gait_panel.SetActive(true);

            BodyHigh.value = PlayerPrefs.GetFloat("valueOffSetZ",(float)(0.5));
            Speed.value = PlayerPrefs.GetFloat("valueTimeScale",(float)(0.6));

            maxData[0] =  PlayerPrefs.GetFloat("maxMoveDistance",30);
            maxData[1] =  PlayerPrefs.GetFloat("maxMoveAngle",0);
            maxData[2] =  PlayerPrefs.GetFloat("maxTurnAngle",15);
            maxData[3] =  PlayerPrefs.GetFloat("maxTimeScale",3);
            maxData[4] =  PlayerPrefs.GetFloat("maxOffSetZ",120);
            maxData[5] =  PlayerPrefs.GetFloat("maxOffSetX",30);
            maxData[6] =  PlayerPrefs.GetFloat("maxOffSetY",30);
            maxData[7] =  PlayerPrefs.GetFloat("maxRoll",30);
            maxData[8] =  PlayerPrefs.GetFloat("maxPitch",30);
            maxData[9] =  PlayerPrefs.GetFloat("maxYaw",30);
            maxData[10] = PlayerPrefs.GetFloat("maxHomeDistance",120);
            maxData[11] = PlayerPrefs.GetFloat("maxTilt",5);
            maxData[12] = PlayerPrefs.GetFloat("maxPan",5);
            
            maxData[13] = PlayerPrefs.GetFloat("gait",0);
            OffSetAccelY = 0;
            gaitValue = PlayerPrefs.GetFloat("gait",0);
            gait_text.text = "GAIT " + gaitValue + " SELECTED";
        }
    }
}