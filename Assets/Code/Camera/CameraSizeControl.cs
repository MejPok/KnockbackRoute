using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraSizeControl : MonoBehaviour
{
    [SerializeField] float MinSize = 5f;
    [SerializeField] float MaxSize = 10f;
    [SerializeField] float MinSpeed;
    [SerializeField] float MaxSpeed = 12f;
    [SerializeField] AnimationCurve increaseRatio = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] float zoomSmoothSpeed = 5f;

    CinemachineCamera cinemachineCamera;

    void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        PlayerShoot player = PlayerShoot.Instance;
        MaxSpeed = player.maxVelocity;
        if (player == null)
        {
            return;
        }

        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        if (playerRigidbody == null)
        {
            return;
        }

        float normalizedSpeed = Mathf.InverseLerp(MinSpeed, MaxSpeed, playerRigidbody.linearVelocity.magnitude);
        float zoomRatio = Mathf.Clamp01(increaseRatio.Evaluate(normalizedSpeed));
        float targetSize = Mathf.Lerp(MinSize, MaxSize, zoomRatio);

        LensSettings lens = cinemachineCamera.Lens;
        float blend = 1f - Mathf.Exp(-zoomSmoothSpeed * Time.deltaTime);
        lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, targetSize, blend);
        cinemachineCamera.Lens = lens;
    }
}
