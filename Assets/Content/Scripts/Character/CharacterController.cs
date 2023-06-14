using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent( typeof( CharacterCore ) )]
public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private PlayerCamera playerCamera;

    [SerializeField]
    private float resetSpeed = 33f;

    [SerializeField]
    private Image resetIndicator;

    [SerializeField]
    private Light spotLight;

    [BoxGroup( "Movement" )]
    [SerializeField]
    private float rotateSpeed = 180f;

    [BoxGroup( "Movement" )]
    [SerializeField]
    private float screenFace = 0.1f;

    private Vector3 localLightOffset;

    public PlayerControls Controls { get; private set; }

    private CharacterMove characterMove;

    private CharacterMagnet characterMagnet;

    private CharacterJump characterJump;

    private CharacterCore character;

    private bool jumpActive = false;

    private float resetAmount;

    private void Awake()
    {
        Controls = new PlayerControls();

        Controls.Enable();

        characterMove = GetComponent<CharacterMove>();

        characterJump = GetComponent<CharacterJump>();

        characterMagnet = GetComponent<CharacterMagnet>();

        character = GetComponent<CharacterCore>();

        localLightOffset = spotLight.transform.position - transform.position;

        spotLight.transform.SetParent( null );
    }

    private void Update()
    {
        Vector2 moveInput = Controls.Player.Move.ReadValue<Vector2>();

        characterMove.SetMoveDirection( moveInput );

        if ( Level.Instance != null && Level.Instance.ActiveSegment != null )
            Level.Instance.ActiveSegment.OffsetScreenBias( moveInput.y );

        if ( Controls.Player.Reset.ReadValue<float>() > 0.5f )
        {
            if ( resetAmount < 200 )
            {
                resetAmount += resetSpeed * Time.deltaTime;

                resetIndicator.fillAmount = resetAmount / 100;

                if ( resetAmount >= 100 )
                {
                    resetIndicator.fillAmount = 0;

                    resetAmount = 201;

                    GameManager.Instance.Respawn();
                }
            }
        }
        else
        {
            resetIndicator.fillAmount = 0;

            resetAmount = 0;
        }

        spotLight.transform.position = transform.position + localLightOffset;
    }

    private void FixedUpdate()
    {
        characterMove.SimulateBehaviour();
        characterJump.SimulateBehaviour();

        if ( characterMagnet.IsAiming )
            transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.LookRotation( Vector3.right * Mathf.Sign( characterMagnet.AimDirection ) + Vector3.back * screenFace * Mathf.Sign( characterMagnet.AimDirection ), Vector3.up ), rotateSpeed * Time.fixedDeltaTime );
        else if ( Mathf.Abs( character.Rigidbody.velocity.x ) > 1f )
            transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.LookRotation( Vector3.right * Mathf.Sign( character.Rigidbody.velocity.x ) + Vector3.back * screenFace, Vector3.up ), rotateSpeed * Time.fixedDeltaTime );

        character.Animator.SetBool( "Aim", ( ( characterMagnet.IsAiming && characterMagnet.AimValid ) || characterMagnet.GravActive ) );

        if ( character.Grounded && jumpActive && character.Rigidbody.velocity.y < 0 )
        {
            EndJump();
        }
    }

    private void OnJumpDown( InputAction.CallbackContext context )
    {
        characterJump.SetJump();

        jumpActive = true;
    }

    private void OnJumpRelease( InputAction.CallbackContext context )
    {
        EndJump();
    }


    private void OnEnable()
    {
        Controls.Player.Jump.performed += OnJumpDown;

        Controls.Player.Jump.canceled += OnJumpRelease;
    }

    private void OnDisable()
    {
        Controls.Player.Jump.performed -= OnJumpDown;

        Controls.Player.Jump.canceled -= OnJumpRelease;
    }

    private void EndJump()
    {
        characterJump.JumpRelease();

        jumpActive = false;
    }

    public void Respawned()
    {
        character.ZeroCharacter();
    }
}
