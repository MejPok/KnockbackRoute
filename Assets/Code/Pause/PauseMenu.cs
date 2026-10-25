using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject escPanel;

    public bool Paused;

    public TextMeshProUGUI seedText;
    public TMP_InputField input;

    public void ReloadSceneWithNewSeed()
    {
        PlayerPrefs.SetInt("seed", int.Parse(input.text));
        PlayerPrefs.SetInt("useLoadedSeed", 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Paused = !Paused;
        }

        if (Paused)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
            
        }
        
        escPanel.SetActive(Paused);

        seedText.text = "Current Seed: " + RoomGenerator.Instance.gameObject.GetComponent<RoomDecider>().CurrentSeed;

    }

    public void ReDoTutorial()
    {
        GetComponent<Tutorial>().ResetTutorial();
    }


}
