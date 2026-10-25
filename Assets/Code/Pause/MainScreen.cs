using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreen : MonoBehaviour
{
    public GameObject creditsWindow;
    public bool creditsActive = false;
    public void MoveToTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Credits()
    {
        creditsActive = !creditsActive;
        creditsWindow.SetActive(creditsActive);
    }
}
