using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]

public class PhotonManager : Photon.MonoBehaviour {

    public InputField NameInput;
    // Use this for initialization
    void Start () {
        //isLocal = GetComponent<PhotonView>().isMine;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    void OnJoinedLobby()
    {
        Debug.Log("join lobby");
        GameObject.Find("CreateRoom").GetComponent<Button>().interactable = true;
        GameObject.Find("RoomIn").GetComponent<Button>().interactable = true;
    }

    public void CreateRoom()
    {
        string userName = NameInput.textComponent.text;
        PhotonNetwork.autoCleanUpPlayerObjects = false;
        //カスタムプロパティ
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        PhotonNetwork.SetPlayerCustomProperties(customProp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;
        //ロビーで見えるルーム情報としてカスタムプロパティのuserName,userIdを使いますよという宣言
        roomOptions.customRoomPropertiesForLobby = new string[] { "userName"};
        roomOptions.maxPlayers = 2; //部屋の最大人数
        roomOptions.isOpen = true; //入室許可する
        roomOptions.isVisible = true; //ロビーから見えるようにする
        //userIdが名前のルームがなければ作って入室、あれば普通に入室する。
        PhotonNetwork.JoinOrCreateRoom(userName, roomOptions, null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom");
        GameObject.Find("StatusText").GetComponent<Text>().text = "OnJoinedRoom";
        GameObject.Find("CreateRoom").GetComponent<Button>().interactable = false;
        GameObject.Find("RoomIn").GetComponent<Button>().interactable = false;
        GameObject.Find("ConnectPhoton").GetComponent<Button>().interactable = false;
        GameObject.Find("Start").GetComponent<Button>().interactable = true;
    }

    //ルーム一覧が取れると
    void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        if (rooms.Length == 0)
        {
            Debug.Log("ルームが一つもありません");
        }
        else
        {
            //ルームが1件以上ある時ループでRoomInfo情報をログ出力
            for (int i = 0; i < rooms.Length; i++)
            {
                Debug.Log("RoomName:" + rooms[i].name);
                Debug.Log("userName:" + rooms[i].customProperties["userName"]);
                Debug.Log("userId:" + rooms[i].customProperties["userId"]);
                GameObject.Find("StatusText").GetComponent<Text>().text = rooms[i].name;
            }
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(NameInput.textComponent.text);
    }

    public void OnStart()
    {
        if (PhotonNetwork.player.ID == 1)
        {
            photonView.RPC("InstantiatePlayer", PhotonTargets.All);
        }
     
    }

    [PunRPC]
    public void InstantiatePlayer()
    {
        Rigidbody rb;
        Vector3 initPos = new Vector3(-68, 3, 79 +  (PhotonNetwork.player.ID * 2));
        if (PhotonNetwork.player.ID == 1) {
            rb = PhotonNetwork.Instantiate("RaceC", initPos,
                       Quaternion.Euler(Vector3.zero), 0).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ;
        }
        else if (PhotonNetwork.player.ID == 2) {
            rb = PhotonNetwork.Instantiate("RegularC", initPos,
                     Quaternion.Euler(Vector3.zero), 0).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ;
        }

        GameObject.Find("Lobby").SetActive(false);
    }
}
