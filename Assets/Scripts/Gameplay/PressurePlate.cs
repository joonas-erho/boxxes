using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Trigger
{
    public Sprite onSprite;
    public Sprite offSprite;

    public SpriteRenderer sr;

    public void Enable()
    {
        sr.sprite = onSprite;       
        on = true;
        CheckDoors();
    }

    public void Disable()
    {
        sr.sprite = offSprite;        
        on = false;
        CheckDoors();
    }
}