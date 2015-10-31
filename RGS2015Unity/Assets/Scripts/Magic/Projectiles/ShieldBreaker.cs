using UnityEngine;
using System.Collections;

public class ShieldBreaker : ControlledProjectile
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
            if (!explosion_obj.IsExploding())
            {
                // life over
                this.Destroy();
                slot.Empty(ManaSlotCooldown.Normal);
            }
        }


        // Visual
        lighting.Light(transform.position, glow_color * 0.5f, true);

        base.Update();
    }

    private void Explode()
    {
        explosion_obj.Explode(4, 15);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.GetComponent<Collider2D>().enabled = false;

        // particles
        trail.Clear();
        trail.enableEmission = false;

        exploded = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Mage m = collision.collider.GetComponent<Mage>();
        if (m != null)
        {
            m.Hit();
            Explode();
        }

        base.OnCollisionEnter2D(collision);
    }
}
