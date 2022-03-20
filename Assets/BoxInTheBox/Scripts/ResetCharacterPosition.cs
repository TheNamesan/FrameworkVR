using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCharacterPosition : MonoBehaviour
{
    public Vector3 resetPosition;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Touch" + hit.gameObject.name);
        if(hit.gameObject.tag == "PlayerCheck")  
        {
            transform.position = resetPosition;
        }
    }
}
