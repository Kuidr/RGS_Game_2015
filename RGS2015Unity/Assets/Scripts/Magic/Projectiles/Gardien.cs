using UnityEngine;
using System.Collections;

public class Gardien : ControlledProjectile
{
    // Visual
    private static BackgroundLighting lighting;
    private ParticleSystem trail;
    public Color glow_color;

    protected override void Awake()
    {
        lighting = FindObjectOfType<BackgroundLighting>();
        trail = GetComponent<ParticleSystem>();
        base.Awake();
    }
    protected override void Update()
    {
        // Visual
        lighting.Light(transform.position, glow_color * 0.2f, true);

        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}
