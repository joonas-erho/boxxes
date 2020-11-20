using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;



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

        Vector3 position = transform.position;
        

        switch (parent.dir)
        {
            case Direction.LEFT:
                position.x -= 1f;
                break;
            case Direction.RIGHT:
                position.x += 1f;
                break;
            case Direction.UP:
                position.y += 1f;
                break;
            case Direction.DOWN:
                position.y -= 1f;
                break;
        }

        Vector3 bottomCorner = position;
        bottomCorner.x -= 0.4f;
        bottomCorner.y -= 0.4f;
        Vector3 topCorner = position;
        topCorner.x += 0.4f;
        topCorner.y += 0.4f;


        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(bottomCorner, topCorner);

        if (hitColliders.Length == 0)
        {
            obj = ObjectInFront.NOTHING;
        }

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("Between " + bottomCorner + " and" + topCorner + " there is " + hitCollider);

            parent.blocked = false;

            if (hitCollider.CompareTag("Wall"))
            {
                parent.blocked = true;
            }
            
            if (hitCollider.CompareTag("Box"))
            {
                parent.blocked = true;
                currentBoxInFront = hitCollider.transform.gameObject;
            }
            
            if (hitCollider.CompareTag("Reflector Box"))
            {
                parent.blocked = true;
                currentBoxInFront = hitCollider.transform.gameObject;
            }
            
            if (hitCollider.CompareTag("Door") && !hitCollider.GetComponent<Door>().open)
            {
                parent.blocked = true;
            }

            if (hitCollider.CompareTag("Laser"))
            {
                currentLaserInFront = hitCollider.transform.gameObject;
            }
            else currentLaserInFront = null;

            if (hitCollider.CompareTag("Plate"))
            {
                currentPlateInFront = hitCollider.transform.gameObject;
            }
            else currentPlateInFront = null;
        }
    }
    /*
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
    }*/
}
