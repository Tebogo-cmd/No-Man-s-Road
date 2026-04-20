using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject player;
    private float snap = 1f;  //influences speed at which dog moves horizontal
    private float tempX = 0;
    void Start()
    {

      
        
        player = GameObject.Find("Player");

    }


    private void Update()
    {
        tempX = Mathf.MoveTowards(transform.position.x, player.transform.position.x, snap * Time.deltaTime);
        transform.position = new Vector3(tempX, transform.position.y, transform.position.z);
    }




}
