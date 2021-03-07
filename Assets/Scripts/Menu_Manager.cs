using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Manager : MonoBehaviour
{
    public Animator anim_settings;
    public Animator anim_about;
    public Animator anim_mainMenu;
    public Animator anim_exitMenu;
    private Animator PreSelected = null;
    public InputField Name;
    public InputField Ip;
    public InputField Port;
    public ToggleController StreamBt;
    public Transform pausedMenu;
    public Text playText;
    public Text textDebug;
    public Text sliderText;
    public Slider VideoSel;
    private string NameData;
    private string IpData;
    private string PortData;
    private string StreamBtData;
    private string VideoSource;

    private void OnEnable()
    {
        Name.text = PlayerPrefs.GetString("Name","SpiderBot");
        Ip.text = PlayerPrefs.GetString("Ip","10.0.0.247");
        Port.text = PlayerPrefs.GetString("Port","5000");
        VideoSource = PlayerPrefs.GetString("VideoSource","FLASK");

        sliderText.text = VideoSource;
        if (VideoSource == "FLASK"){
            VideoSel.value = 0;
        }else if (VideoSource == "MJPG"){
            VideoSel.value = 2;
        }else if (VideoSource == "TCP"){
            VideoSel.value = 1;
        }

        //////////////////////////////////////////////////////
        if (PlayerPrefs.GetString("VideoSource") == "FLASK"){
            StreamBt.isOn = false;
        }else{
            StreamBt.isOn = true;
        }
        StreamBtData = PlayerPrefs.GetString("VideoSource","FLASK");
        //////////////////////////////////////////////////////

        NameData = Name.text;
        IpData = Ip.text;
        PortData = Port.text;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
        }
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        textDebug.text = "Starting";
        StartCoroutine(Connecting());
    }

    IEnumerator Connecting(){
        while (true){
                string sourceURL = "http://" + IpData + ":" + PortData + "/test";
                using (UnityWebRequest uwr = UnityWebRequest.Get(sourceURL)){
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError){
                        playText.text = "Connect SpiderBot";
                        textDebug.text = "No Connected";
                    }else{
                        playText.text = "Play SpiderBot";
                        textDebug.text = uwr.downloadHandler.text;
                        //textDebug.text = "Connected";
                    }
                }
            yield return new WaitForSeconds((float)(1.0));
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void pause()
    {
        if ((anim_exitMenu.gameObject.activeInHierarchy == false) || (anim_mainMenu.GetCurrentAnimatorStateInfo(0).IsName("Opened") == true))
        {
            anim_exitMenu.gameObject.SetActive(true);
            anim_exitMenu.SetBool("Open", false);
            anim_mainMenu.SetBool("Open",true);
            StartCoroutine(DisablePanelDeleyed(anim_mainMenu));

            if (PreSelected != null)
            {
                PreSelected.SetBool("Open",true);
                StartCoroutine(DisablePanelDeleyed(PreSelected));
                PreSelected = null;
            }
        }else{
            anim_exitMenu.SetBool("Open", true);
            StartCoroutine(DisablePanelDeleyed(anim_exitMenu));
            anim_mainMenu.SetBool("Open", false);
        }
    }

    public void SavePlayerPrefs ()
    {
        PlayerPrefs.SetString("Name",Name.text);
        PlayerPrefs.SetString("Ip",Ip.text);
        PlayerPrefs.SetString("Port",Port.text);
        PlayerPrefs.SetString("VideoSource",sliderText.text);
        //////////////////////////////////////////////////////
        if (StreamBt.isOn == false){
            PlayerPrefs.GetString("VideoSource","FLASK");
        }else{
            PlayerPrefs.GetString("VideoSource","TCP");
        }
        StreamBtData = PlayerPrefs.GetString("VideoSource");
        //////////////////////////////////////////////////////
        NameData = Name.text;
        IpData = Ip.text;
        PortData = Port.text;
        ChangeMenu ("Back");
    }

    public void ChangeSlider ()
    {
        if (VideoSel.value == 0){
            sliderText.text = "FLASK";
        }else if (VideoSel.value == 2){
            sliderText.text = "MJPG";
        }else if (VideoSel.value == 1){
            sliderText.text = "TCP";
        }
    }
    public void ChangeScene (string scene)
    {
        textDebug.text = "Button Pushed";
        if (playText.text == "Play SpiderBot"){
            SceneManager.LoadScene(scene);
        }
    }

    public void ChangeMenu (string menu)
    {
        if (menu == "Back")
        {
            Name.text = NameData;
            Ip.text = IpData;
            Port.text = PortData;
            //////////////////////////////////////////////////////
            if (StreamBtData == "FLASK"){
                if (StreamBt.isOn == true)
                {
                    StreamBt.Switching();
                }
            }else{
                if (StreamBt.isOn == false)
                {
                    StreamBt.Switching();
                }
            }
            //////////////////////////////////////////////////////
            if (PreSelected != null)
            {
                PreSelected.SetBool("Open",true);
                StartCoroutine(DisablePanelDeleyed(PreSelected));
                PreSelected = null;
            }
        }else if (menu == "Settings")
        {
            if (PreSelected == null)
            {
                anim_settings.gameObject.SetActive(true);
                anim_settings.SetBool("Open", false);
                PreSelected = anim_settings;
            }else
            {
                PreSelected.SetBool("Open",true);
                StartCoroutine(DisablePanelDeleyed(PreSelected));
                //PreSelected.gameObject.SetActive(false);

                anim_settings.gameObject.SetActive(true);
                anim_settings.SetBool("Open", false);
                PreSelected = anim_settings;
            }
        }else if (menu == "About")
        {
            if (PreSelected == null)
            {
                anim_about.gameObject.SetActive(true);
                anim_about.SetBool("Open", false);
                PreSelected = anim_about;
            }else
            {
                PreSelected.SetBool("Open",true);
                StartCoroutine(DisablePanelDeleyed(PreSelected));
                //PreSelected.gameObject.SetActive(false);

                anim_about.gameObject.SetActive(true);
                anim_about.SetBool("Open", false);
                PreSelected = anim_about;
            }
        }
    }

    IEnumerator DisablePanelDeleyed(Animator anim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
            {
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName("Opened");
            }
			wantToClose = !anim.GetBool("Open");
			yield return new WaitForEndOfFrame();
		}
	}
}