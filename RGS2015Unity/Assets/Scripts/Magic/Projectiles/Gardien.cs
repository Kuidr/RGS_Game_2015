using UnityEngine;
using System.Collections;

public class Gardien : ControlledProjectile
{
    // Visual
    private static BackgroundLighting lighting;
    private ParticleSystem trail;
    public Color glow_color;
    private int life_time_blink = 1; // 0 or 1 for flashing when nearly done
    private const float lifetime = 20f;
    IEnumerator blink_coroutine;

    protected override void Awake()
    {
        lighting = FindObjectOfType<BackgroundLighting>();
        trail = GetComponent<ParticleSystem>();
        base.Awake();
        StartCoroutine(UpdateLifeTime());
    }
    protected override void Update()
    {
        // Visual
        lighting.Light(transform.position, glow_color * 0.2f * life_time_blink, true);

        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    private IEnumerator UpdateLifeTime()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / lifetime;
            if (t > 0.9f && blink_coroutine == null)
            {
                // start blink
                blink_coroutine = Blink();
                StartCoroutine(blink_coroutine);
            }

            yield return null;
        }
        slot.Empty(ManaSlotCooldown.Normal);
        this.Kill();
    }
    private IEnumerator Blink()
    {
        while (true)
        {
            life_time_blink = life_time_blink == 0 ? 1 : 0;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
