using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class WorldSound : MonoBehaviour
{
    public bool timescaled_pitch = true;
    public float base_volume = 1;
    public float base_pitch = 1;
    private float pitch_offset = 0;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void Update()
    {
        // volume
        source.volume = base_volume * GameSettings.Instance.volume_fx;

        // pitch
        source.pitch = Mathf.Max(base_pitch + pitch_offset, 0);
        if (timescaled_pitch) source.pitch *= Time.timeScale;

        // disable when finished
        if (!source.isPlaying) gameObject.SetActive(false);

    }

    public void Play(float delay)
    {
        gameObject.SetActive(true);
        source.PlayDelayed(delay);
        Update();
    }
    public void Play()
    {
        gameObject.SetActive(true);
        source.Play();
        Update();
    }

    public void SetPitchOffset(float offset)
    {
        pitch_offset = offset;
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }
	
}
