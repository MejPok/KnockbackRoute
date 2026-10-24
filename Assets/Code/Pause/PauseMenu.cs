using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject escPanel;

    public bool Paused;
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
    }
}
