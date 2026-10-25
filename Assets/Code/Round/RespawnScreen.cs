using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnScreen : MonoBehaviour
{
    public static RespawnScreen Instance;
    public GameObject respawnScreen;
    public TextMeshProUGUI seedText;

    void Awake()
    {
        Instance = this;
        respawnScreen.SetActive(false);

    }

    public void PlayerDead()
    {
        Time.timeScale = 0;
        respawnScreen.SetActive(true);

        seedText.text = "Failed Seed: " + RoomGenerator.Instance.gameObject.GetComponent<RoomDecider>().CurrentSeed;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
