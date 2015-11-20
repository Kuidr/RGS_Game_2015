using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAudio : MonoBehaviour
{
    public WorldSound bump_sound;
    public WorldSound grunt_sound;
    public List<WorldSound> exclamation_sounds;

    public void PlayBump(float force)
    {
        bump_sound.transform.position = transform.position;
        bump_sound.base_volume = force;
        bump_sound.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        bump_sound.Play();
    }
    public void PlayGrunt()
    {
        grunt_sound.transform.position = transform.position;
        grunt_sound.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        grunt_sound.base_volume = 2;
        grunt_sound.Play();
    }

    public void PlayExclamation(int num)
    {
        if (num >= exclamation_sounds.Count) return;
        exclamation_sounds[num].transform.position = transform.position;
        
        exclamation_sounds[num].Play();
    }

}
