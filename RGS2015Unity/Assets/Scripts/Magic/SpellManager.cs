using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpellCastResult { Cast, OnCooldown, NotEnoughResources, InvalidSpellCode, NotEnoughFreeSlots };

public class SpellManager : MonoBehaviour
{
    public List<Spell> spell_prefabs;
    private List<Spell> spells = new List<Spell>();
    private Dictionary<string, Spell> spellcode_dict = new Dictionary<string, Spell>();

    // events
    public System.Action event_spelllist_populated;
    public System.Action<Spell> event_spell_cast;


    // PUBLIC MODIFIERS

    public SpellCastResult TryCast(Mage caster, string spellcode_uppercase, int free_slots, ref int crystals)
    {
        if (spellcode_dict.ContainsKey(spellcode_uppercase))
        {
            Spell spell = spellcode_dict[spellcode_uppercase];
            if (spell.IsOnCooldown()) return SpellCastResult.OnCooldown;
            if (spell.cost > crystals) return SpellCastResult.NotEnoughResources;
            if (spell.GetFreeSlotsRequired() > free_slots) return SpellCastResult.NotEnoughFreeSlots;


            crystals -= spell.cost;
            spell.Cast(caster);
            if (event_spell_cast != null) event_spell_cast(spell);
            return SpellCastResult.Cast;
        }
        else
        {
            return SpellCastResult.InvalidSpellCode;
        }
    }


    // PUBLIC ACCESSORS

    public List<Spell> GetSpells()
    {
        return spells;
    } 


    // PRIVATE MODIFIERS

    private void Start()
    {
        PopulateSpellList();
    }
    private void PopulateSpellList()
    {
        // create spell instances (one for each spell prefab)
        foreach (Spell spell in spell_prefabs)
        {
            Spell spell_instance = Instantiate(spell);
            spell_instance.transform.SetParent(transform);
            spell_instance.Initialize(this);

            spells.Add(spell_instance);
            spellcode_dict[spell_instance.spellcode] = spell_instance;
        }

        // sort spells list by spell cost
        spells.Sort(new SpellComparer());

        // send populate event
        if (event_spelllist_populated != null) event_spelllist_populated();
    }
}
