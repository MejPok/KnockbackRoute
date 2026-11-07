using Assets.Code.Cavas.Shop;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    public int Money;

    public int lastEnteredRoom;

    float timerSinceLastNewRoom;

    public Transform generator;

    public GameObject Popup;

    void Update()
    {
        timerSinceLastNewRoom += Time.deltaTime;

        Transform root = generator.GetChild(0);

        if(root == null)
        {
            return;
        }

        for(int i = 0; i < root.childCount; i++)
        {
            root.GetChild(i).GetComponent<RoomFootprint>().roomId = i;
        }

    }

    public float Scoring(float time)
    {
        float returnedScale = 1f;
        if(time <= 3f)  returnedScale  = 2f;
        if(time <= 2f)  returnedScale  = 4f;
        if(time <= 1f)  returnedScale  = 6f;
        if(time <= 0.7f)  returnedScale  = 8f;
        if(time <= 0.5f)  returnedScale  = 10f;
        if(time <= 0.2f)  returnedScale  = 12f;

        return returnedScale;
    }
    public void EnteredNewRoom(int newRoomId)
    {
        int moneyGained = (int)Scoring(timerSinceLastNewRoom);


        MoneyGain moneyUpgrade = (MoneyGain)UpgradesManager.Instance.FindRightItem("MoneyGain");
        if (moneyUpgrade != null)
        {
            Money += (int)((moneyGained / 2) * moneyUpgrade.MoneyGainIncreasePercentage);

        } else
        {
            Money += (moneyGained / 2);
        }

        



        BulletGain bulletUpgrade = (BulletGain)UpgradesManager.Instance.FindRightItem("BulletGain");
        if(bulletUpgrade != null)
        {
            PlayerShoot.Instance.GetComponent<BulletInventory>().NormalBulletsCount += (int)((moneyGained / 2) * bulletUpgrade.BulletGainIncreasePercentage);
            
        } else
        {
            PlayerShoot.Instance.GetComponent<BulletInventory>().NormalBulletsCount += (int)(moneyGained / 2);
        }
       

        Debug.Log($"enetered room number {newRoomId} in {timerSinceLastNewRoom}s and gained {moneyGained}");

        var popup = Instantiate(Popup, transform.position, Quaternion.identity);
        popup.GetComponent<TextPopup>().text = "+"+moneyGained;
        timerSinceLastNewRoom = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<RoomFootprint>() != null)
        {
            EnteredNewRoom(collision.GetComponent<RoomFootprint>().roomId);
        }
    }

    void SpendMoney(int amount)
    {
        if (Money - amount >= 0)
        {
            Money -= amount;
            Debug.Log($"Spent {amount} money, remaining {Money}");
        }
    }

    public bool BuyItem(ShopItem item)
    {
        if (Money - item.itemPrice >= 0)
        {
            SpendMoney(item.itemPrice);
            Debug.Log($"Bought {item.itemName} for {item.itemPrice} money, remaining {Money}");
            return true;
        }
        else
        {
            Debug.Log($"Not enough money to buy {item.itemName}, need {item.itemPrice}, have {Money}");
            return false;
        }
    }


}
