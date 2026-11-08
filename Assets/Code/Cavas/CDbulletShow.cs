using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CDbulletShow : MonoBehaviour
{
    public GameObject cdImage;
    public Image bulletImage;

    public TextMeshProUGUI cdText;

    public BulletType bigBullet;

    private void Update()
    {
        var inv = PlayerShoot.Instance.GetComponent<BulletInventory>();

        if (!inv.bulletsEnabled.Contains(bigBullet))
        {
            cdImage.SetActive(false);
        }else
        {
            cdImage.SetActive(true);

            int index = inv.bulletsEnabled.IndexOf(bigBullet);
        
            int cdRemaining = (int)inv.bulletCooldowns[index];

            cdText.text = $"{cdRemaining}";
            if (cdRemaining <= 0) cdText.text = "";
            
        }

        

    }
}
