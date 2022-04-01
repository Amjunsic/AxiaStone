using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using System;

public class TurnManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public static TurnManager Inst {get; private set;}

    private void Awake() => Inst = this;

    public PhotonView PV;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 카드 설정")] int startCardCount;
    [SerializeField] [Tooltip("카드 뽑는 속도")] bool fastDraw;

    [Header("Properties")]
    public bool isLoading;
    public bool myTurn;

    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    public int myNum;
    public int random;

    Hashtable nickNameCP;

    private void Start() 
    {
        nickNameCP = PhotonNetwork.LocalPlayer.CustomProperties;   
    }

    void GameSetup()
    {
        //플레이어 숫자태그 지정
        PV.RPC("InitGameRPC", RpcTarget.AllViaServer);
        PV.RPC("RandomRPC", RpcTarget.MasterClient);
        
        if(fastDraw)
            delay05 = new WaitForSeconds(0.0f);

        myTurn = myNum == random ? true : false;
    }

    //게임 시작할때만 실행, 게임 클라이언트만 실행
    public IEnumerator StartGameCo()
    {
        GameSetup();

        isLoading = true;
        
        //카드 배분 
        for (int i = 0; i < startCardCount; i++)
        {
            yield return delay05;
            OnAddCard?.Invoke(myTurn);
            yield return delay05;
            OnAddCard?.Invoke(!myTurn);
        }
        PV.RPC("StartTurnCoRPC", RpcTarget.AllViaServer);
    }

    IEnumerator StartTurnCo(string NickNameCP)
    {
        print("턴 시작됨");
        isLoading = true;

        yield return delay07;
        if(myTurn)
        {
            GameManager.Inst.NotificationPanel("My Turn");
            yield return delay07;

            //함수를 실행시키는 사람을 판단
            if (PhotonNetwork.IsMasterClient)
            {
                OnAddCard.Invoke(myTurn);
            }
            else
            {
                OnAddCard.Invoke(!myTurn);
            }
            
            yield return delay07;
            EndTurnButton.Inst.Setup(myTurn);
            OnTurnStarted.Invoke(myTurn);
        }
        else if(!myTurn)
        {
            GameManager.Inst.NotificationPanel("Other Turn");
            EndTurnButton.Inst.Setup(myTurn);
            yield return delay07;
        }
        isLoading = false;
        print("턴시작 종료됨");
    }

    public void EndTurn()
    {
        PV.RPC("ChangeTurnRPC", RpcTarget.AllViaServer);
        PV.RPC("StartTurnCoRPC", RpcTarget.AllViaServer);
    }

    #region RPC
    [PunRPC]
    void InitGameRPC()
    {
        print("게임시작");

        for (int i = 0; i < 2; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                myNum = i;
        }
    }

    [PunRPC]
    void RandomRPC()
    {
        random = Random.Range(0, 2);
    }

    [PunRPC]
    void ChangeTurnRPC()
    {
        myTurn = !myTurn;
    }

    [PunRPC]
    void StartTurnCoRPC()
    {
        print("턴시작 호출");
        StartCoroutine(StartTurnCo(nickNameCP["Tag"].ToString()));
    }
    #endregion
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(random);
            stream.SendNext(!myTurn);
        }
        else
        {
            random = (int)stream.ReceiveNext();
            myTurn = (bool)stream.ReceiveNext();
        }

    }
}
