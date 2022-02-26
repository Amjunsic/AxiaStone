using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardManager : MonoBehaviourPunCallbacks
{
    public static CardManager Inst { get; private set; }

    private void Awake() => Inst = this;

    [Header("Object")]
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform CardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherLeft;
    [SerializeField] Transform otherRight;

    [Header("CardList")]
    [SerializeField] List<Card> MyCards;
    [SerializeField] List<Card> OtherCards;

    List<Item> itemBuffer;

    public PhotonView PV;
    public GameObject Card;
    public bool turn;

    #region 카드셋팅
    //카드 뽑는 메소드
    public Item PopItem()
    {
        //아이템버퍼에 카드가 없을때
        if (itemBuffer.Count == 0)
            PV.RPC("SetUpItemBuffer", RpcTarget.MasterClient);

        Item item = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return item;
    }

    void AddCard(bool isMine)
    {
        PV.RPC("AddCardRPC", RpcTarget.MasterClient, isMine);
    }
    #endregion

    #region 카드정렬
    //카드 레이어 정렬 메소드
    void SetOriginOrder(bool isMine)
    {
        int count = isMine ? MyCards.Count : OtherCards.Count;
        for(int i = 0; i < count; i++)
        {
            var targetCard = isMine ? MyCards[i] : OtherCards[i];
            targetCard.GetComponent<Order>().originOredr(i);
        }
    }

    //카드 정렬 메소드
    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();

        if(isMine)
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, MyCards.Count, 0.5f, Vector3.one * 1.9f);
        else
            originCardPRSs = RoundAlignment(otherLeft, otherRight, OtherCards.Count, -0.5f, Vector3.one * 1.9f);

        var targetCards = isMine ? MyCards : OtherCards;
        for(int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    //원의 방정식을 이용하여 카드 정렬
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];//카드간의 간격 설정 리스트
        List<PRS> result = new List<PRS>(objCount);//결과값

        switch(objCount)
        {
            case 1: objLerps = new float[] {0.5f}; break;
            case 2: objLerps = new float[] {0.27f, 0.73f}; break;
            case 3: objLerps = new float[] {0.1f, 0.5f, 0.9f}; break;
            default:
                //0~1 까지 카드의 개수만큼 나눈 값을 카드간의 간격으로 지정
                float interval = 1f/ (objCount - 1);
                for(int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        //원의방정식 이용하여 카드 회전
        for(int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);//두점사이의 거리 구하기
            var targetRot = Utils.QI;
            
            //카드 1~3 개는 카드회전 필요없음
            if(objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            result.Add(new PRS(targetPos, targetRot, scale));
        }

        return result;
    }
    #endregion

    #region RPC

    //방장만 실행됨
    [PunRPC]
    void AddCardRPC(bool isMine)
    {
        Item item = PopItem();//카드 뽑기 방장만 뽑음
        var cardObject = Instantiate(Card, CardSpawnPoint.position, Utils.QI);//카드 생성
        var card = cardObject.GetComponent<Card>();
        
        string Data = JsonUtility.ToJson(item);//뽑은 카드 보냄
        card.SetUp(item, isMine);

        (isMine ? MyCards : OtherCards).Add(card);

        PV.RPC("OtherCardAdd", RpcTarget.Others, Data, !isMine);

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    //상대방에게만 살행됨
    [PunRPC]
    void OtherCardAdd(string Data, bool isMine)
    {
        var cardObject = Instantiate(Card, CardSpawnPoint.position, Utils.QI);
        var card = cardObject.GetComponent<Card>();
        var item = JsonUtility.FromJson<Item>(Data);
        card.SetUp(item, isMine);

        (isMine ? MyCards : OtherCards).Add(card);

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    [PunRPC]
    //카드 뭉치 설정 메소드
    void SetUpItemBuffer()
    {
        //아이템 버퍼에 카드 저장
        itemBuffer = new List<Item>();
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            Item item = itemSO.items[i];
            for (int j = 0; j < item.percent; j++)
                itemBuffer.Add(item);
        }

        //저장된 카드 뒤섞기
        for (int i = 0; i < itemBuffer.Count; i++)
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
        string itemBufferData = JsonUtility.ToJson(new Serialization<Item>(itemBuffer));
        PV.RPC("SetUpBuffer", RpcTarget.Others, itemBufferData);

    }

    //카드뭉치 동기화 RPC
    [PunRPC]
    void SetUpBuffer(string Data) => itemBuffer = JsonUtility.FromJson<Serialization<Item>>(Data).target;

    #endregion

    private void Start()
    {
        PV.RPC("SetUpItemBuffer", RpcTarget.MasterClient);
        TurnManager.OnAddCard += AddCard;
    }
}
