using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;

    public Trigger[] triggers;

    public SpriteRenderer sr;

    public Sprite openDoor;
    public Sprite closedDoor;

    public void CheckTriggers()
    {
        bool shouldBeOpened = true;
        for (int i = 0; i < triggers.Length; i++)
        {
            if (!triggers[i].on)
            {
                shouldBeOpened = false;
                break;
            }
        }

        if (shouldBeOpened && !open) OpenDoor();
        else if (!shouldBeOpened && open) CloseDoor();
    }

    private void OpenDoor()
    {
        open = true;
        sr.sprite = openDoor;
    }

    private void CloseDoor()
    {
        open = false;
        sr.sprite = closedDoor;
    }
}
