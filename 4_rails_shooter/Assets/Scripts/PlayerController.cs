using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    [Header("Player Stats")]
    [SerializeField] float lateralMoveSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float debugSprintMultiplier = 20f;

    [SerializeField] private float controllerRotationSpeed = 200f;
    [SerializeField] private float mouseRotationSpeed = 5f;


    bool isMoving = false;
    Vector2 moveVectorForThisTick = Vector2.zero;

    bool isRotating = false;
    float rotationThisTick = 0f;

    void Start()
    {
        characterController = transform.GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if(isMoving)
        {
            MoveCharacter(moveVectorForThisTick);
        }

        if(isRotating)
        {
            RotateCharacter(rotationThisTick);
        }

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            isMoving = true;
        }
        else if (ctx.canceled)
        {
            isMoving = false;
            return;
        }

        print("OnMove() : " + ctx.action.name);

        moveVectorForThisTick = ctx.ReadValue<Vector2>();
    }


    public void OnLook(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            isRotating = true;
        }
        else if (ctx.canceled)
        {
            isRotating = false;
            return;
        }

        var lookVector = ctx.ReadValue<Vector2>();
        rotationThisTick = lookVector.x;


        if(ctx.action.GetBindingDisplayString() == "Delta")
        {
            rotationSpeed = mouseRotationSpeed;
        }
        else
        {
            rotationSpeed = controllerRotationSpeed;
        }

    }


    private void MoveCharacter(Vector2 vec)
    {
        Vector3 moveVector = transform.right * vec.x;

        // moveVector += transform.up * vec.y       // y as up-down
        // moveVector += transform.forward * vec.y  // y as depth
        

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            moveVector *= debugSprintMultiplier;
        }


        var currentPos = transform.position;
        currentPos += Time.deltaTime * moveVector * lateralMoveSpeed;
        transform.position = currentPos;

        //characterController.Move(Time.deltaTime * moveVector * lateralMoveSpeed);
    }


    private void RotateCharacter(float degrees)
    {
        transform.Rotate(Vector3.up * degrees * rotationSpeed * Time.deltaTime);
    }
}
