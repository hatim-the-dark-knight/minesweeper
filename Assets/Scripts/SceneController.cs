using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static int gameMode;

    public void Play()
    {
        Dropdown dropdown = transform.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        gameMode = dropdown.value;
        SceneManager.LoadScene(1);
    }
    public void Back()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetGameMode(int mode)
    {
        gameMode = mode;
    }

    public static int GetParameters()
    {
        return gameMode;
    }
}
