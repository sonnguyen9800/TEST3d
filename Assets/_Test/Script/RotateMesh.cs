using Fusion;
using UnityEngine;

public class RotateMesh : NetworkBehaviour
{

    public void Rotate(Vector3 movement)
    {
        if(movement.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(-movement);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 5.0f);
        //rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, 5.0f));

    }

}
