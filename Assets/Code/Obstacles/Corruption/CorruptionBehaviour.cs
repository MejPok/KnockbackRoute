using System;
using System.Collections.Generic;
using System.Linq;
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
            roomsCorrupted += 1;
        }
    }

    List<Tilemap> tilemaps = new List<Tilemap>();

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
        }
    }
}
