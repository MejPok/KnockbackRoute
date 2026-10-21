using UnityEngine;
using UnityEngine.UI;

public class HookStaminaShow : MonoBehaviour
{
    public Slider staminaSlider;
    PlayerHook pHook;
    void Start()
    {
        pHook = PlayerShoot.Instance.GetComponent<PlayerHook>();
    }
    void Update()
    {
        staminaSlider.minValue = 0;
        staminaSlider.maxValue = pHook.maxHookStamina;
        staminaSlider.value = pHook.hookStamina;
    }
}
