using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell : MonoBehaviour
{
    // general
    public string name;
    public string spellcode;
    public int cost_a, cost_b, cost_x, cost_y;

    // cooldown
    public int cooldown_time; // seconds
    private float last_cast_time = -10000;

    // references
    private SpellManager manager;

    // effects
    public List<MagicEffect> effects;


    public void Initialize(SpellManager manager)
    {
        this.manager = manager;
    }
    public bool Cast(Mage caster)
    {
        Debug.Log(last_cast_time + "   " + Time.time);
        if (Time.time - last_cast_time > cooldown_time)
        {
            last_cast_time = Time.time;

            foreach (MagicEffect effect in effects)
            {
                effect.Do(caster);
            }

            return true;
        }

        Debug.Log("not cast");

        return false;
    }
}
