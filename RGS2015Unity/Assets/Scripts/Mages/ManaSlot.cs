using UnityEngine;
using System.Collections;

public enum ManaSlotCooldown { Normal, Short, Long };

public class ManaSlot
{
    private const float CooldownNormal = 5f;
    private const float CooldownShort = 2f;
    private const float CooldownLong = 10f;

    private Projectile projectile = null;
    private float cooldown_time = 0;
    private float fill_time = -1000; // Time.time when filled

    public bool Fill(Projectile projectile)
    {
        if (IsAvailable())
        {
            this.projectile = projectile;
            projectile.SetParentManaSlot(this);
            return true;
        }

        return false;
    }
    public void Empty(ManaSlotCooldown cooldown)
    {
        fill_time = Time.time;
        projectile = null;

        cooldown_time = cooldown == ManaSlotCooldown.Normal ? CooldownNormal :
                        cooldown == ManaSlotCooldown.Short ? CooldownShort :
                        CooldownLong;
    }

    public bool IsAvailable()
    {
        return projectile == null && Time.time - fill_time > cooldown_time;
    }
    public float GetFillTime()
    {
        return fill_time;
    }
    public Projectile GetProjectile()
    {
        return projectile;
    }
}
