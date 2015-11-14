using UnityEngine;
using System.Collections;

public class BackgroundLighting : MonoBehaviour
{
    public SpriteRenderer glowspot_prefab;
    private SpriteRenderer[][] glowspots;
    private int rows = 12, cols = 21;
    private float width = 16, height = 9;
    private Vector2 grid_start = new Vector2();

    private int check_size = 2; // number of grid units in all directions to check a light source against
    private float max_glow = 0.15f;


    private void Start()
    {
        
        width -= 3;
        float start_x = -width / 2f;
        float start_y = -height / 2f;
        grid_start = new Vector2(start_x, start_y);

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
        //raycast = false;
        float mult = 1;

        int grid_x = (int)(((pos.x - grid_start.x)/width) * (cols - 1));
        int grid_y = (int)(((pos.y - grid_start.y)/height) * (rows - 1));

        for (int x = (int)Mathf.Max(0, grid_x - check_size); x < (int)Mathf.Min(cols, grid_x + check_size + 1); ++x)
        {
            for (int y = (int)Mathf.Max(0, grid_y - check_size); y < (int)Mathf.Min(rows, grid_y + check_size + 1); ++y)
            {

                //Debug.DrawLine(pos, (Vector2)glowspots[x][y].transform.position, Color.red);

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
                c.a = Mathf.Min(c.a, max_glow);
                glowspots[x][y].color = c;
            }
        }
    }
}
