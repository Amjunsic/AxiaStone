using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class Entity: MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField] Item item;
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] GameObject sleepParticle;
    

    public PhotonView PV;

    public int attack;
    public int health;
    public int liveCount;

    public bool isMine;
    public bool attackAble;
    public bool isBossOrEmpty;
    public Vector3 originPos;

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy() 
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if(isBossOrEmpty)
            return;
        sleepParticle.SetActive(liveCount == TurnManager.Inst.turnCount);
    }

    public void SetUp(Item item)
    {
            this.item = item;

            attack = item.attack;
            health = item.health;

            attackTMP.text = item.attack.ToString();
            healthTMP.text = item.health.ToString();
            nameTMP.text = this.item.name;
            character.sprite = AssetManager.Inst.sprites[item.spriteCount];
            liveCount = TurnManager.Inst.turnCount;
    }

    public void MoveTranform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if(useDotween)
            transform.DOMove(pos, dotweenTime);
        else
            transform.position = pos;
    }

    #region 엔티티위치
    private void OnMouseDown() 
    {
        if(isMine)
            EntityManager.Inst.EntityMouseDown(this);
    }
    
    private void OnMouseUp() 
    {
        if(isMine)
            EntityManager.Inst.EntityMouseUp();
    }

    private void OnMouseDrag() 
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDrag();
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(attack);
            stream.SendNext(health);
        }
        else
        {
            attack = (int)stream.ReceiveNext();
            health = (int)stream.ReceiveNext();
        }
    }
}