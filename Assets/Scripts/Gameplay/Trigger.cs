using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Door[] doors;

    public bool on = false;

    public void CheckDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].CheckTriggers();
        }
    }
}
