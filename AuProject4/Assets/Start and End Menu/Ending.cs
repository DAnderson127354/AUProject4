using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Mainmenu()
    {
            {
                SceneManager.LoadScene(0);
            }

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
