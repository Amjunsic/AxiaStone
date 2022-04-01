using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class EntityManager : MonoBehaviourPunCallbacks
{
    public static EntityManager Inst {get; private set;}

    [SerializeField] GameObject entityPrefab;
    [SerializeField] List<Entity> myEntities;
    [SerializeField] List<Entity> otherEntities;
    [SerializeField] Entity myEmptyEntity;
    [SerializeField] Entity myBossEntity;
    [SerializeField] Entity otherBossEntity;

    [SerializeField] Transform OtherCardSpawnPoint;

    const int Max_ENTITY_COUNT = 7;//엔티티 최대 소환 개수
    Hashtable nickNameCP;
    public PhotonView PV;

    void Awake() => Inst = this;

    private void Start() 
    {
        // if(PhotonNetwork.IsMasterClient)
        //     PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Tag", "Admin"} });
        // else
        //     PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Tag", "Player" } });

       nickNameCP = PhotonNetwork.LocalPlayer.CustomProperties;    
    }

    #region 프로퍼티
    public bool IsFullMyEntities => myEntities.Count >= Max_ENTITY_COUNT && !ExistMyEmptyEntity;//나의 엔티티가 최대로 소환되었는지 값을 보내는 프로퍼티
    bool IsFullOtherEntities => otherEntities.Count >= Max_ENTITY_COUNT;//상대의 엔티티가 최대로 소환되었는지 값을 보내는 프로퍼티
    bool ExistMyEmptyEntity => myEntities.Exists(x => x == myEmptyEntity);//내 엔티티중에 전투중 사망한 엔티티가 있는지 보내는 프로퍼티
    int MyEmptyEntityIndex => myEntities.FindIndex(x => x == myEmptyEntity);//내 엔티티중에 전투중 사망한 엔티티의 위치를 보내는 프로퍼티
    #endregion

    //필드 정렬
    public void EntityAlignment(bool isMine)
    {
        float targetY = isMine ? -4f : 4f;
        var targetEntities = isMine ? myEntities : otherEntities;

        for(int i = 0; i < targetEntities.Count; i++)
        {
            float targetX = (targetEntities.Count -1) * -3.5f + i * 7f;

            var targetEntity = targetEntities[i];
            targetEntity.originPos = new Vector3(targetX, targetY);
            targetEntity.MoveTranform(targetEntity.originPos, true, 0.5f);
            targetEntity.GetComponent<Order>()?.originOredr(i);
        }
    }

    //패에 있는 드래그 하고있는카드 저장
    public void InsertMyEmptyEntity(float xPos)
    {
        if(IsFullMyEntities)
            return;

        if(!ExistMyEmptyEntity)
            myEntities.Add(myEmptyEntity);

        Vector3 emptyEntityPos = myEmptyEntity.transform.position;
        emptyEntityPos.x = xPos;
        myEmptyEntity.transform.position = emptyEntityPos;

        int _emptyEntityIndex = MyEmptyEntityIndex;
        myEntities.Sort((entity1, entity2) => entity1.transform.position.x.CompareTo(entity2.transform.position.x));
        //정렬을 한번만 실행하도록함
        if(MyEmptyEntityIndex != _emptyEntityIndex)
            EntityAlignment(true);
    }

    public void RemoveMyEmptyEntity()
    {
        if(!ExistMyEmptyEntity)
            return;

        myEntities.RemoveAt(MyEmptyEntityIndex);
        EntityAlignment(true);
    }

    //구현 방향은 본인이 엔티티를 소환했을경우만 처리함
    public bool SpwanEntity(bool isMine, Item item)
    {
        if (IsFullMyEntities || !ExistMyEmptyEntity)
            return false;
        
        int index = MyEmptyEntityIndex;
        string Data = JsonUtility.ToJson(item);
        PV.RPC("SpwanEntityRPC", RpcTarget.AllViaServer, isMine, index, Data, nickNameCP["Tag"].ToString());

        return true;
    }

    //rpc를 호출한 사람이 누구인지 확인할수있는 방법을 찾아서 해결해야함(대충 해결됨)
    [PunRPC]
    void SpwanEntityRPC(bool isMine, int index, string Data, string NickName)
    {
        Hashtable nickNameCP = PhotonNetwork.LocalPlayer.CustomProperties;
        //임시로 닉네임으로만 호출한사람을 확인함
        if(NickName != nickNameCP["Tag"].ToString())
        {
            isMine = !isMine;
        }

        var item = JsonUtility.FromJson<Item>(Data);
        var spawnPos = isMine ? Utils.MousePos : OtherCardSpawnPoint.position;

        var entityobject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityobject.GetComponent<Entity>();

        if(isMine)
        {
            myEntities[MyEmptyEntityIndex] = entity;
            entity.isMine = isMine;
        }
        else
        {
            otherEntities.Insert(index, entity);
            entity.isMine = isMine;
        }
        entity.SetUp(item);
        EntityAlignment(isMine);
    }
}
