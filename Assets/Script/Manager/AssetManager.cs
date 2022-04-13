using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Inst {get; private set;}

    private void Awake() => Inst = this;

    public Sprite[] sprites;
    public Sprite[] emoji;


}
