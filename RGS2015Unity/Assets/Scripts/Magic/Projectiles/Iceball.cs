using UnityEngine;
using System.Collections;

public class Iceball : ControlledProjectile
{
    // Visual
    private static BackgroundLighting lighting;
    private ParticleSystem trail;
    public Color glow_color;

    // Impact
    public IceExplosion explosion_obj;
    private bool frozen = false;
    private bool exploded = false;


    protected override void Awake()
    {
        lighting = FindObjectOfType<BackgroundLighting>();
        trail = GetComponent<ParticleSystem>();
        base.Awake();
    }
    protected override void Update()
    {
        if (exploded)
        {
            if (!explosion_obj.IsExploding()) UnExplode();
        }

        // Visual
        lighting.Light(transform.position, glow_color * 0.2f, true);

        base.Update();
    }

    private void Explode(Rigidbody2D target)
    {
        explosion_obj.Explode();

        // push target
        Vector2 to_target = target.transform.position - transform.position;
        Vector2 force = to_target.normalized * Mathf.Max(0, (3f - to_target.magnitude));
        target.AddForceAtPosition(force, transform.position, ForceMode2D.Impulse);

        // particles
        trail.Clear();
        trail.enableEmission = false;

        exploded = true;
    }
    private void UnExplode()
    {
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        transform.GetComponent<Collider2D>().enabled = true;

        explosion_obj.gameObject.SetActive(false);
        trail.Play();
        trail.enableEmission = true;

        exploded = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.collider.GetComponent<Ball>();
        if (ball != null && !frozen)
        {
            Rigidbody2D ball_rb = ball.GetComponent<Rigidbody2D>();
            ball_rb.velocity = Vector2.zero;
            StartCoroutine(FreezeThenExplode(ball_rb));
            return;
        }
        base.OnCollisionEnter2D(collision);
    }


    private IEnumerator FreezeThenExplode(Rigidbody2D target)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        transform.GetComponent<Collider2D>().enabled = false;
        trail.Pause();
        frozen = true;

        yield return new WaitForSeconds(0.7f);

        frozen = false;
        Explode(target);
    }
}
