using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//카드의 원래 위치,회전,크기 저장 클래스
[System.Serializable]
public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }

}

public class Utils : MonoBehaviour
{
    public static Quaternion QI => Quaternion.identity;
}
