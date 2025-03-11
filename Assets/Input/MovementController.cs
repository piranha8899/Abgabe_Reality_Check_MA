using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    private float verticalVelocity = 0.0f;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;

    void Awake()
    {
       playerInput = new PlayerInput();
       characterController = GetComponent<CharacterController>();

       //Controls lesen und auf Funktion unten verweisen
       playerInput.CharacterControls.Walk.started += onMovementInput;
       playerInput.CharacterControls.Walk.performed += onMovementInput;
       playerInput.CharacterControls.Walk.canceled += onMovementInput;

     }

    void onMovementInput (InputAction.CallbackContext context) //Funktion für Controls
    {
        //Debug.Log(context.ReadValue<Vector2>()); // Log Kontrolle
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x; //x-Movement auf x-Achse übertragen
        currentMovement.z = currentMovementInput.y; // y-Movement auf z-Achse übertragen
        isMovementPressed = currentMovementInput.x !=0 || currentMovementInput.y !=0; 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Schwerkraft
        if (characterController.isGrounded)
        {
            //Debug.Log("Object on ground.");
            verticalVelocity = 0; // reset bei Bodenkontakt
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        
        // Totale Bewegung auf den Vector3 der Schwerkraft
        Vector3 totalMovement = currentMovement * moveSpeed;
        totalMovement.y = verticalVelocity; //Verhindern, dass y-Achse verändert wird
        
        characterController.Move(totalMovement * Time.deltaTime);
    }

    void OnEnable()
    {
        //Input Action Map aktivieren
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //Input Action Map deaktivieren
        playerInput.CharacterControls.Disable();
    }
}
