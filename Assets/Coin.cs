using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public bool caught;
    [HideInInspector] public bool isBomb;
    [HideInInspector] public int value = 10;

    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Init(int v, Color c, float gravity, bool bomb = false, Sprite bombSprite = null)
    {
        value = v;
        isBomb = bomb;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = c;
        if (bomb)
        {
            if (bombSprite != null) { sr.sprite = bombSprite; sr.color = Color.white; }
            transform.localScale *= 1.25f;
        }
        GetComponent<Rigidbody2D>().gravityScale = gravity;
    }

    void Update()
    {
        if (!caught && transform.position.y < -5.3f)
        {
            caught = true;
            if (!isBomb) GameManager.Instance.MissCoin();
            Destroy(gameObject);
        }
    }
}
