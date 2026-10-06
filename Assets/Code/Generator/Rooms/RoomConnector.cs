using System.Collections;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    public enum Direction { North, East, South, West }

    public enum ConnectionType { Entry, Exit }

    public ConnectionType type;
    public Direction direction;
}


