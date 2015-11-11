using UnityEngine;
using System.Collections;

public class SESilence : SpellEffect
{
    public float duration = 1;

    public override void Do(Mage caster, Spell origin_spell)
    {
        caster.opponent.Silence(duration);
        base.Do(caster, origin_spell);
    }
}
