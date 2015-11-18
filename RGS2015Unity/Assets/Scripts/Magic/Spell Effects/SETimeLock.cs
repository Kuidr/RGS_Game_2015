using UnityEngine;
using System.Collections;

public class SETimeLock : SpellEffect
{
    public override void Do(Mage caster, Spell origin_spell)
    {
        SETimeLock st = Instantiate(this);
        st.StartCoroutine(st.UpdateTimeLock(caster));

        base.Do(caster, origin_spell);
    }


    public IEnumerator UpdateTimeLock(Mage caster)
    {
        //float t = 0;
        TimeScaleManager.Instance.AddMultiplier("SEslowtime", 0.5f, true);
        
        foreach (ManaSlot slot in caster.GetOpponent().GetManaSlots())
        {
            ControlledProjectile cp = slot.GetProjectile();
            if (cp != null) cp.TimeLock();
        }

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(5f));

        foreach (ManaSlot slot in caster.GetOpponent().GetManaSlots())
        {
            ControlledProjectile cp = slot.GetProjectile();
            if (cp != null) cp.UnTimeLock();
        }

        TimeScaleManager.Instance.RemoveMultiplier("SEslowtime", true);

        Destroy(gameObject);
    }
}
