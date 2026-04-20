using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOverAnim : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created\

    public GameObject bomb;
    public ParticleSystem explosion;
    public ParticleSystem fire;
    public GameObject player;
    public GameObject bombContainer;
  
    public TextMeshProUGUI gameOverText;
    private float stopY = 2f;
    private float startY = 10f;
    
    private float snap= 0.6f;
    private float tempY;
    void OnEnable()
    {
        // Camera sits behind the player at a negative Z offset and looks toward +Z.
        // Placing the bomb at exactly player.Z buries it inside the player mesh.
        // A small negative offset (toward the camera) moves it in front of the player
        // without getting close enough to the camera to disappear.
        Camera cam = Camera.main;
        float camToPlayerZ = cam != null
            ? Mathf.Abs(cam.transform.position.z - player.transform.position.z)
            : 10f;
        float offsetZ = Mathf.Clamp(camToPlayerZ * 0.25f, 2f, 5f); // 25% toward camera, clamped 2-5 units

        bombContainer.transform.position = new Vector3(
            player.transform.position.x,
            startY,
            player.transform.position.z - offsetZ
        );

        StartCoroutine(dropBomb());
    }

   

    IEnumerator dropBomb() //gradually increase temp
    {
        bomb.SetActive(true);
        float bombTimer = 0;
        while (true)
        {

            tempY = Mathf.MoveTowards(bombContainer.transform.position.y, stopY,snap);
            bombContainer.transform.position = new Vector3(bombContainer.transform.position.x, tempY, bombContainer.transform.position.z);

            if (bombContainer.transform.position.y <= stopY)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }

        while (bombTimer < 1.5f)
        {
            bomb.transform.Rotate(2000f * Vector3.up);
            yield return new WaitForSecondsRealtime(0.1f);
            bombTimer += 0.1f;
        }
        fire.gameObject.SetActive(true);
        fire.Play();
        yield return new WaitForSecondsRealtime(2f);
        explosion.gameObject.SetActive(true);
        explosion.Play();
        fire.Stop();
        StartCoroutine(ShowText());
        bomb.SetActive(false);

    }

  


    IEnumerator ShowText()
    {
        gameOverText.gameObject.SetActive(true);
        float currentSize = gameOverText.fontSize;
        while (true)
        {
            gameOverText.fontSize = currentSize + 2;
            yield return new WaitForSecondsRealtime(0.5f);
            gameOverText.fontSize = currentSize - 2;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        gameOverText.gameObject.SetActive(false);
        bomb.SetActive(false);
    }

     

}
