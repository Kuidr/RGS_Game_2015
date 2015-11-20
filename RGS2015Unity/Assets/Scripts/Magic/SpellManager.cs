using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpellManager : MonoBehaviour
{
    public List<Spell> spell_prefabs;
    private List<Spell> spells = new List<Spell>();
    private Dictionary<string, Spell> spellcode_dict = new Dictionary<string, Spell>();

    // events
    public System.Action event_spelllist_populated;
    public System.Action<SpellCastResult> event_spell_cast;


    // PUBLIC MODIFIERS

    public SpellCastResult TryCast(Mage caster, string spellcode_uppercase, ref int crystals)
    {
        SpellCastResult result = new SpellCastResult();
        Spell spell = null;

        if (spellcode_dict.ContainsKey(spellcode_uppercase))
        {
            spell = spellcode_dict[spellcode_uppercase];

            // check prerequisites
            result.on_cooldown = spell.IsOnCooldown();
            result.not_enough_resources = spell.cost > crystals;
            result.not_enough_free_slots = spell.GetFreeSlotsRequired() > caster.NumFreeSlots();

            if (!result.on_cooldown && !result.not_enough_free_slots && !result.not_enough_resources)
            {
                // successful cast
                crystals -= spell.cost;
                spell.Cast(caster);
                result.success = true;
            } 
        }
        else
        {
            // bad spell code
            result.invalid_spell_code = true;
        }


        // return and events
        result.spell = spell;
        if (event_spell_cast != null) event_spell_cast(result);
        return result;
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

public class SpellCastResult
{
    public Spell spell;
    public bool success = false;
    public bool invalid_spell_code = false;
    public bool on_cooldown = false;
    public bool not_enough_resources = false;
    public bool not_enough_free_slots = false;
}