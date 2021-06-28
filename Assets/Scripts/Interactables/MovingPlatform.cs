using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : InteractableBase {
    
    [SerializeField] private bool repeatable; // Whether the object will continue looping through the positions, repeating back to the first index when at the end.
    [SerializeField] private bool continuous; // Whether the object will autonomously travel between positions continuously, without need for input.

    [SerializeField] private float speed;

    [SerializeField] private float waitTime;

    [SerializeField] private int index = 0;

    [SerializeField] Vector3[] positions;

    private Vector3 oldPos;

    private float timer;
    
    private void OnValidate(){
        index = Mathf.Clamp(index, 0, Mathf.Max(positions.Length - 1, 0));
    }

    private void Start(){
        gameObject.layer = LayerMask.NameToLayer("MovingPlatform");
    }

    private void FixedUpdate(){
        oldPos = transform.position;

        if(timer > 0f){
            timer -= Time.deltaTime;
        }

        if(_active){
            if(index >= positions.Length){
                if(repeatable){
                    index = 0;
                } else {
                    return;
                }
            }

            Vector3 dir = (positions[index] - transform.position);

            if(dir.magnitude <= (speed * Time.deltaTime)){
                transform.position = positions[index];
                Deactivate();
            } else {
                transform.position += dir.normalized * speed * Time.deltaTime;
            }
        } else if(continuous){
            Interact();
        }
    }

    private void OnTriggerStay(Collider other){
        GameObject obj = other.gameObject;

        if(obj.TryGetComponent<CharacterController>(out CharacterController cc)){
            cc.Move(transform.position - oldPos);
            print("true");

        } else if(obj.TryGetComponent<Rigidbody>(out Rigidbody rb)){
            rb.MovePosition(rb.transform.position + (transform.position - oldPos));

        } else {
            obj.transform.position += transform.position - oldPos;
        }
    }

    //private void OnCollisionEnter(Collision collisionInfo){
    //    collisionInfo.collider.gameObject.transform.parent = transform;
    //}

    //private void OnCollisionExit(Collision collisionInfo){
    //    collisionInfo.collider.gameObject.transform.parent = null;
    //}

    public override void Interact(){
        if(!_active && timer <= 0f){
            Activate();
        }
    }

    public override void Activate(){
        _active = true;
        index++;
    }

    public override void Deactivate(){
        _active = false;
        oldPos = transform.position;
        timer = waitTime;
    }

    /* The two ContextMenu function below are helper function for more easily creating the waypoints for the platform. To use each function, set the index variable
        in the inspector to the index of the position you want to work with. On the moving platform script, click the three dots icon and at the bottom you will see
        the two ContextMenu functions. The first sets the objects current position to the position specified at the index value you selected. The second function
        does the reverse. It sets the position at the specified index to be the objects position. This is useful for putting the object back to the first position
        in the array. */

    [ContextMenu("Set Position at Index to Obj Position")]
    private void SetPosToObjPos(){
        if(positions.Length == 0){ return; }

        positions[index] = transform.position;
    }

    [ContextMenu("Set Obj Position to Position at Index")]
    private void SetObjPosToPos(){
        if(positions.Length == 0){ return; }

        transform.position = positions[index];
    }

    // This function draws the Gizmo points and lines to help visualise the path of the platform.
    private void OnDrawGizmos(){
        if(positions.Length == 0){ return; }
        
        for(int i = 0; i < positions.Length; i++){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(positions[i], 0.2f);
            
            if(i == positions.Length - 1){
                if(repeatable){
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(positions[i], positions[0]);
                }
            } else {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }

        Gizmos.color = Color.white;
    }
}
