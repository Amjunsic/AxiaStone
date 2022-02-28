using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public ItemSO itemSO;
    public List<Item> Allitems;
    public GameObject[] slot;
    
    private void Start() 
    {
        Allitems = new List<Item>();

        for(int i = 0; i < itemSO.items.Length; i++)
        {
            Allitems.Add(itemSO.items[i]);
        }
        
        for(int i = 0; i < slot.Length; i++)
        {
            slot[i].SetActive(i < Allitems.Count);
            slot[i].GetComponent<UICard>().SetUp(Allitems[i]);
        }
    }
}
