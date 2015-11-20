using UnityEngine;
using System.Collections;

public class SEDarkness : SpellEffect
{
    private float seconds = 5f;

    public override void Do(Mage caster, Spell origin_spell)
    {
        SEDarkness st = Instantiate(this);
        st.StartCoroutine(st.UpdateDarkness());

        base.Do(caster, origin_spell);
    }


    public IEnumerator UpdateDarkness()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0;

        float start_time = Time.time;

        // fade in
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            color.a = t*t;
            sr.color = color;
            yield return null;
        }

        // dark with blinks
        while (Time.time - start_time < seconds)
        {
            yield return new WaitForSeconds(Random.value * 1.75f);
            t = 0;
            float speed = Random.Range(1, 3f);
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                color.a = 1 - Mathf.Sin(t*Mathf.PI)*0.2f;
                sr.color = color;
                yield return null;
            }
        }

        // fade out
        t = 0;
        float a_start = color.a;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            color.a = Mathf.Lerp(a_start, 0, t * t);
            sr.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}
