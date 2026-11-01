using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public bool caught;
    [HideInInspector] public int value = 10;

    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Init(int v, Color c, float gravity)
    {
        value = v;
        GetComponent<SpriteRenderer>().color = c;
        GetComponent<Rigidbody2D>().gravityScale = gravity;
    }

    void Update()
    {
        if (!caught && transform.position.y < -5.3f)
        {
            caught = true;
            GameManager.Instance.Miss();
            Destroy(gameObject);
        }
    }
}
