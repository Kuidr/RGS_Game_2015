using UnityEngine;
using System.Collections;

public class BackgroundLighting : MonoBehaviour
{
    public SpriteRenderer glowspot_prefab;
    private SpriteRenderer[][] glowspots;
    private int rows = 12, cols = 21;
    private float width = 16, height = 9;

    private void Start()
    {
        width -= 3;
        float start_x = -width / 2f;
        float start_y = -height / 2f;

        glowspots = new SpriteRenderer[cols][];
        for (int x = 0; x < cols; ++x)
        {
            glowspots[x] = new SpriteRenderer[rows];
            for (int y = 0; y < rows; ++y)
            {
                SpriteRenderer g = Instantiate(glowspot_prefab);
                g.transform.parent = transform;
                
                g.transform.position = new Vector2(start_x + ((float)x / (cols-1)) * width,
                    start_y + ((float)y / (rows-1)) * height);
                glowspots[x][y] = g;
            }
        }
    }

    public void Update()
    {
        // reset
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {
                glowspots[x][y].color = new Color(0, 0, 0, 0);
            }
        }
    }
    public void Light(Vector2 pos, Color color, bool raycast=false)
    {
        float mult = 1;

        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {

                if (raycast)
                {
                    Vector2 dir = ((Vector2)glowspots[x][y].transform.position - pos);
                    RaycastHit2D hit = Physics2D.Raycast(pos, dir.normalized, dir.magnitude);
                    if (hit.collider != null)
                    {
                        //Debug.DrawLine(pos, hit.point, Color.red);
                        mult = 0.35f;
                    }
                    else mult = 1;
                }

                float dist = Vector2.Distance(glowspots[x][y].transform.position, pos);
                Color c = glowspots[x][y].color;
                c += color * mult * Mathf.Clamp(3f - dist, 0, 3f) / 3f;
                c.a = Mathf.Min(c.a, 0.1f);
                glowspots[x][y].color = c;
            }
        }
    }
}
