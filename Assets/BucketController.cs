using UnityEngine;

public class BucketController : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        transform.Translate(x * speed * Time.deltaTime, 0, 0);

        if (Input.GetMouseButton(0))
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(p.x, transform.position.y, 0);
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -limit, limit);
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.over) return;
        Coin coin = other.GetComponent<Coin>();
        if (coin == null || coin.caught) return;

        coin.caught = true;
        GameManager.Instance.Catch(coin.value);
        Destroy(other.gameObject);
    }
}
