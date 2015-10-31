using UnityEngine;
using System.Collections;

public class SettingsPage : UIMenuPage
{
    public MainMenuPage page_mainmenu;


    protected override void OnStartTransitionIn()
    {
        base.OnStartTransitionIn();
    }

    public void OnButtonBack()
    {
        TransitionOut();
        page_mainmenu.TransitionIn();
    }
}
