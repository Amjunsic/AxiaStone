using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Inst {get; private set;}
    void Awake() => Inst = this;

    [Header("StartPanel")]
    public GameObject StartPanel;
    public GameObject StartButton;
    public GameObject WaitingText;
    public Text PlayerCount;

    [Header("NotificationPanel")]
    [SerializeField]UIManager notificationPanel;

    [Header("NickName")]
    [SerializeField] TMP_Text MyNickName;
    [SerializeField] TMP_Text OtherNickName;

    PhotonView PV;
    //string OtherPlayerNickName = PhotonNetwork.PlayerListOthers[0].NickName;

    private void Start()
    {
        PV = photonView;
            
        if (PhotonNetwork.LocalPlayer.IsMasterClient)//방장 일때
        {
            StartButton.SetActive(true);//게임 시작 버튼 활성화
            WaitingText.SetActive(false);//게임이 시작될때까지 기다려 주세요... 텍스트 비활성화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Owner", "Admin" } });
        }
        else// 방장이 아닐때
        {
            StartButton.SetActive(false);//게임시작 버튼 비활성화
            WaitingText.SetActive(true);//게임이 시작될때까지 기다려 주세요... 텍스트 활성화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Owner", "Player" } });
        }
    }

    private void Update() 
    {
        //출력되는 텍스트: 방에 입장한 사람수 / 2
        PlayerCount.text = PhotonNetwork.PlayerList.Length.ToString() + " " + "/" + " " + "2";
    }

    //게임시작 버튼 클릭시 실행되는 메소드
    public void StartGameClick()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount !=2)
            return;

        PV.RPC("StartPanelRPC", RpcTarget.AllBuffered);
        StartCoroutine(TurnManager.Inst.StartGameCo());
    }

    public void NotificationPanel(string message)
    {
        notificationPanel.Show(message);
    }

    [PunRPC]
    void StartPanelRPC()
    {
        StartPanel.SetActive(false);

        MyNickName.text = PhotonNetwork.NickName;
        OtherNickName.text = PhotonNetwork.PlayerListOthers[0].NickName;
    }
}
