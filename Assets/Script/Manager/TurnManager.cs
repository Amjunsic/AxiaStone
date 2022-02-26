using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
    WaitForSeconds delay06 = new WaitForSeconds(0.6f);

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    int myNum;
    int random;

    void GameSetup()
    {
        //플레이어 숫자태그 지정
        PV.RPC("InitGameRPC", RpcTarget.AllViaServer);
        PV.RPC("RandomRPC", RpcTarget.MasterClient);
        
        if(fastDraw)
            delay05 = new WaitForSeconds(0.0f);

        myTurn = random == myNum ? true : false;

        //StartCoroutine(StartGameCo()); 
    }

    public IEnumerator StartGameCo()
    {
        GameSetup();

        isLoading = true;
        
        for (int i = 0; i < startCardCount; i++)
        {
            yield return delay05;
            OnAddCard?.Invoke(myTurn);
            yield return delay05;
            OnAddCard?.Invoke(!myTurn);
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;

        yield return delay06;
        OnAddCard.Invoke(myTurn);
        yield return delay06;
        isLoading = false;
        //OnTurnStarted.Invoke(myTurn);
    }

    #region RPC
    [PunRPC]
    void InitGameRPC()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount != 2)
            return;

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
        random = Random.Range(0, 1);
    }
    #endregion
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(random);
        }
        else
        {
            random = (int)stream.ReceiveNext();
        }

    }
}
