using UnityEngine;
using System.Collections;

public class MageAudio : MonoBehaviour
{
    public WorldSound bump_sound;

    public void PlayBump(float force)
    {
        bump_sound.transform.position = transform.position;
        bump_sound.base_volume = force;
        bump_sound.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        bump_sound.Play();
    }
}
