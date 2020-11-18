using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public enum ObjectInFront { NOTHING, WALL, BOX, REFLECTOR_BOX, DOOR, LASER_RECEIVER };

public class Detector : MonoBehaviour
{
    public ObjectInFront obj;
    public PlayerController parent;
    
    public GameObject currentBoxInFront;
    public GameObject currentPlateInFront;
    public GameObject currentLaserInFront;

    private void Update()
    {
        if(currentPlateInFront != null && (parent.transform.position == currentPlateInFront.transform.position))
        {
            currentPlateInFront = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall"))
        {
            parent.blocked = true;
            obj = ObjectInFront.WALL;
        }

        if (collision.CompareTag("Box"))
        {           
            parent.blocked = true;
            obj = ObjectInFront.BOX;
            currentBoxInFront = collision.transform.gameObject;
        }

        if (collision.CompareTag("Reflector Box"))
        {
            parent.blocked = true;
            obj = ObjectInFront.REFLECTOR_BOX;
            currentBoxInFront = collision.transform.gameObject;
        }

        if (collision.CompareTag("Plate"))
        {
            currentPlateInFront = collision.transform.gameObject;
        }

        if (collision.CompareTag("Laser"))
        {
            currentLaserInFront = collision.transform.gameObject;
        }

        if (collision.CompareTag("Door") && !collision.GetComponent<Door>().open)
        {
            parent.blocked = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        obj = ObjectInFront.NOTHING;
        if (collision.CompareTag("Box") || collision.CompareTag("Reflector Box"))
        {
            parent.blocked = false;
            currentBoxInFront = null;
        }
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            parent.blocked = false;
        }
        if (collision.CompareTag("Plate")) currentPlateInFront = null;
        if (collision.CompareTag("Laser")) currentLaserInFront = null;
    }
}
