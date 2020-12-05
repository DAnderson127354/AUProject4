using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public void Mainmenu()
    {
            {
                SceneManager.LoadScene(2);
            }

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
