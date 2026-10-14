using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Generation/Room Definition")]
public class RoomDefinition : ScriptableObject
{
    public GameObject prefab;
    public string[] tags;
    public int difficulty;
    public RoomConnector entry;
    public RoomConnector exit;
    public GameObject[] specialObstacleObject;
    
    public void FindConnectionPoints()
    {
        foreach(RoomConnector con in GetConnectors())
        {
            if(con.type == RoomConnector.ConnectionType.Entry)
            {
                entry = con;
            } else if(con.type == RoomConnector.ConnectionType.Exit)
            {
                exit = con;
            }
        }
    }

    RoomConnector[] GetConnectors()
    {
        return prefab != null
            ? prefab.GetComponentsInChildren<RoomConnector>(true)
            : System.Array.Empty<RoomConnector>();
    }

    public void FindSpecialObstacles()
    {
        specialObstacleObject.AddRange(prefab.GetComponentsInChildren<ButtonBullet>(true));
        specialObstacleObject.AddRange(prefab.GetComponentsInChildren<ButtonDoor>(true));
    }
}
