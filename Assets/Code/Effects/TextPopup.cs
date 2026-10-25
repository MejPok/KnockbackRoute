using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    public float speed;
    public float time;

    public string text;
    public TextMeshPro textObject;

    float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;

        transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
        textObject.color = new Color(1,1,1,1 - timer/time);

        textObject.text = text;

    }

    void Start()
    {
        Destroy(gameObject, time);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
    }
}
