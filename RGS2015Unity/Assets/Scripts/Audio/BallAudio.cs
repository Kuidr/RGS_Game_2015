using UnityEngine;
using System.Collections;

public class BallAudio : MonoBehaviour
{
    public WorldSound bump_sound_prefab;
    public int bump_sound_pool_buffer = 2;

    public void Awake()
    {
        if (bump_sound_prefab != null) ObjectPool.Instance.RequestObjects(bump_sound_prefab, bump_sound_pool_buffer, true);
    }

    public void PlayBump(float force)
    {
        if (bump_sound_prefab == null) return;
        WorldSound s = ObjectPool.Instance.GetObject(bump_sound_prefab, false);

        s.transform.position = transform.position;
        s.base_volume = force;
        s.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        s.Play();
    }
	
}
