﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mage : MonoBehaviour
{
    public Mage opponent;

    // General
    private int player_num = 1;
    private string player_name = "Player";
    private Color player_color = Color.red;

    private PlayerController pc;
    private Rigidbody2D rb;
    public Transform floating_pos;
    private Hover hover;
    private MageAudio mage_audio;

    // Sprites
    public SpriteRenderer spriterenderer_body, spriterenderer_highlights;
    public Sprite[] body_sprites;
    public Sprite[] highlights_sprites; // the sprites to be colored


    // Resources
    private int crystals;

    // Spells and Projectile
    private List<ManaSlot> mana_slots;
 
    private SpellManager spellmanager;
    public Transform cast_point;

    // Spell Code Visuals
    public TextMesh spell_code_text;
    private Color spell_code_initial_color;

    // State
    private int hearts;
    private bool invincible = true;
    private bool casting_allowed = true;

    // Events
    public System.Action<ManaSlot> event_fill_slot;
    public System.Action event_crystal_count_change;
    public System.Action<Mage> event_hearts_change;
    public System.Action<SpellCastResult> event_spell_cast;

    // Exclamation 
    private Dictionary<string, int> exclamation_codes = new Dictionary<string, int>()
    {
        { "XXA", 0 },
        { "XXX", 1 },
        { "XXY", 2 },
        { "XXB", 3 }
    };


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
    public void Hit()
    {
        TakeHit(1);
    }
    public void Silence(float duration)
    {
        spell_code_text.text = "";
        StartCoroutine(UpdateSilence(duration));
    }

    // PUBLIC ACCESSORS

    public string GetPlayerName()
    {
        return player_name;
    }
    public int GetPlayerNumber()
    {
        return player_num;
    }
    public Color GetPlayerColor()
    {
        return player_color;
    }
    public Mage GetOpponent()
    {
        return opponent;
    }
    public Collider2D GetHitCollider()
    {
        return GetComponent<Collider2D>();
    }

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
        return GameSettings.Instance.GetNumHearts();
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
        spell_code_initial_color = spell_code_text.color;
        mage_audio = GetComponentInChildren<MageAudio>();
    }
    private void Start()
    {
        Refresh();
    }
    public void Inititalize(int number, MatchManager manager)
    {
        // player info
        player_num = number;
        player_name = GameSettings.Instance.player_name[number - 1];
        player_color = GameSettings.Instance.GetPlayerColor(number);

        // player_state
        hearts = GameSettings.Instance.GetNumHearts();
        crystals = GameSettings.Instance.GetNumCrystals();

        // Player controller
        if (GameSettings.Instance.IsAIControlled(number))
        {
            gameObject.AddComponent<AIPlayerController>();
            GetComponent<AIPlayerController>().Initialize(this, opponent, manager);
        }
        else
        {
            gameObject.AddComponent<HumanPlayerController>();
            GetComponent<HumanPlayerController>().Initialize(
                GameSettings.Instance.GetHumanControlScheme(number));
        }
        this.pc = GetComponent<PlayerController>();

        // input events
        pc.InputCast += OnCastSpell;
        pc.InputSpellCodeA += OnSpellCodeA;
        pc.InputSpellCodeB += OnSpellCodeB;
        pc.InputSpellCodeX += OnSpellCodeX;
        pc.InputSpellCodeY += OnSpellCodeY;

        // other references
        rb = GetComponent<Rigidbody2D>();
        hover = GetComponent<Hover>();

        // sprites
        SetGraphicsNormal();
        spriterenderer_highlights.color = player_color;

        // spells
        spellmanager = FindObjectOfType<SpellManager>();
        if (spellmanager == null) Debug.LogError("SpellManager not found");

        mana_slots = new List<ManaSlot>(GameSettings.Instance.GetNumSlots());
        for (int i = 0; i < GameSettings.Instance.GetNumSlots(); ++i)
            mana_slots.Add(new ManaSlot());
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
        if (!invincible && collision.collider.CompareTag("Ball"))
        {
            TakeHit(collision.relativeVelocity.magnitude / 1f);
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            mage_audio.PlayBump(collision.relativeVelocity.magnitude / 5f);
        }
    }

    private void Refresh()
    {
        SetGraphicsNormal();
        spell_code_text.text = "";
        StopAllCoroutines();
        StartCoroutine(FloatUp());
    }
    private void TakeHit(float force)
    {
        if (IsDead() || invincible) return;

        // sprites
        SetGraphicsHit();

        // sound
        mage_audio.PlayBump(force);
        mage_audio.PlayGrunt();

        // fall down
        hover.StartFadeOut();
        rb.gravityScale = 1;
        casting_allowed = false;

        // dispel slot projectiles
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.GetProjectile() != null) slot.GetProjectile().Kill();
            slot.Empty(ManaSlotCooldown.Short);
        }

        // damage
        hearts -= 1;
        if (event_hearts_change != null) event_hearts_change(this);
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
        Explode(10, 10);
    }
    public void Explode(float radius, float max_force)
    {

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            // Projectiles / ball
            if (col.CompareTag("Projectile") || col.CompareTag("Ball"))
            {
                // Apply force in radius (exponentially decreasing with distance)
                Rigidbody2D r = col.GetComponent<Rigidbody2D>();
                Vector2 v = r.position - (Vector2)transform.position;
                float force = (1 - Mathf.Pow(v.magnitude / radius, 2f)) * max_force;
                r.AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
                continue;
            }

            // Resources
            else if (col.CompareTag("Crystal") || col.CompareTag("Rock"))
            {
                Resource res = col.GetComponent<Resource>();
                Vector2 v = col.transform.position - transform.position;
                float break_radius = radius / 1.5f;

                if (v.magnitude < break_radius)
                {
                    res.Break();
                    float force = (1 - Mathf.Pow(v.magnitude / break_radius, 2f)) * max_force;
                    col.GetComponent<Rigidbody2D>().AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnCastSpell()
    {
        if (casting_allowed && spell_code_text.text != "")
        {
            // Exclamation
            int exclamation_num;
            if (exclamation_codes.TryGetValue(spell_code_text.text, out exclamation_num))
            {
                Debug.Log("exclaim");
                mage_audio.PlayExclamation(exclamation_num);
                spell_code_text.text = "";
                return;
            }


            // Spell
            int crystals_old = crystals;
            SpellCastResult result = spellmanager.TryCast(this, spell_code_text.text, ref crystals);
            
            // crystals spent
            if (crystals != crystals_old)
            {
                if (event_crystal_count_change != null) event_crystal_count_change();
            }

            // event
            if (event_spell_cast != null) event_spell_cast(result);


            // bad cast
            if (!result.success)
            {
                if (result.invalid_spell_code)
                {
                    StartCoroutine(FlashSpellCodeText());
                }
                else
                {
                    StartCoroutine(MakeRedSpellCodeText());
                }
            }
            // good cast
            else
            {
                spell_code_text.text = "";
            }
        }   
    }
    private void OnSpellCodeA()
    {
        if (!casting_allowed) return;
        spell_code_text.text += "A";
    }
    private void OnSpellCodeB()
    {
        if (!casting_allowed) return;
        spell_code_text.text += "B";
    }
    private void OnSpellCodeX()
    {
        if (!casting_allowed) return;
        spell_code_text.text += "X";
    }
    private void OnSpellCodeY()
    {
        if (!casting_allowed) return;
        spell_code_text.text += "Y";
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

        while (true)
        {
            spriterenderer_highlights.color = Color.white;
            yield return new WaitForSeconds(0.025f);
            spriterenderer_highlights.color = player_color;
            yield return new WaitForSeconds(0.025f);
            if (Time.time - start_time >= duration) break;
        }

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
        hover.StartFadeIn();
    }
    private IEnumerator FlashSpellCodeText()
    {
        casting_allowed = false;
        Color[] colors = new Color[] { Color.clear, Color.red };

        for (int i = 0; i < 6; ++i)
        {
            spell_code_text.color = colors[i%2];
            yield return new WaitForSeconds(0.075f);
        }
        spell_code_text.color = spell_code_initial_color;
        spell_code_text.text = "";

        casting_allowed = true;
    }
    private IEnumerator MakeRedSpellCodeText()
    {
        casting_allowed = false;
        Color[] colors = new Color[] { Color.clear, Color.red };

        spell_code_text.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spell_code_text.color = spell_code_initial_color;
        spell_code_text.text = "";

        casting_allowed = true;
    }
    private IEnumerator UpdateSilence(float duration)
    {
        casting_allowed = false;
        yield return new WaitForSeconds(duration);
        casting_allowed = true;
    }

    private void SetGraphicsHit()
    {
        spriterenderer_body.sprite = body_sprites[1];
        spriterenderer_highlights.sprite = highlights_sprites[1];
    }
    private void SetGraphicsNormal()
    {
        spriterenderer_body.sprite = body_sprites[0];
        spriterenderer_highlights.sprite = highlights_sprites[0];
    }


    // PRIVATE ACCESSORS HELPERS

    public int NumFreeSlots()
    {
        int count = 0;
        foreach (ManaSlot slot in mana_slots)
        {
            if (slot.IsAvailable()) count++;
        }
        return count;
    }


}
