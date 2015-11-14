using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    private static MatchManager matchmanager;
    private CameraShake camshake;
    private BallAudio ball_audio;

    private static Vector2? start_pos;

    // Events
    public System.Action<Ball> event_ball_hit_mage;

    // Growth
    public Ball ball_prefab;
    private const float MaxScale = 2f;
    private const float MinScale = 1f;
    private const float ScalePerStone = 0.01f;

    // Breaking
    public Resource rock_prefab;


    private void Awake()
    {
        matchmanager = FindObjectOfType<MatchManager>();
        if (matchmanager == null) Debug.LogError("MatchManager not found");
        matchmanager.RegisterBall(this);

        // start pos (set by first ball)
        if (start_pos == null) start_pos = transform.position;
        else transform.position = (Vector2)start_pos;

        // other
        camshake = Camera.main.GetComponent<CameraShake>();
        ball_audio = GetComponentInChildren<BallAudio>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            ball_audio.PlayBump(collision.relativeVelocity.magnitude / 5f);
            camshake.Shake(new CamShakeInstance(0.03f, 0.1f));
        }
        else if (collision.collider.CompareTag("Rock"))
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
            camshake.Shake(new CamShakeInstance(0.15f, 1.5f));
            BreakApart();
            if (event_ball_hit_mage != null) event_ball_hit_mage(this);
        }
    }

    private void BreakApart()
    {
        float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

        for (int i = 0; i < 50f * radius; ++i)
        {
            float a = Random.value * Mathf.PI * 2f;
            Vector2 pos = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius * Random.value;

            Resource stone = Instantiate<Resource>(rock_prefab);
            stone.transform.position = (Vector2)transform.position + pos;
            stone.GetComponent<Rigidbody2D>().AddForce(pos.normalized * Random.value * 3f, ForceMode2D.Impulse);
            stone.Break();
        }
        
        Destroy(gameObject);
    }

}
