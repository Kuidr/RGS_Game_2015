using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellInfoUI : MonoBehaviour
{
    public SpellManager spell_manager;
    public SpellInfo spell_info_prefab;

    public void Start()
    {
        Populate();
    }

    private void Populate()
    {
        foreach (Spell spell in spell_manager.GetSpells())
        {
            SpellInfo info = Instantiate(spell_info_prefab);
            info.transform.SetParent(transform);

            info.icon.sprite = spell.icon_sprite;
            info.name_text.text = spell.name;
            info.spellcode.text = spell.GetSpellCodeCostText();
            info.screenshot.sprite = spell.info_screenshot;
            info.description_text.text = spell.info_text;
        }
    }
}
