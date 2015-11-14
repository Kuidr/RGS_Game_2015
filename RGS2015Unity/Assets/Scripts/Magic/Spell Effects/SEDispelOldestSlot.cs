using UnityEngine;
using System.Collections;

public class SEDispelOldestSlot : SpellEffect
{
    private float max_circle_scale = 0.35f;
    private float delay = 1f;

    public override void Do(Mage caster, Spell origin_spell)
    {
        SEDispelOldestSlot dispel = Instantiate(this);
        dispel.StartCoroutine(dispel.UpdateDispel(caster));
        
        base.Do(caster, origin_spell);
    }

    public IEnumerator UpdateDispel(Mage caster)
    {
        // find a good projectile to dispel
        ManaSlot slot = null;
        ControlledProjectile p = null;
        foreach (ManaSlot s in caster.GetManaSlots())
        {
            if (!s.dispelling)
            {
                p = s.GetProjectile();
                if (p != null)
                {
                    slot = s;
                    s.dispelling = true;
                    break;
                }
            }
        }

        // dispel
        if (slot != null)
        {
            // effect and delay
            SpriteRenderer circle = GetComponentInChildren<SpriteRenderer>();
            float t = 0;
            while (t < 1)
            {
                if (p == null) break;
                t += Time.deltaTime / (delay + 0.0001f);
                circle.transform.localScale = new Vector3(t, t, t) * max_circle_scale;
                circle.transform.position = p.transform.position;
                yield return null;
            }
            circle.transform.localScale = Vector3.zero;

            // dispel
            if (p != null)
            {
                slot.Empty(ManaSlotCooldown.Short);
                p.Destroy();
            }
        }

        
    }
}
