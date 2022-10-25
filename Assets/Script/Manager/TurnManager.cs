using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;
using System;
using TMPro;

public class TurnManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public static TurnManager Inst {get; private set;}

    private void Awake() => Inst = this;

    public PhotonView PV;

    #region Develop
    [Header("Develop")]
    [SerializeField] [Tooltip("시작 카드 설정")] int startCardCount;
    [SerializeField] [Tooltip("카드 뽑는 속도")] bool fastDraw;
    #endregion
    
    #region Properties
    [Header("Properties")]
    public bool isLoading;
    public bool myTurn;
    public int turnCount = 0;
    [SerializeField]TMP_Text turnCountText;
    #endregion

    #region WaitForSeconds
    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);
    #endregion

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    public static int myNum; //0아니면 1값만 가지게됨

    void GameSetup()
    {
        //플레이어 숫자태그 지정
        PV.RPC(nameof(InitGameRPC), RpcTarget.AllViaServer);
        
        if(fastDraw)
            delay05 = new WaitForSeconds(0.0f);

        myTurn = PhotonNetwork.IsMasterClient;
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
        PV.RPC(nameof(StartTurnCoRPC), RpcTarget.AllViaServer);
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;

        yield return delay07;
        if(myTurn)
        {
            GameManager.Inst.NotificationPanel("My Turn");
            yield return delay07;
           
            //함수를 실행시키는 사람을 판단-OnAddCard가 MasterClient에서만 실행되기 때문에 비교
            if (PhotonNetwork.IsMasterClient)
                OnAddCard.Invoke(myTurn);
            else
                OnAddCard.Invoke(!myTurn);
            
            yield return delay07;
        }
        else
        {
            GameManager.Inst.NotificationPanel("Other Turn");
            yield return delay07;
        }
        EndTurnButton.Inst.Setup(myTurn);
        OnTurnStarted.Invoke(IsMyTurn());
        isLoading = false;
    }

    public void EndTurn()
    {
        PV.RPC(nameof(ChangeTurnRPC), RpcTarget.AllViaServer);
        PV.RPC(nameof(StartTurnCoRPC), RpcTarget.AllViaServer);
    }

    public bool IsMyTurn()
    {
        int isEven = turnCount % 2;
        if (isEven == myNum) return true;
        return false;
    }

    #region RPC
    [PunRPC]
    void InitGameRPC()
    {

        for (int i = 0; i < 2; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                myNum = i;
        }
    }

    [PunRPC]
    void ChangeTurnRPC()
    {
        turnCount++;
        turnCountText.text = turnCount.ToString();
        myTurn = !myTurn;
    }

    [PunRPC]
    void StartTurnCoRPC()
    {
        StartCoroutine(StartTurnCo());
    }
    #endregion

    #region Synchronization
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(!myTurn);
        }
        else
        {
            myTurn = (bool)stream.ReceiveNext();
        }

    }
    #endregion
}
