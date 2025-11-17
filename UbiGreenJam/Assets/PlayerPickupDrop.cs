using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask pickupLayer = ~0;
    public Transform holdPoint;
    public Camera aimCamera;

    Rigidbody heldRb;

    // Anchor rigidbody used to keep a physical joint while preserving colliders
    Rigidbody holdAnchorRb;
    FixedJoint currentJoint;
    CollisionDetectionMode previousCollisionMode;

    void Start()
    {
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(transform);
            hp.transform.localPosition = new Vector3(0f, 0.5f, 1f);
            hp.transform.localRotation = Quaternion.identity;
            holdPoint = hp.transform;
        }

        // Ensure the holdPoint has a kinematic Rigidbody to connect joints to
        holdAnchorRb = holdPoint.GetComponent<Rigidbody>();
        if (holdAnchorRb == null)
        {
            holdAnchorRb = holdPoint.gameObject.AddComponent<Rigidbody>();
            holdAnchorRb.isKinematic = true;
            // Keep default collision detection for the anchor (anchor will be moved with MovePosition)
        }
    }

    void Update()
    {
        // Pick on left mouse button press
        if (Input.GetMouseButtonDown(0) && heldRb == null)
        {
            TryPickupInFront();
        }

        // If the left mouse button is released, drop
        if (Input.GetMouseButtonUp(0) && heldRb != null)
        {
            Drop();
        }
    }

    void FixedUpdate()
    {
        // Move the kinematic anchor to the hold point position using MovePosition/MoveRotation so physics can resolve collisions
        if (holdAnchorRb != null)
        {
            holdAnchorRb.MovePosition(holdPoint.position);
            holdAnchorRb.MoveRotation(holdPoint.rotation);
        }
    }

    void TryPickupInFront()
    {
        Ray ray;
        if (aimCamera != null)
        {
            ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        }
        else
        {
            ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer, QueryTriggerInteraction.Ignore))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb != null && !rb.isKinematic)
            {
                Pickup(rb);
            }
        }
    }

    void Pickup(Rigidbody rb)
    {
        if (rb == null) return;

        // If an existing joint somehow remains, remove it
        if (currentJoint != null)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }

        heldRb = rb;

        // Stop motion but keep object dynamic so colliders remain active and collisions are resolved
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;

        // Improve collision handling while moving (helps prevent tunneling)
        previousCollisionMode = heldRb.collisionDetectionMode;
        heldRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Ensure object is NOT parented to the player - physics (joint) will keep it in place
        heldRb.transform.SetParent(null, worldPositionStays: true);

        // Create a FixedJoint on the object and connect it to the kinematic anchor at the hold point.
        currentJoint = heldRb.gameObject.AddComponent<FixedJoint>();
        currentJoint.connectedBody = holdAnchorRb;
        currentJoint.breakForce = Mathf.Infinity;
        currentJoint.breakTorque = Mathf.Infinity;
    }

    void Drop()
    {
        if (heldRb == null) return;

        // Remove the joint so physics is free again
        if (currentJoint != null)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }

        // Restore collision detection mode
        heldRb.collisionDetectionMode = previousCollisionMode;

        heldRb = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin;
        Vector3 dir;
        if (aimCamera != null)
        {
            origin = aimCamera.transform.position;
            dir = aimCamera.transform.forward;
        }
        else
        {
            origin = transform.position + Vector3.up * 0.5f;
            dir = transform.forward;
        }
        Gizmos.DrawLine(origin, origin + dir * pickupRange);
        Gizmos.DrawWireSphere(origin + dir * pickupRange, 0.05f);
    }
}