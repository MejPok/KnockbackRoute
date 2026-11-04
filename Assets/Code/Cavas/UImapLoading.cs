using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UImapLoading : MonoBehaviour
{
    public bool loading = true;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingTextRoomCount;

    public Slider loadingSlider;

    float timer = 0f;
    float dotCDshow = 1f;

    int dotCount = 0;
    int maxDotCount = 3;

    RoomGenerator generator;

    private void Start()
    {
        generator = RoomGenerator.Instance;
        generator.SetMapLoadUI(this.gameObject);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= dotCDshow)
        {
            timer = 0;
            if (loading)
            {
                dotCount++;
                if (dotCount > maxDotCount)
                {
                    dotCount = 0;
                    
                }
            }
        }

        string dots = new string('.', dotCount);
        loadingText.text = "Loading map" + dots;

        loadingTextRoomCount.text = $"{generator.roomsGenerated} / {generator.roomCount} rooms built";

        loadingSlider.maxValue = generator.roomCount;
        loadingSlider.minValue = 0;
        loadingSlider.value = generator.roomsGenerated;

        
    }

    public void RestartGeneration()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
