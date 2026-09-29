using UnityEngine;

public class BulletCrate : MonoBehaviour
{
    public int bulletAmount;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.parent.GetComponent<BulletInventory>().NormalBulletsCount += bulletAmount;
            Destroy(gameObject);
        }
    }
}
