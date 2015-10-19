using UnityEngine;
using System.Collections;



public abstract class ControlledProjectile : Projectile
{
    // General
    protected ManaSlot slot;

    // Movement
    protected float steering_force_factor = 1;
    protected float max_steering_force = 3;
    protected float max_speed = 3;
   


    // PUBLIC MODIFIERS

    public override void Initialize(Mage caster, Vector2 pos)
    {
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


    // PRIVATE / PROTECTED MODIFIERS

    protected override void Update()
    {
        // Clip speed (in case pushed)
        rb.velocity = Clip(rb.velocity, max_speed);

        base.Update();
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile p = collision.collider.GetComponent<Projectile>();
        if (p != null && Defeats(p.type, this.type))
        {
            // collided projectile defeats this projectile
            this.Destroy(ManaSlotCooldown.Long);
        }
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
