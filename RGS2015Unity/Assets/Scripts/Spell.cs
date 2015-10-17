using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{
    // general
    public string name;
    public int cost_a, cost_b, cost_x, cost_y;

    // cooldown
    public int cooldown_time; // seconds
    private float last_cast_time = -10000;

    // references
    private SpellManager manager;


    public void Initialize(SpellManager manager)
    {
        this.manager = manager;
    }
    public bool Cast(Mage mage)
    {
        if (Time.time - last_cast_time > cooldown_time)
        {
            last_cast_time = Time.time;

        }


        return false;
    }
}
