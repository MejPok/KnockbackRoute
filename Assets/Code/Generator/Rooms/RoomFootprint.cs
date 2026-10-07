using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class RoomFootprint : MonoBehaviour
{
    public BoxCollider2D[] boxColliders { get
        {
            return GetComponents<BoxCollider2D>();
        }
    }
}
