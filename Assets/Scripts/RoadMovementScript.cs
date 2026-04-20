using UnityEngine;

public class RoadMovementScript : MonoBehaviour
{
    private float repeatWidth;
    private Vector3 originalPos;

    [Tooltip("The wall and road (player) speed")]
    public float wrSpeed { get; private set; } = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
            repeatWidth = GetComponent<BoxCollider>().size.z/2;
            originalPos = transform.position; 
        
        
    }

    // Update is called once per frame
    void Update()
    {
       //We need to manage this so that the player does not fall through
        transform.Translate(Vector3.back*Time.deltaTime*wrSpeed);  //walls back

        if ((originalPos.z - repeatWidth) >= transform.position.z)  //we hit the midpoint (move the wall back to its original position)
        {
      

                 transform.position = originalPos;
        }
        

    }
}
