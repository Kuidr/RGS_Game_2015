using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpellCastResult { Cast, OnCooldown, NotEnoughResources, InvalidSpellCode };

public class SpellManager : MonoBehaviour
{
    public List<Spell> spell_prefabs;
    private Dictionary<string, Spell> spellcode_dict = new Dictionary<string, Spell>();


    public void Start()
    {
        foreach (Spell spell in spell_prefabs)
        {
            Spell spell_instance = Instantiate(spell);
            spell_instance.transform.parent = transform;

            spellcode_dict[spell.spellcode] = spell_instance;
        }
    }
    public SpellCastResult TryCast(Mage caster, string spellcode, ref int resources)
    {
        if (spellcode_dict.ContainsKey(spellcode))
        {
            Spell spell = spellcode_dict[spellcode];
            if (spell.OnCooldown()) return SpellCastResult.OnCooldown;
            if (spell.GetCost() > resources) return SpellCastResult.NotEnoughResources;


            resources -= spell.GetCost();
            spell.Cast(caster);
            return SpellCastResult.Cast;
        }
        else
        {
            return SpellCastResult.InvalidSpellCode;
        }
    }
}
