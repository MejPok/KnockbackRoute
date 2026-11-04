using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(menuName = "Bullets/BulletType")]
public class BulletType : ScriptableObject
{
    [Header("Basic Properties")]
    public string bulletName;
    public GameObject bulletPrefab;
    public Key shootKey;
    public GameObject ParticleSystem;

    [Header("Shoot Properties")]

    public float RecoilImpulse = 8f;
    public float velocityBoostPerShot = 0.5f;
    public float bulletSpeed = 14f;
    public int numberOfBulletsRequested = 1;

    [Header("Behaviour Properties")]

    public float sizeMultiplier = 1f;
    public float lifetime = 4f;

    [Header("Screen Shake Properties")]

    public bool enableScreenShake = false;

    public float shakeDuration = 0f;
    public float shakeStrength = 0f;


}
