using UnityEngine;

public class ObjectDeactivator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private const float lowerYbound = -3f;


   private MotionObjSpawner motionObjSpawnerScript;
   public string scriptParentTagName;
    public GameObject barel;
    private Quaternion target;
    void Start()
    {
        motionObjSpawnerScript = GameObject.FindGameObjectWithTag(scriptParentTagName).GetComponent<MotionObjSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        //Has the object moved passed the screen ??
        if (transform.position.y < lowerYbound)
        {
            //change the object State in the pool,update the number of Active Objects. And Finally Deactivate the Object.

            motionObjSpawnerScript.ObjectPool[gameObject.GetComponent<ObjectIndexer>().Index].State = false;
            --motionObjSpawnerScript.NumberOfActiveObjs;
            gameObject.SetActive(false);
     
    
        }
    }


    //Reset barel 
}
