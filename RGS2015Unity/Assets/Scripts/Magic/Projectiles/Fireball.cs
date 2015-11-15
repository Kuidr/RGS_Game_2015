  using UnityEngine;
using System.Collections;

public class Fireball : ControlledProjectile
{
    // Visual
    private static BackgroundLighting lighting;
    private ParticleSystem trail;
    public Color glow_color;

    // Impact
    public Explosion explosion_obj;
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

    private void Explode()
    {
        explosion_obj.gameObject.SetActive(true);
        explosion_obj.Explode(2, 5);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.GetComponent<Collider2D>().enabled = false;

        // particles
        trail.Clear();
        trail.enableEmission = false;

        exploded = true;
    }
    private void UnExplode()
    {
        rb.isKinematic = false;
        rb.velocity = GeneralHelpers.RandomDirection2D() * max_speed / 2f;
        transform.GetComponent<Collider2D>().enabled = true;

        explosion_obj.gameObject.SetActive(false);
        trail.Play();
        trail.enableEmission = true;

        exploded = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.collider.GetComponent<Ball>();
        if (ball != null)
        {
            Explode();
            return;
        }
        base.OnCollisionEnter2D(collision);
    }

}
