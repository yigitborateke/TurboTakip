using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
