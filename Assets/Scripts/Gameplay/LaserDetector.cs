using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{
    public ObjectInFront obj;
    public Direction newDir;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            obj = ObjectInFront.WALL;
        }

        if (collision.CompareTag("Box"))
        {
            obj = ObjectInFront.BOX;
        }

        if (collision.CompareTag("Reflector"))
        {
            obj = ObjectInFront.REFLECTOR_BOX;
            //add new direction
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        obj = ObjectInFront.NOTHING;
    }
}
