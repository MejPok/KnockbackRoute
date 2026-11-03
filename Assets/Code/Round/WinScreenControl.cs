using UnityEngine;
using UnityEngine.SceneManagement;


public class WinScreenControl : MonoBehaviour
{
    public GameObject winScreen;

    void Awake()
    {
        winScreen.SetActive(false);
    }
    public void PlayerWin()
    {
        winScreen.SetActive(true);
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReloadScene()
    {
        Debug.Log("Reloading scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
