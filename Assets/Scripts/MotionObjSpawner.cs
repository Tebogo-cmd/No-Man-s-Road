using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[RequireComponent(typeof(ObjectIndexer))]
public class MotionObjSpawner : MonoBehaviour
{

    public List<GameObject> objects;  //default list of objects assigned via Inspector (Gets removed later and {objects==null))
    public GameObject startingPosObj;
    private int poolIndex;           //object array index
    private int MaxObjectCount = 0;  //maximum number of Objects in the pool;
    public int NumberOfActiveObjs { get; set; } //Lets track number of active objects in the pool (All objects with state == true)

    private const float poolCheckerWaitTime = 3; //if pool is empty, how long do we wait before checking for inactive objects
    private const float spawnDelay = 2.5f;   //wait time between spawns

    public struct ObjectForm{   //Base structure that defines an object in the pool

        
        public GameObject Object;
        public bool State;
        public bool isBig; //The object size (specifically width)

     }


    public ObjectForm[] ObjectPool;  //Array to hold all objects assigned to the pool 


    void Start()
    {
        ObjectPool = new ObjectForm[objects.Count]; 
        MaxObjectCount = objects.Count;
       // startingPosObj = GameObject.Find("StartingPosition"); //anchor position for all objects

        ObjectPoolSetup();    //fill the ObjectPool array and drop the objects list
        StartCoroutine(Spawner());  //Continuously spawn objects from the pool
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }



     IEnumerator Spawner()
    {
        
          
        while(true)
        {

            poolIndex = Random.Range(0, ObjectPool.Length); //{maybe we should just shuffle  the pool } -> instead of random Index sample
           // Debug.Log("poolIndex: " + poolIndex);

            if (NumberOfActiveObjs == MaxObjectCount)
            {
                yield return new WaitForSeconds(poolCheckerWaitTime);
                

            }

            if (!ObjectPool[poolIndex].State ) //We can choose object from pool->
            {
                //move object to starting pos
                ObjectPool[poolIndex].Object.transform.position = startingPosObj.transform.position;
                //show the object on the scene
                ObjectPool[poolIndex].Object.SetActive(true);
                //update the object status in the pool
                ObjectPool[poolIndex].State = true;
                //increment the number of active objects
                NumberOfActiveObjs += 1;

                ObjectPool[poolIndex].Object.GetComponent<ObjectIndexer>().Index = poolIndex; //See ObjectDeactivator.cs for Index usage

         
                if (ObjectPool[poolIndex].isBig)
                    ObjectPool[poolIndex].Object.transform.position = new Vector3(Random.Range(-3.15f, 8.84f), transform.position.y, transform.position.z);
                else
                    ObjectPool[poolIndex].Object.transform.position =  new Vector3(Random.Range(-5f, 9.98f), transform.position.y, transform.position.z);
               
                yield return new WaitForSeconds(spawnDelay); //delay between spawns

            }



            yield return null; 
        }

    }

    void ObjectPoolSetup()
    {
       
        for(int i = 0; i < objects.Count; i++)
        {
           //##ref 0: The copying from objects List -> to pool array must be optimized!-> needs refactoring
            ObjectPool[i].Object = Instantiate(objects[i], objects[i].transform.position, objects[i].transform.rotation); //get the reference
            ObjectPool[i].Object.SetActive(false); //hide from the scene
            ObjectPool[i].State = false;   //update the state in the ObjectPool
            //Setup the object size {probably not the most efficient way of setting up the sizes}  ##ref 1;
            if (ObjectPool[i].Object.name.Equals("Armor_Car(Clone)"))
            {
                ObjectPool[i].isBig = true;
            }
            else
                ObjectPool[i].isBig = false;
        }

        objects.Clear();   
        objects = null;  //ditch the list
    }


}
