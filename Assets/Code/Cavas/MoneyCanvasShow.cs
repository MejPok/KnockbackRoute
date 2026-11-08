using TMPro;
using UnityEngine;

public class MoneyCanvasShow : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private void Update()
    {
        int money = PlayerShoot.Instance.GetComponent<MoneyCount>().Money;

        moneyText.text = $"Money: {money}";
    }
}
