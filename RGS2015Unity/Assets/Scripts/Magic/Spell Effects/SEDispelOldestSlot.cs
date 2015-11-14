using UnityEngine;
using System.Collections;

public class SEDispelOldestSlot : SpellEffect
{
    private float max_circle_scale = 0.35f;
    private float delay = 2f;

    public override void Do(Mage caster, Spell origin_spell)
    {
        SEDispelOldestSlot dispel = Instantiate(this);
        dispel.StartCoroutine(dispel.UpdateDispel(caster));

        
        base.Do(caster, origin_spell);
    }

    public IEnumerator UpdateDispel(Mage caster)
    {
        ManaSlot slot = caster.GetOldestFilledManaSlot();
        ControlledProjectile p = slot.GetProjectile();
        if (p != null)
        {
            // effect and delay
            SpriteRenderer circle = GetComponentInChildren<SpriteRenderer>();
            float t = 0;
            while (t < 1)
            {
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
