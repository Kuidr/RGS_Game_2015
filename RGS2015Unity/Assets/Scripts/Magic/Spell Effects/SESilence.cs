using UnityEngine;
using System.Collections;

public class SESilence : SpellEffect
{
    public float duration = 1;

    public override void Do(Mage caster, Spell origin_spell)
    {
        caster.opponent.Silence(duration);

        SESilence silence = Instantiate(this);
        silence.StartCoroutine(silence.AdjustVolume());

        base.Do(caster, origin_spell);
    }

    private IEnumerator AdjustVolume()
    {
        float v = AudioListener.volume;
        AudioListener.volume = 0;
        yield return new WaitForSeconds(duration);
        AudioListener.volume = v;
    }
}
