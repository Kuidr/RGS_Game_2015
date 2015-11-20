using UnityEngine;
using System.Collections;

public class StartPopupPage : UIMenuPage
{
    //public UIMenuPage fade_page;
    public UIMenuPage loading_page;

	public void StartMatch()
    {
        this.TransitionOut();
        loading_page.on_transitioned_in = new System.Action(() => Application.LoadLevel("Game"));
        loading_page.TransitionIn();
        //fade_page.TransitionIn();
    }
}
