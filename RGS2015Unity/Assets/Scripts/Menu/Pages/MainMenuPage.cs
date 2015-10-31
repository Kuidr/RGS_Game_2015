using UnityEngine;
using System.Collections;

public class MainMenuPage : UIMenuPage 
{
    public MatchSetupPage page_match_setup;
    public SettingsPage page_settings;


    new public void Start()
    {
        base.Start();
    }

    protected override void OnStartTransitionIn()
    {
        base.OnStartTransitionIn();
    }

    public void OnButtonMatch()
    {
        TransitionOut();
        page_match_setup.TransitionIn();
    }
    public void OnButtonSettings()
    {
        TransitionOut();
        page_settings.TransitionIn();
    }
}
