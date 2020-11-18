using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedFloor : MonoBehaviour
{
    public SpriteRenderer sr;

    public Sprite chasm;

    public bool isSafe = true;

    public void Break()
    {
        sr.sprite = chasm;
        isSafe = false;
    }
}
