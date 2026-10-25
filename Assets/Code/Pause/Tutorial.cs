using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    void Start()
    {
        tutorialText.text = " ";
        
        doTutorial = PlayerPrefs.GetInt("tutorial") == 0 ? true : false ;

    }

    float timer;
    public float textTimeThreshold;
    bool doTutorial;

    string[] texts = { "Hey", "Aim with your mouse and shoot by pressing Space", "Hold E while aiming at wall to use Hook", "Run AWAY from the corruption", "Press ESC to pause the game", "Good luck", " " };

    void Update()
    {
        if (!doTutorial)
        {
            return;
        }
        timer += Time.deltaTime;
        int index = (int)(timer / textTimeThreshold);
        if(index < texts.Length)
        {
            tutorialText.text = texts[index];
        } else
        {
            PlayerPrefs.SetInt("tutorial", 1);
            doTutorial = false;
        }

    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("tutorial", 0);
        doTutorial = true;
    }
}
