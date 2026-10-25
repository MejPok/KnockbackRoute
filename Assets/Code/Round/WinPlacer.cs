using Unity.Mathematics;
using UnityEngine;

public class WinPlacer : MonoBehaviour
{
    bool placed = false;
    public GameObject winPrefab;
    public void PlaceWinObject(Transform transformEnd)
    {
        placed = true;
        var win = Instantiate(winPrefab, transformEnd.position, Quaternion.identity);
    }
}
