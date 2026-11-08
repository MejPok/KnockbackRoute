using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class ChargeRing : MonoBehaviour
{
    public int segments = 64;
    public float radius = 1.2f;
    public float lineWidth = 0.08f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = false;
        line.positionCount = segments + 1;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.enabled = false;
    }

    public void SetCharge(float amount)
    {
        amount = Mathf.Clamp01(amount);

        int points = Mathf.Max(2, Mathf.CeilToInt(segments * amount));
        line.enabled = amount > 0f;
        line.loop = false;
        line.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            float angle = (i / (float)(points - 1)) * amount * Mathf.PI * 2f;

            line.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            ));
        }
    }
}
