using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellSlotIcon : MonoBehaviour
{
    public Image icon;
    public Sprite empty_slot;

    public void SetCooldown(ManaSlot slot, Sprite cd_sprite)
    {
        icon.sprite = cd_sprite;
        StopAllCoroutines();
        StartCoroutine(UpdateCoolingDown(slot));
    }
    public void SetSpellIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    private IEnumerator UpdateCoolingDown(ManaSlot slot)
    {
        float cd = 0;
        float rev = 360f * slot.GetCooldownTime(); // one revolution for each second of cooldown

        while (true)
        {
            cd = Mathf.Pow(1 - slot.GetCooldownPercent(), 2);
            transform.rotation = Quaternion.Euler(0, 0, cd * rev);

            if (!slot.IsOnCooldown()) break;
            yield return null;
        }
        SetSpellIcon(empty_slot);
    }
}
