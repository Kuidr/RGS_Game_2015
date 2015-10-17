using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellManager : MonoBehaviour
{
    public List<Spell> spell_prefabs;
    private Dictionary<string, Spell> spellcode_dict = new Dictionary<string, Spell>();

    public Mage test_caster;


    public void Start()
    {
        foreach (Spell spell in spell_prefabs)
        {
            Spell spell_instance = Instantiate(spell);
            spell_instance.transform.parent = transform;

            spellcode_dict[spell.spellcode] = spell_instance;
        }
    }
    public void Cast(Mage caster, string spellcode)
    {
        spellcode_dict[spellcode].Cast(caster);
    }
}
