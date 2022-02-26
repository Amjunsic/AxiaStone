using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("StartPanel")]
    public GameObject StartPanel;
    public GameObject StartButton;
    public GameObject WaitingText;

    PhotonView PV;

    private void Start()
    {
        PV = photonView;

        if (master())//방장 일때
        {
            StartButton.SetActive(true);//게임 시작 버튼 활성화
            WaitingText.SetActive(false);//게임이 시작될때까지 기다려 주세요... 텍스트 비활성화
        }
        else// 방장이 아닐때
        {
            StartButton.SetActive(false);//게임시작 버튼 비활성화
            WaitingText.SetActive(true);//게임이 시작될때까지 기다려 주세요... 텍스트 활성화
        }
    }

    bool master()
    {
        return PhotonNetwork.LocalPlayer.IsMasterClient;
    }

    //게임시작 버튼 클릭시 실행되는 메소드
    public void StartGameClick()
    {
        PV.RPC("StartPanelRPC", RpcTarget.AllBuffered);
        StartCoroutine(TurnManager.Inst.StartGameCo());
    }

    [PunRPC]
    void StartPanelRPC()
    {
        StartPanel.SetActive(false);
    }
}
