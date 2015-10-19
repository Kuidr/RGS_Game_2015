using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    // Growth
    public Ball ball_prefab;
    private const float MaxScale = 2f;
    private const float MinScale = 1f;
    private const float ScalePerStone = 0.01f;

    // Breaking
    public Mineral stone_prefab;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Stone"))
        {
            float s = transform.localScale.x;
            s += ScalePerStone;
            
            if (s >= MaxScale)
            {
                // Split in two
                Ball new_ball = Instantiate(ball_prefab);
                new_ball.transform.position = transform.position;
                new_ball.transform.localScale = Vector3.one;
                s = MinScale;
            }

            transform.localScale = Vector3.one * s;
        }
        else if (collision.collider.CompareTag("Mage"))
        {
            BreakApart();
        }
    }

    private void BreakApart()
    {
        float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

        for (int i = 0; i < 50f * radius; ++i)
        {
            float a = Random.value * Mathf.PI * 2f;
            Vector2 pos = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius * Random.value;

            Mineral stone = Instantiate<Mineral>(stone_prefab);
            stone.transform.position = (Vector2)transform.position + pos;
            stone.GetComponent<Rigidbody2D>().AddForce(pos.normalized * Random.value * 3f, ForceMode2D.Impulse);
            stone.Break();
        }
        
        Destroy(gameObject);
    }

}
