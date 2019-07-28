using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform spriteTrans;
    [SerializeField] private Transform frontDetectorStart;
    [SerializeField] private Transform frontDetectorEnd;
    [SerializeField] private Transform backDetectorStart;
    [SerializeField] private Transform backDetectorEnd;
    [SerializeField] private Transform wallDetectorStart;
    [SerializeField] private Transform wallDetectorEnd;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private float speedMultiplier = 5f;
    [SerializeField] private float jumpIntensity = 500f;
    public Rigidbody2D playerRB;
    public Transform playerTR;
    private SpriteRenderer spriteRenderer;
    private GameObject safePosition; //game object used to hold a transform with a safe position and rotation as a checkpoint before the hazards in the level
    private LizardState[] possibleStates;
    public LizardState currentState;
    private int collisionCount = 0;
    private Transform[] lightDetectors;
    private bool[] detectorsHit;
    private LightData[] lightsInScene;
    private float shadeAmount = 1f;
    private int moveDirectionModifier;
    private Vector2 playerFinalPos;
    private Vector2 playerinput;
    [HideInInspector] public Vector3 frontLine;
    [HideInInspector] public Vector3 backLine;
    [HideInInspector] public int reverseInput = 1;
    private Vector2 wallAnchor;
    private Vector3 gravityDirection;
    private Vector3 spriteScale;
    private Animator lizAnim;
    [SerializeField] private GameObject[] lives;
    private int playerLives = 3;

    public Transform FrontDetectorStart
    {
        get { return frontDetectorStart; }
    }

    public Transform BackDetectorStart
    {
        get { return backDetectorStart; }
    }

    public Transform FrontDetectorEnd
    {
        get { return frontDetectorEnd; }
    }

    public Transform BackDetectorEnd
    {
        get { return backDetectorEnd; }
    }

    public Transform WallDetectorStart
    {
        get { return wallDetectorStart; }
    }

    public Transform WallDetectorEnd
    {
        get { return wallDetectorEnd; }
    }

    public float JumpIntensity
    {
        get { return jumpIntensity; }
    }

    public float SpeedMultiplier
    {
        get { return speedMultiplier; }
    }

    public int CollisionCount
    {
        get { return collisionCount; }
    }

    public Animator LizAnim
    {
        get { return lizAnim; }
    }

    void Start()
        // Use this for initialization
    {
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        playerTR = gameObject.GetComponent<Transform>();
        spriteTrans = gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.transform;
        lizAnim = gameObject.GetComponentInChildren<Animator>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1f, 1f);
        lightsInScene = FindObjectsOfType<LightData>();
        possibleStates = new LizardState[4];
        possibleStates[0] = new LizardStateGrounded(this);
        possibleStates[1] = new LizardStateFalling(this);
        possibleStates[2] = new LizardStateWallJumping(this);
        possibleStates[3] = new LizardStateWallTransition(this);
        currentState = possibleStates[1];
        currentState.OnStateEnter();
//        SetState(1);
        lightDetectors = new Transform[] {FrontDetectorStart, WallDetectorStart, BackDetectorStart};
        detectorsHit = new bool[lightDetectors.Length];
        safePosition = new GameObject();
        safePosition.hideFlags = HideFlags.HideInHierarchy; // used to hide proxy gameobject from hierarchy
        safePosition.transform.SetPositionAndRotation(playerTR.position, playerTR.rotation); // used to set the initial position of the player as a failsafe for checkpoint
//        lives = new GameObject[3];
    }


    // Update is called once per frame
    void Update()
    {
        Walk();
        currentState.Jump();
        ShadeSprite();
        HazardTrigger();
        updateLives();
//        Debug.Log(currentState);
//        Debug.Log(CollisionCount);
    }

    void FixedUpdate()
    {
        currentState.ApplyDownForce();
        LightDetection();
    }

    public void SetState(int stateIndex)
    {
        if (currentState == possibleStates[stateIndex])
            return;
        currentState.OnStateExit();
        currentState = possibleStates[stateIndex];
        currentState.OnStateEnter();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            collisionCount = CollisionCount + 1;
//            if (backLine == Vector3.zero && frontLine == Vector3.zero) return;
            playerRB.gravityScale = 0f;
            SetState(0);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
            collisionCount = CollisionCount - 1;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard") && other.isTrigger)
        {
            playerTR.SetPositionAndRotation(safePosition.transform.position, safePosition.transform.rotation);
            playerLives -= 1;
            return;
        }

        if (other.CompareTag("HazardDetector"))
        {
            safePosition.transform.SetPositionAndRotation(playerTR.position, playerTR.rotation);
        }
    }

    void Walk()
    {
        FlipSprite();
        currentState.Move();
    }

    void FlipSprite()
    {
        spriteScale.x = Input.GetAxisRaw("Horizontal") != 0f ? -Input.GetAxisRaw("Horizontal") * reverseInput : spriteTrans.localScale.x;
        spriteTrans.localScale = spriteScale;
    }

    void ShadeSprite()
    {
        for (int i = 0; i < detectorsHit.Length; i++)
        {
            if (detectorsHit[i] && shadeAmount < detectorsHit.Length + 1)
            {
                shadeAmount++;
            }
            else if (!detectorsHit[i] && shadeAmount > 1)
            {
                shadeAmount--;
            }
        }

        //        shadeAmount = Mathf.Lerp(shadeAmount, (shadeAmount + 1) / detectorsHit.Length, 5f);

        spriteRenderer.color = Color.HSVToRGB(0, 0, shadeAmount / (detectorsHit.Length + 1));
        //Debug.Log(shadeAmount);
        //Debug.Log(spriteRenderer.color);
    }

    void updateLives()
    {
        if (playerLives > 0)
        {
            if (playerLives < lives.Length)
            {
                lives[playerLives].SetActive(false);
            }
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    void LightDetection()
    {
        for (int i = 0; i < lightDetectors.Length; i++)
        {
            int lightsHitting = 0;
            for (int j = 0; j < lightsInScene.Length; j++)
            {
                if ((playerTR.position - lightsInScene[j].LightTrans.position).magnitude < lightsInScene[j].LightRange)
                {
                    if (!Physics2D.Linecast(lightDetectors[i].transform.position, lightsInScene[j].LightTrans.position, groundLayer))
                    {
                        lightsHitting++;
                    }

//                    if (!Physics2D.Linecast(lightDetectors[i].transform.position, lightsInScene[j].LightTrans.position, groundLayer))
//                        Debug.DrawLine(lightDetectors[i].transform.position, lightsInScene[j].LightTrans.position, Color.yellow);
                }
            }

            detectorsHit[i] = lightsHitting > 0;
            //            Debug.Log("detectors hit" + i + " : " + detectorsHit[i]);
            //            Debug.Log("lights hitting : " + lightsHitting);
        }
    }

    public bool HazardTrigger()
    {
        int numberOfDetectorshit = 0;
        for (int i = 0; i < detectorsHit.Length; i++)
        {
            if (detectorsHit[i])
            {
                numberOfDetectorshit++;
            }
        }

        if (numberOfDetectorshit >= 3)
        {
            return true;
        }

//        Debug.Log(numberOfDetectorshit);
        return false;
    }
}