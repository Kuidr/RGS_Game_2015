using UnityEngine;
using System.Collections;

public class IceExplosion : MonoBehaviour
{
    public ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        //gameObject.SetActive(false);
    }

    public void Explode()
    {
        gameObject.SetActive(true);
        ps.Clear();
        ps.time = 0;
        ps.Play();
    }
    public void Update()
    {
        if (!ps.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    public bool IsExploding()
    {
        return ps.isPlaying;
    }
}
