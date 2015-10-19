using UnityEngine;
using System.Collections;

public class SpellShield : MonoBehaviour
{
    private Circle circle;
    private float radius = 0.5f;
    private const float Force = 5f;

    private void Start()
    {
        circle = GetComponent<Circle>();
        circle.Set(transform.position, radius, Color.clear, 0.1f);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (rb.position - (Vector2)transform.position).normalized;
            rb.AddForce(dir * Force, ForceMode2D.Impulse);

            StopAllCoroutines();
            StartCoroutine(ActivateShieldVisual());
        }
    }

    private IEnumerator ActivateShieldVisual()
    {
        Color c = Color.white;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            c = Color.Lerp(Color.white, Color.clear, t);
            circle.Set(transform.position, radius + radius*0.2f*t, c, 0.1f);

            yield return null;
        }
    }

}
