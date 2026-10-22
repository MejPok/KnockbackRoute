using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CorruptionBehaviour : MonoBehaviour
{
    public float corruptionRoomSpreadSpeed;
    float spreadTimer;

    public float startBuffer;
    bool startBufferPeriod = true;
    public int roomsCorrupted;

    Transform roomTransform;

    void Start()
    {
        roomTransform = RoomGenerator.Instance.generatedRoomsRoot;
    }

    void Update()
    {
        if (startBufferPeriod)
        {
            spreadTimer += Time.deltaTime;
            if(spreadTimer >= startBuffer)
            {
                startBufferPeriod = false;
                spreadTimer = 0;
            }
            return;
        }

        spreadTimer += Time.deltaTime;
        if(spreadTimer >= corruptionRoomSpreadSpeed)
        {
            spreadTimer = 0;
            PopulateTilemaps();
            CorruptRoomUpTo(roomsCorrupted);
            FindOverlapWithPlayer();
            roomsCorrupted += 1;
        }
    }

    List<Tilemap> tilemaps = new List<Tilemap>();
    List<BoxCollider2D> corruptionColliders = new List<BoxCollider2D>();

    void PopulateTilemaps()
    {
        roomTransform = RoomGenerator.Instance.generatedRoomsRoot;

        tilemaps = roomTransform.GetComponentsInChildren<Tilemap>().ToList();
    }

    void CorruptRoomUpTo(int index)
    {
        System.Random random = new System.Random();
        
        for(int i = 0; i < tilemaps.Count && i <= index; i++)
        {
            tilemaps[i].color = new Color((float)((float)random.Next(256) / (float)255), (float)((float)random.Next(256) /(float)255), (float)((float)random.Next(256) / (float)255), 1);

            var addedFootprints = tilemaps[i].gameObject.transform.parent.GetComponentsInParent<BoxCollider2D>();
            corruptionColliders.AddRange(addedFootprints);
        }
    }

    public int findCorruptionOverlapBackwardsRoomNumber = 20;
    void FindOverlapWithPlayer()
    {
        bool isPlayerInside = false;
        for(int i = corruptionColliders.Count - 1; i >= 0 && i >= corruptionColliders.Count - findCorruptionOverlapBackwardsRoomNumber; i--)
        {
            if (corruptionColliders[i].OverlapPoint(PlayerShoot.Instance.transform.position))
            {
                isPlayerInside = true;
            }
        }
        PlayerShoot.Instance.GetComponent<PlayerCorruptionHp>().InsideCorruption = isPlayerInside;
    }




}
