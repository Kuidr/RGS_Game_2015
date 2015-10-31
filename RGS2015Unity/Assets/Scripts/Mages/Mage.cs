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

    // Resources
    private int crystals = 10;


    // Spells and Projectile
    private const int StartingManaSlots = 4;
    private List<ManaSlot> mana_slots;
 
    private SpellManager spellmanager;
    public Transform cast_point;
    public TextMesh spell_code_text;

    // State
    private int hearts_max = 3;
    private int hearts = 3;
    private bool invincible = true;
    private bool casting_allowed = true;

    // Events
    public System.Action<ManaSlot> event_fill_slot;
    public System.Action event_crystal_count_change;
    public System.Action event_hearts_change;


    // PUBLIC MODIFIERS

    public bool ManaSlotAvailable()
    {
        foreach (ManaSlot ms in mana_slots)
        {
            if (ms.IsAvailable()) return true;
        }
        return false;
    }
    public bool FillManaSlot(ControlledProjectile p, Spell origin_spell)
    {
        for (int i = 0; i < mana_slots.Count; ++i)
        {
            if (mana_slots[i].IsAvailable())
            {
                mana_slots[i].Fill(p, origin_spell);

                if (event_fill_slot != null) event_fill_slot(mana_slots[i]);
                return true;
            }
        }
        return false;
    }
    public void AddCrystals(int number)
    {
        crystals += number;
        if (event_crystal_count_change != null) event_crystal_count_change();
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
    public int GetCrystalCount()
    {
        return crystals;
    }
    public int GetMaxHearts()
    {
        return hearts_max;
    }
    public int GetHearts()
    {
        return hearts;
    }
    public bool IsDead()
    {
        return hearts == 0;
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
        spellmanager = FindObjectOfType<SpellManager>();
        if (spellmanager == null) Debug.LogError("SpellManager not found");

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
        if (IsDead() || invincible) return;

        // fall down
        rb.gravityScale = 1;
        casting_allowed = false;

        // dispel slot projectiles
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.GetProjectile() != null) slot.GetProjectile().Destroy();
            slot.Empty(ManaSlotCooldown.Short);
        }

        // damage
        hearts -= 1;
        if (event_hearts_change != null) event_hearts_change();
        if (hearts == 0)
        {
            Die();
            return;
        }

        // recovery
        invincible = true;
        StopAllCoroutines();
        StartCoroutine(RefreshAfterWait());
    }
    private void Die()
    {
    }

    private void OnCastSpell()
    {
        if (casting_allowed)
        {
            int crystals_old = crystals;
            SpellCastResult result = spellmanager.TryCast(this, pc.InputSpellCode, ref crystals);
            
            // crystals spent
            if (crystals != crystals_old)
            {
                if (event_crystal_count_change != null) event_crystal_count_change();
            }
        }
            
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
        rb.isKinematic = true;
        casting_allowed = true;
        StartCoroutine(FlashInvincible());

        while (t < 1)
        {
            t += Time.deltaTime / 2f;
            rb.MovePosition(Vector2.Lerp(transform.position, floating_pos.position, Mathf.Pow(t, 4)));
            yield return null;
        }

        // insure final pos, rotation
        transform.position = floating_pos.position;
        transform.rotation = Quaternion.identity;
        rb.isKinematic = false;
    }


}
