using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    private const float AutoCastTime = 0.5f;
    private float auto_cast_timer = 0;

    public Vector2 InputMove { get; protected set; }
    public Action InputSpellCodeA { get; set; }
    public Action InputSpellCodeB { get; set; }
    public Action InputSpellCodeX { get; set; }
    public Action InputSpellCodeY { get; set; }
    public Action InputCast { get; set; }

    protected void StartAutoCast()
    {
        auto_cast_timer = AutoCastTime;
    }

    protected void Start()
    {
        InputMove = Vector2.zero;
    }
    protected void Update()
    {
        // Auto Cast
        if (auto_cast_timer > 0)
        {
            auto_cast_timer -= Time.deltaTime;
            if (auto_cast_timer <= 0 && InputCast != null)
            {
                // cast
                InputCast();
            }
        }
    }
}
