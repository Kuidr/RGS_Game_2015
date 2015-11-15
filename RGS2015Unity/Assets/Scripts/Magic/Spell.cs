using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Spell : MonoBehaviour
{
    // general
    public string name;
    public string spellcode;
    public int cost;
    public Sprite icon_sprite;
    public WorldSound cast_sound_prefab;
    private int free_slots_required;

    // cooldown
    public float cooldown_time; // seconds
    private float last_cast_time = -10000;

    // references
    private SpellManager manager;

    // effects
    public List<SpellEffect> effects;


    public void Initialize(SpellManager manager)
    {
        this.manager = manager;

        // calculate free slots required
        free_slots_required = 0;
        foreach (SpellEffect effect in effects)
        {
            free_slots_required += effect.FreeSlotsRequired;
        }

        // sounds
        if (cast_sound_prefab != null) ObjectPool.Instance.RequestObjects(cast_sound_prefab, 3, true);
    }
    /// <summary>
    /// This will force the casting of the spell regardless of resource cost and cooldown.
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public bool Cast(Mage caster)
    {
        last_cast_time = Time.time;

        PlayCastSound();

        foreach (SpellEffect effect in effects)
        {
            effect.Do(caster, this);
        }

        return true;
    }

    public bool IsOnCooldown()
    {
        return Time.time - last_cast_time < cooldown_time;
    }
    public float GetCooldownPercent()
    {
        return (Time.time - last_cast_time) / cooldown_time;
    }
    public int GetFreeSlotsRequired()
    {
        return free_slots_required;
    }


    private void Awake()
    {
        spellcode = spellcode.ToUpper(); // insure uppercase spellcode 
    }
    private void PlayCastSound()
    {
        if (cast_sound_prefab == null) return;
        WorldSound s = ObjectPool.Instance.GetObject(cast_sound_prefab, false);

        s.transform.position = transform.position;
        s.base_volume = 1;
        s.SetPitchOffset(UnityEngine.Random.Range(-0.05f, 0.05f));
        s.Play();
    }

}

public class SpellComparer : IComparer<Spell>
{
    public int Compare(Spell x, Spell y)
    {
        int comp_cost = x.cost.CompareTo(y.cost);
        if (comp_cost != 0) return comp_cost;

        int comp_spellcode = x.spellcode.CompareTo(y.spellcode);
        return comp_spellcode;
    }
}
