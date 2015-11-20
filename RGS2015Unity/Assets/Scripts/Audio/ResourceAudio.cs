using UnityEngine;
using System.Collections;

public class ResourceAudio : MonoBehaviour
{
    public WorldSound break_sound_prefab;

    public void PlayBreak()
    {
        if (break_sound_prefab == null) return;
        WorldSound s = ObjectPool.Instance.GetObject(break_sound_prefab, false);

        s.transform.position = transform.position;
        s.base_volume = 1;
        s.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        s.Play(Random.value * 0.1f);
    }
}
