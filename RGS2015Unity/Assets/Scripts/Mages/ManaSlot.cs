using UnityEngine;
using System.Collections;

public enum ManaSlotCooldown { Normal, Short, Long };

public class ManaSlot
{
    private const float CooldownNormal = 5f;
    private const float CooldownShort = 2f;
    private const float CooldownLong = 10f;

    private Spell origin_spell;
    private ControlledProjectile projectile = null;
    private float cooldown_time = 0;
    private float fill_time = -1000; // Time.time when filled
    public bool dispelling = false;

    // events
    public System.Action<ManaSlot> event_emptied;


    public bool Fill(ControlledProjectile projectile, Spell origin_spell)
    {
        if (IsAvailable())
        {
            this.projectile = projectile;
            this.origin_spell = origin_spell;
            projectile.SetManaSlot(this);
            return true;
        }

        return false;
    }
    public void Empty(ManaSlotCooldown cooldown)
    {
        fill_time = Time.time;

        cooldown_time = cooldown == ManaSlotCooldown.Normal ? CooldownNormal :
                        cooldown == ManaSlotCooldown.Short ? CooldownShort :
                        CooldownLong;

        if (event_emptied != null) event_emptied(this);

        dispelling = false;
    }

    public bool IsAvailable()
    {
        return projectile == null && !IsOnCooldown();
    }
    public bool IsOnCooldown()
    {
        return Time.time - fill_time < cooldown_time;
    }
    public float GetFillTime()
    {
        return fill_time;
    }
    public float GetCooldownPercent()
    {
        return (Time.time - fill_time) / cooldown_time;
    }
    public float GetCooldownTime()
    {
        return cooldown_time;
    }
    
    public ControlledProjectile GetProjectile()
    {
        return projectile;
    }
    public Spell GetOriginSpell()
    {
        return origin_spell;
    }
}
