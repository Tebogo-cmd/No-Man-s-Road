using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    //not the most efficient, but getting access to all buttons and groups makes stuff easir
    public GameObject BaseUI;
    public GameObject HelpUI;

    void Start()
    {
        BaseUI.SetActive(true);
        HelpUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


    public void ExitHelp() //user is closing the help screen
    {
        HelpUI.SetActive(false);
        BaseUI.SetActive(true);
    }
    public void ViewHelp() //user wants to view the help screen
    {
        BaseUI.SetActive(false);
        HelpUI.SetActive(true);
    }
    public void PlayGame() //game is starting dump UI  and change scene
    {
        if (BaseUI.activeSelf)
            BaseUI.SetActive(false);
        
        if (HelpUI.activeSelf)
            HelpUI.SetActive(false);

      
        
     SceneManager.LoadScene("Game");
    }
}
