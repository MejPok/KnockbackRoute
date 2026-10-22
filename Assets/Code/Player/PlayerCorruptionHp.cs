using UnityEngine;
using UnityEngine.UI;

public class PlayerCorruptionHp : MonoBehaviour
{
    public int Hp;
    public int maxHp;

    public float regenCooldown;
    float healTimer;
    public Image canvasLowHpEffect;

    public bool InsideCorruption;


    public float damageMultiplier;
    public float damageSpeed;
    float fulltimeInCorruption;
    float timeInCorruption;
    void Update()
    {
        if(Hp != maxHp && !InsideCorruption)
        {
            healTimer += Time.deltaTime;
            if(healTimer >= regenCooldown)
            {
                healTimer = 0;
                Hp += 1;
            }
        }
        
        if (InsideCorruption)
        {
            fulltimeInCorruption += Time.deltaTime;
            timeInCorruption += Time.deltaTime;

            if(timeInCorruption >= damageSpeed)
            {
                timeInCorruption = 0;
                DealDamage((int)(fulltimeInCorruption * damageMultiplier));
            }

        } else
        {
            fulltimeInCorruption = 0;
            timeInCorruption = 0;
        }

        canvasLowHpEffect.color = new Color(1,0,0,1 - HpPercentage());
    }

    public void DealDamage(int damage)
    {
        Hp -= damage;
        
    }

    public float HpPercentage()
    {
        return (float)Hp / (float)maxHp;
    }


}
