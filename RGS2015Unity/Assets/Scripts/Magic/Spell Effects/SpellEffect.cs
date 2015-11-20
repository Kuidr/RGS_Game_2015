using UnityEngine;
using System.Collections;

public class SpellEffect : MonoBehaviour
{
    public virtual int FreeSlotsRequired { get { return 0; } }

    public virtual void Do(Mage caster, Spell origin_spell)
    {
    }
}
