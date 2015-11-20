using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public PauseMenuPage pause_menu;
    public MatchManager match_manager;
    public bool Paused { get; private set; }

    public void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetButtonDown("Start1") ||
            Input.GetButtonDown("Start2"))
            && match_manager.GetMatchState() == MatchState.InMatch)
        {
            if (Paused && pause_menu.IsTopPage() && !pause_menu.Hidden()) pause_menu.ButtonResume();
            else
            {
                Pause();
                pause_menu.TransitionIn();
            }
        }
    }
    public void TogglePause()
    {
        if (Paused) UnPause();
        else Pause();
    }
    public void Pause()
    {
        TimeScaleManager.Instance.AddMultiplier("pause", 0, true);
        Paused = true;
    }
    public void UnPause()
    {
        TimeScaleManager.Instance.RemoveMultiplier("pause");
        Paused = false;
    }

}
