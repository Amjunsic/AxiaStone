using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviourPunCallbacks
{
    #region 변수
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] TMP_Text abilityTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;
    #endregion

    string Name;
    string Attack;
    string Health;
    string Cost;
    string Ability;
    int spriteCount;
    
    public Item item;
    public PhotonView PV;
    public PRS originPRS;
    
    bool isFront;

    #region 카드 셋업
    public void SetUp(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;
        if (isFront)
        {
            character.sprite = AssetManager.Inst.sprites[item.spriteCount];
            nameTMP.text = item.name;
            attackTMP.text = item.attack.ToString();
            healthTMP.text = item.health.ToString();
            costTMP.text = item.cost.ToString();
            abilityTMP.text = item.ability.ToString();

        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
            costTMP.text = "";
            abilityTMP.text = "";
        }
    }

    public void SetUp(string Data, bool isFront)
    {
        var item = JsonUtility.FromJson<Item>(Data);

        this.item = item;
        this.isFront = isFront;
        if (isFront)
        {
            character.sprite = AssetManager.Inst.sprites[item.spriteCount];
            nameTMP.text = item.name;
            attackTMP.text = item.attack.ToString();
            healthTMP.text = item.health.ToString();
            costTMP.text = item.cost.ToString();
            abilityTMP.text = item.ToString();
        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
            costTMP.text = "";
            abilityTMP.text = "";
        }
    }

    [PunRPC]
    public void SetUpRPC(string Data, bool isFront)
    {
        var item = JsonUtility.FromJson<Item>(Data);

        this.item = item;
        this.isFront = isFront;
        if (isFront)
        {
            character.sprite = AssetManager.Inst.sprites[spriteCount];
            nameTMP.text = Name;
            attackTMP.text = Attack;
            healthTMP.text = Health;
            costTMP.text = Cost;
            abilityTMP.text = Ability;

        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
            costTMP.text = "";
            abilityTMP.text = "";
        }
    }
    #endregion


    #region 마우스 위치
    private void OnMouseOver() 
    {
        if(isFront)
            CardManager.Inst.CardMouseOver(this);
    }

    private void OnMouseDown() 
    {
        if(isFront)
            CardManager.Inst.CardMouseDown();    
    }

    private void OnMouseUp() 
    {
        if(isFront)
            CardManager.Inst.CardMouseUp();
    }

    private void OnMouseExit() 
    {
        if(isFront)
            CardManager.Inst.CardMouseExit(this);
    }
    #endregion

    //카드 두트윈 이용 함수
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if(useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale,dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;

        }
    }



}
