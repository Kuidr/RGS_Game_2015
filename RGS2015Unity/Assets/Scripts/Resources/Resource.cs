using UnityEngine;
using System.Collections;


public enum ResourceType { Rock, Crystal };

public class Resource : MonoBehaviour
{
    // General
    public ResourceType type;
    public const float WidthHeight = 0.2828f;
    private ResourceAudio resource_audio;

    // Visual
    private BackgroundLighting back_lighting;
    

    public void Break()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Shrink());
        resource_audio.PlayBreak();
    }

    private void Awake()
    {
        back_lighting = FindObjectOfType<BackgroundLighting>();
        resource_audio = GetComponent<ResourceAudio>();
    }
    private void Update()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        if (c.r + c.g + c.b > 0.5f)
            back_lighting.Light(transform.position, c * 0.1f * (transform.lossyScale.x / 0.2f));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Break();
    }

    private IEnumerator Shrink()
    {
        Vector3 scale_original = transform.localScale;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(scale_original, Vector3.zero, t*t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
