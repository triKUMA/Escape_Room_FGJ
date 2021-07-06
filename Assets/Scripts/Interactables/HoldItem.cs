using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldItem : InteractableBase {

    [SerializeField] private float distanceFromPlayer = 5f;

    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float rotateSpeed = 1f;

    private Rigidbody rb;

    // Makes sure that _interactable is never turned off in the inspector. If you do not want a holdable item, do not attach this script.
    private void OnValidate() {
        _interactable = true;
    }

    private void Start() {
        // Set specific Rigidbody settings explicitly.
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate() {
        if(_active) {
            // Get hold items desired position, which is the point where the player would hold the item.
            Vector3 desiredPos = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromPlayer);

            // Set the object's velocity as a direction towards that point, so that object moves towards point smoothly, instead of instantaneously.
            rb.velocity = (desiredPos - transform.position) * moveSpeed;
        }
    }

    public void Rotate() {
        rb.MoveRotation(Quaternion.Euler(transform.eulerAngles + (Vector3.up * rotateSpeed * Time.deltaTime)));
    }

    public override void Interact() {
        return;
    }

    // Disables gravity and rotation, enables object to move towards hold point.
    public override void Activate() {
        _active = true;
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    // Deactivates holding, and reactivates gravity and rotation.
    public override void Deactivate() {
        _active = false;
        rb.useGravity = true;
        rb.freezeRotation = false;
    }
}
