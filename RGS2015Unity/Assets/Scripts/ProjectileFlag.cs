using UnityEngine;
using System.Collections;

public class ProjectileFlag : MonoBehaviour
{
    private Projectile projectile;
    private const float Height = 0.2f;

    public void Initialize(Projectile projectile, Vector2 pos, Color color)
    {
        this.projectile = projectile;
        transform.position = pos;
        GetComponent<SpriteRenderer>().color = color;
    }
	private void Update()
    {
        if (projectile == null) return;
        transform.position = Vector2.Lerp(transform.position,
            (Vector2)projectile.transform.position + Vector2.up * Height, Time.deltaTime * 6f);
    }
}
