using UnityEngine;
using System.Collections;



public abstract class ControlledProjectile : Projectile
{
    private const int CRYSTALS_ON_DESTROY = 5;

    // General
    protected ManaSlot slot;
    new private Collider2D collider;

    // Movement
    protected Vector2 input_direction = new Vector2();
    protected float steering_force_factor = 1;
    protected float max_steering_force = 3;
    protected float max_speed = 3;

    // Time lock
    private Vector2 velocity_time_locked;
    protected bool time_locked = false;

    // Visual
    public ProjectileFlag flag_prefab;
    protected ProjectileFlag flag;

    // Sound
    public WorldSound destroyed_sound_prefab;


    // PUBLIC MODIFIERS

    public override void Initialize(Mage caster, Vector2 pos)
    {
        if (destroyed_sound_prefab != null) ObjectPool.Instance.RequestObjects(destroyed_sound_prefab, 3, false);

        // Projectile Flag
        flag = Instantiate(flag_prefab);
        flag.Initialize(this, caster.transform.position, caster.GetPlayerColor());

        base.Initialize(caster, pos);
    }
    public void SetManaSlot(ManaSlot slot)
    {
        this.slot = slot;
    }
    public void UpdateConmtrolledMovement(Vector2 input_move)
    {
        if (time_locked) return;

        input_direction = new Vector2(input_move.x, input_move.y);
        if (input_direction.magnitude > 0.3f)
        {
            Vector2 desired_velocity = input_direction * max_speed;

            // Steering force
            Vector2 steering_force = Clip(desired_velocity - rb.velocity, max_steering_force * steering_force_factor);
            rb.AddForce(steering_force);
        }

        // Clip speed
        rb.velocity = Clip(rb.velocity, max_speed);
    }
    public virtual void Kill(bool silent = false)
    {
        Destroy(flag.gameObject);
        Destroy(gameObject);
        Debug.Log(name);

        // sound
        if (!silent) PlayDestroySound();
    }
    public void TimeLock()
    {
        time_locked = true;
        velocity_time_locked = rb.velocity;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Pause();
        }
    }
    public void UnTimeLock()
    {
        time_locked = false;
        rb.isKinematic = false;
        rb.velocity = velocity_time_locked;
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }

    // PUBLIC ACCESSORS

    public Collider2D GetCollider()
    {
        return collider;
    }


    // PRIVATE / PROTECTED MODIFIERS

    protected override void Awake()
    {
        collider = GetComponent<Collider2D>();
        base.Awake();
    }
    protected override void Update()
    {
        // Clip speed (in case pushed)
        rb.velocity = Clip(rb.velocity, max_speed);

        base.Update();
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile p = collision.collider.GetComponent<Projectile>();


        if (p != null && Defeats(p, this))
        {
            // collided projectile defeats this projectile
            slot.Empty(ManaSlotCooldown.Long);
            caster.opponent.AddCrystals(CRYSTALS_ON_DESTROY);
            this.Kill();

            return;
        }

        if (collision.collider.CompareTag("Wall"))
        {
            StartCoroutine(WeakenSteeringForce());
        }
        else if (collision.collider.CompareTag("Crystal"))
        {
            caster.AddCrystals(1);
        }
    }
    private IEnumerator WeakenSteeringForce()
    {
        float t = 0;
        steering_force_factor = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.5f;
            steering_force_factor = Mathf.Pow(t, 2f);
            yield return null;
        }
        steering_force_factor = 1;
    }
    private void PlayDestroySound()
    {
        if (destroyed_sound_prefab == null) return;
        WorldSound s = ObjectPool.Instance.GetObject(destroyed_sound_prefab, false);

        s.transform.position = transform.position;
        s.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        s.Play(Random.value * 0.1f);
    }

    // PRIVATE / PROTECTED ACCESSORS AND HELPERS

    protected Vector2 Clip(Vector2 v, float max_magnitude)
    {
        if (v.magnitude > max_magnitude)
            return v.normalized * max_magnitude;
        return v;
    }
    protected float Clip(float x, float max_magnitude)
    {
        if (Mathf.Abs(x) > max_magnitude)
            return Mathf.Sign(x) * max_magnitude;
        return x;
    }


}
