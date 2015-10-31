using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GGPage : InGameMenuPage
{
    private MatchManager match_manager;
    public Text heading;


    public void Awake()
    {
        match_manager = Object.FindObjectOfType<MatchManager>();
        if (match_manager == null) Debug.LogError("GameManager missing.");
    }
    public void OnEnable()
    {
        //UIAudio.Instance.PlayPause();

        
        int result = match_manager.GetWinnerPlayerNum();
        if (result == 1)
        {
            heading.text = GameSettings.Instance.player_name[0] + " Wins";
        }
        else if (result == 2)
        {
            heading.text = GameSettings.Instance.player_name[1] + " Wins";
        }
        else
            heading.text = "Draw";
        
    }
}
