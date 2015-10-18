using UnityEngine;
using System.Collections;

public class ManaSlot : MonoBehaviour
{
    private Projectile projectile = null;
    private float cooldown_time = 10;
    private float cooldown_start_time = -1000;

    public bool Fill(Projectile projectile)
    {
        if (Available())
        {
            this.projectile = projectile;
            projectile.SetParentManaSlot(this);
            return true;
        }

        return false;
    }

    public bool Available()
    {
        return projectile == null && Time.time - cooldown_start_time > cooldown_time;
    }
    public Projectile GetProjectile()
    {
        return projectile;
    }
    public void Empty()
    {
        cooldown_start_time = Time.time;
        projectile = null;
    }

}
