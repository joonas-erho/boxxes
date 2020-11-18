using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    public List<GameObject> laserChildren = new List<GameObject>();
    public Direction dir;
    public Direction originalDir;
    public ObjectInFront obj;

    public LaserDetector ld;

    public GameObject fullLaser;
    public GameObject halfLaser;

    public Vector3 checkerPosition;

    public LaserReceiver currentlyActivatedReceiver;


    void Start()
    {
        SpawnLasers();
    }

    public void SpawnLasers()
    {
        DestroyLasers();
        dir = originalDir;
        SpawnLaser();
    }

    private void SpawnLaser()
    {
        checkerPosition = this.transform.position;
        obj = ObjectInFront.NOTHING;
        int i = 0;

        Direction reflectionDir = Direction.LEFT;

        while (i < 100)
        {
            i++;

            Vector3 newPos = checkerPosition;
            newPos.y += 0.1f;

            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(checkerPosition, checkerPosition);

            if (hitColliders.Length == 0)
            {
                obj = ObjectInFront.NOTHING;
            }

            foreach (var hitCollider in hitColliders)
            {
                Debug.Log("At: " + checkerPosition + " " + hitCollider);

                if (hitCollider.CompareTag("Wall"))
                {
                    obj = ObjectInFront.WALL;
                    break;
                }
                if (hitCollider.CompareTag("Box"))
                {
                    obj = ObjectInFront.BOX;
                    break;
                }
                if (hitCollider.CompareTag("Reflector Box"))
                {
                    obj = ObjectInFront.REFLECTOR_BOX;
                    reflectionDir = hitCollider.GetComponent<Reflector>().dir;
                    break;
                }
                if (hitCollider.CompareTag("Laser Receiver") && dir == hitCollider.GetComponent<LaserReceiver>().dir)
                {
                    obj = ObjectInFront.LASER_RECEIVER;
                    hitCollider.GetComponent<LaserReceiver>().Enable();
                    currentlyActivatedReceiver = hitCollider.GetComponent<LaserReceiver>();
                    break;
                }
                if (hitCollider.CompareTag("Negative Laser Receiver") && dir == hitCollider.GetComponent<LaserReceiver>().dir)
                {
                    obj = ObjectInFront.LASER_RECEIVER;
                    hitCollider.GetComponent<LaserReceiver>().Disable();
                    currentlyActivatedReceiver = hitCollider.GetComponent<LaserReceiver>();
                    break;
                }
            }


            if (obj == ObjectInFront.WALL) break;

            if (obj == ObjectInFront.BOX)
            {
                AdjustHalfLaser(dir);
                break;
            }

            if (obj == ObjectInFront.REFLECTOR_BOX)
            {
                AdjustHalfLaser(dir);
                AdjustOtherHalfLaser(reflectionDir);
                dir = reflectionDir;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Laser Receiver") && dir == hitCollider.GetComponent<LaserReceiver>().dir)
                    {
                        hitCollider.GetComponent<LaserReceiver>().Enable();
                        currentlyActivatedReceiver = hitCollider.GetComponent<LaserReceiver>();
                        break;
                    }
                    if (hitCollider.CompareTag("Negative Laser Receiver") && dir == hitCollider.GetComponent<LaserReceiver>().dir)
                    {
                        hitCollider.GetComponent<LaserReceiver>().Disable();
                        currentlyActivatedReceiver = hitCollider.GetComponent<LaserReceiver>();
                        break;
                    }
                }
            }
            
            if (obj == ObjectInFront.NOTHING)
            {
                AdjustFullLaser(dir);
            }

            if (obj == ObjectInFront.LASER_RECEIVER)
            {
                AdjustFullLaser(dir);
                break;
            }

            switch (dir)
            {
                case Direction.LEFT:
                    checkerPosition.x -= 1;
                    break;
                case Direction.RIGHT:
                    checkerPosition.x += 1;
                    break;
                case Direction.UP:
                    checkerPosition.y += 1;
                    break;
                case Direction.DOWN:
                    checkerPosition.y -= 1;
                    break;
            }
        }
    }

    private void AdjustHalfLaser(Direction dir)
    {
        Vector3 rotation = new Vector3(0, 0, 0);

        switch (dir)
        {
            case Direction.LEFT:
                rotation.z = 270;
                break;
            case Direction.RIGHT:
                rotation.z = 90;
                break;
            case Direction.UP:
                rotation.z = 180;
                break;
            case Direction.DOWN:
                rotation.z = 0;
                break;
        }

        Spawn(halfLaser, checkerPosition, rotation);
    }

    private void AdjustOtherHalfLaser(Direction dir)
    {
        Vector3 rotation = new Vector3(0, 0, 0);

        switch (dir)
        {
            case Direction.LEFT:
                rotation.z = 90;
                break;
            case Direction.RIGHT:
                rotation.z = 270;
                break;
            case Direction.UP:
                rotation.z = 0;
                break;
            case Direction.DOWN:
                rotation.z = 180;
                break;
        }

        Spawn(halfLaser, checkerPosition, rotation);
    }

    private void AdjustFullLaser(Direction dir)
    {
        Vector3 rotation = new Vector3(0, 0, 0);

        switch (dir)
        {
            case Direction.LEFT:
                rotation.z = 90;
                break;
            case Direction.RIGHT:
                rotation.z = 90;
                break;
            case Direction.UP:
                rotation.z = 0;
                break;
            case Direction.DOWN:
                rotation.z = 0;
                break;
        }

        Spawn(fullLaser, checkerPosition, rotation);
    }

    private void Spawn(GameObject prefab, Vector3 position, Vector3 rotation)
    {
        GameObject laser = Instantiate(prefab, position, Quaternion.Euler(rotation));
        laser.transform.SetParent(this.transform);
        laserChildren.Add(laser);
        laser.GetComponent<Laser>().parent = this;
    }

    public void DestroyLasers()
    {
        for (int i = laserChildren.Count - 1; i > -1; i--)
        {
            Destroy(laserChildren[i]);
        }

        if (currentlyActivatedReceiver != null)
        {
            if(currentlyActivatedReceiver.CompareTag("Negative Laser Receiver")) currentlyActivatedReceiver.Enable();
            else currentlyActivatedReceiver.Disable();
            currentlyActivatedReceiver = null;
        }
    }
}
