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

    // Spells and Projectile
    private const int StartingManaSlots = 4;
    private List<ManaSlot> mana_slots;
 
    public SpellManager spellmanager;
    public Transform cast_point;
    public TextMesh spell_code_text;

    // State
    private bool invincible = true;
    private bool casting_allowed = true;


    // PUBLIC MODIFIERS

    public bool ManaSlotAvailable()
    {
        foreach (ManaSlot ms in mana_slots)
        {
            if (ms.IsAvailable()) return true;
        }
        return false;
    }
    public bool FillManaSlot(ControlledProjectile p)
    {
        for (int i = 0; i < mana_slots.Count; ++i)
        {
            if (mana_slots[i].IsAvailable())
            {
                mana_slots[i].Fill(p);
                return true;
            }
        }
        return false;
    }


    // PUBLIC ACCESSORS

    public List<ManaSlot> GetManaSlots()
    {
        return mana_slots;
    }
    public ManaSlot GetOldestFilledManaSlot()
    {
        ManaSlot oldest = mana_slots[0];
        float oldest_fill_time = Time.time;
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.IsAvailable()) continue;
            if (slot.GetFillTime() < oldest_fill_time)
            {
                oldest_fill_time = slot.GetFillTime();
                oldest = slot;
            }
        }

        return oldest;
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
        mana_slots = new List<ManaSlot>(StartingManaSlots);
        for (int i = 0; i < StartingManaSlots; ++i)
            mana_slots.Add(new ManaSlot());
    }
    private void Start()
    {
        Refresh();

        // TEST SPELL CREATION

    }
    private void Update()
    {
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.GetProjectile() == null) continue;
            slot.GetProjectile().UpdateConmtrolledMovement(pc.InputMove);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invincible) return;
        if (collision.collider.CompareTag("Ball"))
        {
            TakeHit();
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
        casting_allowed = false;
        invincible = true;
        StopAllCoroutines();
        StartCoroutine(RefreshAfterWait());

        // dispel slot projectiles
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.GetProjectile() == null) continue;
            slot.GetProjectile().Destroy(ManaSlotCooldown.Short);
        }
    }

    private void OnCastSpell()
    {
        spellmanager.Cast(this, pc.InputSpellCode);
    }
    private void OnSpellCodeChange()
    {
        spell_code_text.text = pc.InputSpellCode;
    }
    

    private IEnumerator RefreshAfterWait()
    {
        yield return new WaitForSeconds(2f);
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
        casting_allowed = true;
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
