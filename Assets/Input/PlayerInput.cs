using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5f;
    private Vector2 moveDirection;
    public InputActionReference move;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        //Debug:
        //Debug.Log("Move Direction: " + moveDirection);
    }


private void FixedUpdate()
{
    Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;
    rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
}

   
}
