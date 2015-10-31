using UnityEngine;
using System.Collections;

public class UITransitionCamMove : UITransition
{
    public Transform cam_target;
    private Vector3 cam_start;
    private bool start_set = false;

    public override void UpdateTransition(float transition, bool going_in)
    {
        if (!going_in) return;
        if (!start_set) cam_start = Camera.main.transform.position;

        Vector3 pos = Vector2.Lerp(cam_start, cam_target.position, transition * transition);
        pos.z = cam_start.z;
        Camera.main.transform.position = pos;

        base.UpdateTransition(transition, going_in);
    }
    public override void OnFinishTransitionIn()
    {
        start_set = false;
        base.OnFinishTransitionIn();
    }
}
