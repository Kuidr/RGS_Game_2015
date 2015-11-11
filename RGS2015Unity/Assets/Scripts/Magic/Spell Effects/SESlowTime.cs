using UnityEngine;
using System.Collections;

public class SESlowTime : SpellEffect
{
    public override void Do(Mage caster, Spell origin_spell)
    {
        SESlowTime st = Instantiate(this);
        st.StartCoroutine(st.UpdateSlowTime(caster));

        base.Do(caster, origin_spell);
    }


    public IEnumerator UpdateSlowTime(Mage caster)
    {
        //float t = 0;
        TimeScaleManager.Instance.AddMultiplier("SEslowtime", 0.1f, true);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(5f));

        /*
        while (true)
        {
            t += Time.unscaledDeltaTime / 10f;
            if (t >= 1) break;

            TimeScaleManager.Instance.AddMultiplier("SEslowtime", 1f - Mathf.Sin(t * Mathf.PI), true);

            yield return null;
        }
        */
        TimeScaleManager.Instance.RemoveMultiplier("SEslowtime", true);

        Destroy(gameObject);
    }
}
