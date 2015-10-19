using UnityEngine;
using System.Collections;



public abstract class ControlledProjectile : Projectile
{
    // General
    protected ManaSlot slot;
    new private Collider2D collider;

    // Movement
    protected float steering_force_factor = 1;
    protected float max_steering_force = 3;
    protected float max_speed = 3;
   


    // PUBLIC MODIFIERS

    public override void Initialize(Mage caster, Vector2 pos)
    {
        foreach (ManaSlot slot in caster.GetManaSlots())
        {
            ControlledProjectile cp = slot.GetProjectile();
            if (cp != null)
                Physics2D.IgnoreCollision(collider, cp.GetCollider(), true);
        }
        base.Initialize(caster, pos);
    }
    public void SetManaSlot(ManaSlot slot)
    {
        this.slot = slot;
    }
    public void UpdateConmtrolledMovement(Vector2 input_move)
    {
        Vector2 direction = new Vector2(input_move.x, input_move.y);
        if (direction.magnitude > 0.3f)
        {
            Vector2 desired_velocity = direction * max_speed;

            // Steering force
            Vector2 steering_force = Clip(desired_velocity - rb.velocity, max_steering_force * steering_force_factor);
            rb.AddForce(steering_force);
        }

        // Clip speed
        rb.velocity = Clip(rb.velocity, max_speed);
    }
    public void Destroy(ManaSlotCooldown slot_cooldown)
    {
        if (slot == null) Debug.LogError("ControlledProjectile mana slot not set.");
        slot.Empty(slot_cooldown);
        Destroy(gameObject);
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
            this.Destroy(ManaSlotCooldown.Long);
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            StartCoroutine(WeakenSteeringForce());
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
