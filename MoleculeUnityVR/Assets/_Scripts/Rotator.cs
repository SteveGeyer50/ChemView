using UnityEngine;
using System.Collections;
 
public class Rotator : MonoBehaviour {

    // TODO: when the molecule is upside down, the movement is not intuitive
    //       so figure out how to fix this

    public float speed = 100f;

    bool rotating = false;

    // rotating the molecule
    // TODO: add VR
    // in VR, this should somehow work with the VR sticks/controllers
    // how much of this needs to change for that to work??
    void Update(){

        // allows person to click anywhere and drag the object with
        // LEFT click
        if (Input.GetMouseButtonDown(0))
            rotating = true;

        // turns off rotation when left click is released (for drag)
        if (Input.GetMouseButtonUp(0))
            rotating = false;

        // can toggle rotation using RIGHT click
        if (Input.GetMouseButtonDown(1))
            rotating = !rotating;

        if(rotating){

            float rotX = Input.GetAxis("Mouse X") * speed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * speed * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up,   -rotX); // inverting x
            transform.Rotate(Vector3.right, rotY);

        }

    }
     
}