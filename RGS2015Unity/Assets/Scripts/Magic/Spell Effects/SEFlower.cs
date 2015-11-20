using UnityEngine;
using System.Collections;

public class SEFlower : SpellEffect
{
    public Projectile projectile_prefab;

    public override void Do(Mage caster, Spell origin_spell)
    {
        for (int i = 0; i < 10; ++i)
        {
            float a = (i / 10f) * Mathf.PI * 2f;
            Vector2 pos = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 0.4f;

            Projectile p = Instantiate<Projectile>(projectile_prefab);
            p.Initialize(caster, pos);
            p.GetRigidbody().AddForce(pos.normalized * 2f, ForceMode2D.Impulse);
        }
    }
}
