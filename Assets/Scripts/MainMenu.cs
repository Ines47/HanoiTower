using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioSource buttonSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        buttonSound.Play();
        //Load the Main Scene
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu()
    {
        buttonSound.Play();
        //Load the Menu Scene
        SceneManager.LoadScene(0);
    }
}
