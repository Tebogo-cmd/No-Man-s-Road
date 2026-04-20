using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject player;
    public GameObject testBomb;
    public GameObject spawnPoint;
    Quaternion current;
    Quaternion target;
    private Vector3 direction;
    private float snap = 5f;
    private float lockedAngleTolerance = 20f;
    private float tempAngleToTarget;
    private bool barelState=true;


    private const float bombLifetime = 5f; // destroy bomb after this many seconds if it hasn't hit anything

    void Start()
    {
        player = GameObject.Find("Player");
      


    }
    private void OnEnable()
    {
        transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        barelState = true;

        // Stop any leftover coroutines from before deactivation
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        current = transform.rotation;
        direction = (player.transform.position - transform.position);

        direction.y = 0;

        target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(current,target,snap*Time.deltaTime);
      
        tempAngleToTarget = Quaternion.Angle(transform.rotation, target);

        if ((tempAngleToTarget <= lockedAngleTolerance) && barelState &&(transform.position.z >= player.transform.position.z))  
        {
            /**
             * Also include logic that stops firing if tank is passed player;
             * */
            barelState = false;
           StartCoroutine(ShootAndCoolDown());
        }

    }

    IEnumerator ShootAndCoolDown()
    {
        
          
          


            GameObject tempBomb = Instantiate(testBomb, spawnPoint.transform.position, testBomb.transform.rotation);
            tempBomb.GetComponent<Rigidbody>().AddForce((player.transform.position - spawnPoint.transform.position) * 2, ForceMode.Impulse);
            Destroy(tempBomb, bombLifetime);
            yield return new WaitForSeconds(2f);
            barelState = true;
     
    }

   

}
