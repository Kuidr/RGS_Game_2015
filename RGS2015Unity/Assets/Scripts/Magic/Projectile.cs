using UnityEngine;
using System.Collections;

public enum ProjectileType { Fire, Ice, Water, Curse }

public class Projectile : MonoBehaviour
{
    public ProjectileType proj_type;
    private Mage caster;
    private ManaSlot parent_slot;

    // movement
	private Rigidbody2D rb;
    private float steering_force_factor = 1;
    private float max_steering_force = 3;
    private float max_speed = 3;

	// visual
    private ParticleSystem ps;
	public SpriteRenderer marker;
	
    // impact
    public Explosion explosion_obj;
    private bool exploded = false;



    // PUBLIC MODIFIERS

    public void Initialize(Mage caster, Vector2 pos)
    {
        this.caster = caster;
        marker.color = caster.player_color;
        transform.position = pos;
    }
    public void SetParentManaSlot(ManaSlot slot)
    {
        this.parent_slot = slot;
    }
    public void UpdateConmtrolledMovement(Vector2 input_move)
    {
        if (exploded) return;

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


    // PUBLIC ACCESSORS

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
    public void Destroy(ManaSlotCooldown slot_cooldown)
    {
        if (parent_slot != null) parent_slot.Empty(slot_cooldown);
        Destroy(gameObject);
    }

    // PRIVATE MODIFIERS

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
		ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        // Clip speed
        rb.velocity = Clip(rb.velocity, max_speed);

        if (exploded)
        {
            if (!explosion_obj.IsExploding()) UnExplode();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.collider.GetComponent<Ball>();
        if (ball != null)
        {
            Explode();
            return;
        }

        Projectile proj = collision.collider.GetComponent<Projectile>();
        if (proj != null)
        {
                if (proj.proj_type == ProjectileType.Fire && proj_type == ProjectileType.Ice ||
                    proj.proj_type == ProjectileType.Ice && proj_type == ProjectileType.Water ||
                    proj.proj_type == ProjectileType.Water && proj_type == ProjectileType.Fire ||
                    proj_type == ProjectileType.Curse)
                {
                    if (proj.caster != caster)
                    {
                        this.Destroy(ManaSlotCooldown.Long);
                    }
                }
            return;
        }

        if (this.proj_type == ProjectileType.Curse)
        {
            Mage mage = collision.collider.GetComponent<Mage>();
            if (mage != null)
            {
                Vector3 direction = (mage.transform.position - transform.position).normalized;
                mage.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * 40f, collision.contacts[0].point, ForceMode2D.Impulse);
                this.Destroy(ManaSlotCooldown.Normal);
                return;
            }
        }

        StartCoroutine(WeakenSteeringForce());
        
    }

    private void Explode()
    {
        explosion_obj.Explode(2, 5);
        rb.velocity = Vector2.zero;
        transform.GetComponent<Collider2D>().enabled = false;

        // particles
        ps.Clear();
        ps.enableEmission = false;

        exploded = true;
    }
    private void UnExplode()
    {
        rb.isKinematic = false;
        rb.velocity = GeneralHelpers.RandomDirection2D() * max_speed / 2f;
        transform.GetComponent<Collider2D>().enabled = true;

        ps.Play();
        ps.enableEmission = true;

        exploded = false;
    }

    private IEnumerator WeakenSteeringForce()
    {
        float t = 0;
        while (steering_force_factor < 1)
        {
            t += Time.deltaTime / 5f;
            steering_force_factor = Mathf.Pow(t, 2f);
            yield return null;
        }
        steering_force_factor = 1;
    }


    // PRIVATE ACCESSORS AND HELPERS

    private Vector2 Clip(Vector2 v, float max_magnitude)
    {
        if (v.magnitude > max_magnitude)
            return v.normalized * max_magnitude;
        return v;
    }
    private float Clip(float x, float max_magnitude)
    {
        if (Mathf.Abs(x) > max_magnitude)
            return Mathf.Sign(x) * max_magnitude;
        return x;
    }


}
