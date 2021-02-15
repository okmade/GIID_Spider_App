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
    private string NameData;
    private string IpData;
    private string PortData;
    private string StreamBtData;

    private void OnEnable()
    {
        Name.text = PlayerPrefs.GetString("name","SpiderBot");
        Ip.text = PlayerPrefs.GetString("ip","10.0.0.247");
        Port.text = PlayerPrefs.GetString("port","5000");
        if (PlayerPrefs.GetString("streaming","Y") == "N"){
            StreamBt.isOn = false;
        }else{
            StreamBt.isOn = true;
        }

        NameData = Name.text;
        IpData = Ip.text;
        PortData = Port.text;
        StreamBtData = PlayerPrefs.GetString("streaming","Y");
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
        PlayerPrefs.SetString("name",Name.text);
        PlayerPrefs.SetString("ip",Ip.text);
        PlayerPrefs.SetString("port",Port.text);
        if (StreamBt.isOn == false){
            PlayerPrefs.SetString("streaming","N");
        }else{
            PlayerPrefs.SetString("streaming","Y");
        }
        StreamBtData = PlayerPrefs.GetString("streaming");
        NameData = Name.text;
        IpData = Ip.text;
        PortData = Port.text;
        ChangeMenu ("Back");
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
            if (StreamBtData == "N"){
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