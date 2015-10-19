using UnityEngine;
using System.Collections;

public enum ProjectileType { Fire, Ice, Water, Fragile }

public abstract class Projectile : MonoBehaviour
{
    // General
    public ProjectileType type;
    protected Mage caster;

    // Movement
    protected Rigidbody2D rb;

    // Visual
    public SpriteRenderer player_marker;



    // PUBLIC MODIFIERS

    public virtual void Initialize(Mage caster, Vector2 pos)
    {
        this.caster = caster;
        player_marker.color = caster.player_color;
        transform.position = pos;
    }


    // PUBLIC ACCESSORS / HELPERS

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
    public static bool Defeats(ProjectileType type1, ProjectileType type2)
    {
        return type2 == ProjectileType.Fragile ||
            type1 == ProjectileType.Fire && type2 == ProjectileType.Ice ||
            type1 == ProjectileType.Ice && type2 == ProjectileType.Water ||
            type1 == ProjectileType.Water && type2 == ProjectileType.Fire;
    }
    public static bool Defeats(Projectile p1, Projectile p2)
    {
        ProjectileType type1 = p1.type;
        ProjectileType type2 = p2.type;

        return p1.caster != p2.caster &&
            (type2 == ProjectileType.Fragile ||
            type1 == ProjectileType.Fire && type2 == ProjectileType.Ice ||
            type1 == ProjectileType.Ice && type2 == ProjectileType.Water ||
            type1 == ProjectileType.Water && type2 == ProjectileType.Fire);
    }


    // PRIVATE / PROTECTED MODIFIERS

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile p = collision.collider.GetComponent<Projectile>();
        if (p != null && Defeats(p, this))
        {
            // collided projectile defeats this projectile
            Destroy(gameObject);
        }
    }

}
