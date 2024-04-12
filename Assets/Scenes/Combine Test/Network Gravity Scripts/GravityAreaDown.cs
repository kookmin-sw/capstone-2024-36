using UnityEngine;

public class GravityAreaDown : GravityArea
{
    public override Vector3 GetGravityDirection(GravityBody _gravityBody)
    {
        return transform.up;
    }
}