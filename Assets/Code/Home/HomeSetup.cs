using UnityEngine;

public class HomeSetup : MonoBehaviour
{
    private void Start() {
        if (!PlayerPrefs.HasKey("RoomCount")){

            PlayerPrefs.SetInt("RoomCount", 10);
            
        }

        if (!PlayerPrefs.HasKey("Difficulty"))
        {
            PlayerPrefs.SetInt("Difficulty", 1);
        }


    }
}
