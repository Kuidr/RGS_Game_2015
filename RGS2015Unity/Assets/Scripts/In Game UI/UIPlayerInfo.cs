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

    private IEnumerator flash_crystal_count;
    private IEnumerator flash_slot_icons;


    private void Start()
    {
        PopulateUIRow();

        // events
        mage.event_fill_slot += OnSlotFilled;
        foreach (ManaSlot slot in mage.GetManaSlots()) slot.event_emptied += OnSlotEmptied;
        mage.event_crystal_count_change += OnCrystalCountChange;
        mage.event_hearts_change += OnHeartsChange;
        mage.event_spell_cast += OnSpellCast;
    }
    private void PopulateUIRow()
    {
        // name
        name_text.text = mage.GetPlayerName();

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

    private void OnHeartsChange(Mage mage)
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
    private void OnSpellCast(SpellCastResult result)
    {
        // not enough free slots alert
        if (result.not_enough_free_slots)
        {
            if (flash_slot_icons != null) StopCoroutine(flash_slot_icons);
            flash_slot_icons = FlashSlotIcons();
            StartCoroutine(flash_slot_icons);
        }

        // not enough resources alert
        if (result.not_enough_resources)
        {
            if (flash_crystal_count != null) StopCoroutine(flash_crystal_count);
            flash_crystal_count = FlashCrystalCount();
            StartCoroutine(flash_crystal_count);
        }
    }

    private IEnumerator FlashCrystalCount()
    {
        crystals_text.enabled = true;
        for (int i = 0; i < 12; ++i)
        {
            crystals_text.enabled = !crystals_text.enabled;
            yield return new WaitForSeconds(0.075f);
        }
        crystals_text.enabled = true;
    }
    private IEnumerator FlashSlotIcons()
    {
        foreach (SpellSlotIcon icon in slot_icons.Values)
            icon.SetVisible(true);

        for (int i = 0; i < 12; ++i)
        {
            foreach (SpellSlotIcon icon in slot_icons.Values)
                icon.SetVisible(!icon.IsVisible());
            yield return new WaitForSeconds(0.075f);
        }

        foreach (SpellSlotIcon icon in slot_icons.Values)
            icon.SetVisible(true);
    }
}
