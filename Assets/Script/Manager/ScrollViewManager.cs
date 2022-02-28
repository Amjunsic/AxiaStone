using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewManager : MonoBehaviour, IDragHandler, IEndDragHandler//각각 드래그 시작할때, 중일때, 끝날때
{
    #region 스크롤바
    public Scrollbar scrollbar;
    const int SIZE = 3; //ContentVIew가 상속받고 있는 패널의 개수
    float[] pos = new float[SIZE];
    float distance;
    float targetpos;
    bool isDrag;
    #endregion 
    
    void Start() 
    {
        //패널간의 Scrollbar의 valu조절
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) pos[i] = distance * i;
    }

    //패널을 드래그 중일때
    public void OnDrag(PointerEventData eventData) => isDrag = true;
    
    //패널 드래그가 끝났을때
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f) targetpos = pos[i];
        }
    }

    private void Update() 
    {
        //패널 위치 조정
        if(isDrag == false) scrollbar.value = Mathf.Lerp(scrollbar.value, targetpos, 0.1f);

    }
}
