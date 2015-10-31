using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UISpellList : MonoBehaviour
{
    public VerticalLayoutGroup col_spell_codes;
    public VerticalLayoutGroup col_spell_symbols;
    public VerticalLayoutGroup col_spell_cd_icons;

    public Text prefab_spell_code;
    public Image prefab_spell_symbol;
    public CooldownIcon prefab_cooldown_icon;

    private Dictionary<Spell, SpellUIRow> rows = new Dictionary<Spell, SpellUIRow>();

    private SpellManager spellmanager;


    private void Awake()
    {
        // spell manager
        spellmanager = FindObjectOfType<SpellManager>();
        if (spellmanager == null) Debug.LogError("SpellManager not found");

        // events
        spellmanager.event_spelllist_populated += PopulateUISpellList;
        spellmanager.event_spell_cast += OnSpellCast;
    }
    private void PopulateUISpellList()
    {
        // Create a row of ui info for each spell manager spell
        foreach (Spell spell in spellmanager.GetSpells())
        {
            SpellUIRow row = new SpellUIRow();

            // Code
            row.code = Instantiate(prefab_spell_code);
            row.code.text = spell.spellcode + "." + spell.cost;
            row.code.transform.SetParent(col_spell_codes.transform, false);

            // Symbol
            row.symbol = Instantiate(prefab_spell_symbol);
            row.symbol.sprite = spell.icon_sprite;
            row.symbol.transform.SetParent(col_spell_symbols.transform, false);

            // Cooldown icon
            row.cd_icon = Instantiate(prefab_cooldown_icon);
            row.cd_icon.transform.SetParent(col_spell_cd_icons.transform, false);

            // save row
            rows.Add(spell, row);
        }
    }
    private void OnSpellCast(Spell spell)
    {
        // start cooldown icon
        if (spell.OnCooldown())
        {
            SpellUIRow row = rows[spell];
            if (row != null) row.cd_icon.Enable(spell);
        }
    }

    private class SpellUIRow
    {
        public Text code;
        public Image symbol;
        public CooldownIcon cd_icon;
    }

}
