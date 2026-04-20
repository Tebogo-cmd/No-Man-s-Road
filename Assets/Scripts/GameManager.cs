using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{





    public bool IsGameOver { get; set; } = false;

    public GameObject[] Collectables = new GameObject[4];  //food = 0; multiplier = 1; lives =2 ; eggs

    public MotionObjSpawner motionObjSpawnerScript;
    public PlayerController playerConScript;
    private TextMeshProUGUI ScoreTextTMP;
    private Slider LiveSlider;

    private bool isCoroutineON = false;

    public int[] powerUPsClock = new int[3];  //0 -> clock for Invisible, DoubleJump, Multiplier -> Match enum

    public int Multiplier { get; set; } = 1;  //score multiplier


    public GameObject gameOverScreen;

    void Start()
    {
        //collectable pool sits here
        motionObjSpawnerScript = GameObject.FindGameObjectWithTag("Collectables").GetComponent<MotionObjSpawner>();
        playerConScript = GameObject.Find("Player").GetComponent<PlayerController>();
        ScoreTextTMP = GameObject.FindGameObjectWithTag("ScoreTextUI").GetComponent<TextMeshProUGUI>();
        LiveSlider = GameObject.FindGameObjectWithTag("LiveSliderUI").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        if (!isCoroutineON && (playerConScript.playerInfo.PowerUps.Count != 0)) //we have a powerup/s 
        {

            isCoroutineON = true; //allow the routine to manage the clock for all powerUps
            StartCoroutine(PowerOver()); //finally start the coroutine
        }




        if (!IsGameOver && playerConScript.playerInfo.Lives <= 0)
        {
            StartCoroutine(GameOver());
        }

    }


    public void CollectableObjectDeactivator(GameObject gameObject)
    {
        //Deactivate the collectable and send it back to the pool
        gameObject.SetActive(false);

        int idx = gameObject.GetComponent<ObjectIndexer>().Index;
        if (motionObjSpawnerScript.ObjectPool[idx].State)
        {
            motionObjSpawnerScript.ObjectPool[idx].State = false;
            motionObjSpawnerScript.NumberOfActiveObjs--;
        }
    }


    IEnumerator PowerOver()    //Deal with powerDown
    {
        while (true)
        {

            if (playerConScript.playerInfo.PowerUps.Count == 0 || IsGameOver) //all powerups expired
                break;

            for (int i = 0; i < powerUPsClock.Length; i++)  //go through all powerUps
            {
                if (powerUPsClock[i] > 0)
                {
                    if (--powerUPsClock[i] == 0) //one powerUp is used up
                    {
                        playerConScript.playerInfo.PowerUps.Remove((PlayerInfo.PowerUpType)i);

                        if ((PlayerInfo.PowerUpType)i == PlayerInfo.PowerUpType.Multiplier)
                            Multiplier = 1;  //special case, where we need to restore the Multiplier
                    }

                }

            }

            yield return new WaitForSeconds(1f); //every 1/2 sec decrement any power up
        }


        isCoroutineON = false;  //reset coroutine 
        yield return null;
    }

    IEnumerator GameOver()
    {
        playerConScript.playerAnimator.SetBool("Death_b", true);
        playerConScript.playerAnimator.SetInteger("DeathType_int", 1);
        IsGameOver = true;
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);

    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnExit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }

    void UpdateUI()
    {
        ScoreTextTMP.text = "Score: " + playerConScript.playerInfo.Score;
        LiveSlider.value = playerConScript.playerInfo.Lives;
    }

}
