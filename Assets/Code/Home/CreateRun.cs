using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateRun : MonoBehaviour
{
    public int Difficulty;
    public int NumberOfRooms;
    public void CreateRunSetup()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
