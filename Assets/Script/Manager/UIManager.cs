using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;
    [SerializeField] TMP_Text notificationTMP;
    [SerializeField] GameObject emojiPanel;
    [SerializeField] GameObject EmojiBalloon;
    [SerializeField] GameObject OtherEmojiBalloon;
    [SerializeField] Image Emoji;
    [SerializeField] Image OtherEmoji;

    [ContextMenu("ScaleOne")]
    public void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;

    public void Show(string message)
    {
        notificationTMP.text = message;
        Sequence sequence = DOTween.Sequence()
        .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
        .AppendInterval(3f)
        .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));
    }

    public void PopUp(bool isOn)
    {
        if (isOn)
        {
            ScaleOne();
            return;
        }
        else
            ScaleZero();
    }

    public void ShowMyEmoji(int index)
    {
        int emojiIndex = emojiPanel.transform.GetChild(index).GetSiblingIndex();
        Emoji.sprite = AssetManager.Inst.emoji[emojiIndex];
        
         Sequence sequence = DOTween.Sequence()
        .Append(EmojiBalloon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
        .AppendInterval(2f)
        .Append(EmojiBalloon.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));

        PV.RPC("ShowOtherEmoji",RpcTarget.Others, emojiIndex);
    }

    [PunRPC]
    void ShowOtherEmoji(int index)
    {
        OtherEmoji.sprite = AssetManager.Inst.emoji[index];

        Sequence sequence = DOTween.Sequence()
       .Append(OtherEmojiBalloon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
       .AppendInterval(2f)
       .Append(OtherEmojiBalloon.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));
    }

    private void Start() 
    {
        ScaleZero();
    }
}
