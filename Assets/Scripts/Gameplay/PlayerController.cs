using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ObjectInFront { NOTHING, WALL, BOX, REFLECTOR_BOX, DOOR, LASER_RECEIVER };

public enum Direction { LEFT, UP, RIGHT, DOWN }

public class PlayerController : MonoBehaviour
{
    public float moveTime;

    public Rigidbody2D rb2D;
    public BoxCollider2D boxCollider;

    public LevelManager levelManager;

    public Detector detector;

    public GameObject heldBox;

    public CrackedFloor currentFloor;

    public Direction dir;
    private bool currentlyMoving;

    public bool controllable = false;
    public bool blocked = false;

    private float inverseMoveTime;

    public SpriteRenderer sr;

    public Sprite playerSprite;
    public Sprite holdingBox;
    public Sprite holdingReflectorBox;

    public GameObject currentBoxInFront;
    public GameObject currentPlateInFront;
    public GameObject currentLaserInFront;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void Start()
    {
        StartCoroutine(WaitStart());
        currentlyMoving = false;
        inverseMoveTime = 1f / moveTime;
        dir = Direction.RIGHT;
    }

    private IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(1.40f);
        CheckCollisions();
        controllable = true;
    }

    private void Update()
    {
        if (!controllable) return;

        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(this.transform.position, this.transform.position);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Laser"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            }

            if (hitCollider.CompareTag("Goal"))
            {
                StartCoroutine(levelManager.EndLevel());
                break;
            }

            if (hitCollider.CompareTag("Cracked Floor"))
            {
                currentFloor = hitCollider.GetComponent<CrackedFloor>();
                if(!currentFloor.isSafe) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            }
        }

        //add checker that makes sure player can only move while level is active and is not dead

        if (Input.GetKeyDown(KeyCode.G))
        {
            if(heldBox == null && currentBoxInFront != null)
            {
                heldBox = currentBoxInFront;
                
                if (currentBoxInFront.CompareTag("Box"))
                {
                    sr.sprite = holdingBox;
                }
                else sr.sprite = holdingReflectorBox;
                
                heldBox.SetActive(false);

                if (currentLaserInFront != null)
                {
                    currentLaserInFront.GetComponent<Laser>().parent.SpawnLasers();
                }

                if (currentPlateInFront != null)
                {
                    currentPlateInFront.GetComponent<PressurePlate>().Disable();
                }

                CheckCollisions();
            }
            else if(heldBox != null && !blocked && !currentlyMoving)
            {
                Vector3 rotation = new Vector3(0, 0, 0);
                Vector3 newPos = this.transform.position;
                switch (dir)
                {
                    case Direction.LEFT:
                        newPos.x -= 1;
                        rotation.z = 90;
                        break;
                    case Direction.RIGHT:
                        newPos.x += 1;
                        rotation.z = 270;
                        break;
                    case Direction.UP:
                        newPos.y += 1;
                        break;
                    case Direction.DOWN:
                        newPos.y -= 1;
                        rotation.z = 180;
                        break;
                }

                //This is done if the box is a reflector box
                if(heldBox.GetComponent<Reflector>() != null){
                    heldBox.GetComponent<Reflector>().dir = dir;
                }

                sr.sprite = playerSprite;
               
                heldBox.transform.position = newPos;
                heldBox.transform.rotation = Quaternion.Euler(rotation);
                heldBox.SetActive(true);
                heldBox = null;

                if (currentLaserInFront != null)
                {
                    currentLaserInFront.GetComponent<Laser>().parent.SpawnLasers();
                }

                if (currentPlateInFront != null)
                {
                    currentPlateInFront.GetComponent<PressurePlate>().Enable();
                }

                CheckCollisions();
            }
        }

        int horizontalMovement = (int)Input.GetAxisRaw("Horizontal");
        int verticalMovement = (int)Input.GetAxisRaw("Vertical");

        if (currentlyMoving) return;

        if (horizontalMovement != 0)
        {
            verticalMovement = 0;
        }

        if (horizontalMovement == -1)
        {
            if (dir == Direction.LEFT && !blocked)
            {
                Move(-1, 0);
                return;
            }
            else
            {
                dir = Direction.LEFT;
                StartCoroutine(Turn(180));
                return;
            }
        }
        if (horizontalMovement == 1)
        {
            if (dir == Direction.RIGHT && !blocked)
            {
                Move(1, 0);
                return;
            }

            else
            {
                dir = Direction.RIGHT;
                StartCoroutine(Turn(0));
                return;
            }
        }
        if (verticalMovement == -1)
        {
            if (dir == Direction.DOWN && !blocked)
            {
                Move(0, -1);
                return;
            }

            else
            {
                dir = Direction.DOWN;
                StartCoroutine(Turn(270));
                return;
            }
        }
        if (verticalMovement == 1)
        {
            if (dir == Direction.UP && !blocked)
            {
                Move(0, 1);
                return;
            }

            else
            {
                dir = Direction.UP;
                StartCoroutine(Turn(90));
                return;
            }
        }
    }

    
    private bool Move(int xDir, int yDir)
    {
        //Vector2 start = transform.position;
        //Vector2 end = start + new Vector2(xDir, yDir);

        //Debug.Log("Now moving to: " + xDir + ", " + yDir);
        Vector3 pos = this.transform.position;
        pos.x += xDir;
        pos.y += yDir;
        StartCoroutine(SmoothMovement(pos));
        
        return true;     
    }

    /*
    private IEnumerator HardMovement(Vector3 position)
    {
        Debug.Log("Move starts");
        currentlyMoving = true;
        yield return new WaitForSeconds(0.5f);
        this.transform.position = position;
        currentlyMoving = false;
        Debug.Log("Move done");
    }*/ 

    private IEnumerator SmoothMovement(Vector3 end)
    {
        CrackedFloor cf = null;
        if (currentFloor != null)
        {
            cf = currentFloor;
        }
        //Debug.Log("smoothmove starts");
        currentlyMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, (inverseMoveTime * Time.deltaTime) / 2f);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        if (cf != null)
        cf.Break();
        //yield return new WaitForSeconds(0.05f);
        CheckCollisions();
        currentlyMoving = false;
        yield return null;
    }

    private IEnumerator Turn(int angle)
    {
        currentlyMoving = true;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
        CheckCollisions();
        yield return new WaitForSeconds(0.15f);       
        currentlyMoving = false;
        yield return null;
    }

    private void CheckCollisions()
    {
        currentBoxInFront = null;
        currentLaserInFront = null;
        currentPlateInFront = null;
        blocked = false;

        Vector3 position = transform.position;

        switch (dir)
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

        string info = "Colliding with: ";

        foreach (var hitCollider in hitColliders)
        {
            info += hitCollider + " ";

            if (hitCollider.CompareTag("Wall"))
            {
                blocked = true;
            }

            if (hitCollider.CompareTag("Box"))
            {
                blocked = true;
                currentBoxInFront = hitCollider.transform.gameObject;
            }

            if (hitCollider.CompareTag("Reflector Box"))
            {
                blocked = true;
                currentBoxInFront = hitCollider.transform.gameObject;
            }

            if (hitCollider.CompareTag("Door") && !hitCollider.GetComponent<Door>().open)
            {
                blocked = true;
            }

            if (hitCollider.CompareTag("Laser"))
            {
                currentLaserInFront = hitCollider.transform.gameObject;
            }

            if (hitCollider.CompareTag("Plate"))
            {
                currentPlateInFront = hitCollider.transform.gameObject;
            }
        }

        Debug.Log(info);
    }
}
