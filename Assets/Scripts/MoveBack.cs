using UnityEngine;

public class MoveBack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private RoadMovementScript roadMovementScript;
    private Rigidbody objectRb;
    [SerializeField]
    private float movementSpeed;
    void Start()
    {
        roadMovementScript = GameObject.Find("Ground").GetComponent<RoadMovementScript>();
        movementSpeed = roadMovementScript.wrSpeed*2f;  //move faster than the background
        objectRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.Translate(movementSpeed * Time.deltaTime*Vector3.forward );
      //  objectRb.AddForce(movementSpeed * Vector3.forward,ForceMode.Acceleration);
        
    }
}
