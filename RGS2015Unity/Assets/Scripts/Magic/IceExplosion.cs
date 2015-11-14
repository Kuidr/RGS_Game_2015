using UnityEngine;
using System.Collections;

public class IceExplosion : MonoBehaviour
{
    public ParticleSystem ps;
    private CameraShake camshake;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        //gameObject.SetActive(false);
        camshake = Camera.main.GetComponent<CameraShake>();
    }

    public void Explode()
    {
        gameObject.SetActive(true);
        camshake.Shake(new CamShakeInstance(0.03f, 0.1f));
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
