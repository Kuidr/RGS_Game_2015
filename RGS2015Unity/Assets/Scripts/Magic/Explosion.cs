using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        //gameObject.SetActive(false);
    }

	public void Explode(float radius, float max_force)
    {
        gameObject.SetActive(true);
        ps.Clear();
        ps.time = 0;
        ps.Play();

        
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {   
            // Projectiles / ball
            if (col.CompareTag("Projectile") || col.CompareTag("Ball"))
            {
                // Apply force in radius (exponentially decreasing with distance)
                Rigidbody2D r = col.GetComponent<Rigidbody2D>();
                Vector2 v = r.position - (Vector2)transform.position;
                float force = (1 - Mathf.Pow(v.magnitude / radius, 2f)) * max_force;
                r.AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
                continue;
            }

            // Resources
            else if (col.CompareTag("Crystal") || col.CompareTag("Rock"))
            {
                Resource res = col.GetComponent<Resource>();
                Vector2 v = col.transform.position - transform.position;
                float break_radius = radius / 1.5f;

                if (v.magnitude < break_radius)
                {
                    res.Break();
                    float force = (1 - Mathf.Pow(v.magnitude / break_radius, 2f)) * max_force;
                    col.GetComponent<Rigidbody2D>().AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
                }
            }
        }
    }
    public void Update()
    {
        if (!ps.isPlaying)
        {
            ps.Clear();
            gameObject.SetActive(false);
        }
    }

    public bool IsExploding()
    {
        return ps.isPlaying;
    }

}
