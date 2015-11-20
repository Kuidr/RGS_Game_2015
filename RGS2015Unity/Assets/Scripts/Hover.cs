using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{
    public float intensity = 0.1f;
    private float speed = 1f;

    private Vector2 anchor_pos;
    private Vector2 offset = new Vector2();
    private float tx = 0, ty = 0;
    private float fadein_mult = 0;


    public void StartFadeIn()
    {
        this.enabled = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }
    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private void Start()
    {
        anchor_pos = transform.position;
        tx = Random.value * Mathf.PI * 2f;
        ty = Random.value * Mathf.PI * 2f;

        StartFadeIn();
    }
    private void Update()
    {
        tx += Time.deltaTime * speed * 0.5f;
        ty += Time.deltaTime * speed;

        offset.x = Mathf.Sin(tx) * intensity * 0.3f;
        offset.y = Mathf.Sin(ty) * intensity;

        transform.position = anchor_pos + offset * fadein_mult;
    }
    private IEnumerator FadeIn()
    {
        fadein_mult = 0;
        while (fadein_mult < 1)
        {
            fadein_mult += Time.deltaTime;
            yield return null;
        }
        fadein_mult = 1;
    }
    private IEnumerator FadeOut()
    {
        while (fadein_mult > 1)
        {
            fadein_mult -= Time.deltaTime;
            yield return null;
        }
        fadein_mult = 0;
        this.enabled = false;
    }
}
