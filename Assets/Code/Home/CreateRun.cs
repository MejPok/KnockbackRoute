using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateRun : MonoBehaviour
{
    public int Difficulty;
    public int NumberOfRooms;
    public void CreateRunSetup()
    {
        PlayerPrefs.SetInt("Difficulty", Difficulty);
        PlayerPrefs.SetInt("RoomCount", NumberOfRooms);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
