using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : Trigger
{
    //The direction that the laser must come in from in order to trigger this
    public Direction dir;

    public Sprite onSprite;
    public Sprite offSprite;

    public SpriteRenderer sr;

    private void Awake()
    {
        if(this.CompareTag("Negative Laser Receiver"))
        {
            Enable();
        }
    }

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
