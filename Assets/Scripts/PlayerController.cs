using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputAction playerInput_M; //movement
    public InputAction playerInput_J; //Jump
    private Rigidbody playerRb;
    public Animator playerAnimator;

    [SerializeField] private float jumpForce = 400f;
    private float[] laneX = { -4.786f, 2.213f, 9.2132f }; //allowable horizonal final positions
    private int currentLane; //maps to lane X via 0,1,2  //where 1 is the center lane
    [SerializeField] private float snap = 2.5f; //used for snapping movement



    [SerializeField]
    private bool onGround = true;

    private GameManager gameManagerScript;
    private RoadMovementScript roadMovementScript;
    private float movementSpeed;
    public PlayerInfo playerInfo;
    private void OnEnable()
    {
        playerInput_M.Enable();
        playerInput_J.Enable();
        playerInput_M.performed += HorizantalMove;
        playerInput_J.performed += Jump;
    }
    

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerAnimator.SetBool("Static_b", true);
        playerAnimator.SetFloat("Speed_f", 0.6f);
        currentLane = 1;
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        roadMovementScript = GameObject.Find("Ground").GetComponent<RoadMovementScript>();
        movementSpeed = roadMovementScript.wrSpeed * 2f;  //move faster than the background
        playerInfo = new();
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        //Lets Move the player on Request
        playerRb.MovePosition(new Vector3(Mathf.MoveTowards(playerRb.position.x, laneX[currentLane], snap * Time.deltaTime), playerRb.position.y, transform.position.z));


    }


    private void Jump(InputAction.CallbackContext obj)

    {

        if (onGround && !gameManagerScript.IsGameOver)
        {
            
            playerAnimator.SetFloat("Speed_f", 0f); //turn off running anim
            playerAnimator.SetTrigger("Jump_trig");
                                              
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }


    }

    private void HorizantalMove(InputAction.CallbackContext obj)
    {
        if (onGround && !gameManagerScript.IsGameOver)
        {
            if (obj.ReadValue<float>() > 0) //move to the right
            {
                currentLane = Mathf.Min(2, currentLane + 1); //restrict right movement to index 2
            }
            else //move to the left
            {
                currentLane = Mathf.Max(0, currentLane - 1); //restrict left movement to  index 0
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!gameManagerScript.IsGameOver)
        {

            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == 3)
            {
                onGround = true;
                //    playerAnimator.SetBool("Jump_b", false);
                playerAnimator.SetFloat("Speed_f", 0.6f);

            }


            if (collision.gameObject.CompareTag("egg"))
            {
                gameManagerScript.CollectableObjectDeactivator(collision.gameObject);
                playerInfo.Score = (1 * gameManagerScript.Multiplier) + playerInfo.Score;
            }
            else if (collision.gameObject.CompareTag("gem"))
            {
                gameManagerScript.CollectableObjectDeactivator(collision.gameObject);
                playerInfo.Score = (10 * gameManagerScript.Multiplier) + playerInfo.Score;
            }
            else if (collision.gameObject.CompareTag("multiplier"))
            {
                gameManagerScript.CollectableObjectDeactivator(collision.gameObject);
                gameManagerScript.Multiplier += 1;
                gameManagerScript.powerUPsClock[(int)PlayerInfo.PowerUpType.Multiplier] = 20;  //set up the timer first for the  powerUp
                playerInfo.SetPowerUp(PlayerInfo.PowerUpType.Multiplier);

            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == 3) //jumpable parts of the car
        {
            onGround = true;
        }

        if (other.gameObject.CompareTag("frontCar"))  //got hit by front part of the car
        {
            other.gameObject.GetComponentInParent<MoveBack>().enabled = false;
            other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.back * 100);
          
            StartCoroutine(ActivateMoveBack(other.gameObject));
            Debug.Log("Pushed him");
            playerInfo.Lives -= 50;

        }
        if (other.gameObject.CompareTag("SideCar"))
        {


            playerInfo.Lives -= 15;

        }
        if (other.gameObject.CompareTag("dog"))
        {


            playerInfo.Lives -= 20;

        }
    }

    private void OnDisable()
    {
        playerInput_M.Disable();
        playerInput_J.Disable();
        playerInput_M.performed -= HorizantalMove;
        playerInput_J.performed -= Jump;
    }


    IEnumerator ActivateMoveBack(GameObject other)
    {
        
        yield return new WaitForSeconds(0.5f);
        other.GetComponentInParent<MoveBack>().enabled = true;
    }


}
