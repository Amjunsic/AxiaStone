using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] Sprite active;
    [SerializeField] Sprite inactive;
    [SerializeField] Text btnText;

    public void Setup(bool isActive)
    {
        GetComponent<Image>().sprite = isActive ? active : inactive;
        GetComponent<Button>().interactable = isActive;
        btnText.color = isActive ? new Color32(255, 195, 90, 255) : new Color32(55, 55, 55, 255);

    }

    private void Start() 
    {
        Setup(false);
        TurnManager.OnTurnStarted += Setup;    
    }

    private void OnDestroy() 
    {
        TurnManager.OnTurnStarted -= Setup;    
    }
}
