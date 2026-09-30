using TMPro;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    public TextMeshProUGUI bulletText;
    BulletInventory inventory;
    private void Start()
    {
        inventory = PlayerShoot.Instance.transform.GetComponent<BulletInventory>();

    }
    private void Update()
    {
        if(inventory != null) {
             bulletText.text = "Bullets: " + inventory.NormalBulletsCount;
        } else
        {
            inventory = PlayerShoot.Instance.transform.GetComponent<BulletInventory>();

        }
    }
}
