using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    public Mage mage;

    public Text name_text;
    public LayoutGroup row_hearts;
    public LayoutGroup row_slots;
    public Text crystals_text;

    public Image prefab_heart;
    public SpellSlotIcon prefab_slot_icon;
    public Sprite sprite_slot_empty;
    public Sprite sprite_slot_cd;
    public Sprite sprite_heart_full;
    public Sprite sprite_heart_empty;

    private List<Image> hearts = new List<Image>();
    private Dictionary<ManaSlot, SpellSlotIcon> slot_icons = new Dictionary<ManaSlot, SpellSlotIcon>();


    private void Start()
    {
        PopulateUIRow();

        // events
        mage.event_fill_slot += OnSlotFilled;
        foreach (ManaSlot slot in mage.GetManaSlots()) slot.event_emptied += OnSlotEmptied;
        mage.event_crystal_count_change += OnCrystalCountChange;
        mage.event_hearts_change += OnHeartsChange;
    }
    private void PopulateUIRow()
    {
        // name
        //name_text = mage.plauer

        // hearts
        for (int i = 0; i < mage.GetMaxHearts(); ++i)
        {
            Image heart = Instantiate(prefab_heart);
            heart.transform.SetParent(row_hearts.transform, false);
            hearts.Add(heart);
        }

        // spell slots
        foreach (ManaSlot slot in mage.GetManaSlots())
        { 
            SpellSlotIcon slot_icon = Instantiate(prefab_slot_icon);
            slot_icon.SetSpellIcon(sprite_slot_empty);
            slot_icon.transform.SetParent(row_slots.transform, false);
            slot_icons.Add(slot, slot_icon);
        }

        // crystal count text
        crystals_text.text = mage.GetCrystalCount().ToString();
    }

    private void OnHeartsChange()
    {
        if (hearts.Count != mage.GetMaxHearts()) Debug.LogError("No support now for changing max hearts");

        for (int i = 0; i < mage.GetHearts(); ++i)
        {
            hearts[i].sprite = sprite_heart_full;
        }
        for (int i = mage.GetHearts(); i < mage.GetMaxHearts(); ++i)
        {
            hearts[i].sprite = sprite_heart_empty;
        }
    }
    private void OnCrystalCountChange()
    {
        crystals_text.text = mage.GetCrystalCount().ToString();
    }
    private void OnSlotFilled(ManaSlot slot)
    {
        SpellSlotIcon icon = slot_icons[slot];
        icon.SetSpellIcon(slot.GetOriginSpell().icon_sprite);
    }
    private void OnSlotEmptied(ManaSlot slot)
    {
        SpellSlotIcon icon = slot_icons[slot];
        icon.SetCooldown(slot, sprite_slot_cd);
    }
}
