using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UICard : MonoBehaviour
{

    #region 변수
    [SerializeField] Image card;
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] TMP_Text abilityTMP;
    #endregion

    public Item item;

    public void SetUp(Item item)
    {
        this.item = item;
            character.sprite = AssetManager.Inst.sprites[item.spriteCount];
            nameTMP.text = item.name;
            attackTMP.text = item.attack.ToString();
            healthTMP.text = item.health.ToString();
            costTMP.text = item.cost.ToString();
            abilityTMP.text = item.ability.ToString();

    }
}
