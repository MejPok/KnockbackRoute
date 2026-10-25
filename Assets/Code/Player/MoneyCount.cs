using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    public int Money;

    public int lastEnteredRoom;

    float timerSinceLastNewRoom;

    public Transform generator;

    void Update()
    {
        timerSinceLastNewRoom += Time.deltaTime;

        Transform root = generator.GetChild(0);

        if(root == null)
        {
            return;
        }

        for(int i = 0; i < root.childCount; i++)
        {
            root.GetChild(i).GetComponent<RoomFootprint>().roomId = i;
        }

    }

    public float Scoring(float time)
    {
        float returnedScale = 1f;
        if(time <= 3f)  returnedScale  = 2f;
        if(time <= 2f)  returnedScale  = 3f;
        if(time <= 1f)  returnedScale  = 5f;
        if(time <= 0.7f)  returnedScale  = 7f;
        if(time <= 0.5f)  returnedScale  = 10f;
        if(time <= 0.2f)  returnedScale  = 15f;

        return returnedScale;
    }
    public void EnteredNewRoom(int newRoomId)
    {
        int moneyGained = (int)Scoring(timerSinceLastNewRoom);
        Money += moneyGained;

        Debug.Log($"enetered room number {newRoomId} in {timerSinceLastNewRoom}s and gained {moneyGained}");

        timerSinceLastNewRoom = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<RoomFootprint>() != null)
        {
            EnteredNewRoom(collision.GetComponent<RoomFootprint>().roomId);
        }
    }
}
