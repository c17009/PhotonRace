using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameDirector : Photon.MonoBehaviour{
    private Text UserId;
    private Text CountDown;
    public GameObject CountObj;
    public float countdown = 5;
    public bool startcount = false;
	// Use this for initialization
	void Start () {
        UserId = GameObject.Find("UserId").GetComponent<Text>();
        CountDown = CountObj.GetComponent<Text>();
    }
	
	// Update is called once per frame
    [PunRPC]
	void Update () {
        UserId.text = "UserId: " + PhotonNetwork.player.ID;
        if (!startcount)
        { CountDown.text = "PleaseWait..."; }
        else
        {
            CountdownStart();
        }
    }

    [PunRPC]
    void CountdownStart()
    {
        startcount = true;
        countdown -= Time.deltaTime;
        CountDown.text = ((int)countdown).ToString();

        if(countdown < 1)
        {
            countdown = 5;
            startcount = false;
            CountObj.SetActive(false);


            if (PhotonNetwork.player.ID == 1)
            {
                GameObject.Find("RaceC(Clone)").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None |
                    RigidbodyConstraints.FreezeRotation;
            }else if(PhotonNetwork.player.ID == 2)
            {
                GameObject.Find("RegularC(Clone)").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None |
                    RigidbodyConstraints.FreezeRotation;            }
        }
    }

    public void OnCountStart()
    {
        GetComponent<PhotonView>().RPC("CountdownStart", PhotonTargets.All);
    }
}
