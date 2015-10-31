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
    }
    /// <summary>
    /// This will force the casting of the spell regardless of resource cost and cooldown.
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public bool Cast(Mage caster)
    {
        last_cast_time = Time.time;

        foreach (SpellEffect effect in effects)
        {
            effect.Do(caster);
        }

        return true;
    }

    public bool OnCooldown()
    {
        return Time.time - last_cast_time < cooldown_time;
    }
    public float GetCooldownPercent()
    {
        return (Time.time - last_cast_time) / cooldown_time;
    }

    private void Awake()
    {
        spellcode = spellcode.ToUpper(); // insure uppercase spellcode 
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
