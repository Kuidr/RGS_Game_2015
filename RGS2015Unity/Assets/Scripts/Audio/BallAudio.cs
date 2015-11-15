using UnityEngine;
using System.Collections;

public class BallAudio : MonoBehaviour
{
    public WorldSound break_sound;
    public WorldSound bump_sound;


    public void PlayBump(float force)
    {
        bump_sound.transform.position = transform.position;
        bump_sound.base_volume = force;
        bump_sound.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        bump_sound.Play();
    }
    public void PlayBreak()
    {
        break_sound.transform.position = transform.position;
        break_sound.base_volume = 1;
        break_sound.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        break_sound.Play();
    }
	
}
