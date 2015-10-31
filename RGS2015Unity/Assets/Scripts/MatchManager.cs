using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MatchState { PreMatch, InMatch, PostMatch }

public class MatchManager : MonoBehaviour
{
    private MatchState state = MatchState.PreMatch;

    // General
    public Mage[] players;
    private List<Ball> balls = new List<Ball>();
    public Ball ball_prefab;

    // Timers
    private float time_newball = 15f; // seconds



    // PUBLIC MODIFIERS

    public void RegisterBall(Ball ball)
    {
        balls.Add(ball);
        ball.event_ball_hit_mage += OnBallHitMage;
    }


    // PUBLIC ACCESSORS

    public int GetWinnerPlayerNum()
    {
        return 1;
    }


    // PRIVATE MODIFIERS

    private void Start()
    {
        // DEBUG from game scene
        //-------------------------
        //GameSettings.Instance.SetPlayerControl(1, false, 1);
        //GameSettings.Instance.SetPlayerControl(2, false, 2);

        //-------------------------

        StartMatch();
    }
    private void StartMatch()
    {
        state = MatchState.InMatch;
    }
    private void OnBallHitMage(Ball ball)
    {
        balls.Remove(ball);
        if (balls.Count == 0) StartCoroutine(CreateNewBall());
    }

    private IEnumerator CreateNewBall()
    {
        yield return new WaitForSeconds(time_newball);
        Instantiate(ball_prefab);
    }
}
