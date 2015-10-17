using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mage : MonoBehaviour
{
    public Mage opponent;

    // General
    public int player_number = 1;
    public Color player_color = Color.red;
    public bool ai = false;
    private PlayerController pc;
    private Rigidbody2D rb;
    public SpriteRenderer sprite;
    public Transform floating_pos;

    // Spells and control groups
    private string[] group_names = new string[] { "A", "B" };
    private int active_group = 0;
    public System.Action<int> event_group_swtich;

    private List<Projectile> projectiles;
    public Projectile prefab_fireball, prefab_iceball, prefab_waterball, prefab_shieldbreaker;
    public Transform cast_point;
    public TextMesh spell_code_text;

    // State
    private bool invincible = true;


    // PUBLIC MODIFIERS

    public void RemoveProjectile(Projectile p)
    {
        projectiles.Remove(p);
    }


    // PUBLIC ACCESSORS

    public int GetActiveGroupNum()
    {
        return active_group;
    }
    public List<Projectile> GetProjectiles()
    {
        return projectiles;
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        // Player controller
        if (ai)
        {
            gameObject.AddComponent<AIPlayerController>();
            GetComponent<AIPlayerController>().Initialize(this, opponent);
        }
        else
        {
            gameObject.AddComponent<HumanPlayerController>();
            GetComponent<HumanPlayerController>().Initialize(player_number);
        }
        this.pc = GetComponent<PlayerController>();

        // input events
        pc.InputCast += OnCastSpell;
        pc.InputSpellCodeChange += OnSpellCodeChange;

        // other references
        rb = GetComponent<Rigidbody2D>();

        // color
        sprite.color = player_color;

        // spells
        projectiles = new List<Projectile>();
    }
    private void Start()
    {
        Refresh();

        // TEST SPELL CREATION
        InstantiateSpell("YBB", 0);
        InstantiateSpell("XXA", 0);
        InstantiateSpell("XYX", 0);
        InstantiateSpell("YBB", 0);
        InstantiateSpell("XXA", 0);
        InstantiateSpell("XYX", 0);
        //InstantiateSpell("xyx", 1);
        //InstantiateSpell("xxa", 1);
        //InstantiateSpell("xxa", 1);

    }
    private void Update()
    {
        foreach (Projectile p in projectiles)
        {
            p.UpdateMovement(pc.InputMove);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invincible) return;
        if (collision.collider.CompareTag("Ball"))
        {
            TakeHit();
        }
        else
        {
            Projectile p = collision.collider.GetComponent<Projectile>();
            if (p != null && p.proj_type == ProjectileType.Curse)
            {
                TakeHit();
            }
        }

    }

    private void Refresh()
    {
        StopAllCoroutines();
        StartCoroutine(FloatUp());
    }
    private void TakeHit()
    {
        rb.gravityScale = 1;
        invincible = true;
        StopAllCoroutines();
        StartCoroutine(RefreshAfterWait());
    }

    private void OnCastSpell()
    {
        InstantiateSpell(pc.InputSpellCode, active_group);
    }
    private void OnSpellCodeChange()
    {
        spell_code_text.text = pc.InputSpellCode;
    }
    private void InstantiateSpell(string spell_code, int group)
    {
        Projectile p = null;
        Vector2 offset = GeneralHelpers.RandomDirection2D() * 0.5f;
        Vector2 pos = (Vector2)cast_point.position + offset;


        switch (spell_code)
        {
            case "YBB":
                p = Instantiate<Projectile>(prefab_fireball);
                p.Initialize(this, pos);
                break;
            case "XYX":
                p = Instantiate<Projectile>(prefab_iceball);
                p.Initialize(this, pos);
                break;
            case "XXA":
                p = Instantiate<Projectile>(prefab_waterball);
                p.Initialize(this, pos);
                break;
            case "XXXBBB":
                p = Instantiate<Projectile>(prefab_shieldbreaker);
                p.Initialize(this, pos);
                break;
        }

        if (p != null) projectiles.Add(p);
    }

    private IEnumerator RefreshAfterWait()
    {
        yield return new WaitForSeconds(4f);
        Refresh();
    }
    private IEnumerator FlashInvincible()
    {
        float start_time = Time.time;
        float duration = 2f;
        Color c;

        while (true)
        {
            c = sprite.color;
            c.a = 1 - c.a;
            sprite.color = c;

            yield return new WaitForSeconds(0.025f);

            if (Time.time - start_time >= duration) break;
        }

        c = sprite.color;
        c.a = 1;
        sprite.color = c;

        invincible = false;
    }
    private IEnumerator FloatUp()
    {
        float t = 0;

        rb.angularVelocity = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            rb.MoveRotation(Mathf.Lerp(rb.rotation, 0, t));
            yield return null;
        }
        t = 0;

        rb.gravityScale = 0;
        StartCoroutine(FlashInvincible());

        while (t < 1)
        {
            t += Time.deltaTime / 15f;
            rb.MovePosition(Vector2.Lerp(transform.position, floating_pos.position, t));
            yield return null;
        }

        // insure final pos, rotation
        transform.position = floating_pos.position;
        transform.rotation = Quaternion.identity;
    }


}
