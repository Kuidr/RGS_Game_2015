using UnityEngine;
using System.Collections;

public class SESlowTime : SpellEffect
{
    public override void Do(Mage caster)
    {
        SESlowTime st = Instantiate(this);
        st.StartCoroutine(st.UpdateSlowTime());

        base.Do(caster);
    }


    public IEnumerator UpdateSlowTime()
    {
        float t = 0;

        while (true)
        {
            t += Time.unscaledDeltaTime / 10f;
            if (t >= 1) break;

            Debug.Log(1 - Mathf.Sin(t * Mathf.PI));

            TimeScaleManager.Instance.AddMultiplier("SEslowtime", 1f - Mathf.Sin(t * Mathf.PI), true);

            yield return null;
        }
        TimeScaleManager.Instance.RemoveMultiplier("SEslowtime", true);

        Destroy(gameObject);
    }
}
