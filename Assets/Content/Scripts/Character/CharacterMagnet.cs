using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent( typeof( CharacterController ) )]
public class CharacterMagnet : MonoBehaviour
{
    [SerializeField]
    private List<MagnetPair> magnetPairs = new List<MagnetPair>();

    private MagnetPair CurrentPair => magnetPairs[magnetPairIndex];

    [SerializeField]
    private Transform magnetSpawn;

    [SerializeField]
    private Transform magnetArmOrigin;

    [SerializeField]
    private float minimumAimDistance = 2f;

    [SerializeField]
    private LineRenderer aimLine;

    [SerializeField]
    private float aimLineWidth = 0.05f;

    [SerializeField]
    private float aimLineWidthDamping = 2f;

    [SerializeField]
    private Transform launcherHelper;

    [SerializeField]
    private Transform neckHelper;

    private Vector3 neckRotation = Vector3.zero;

    [SerializeField]
    private Vector3 launcherRotOffset = new Vector3( 0, 0, 90 );

    private Quaternion launcherAimRotation = Quaternion.identity;

    [SerializeField]
    private ParticleSystem throwIndicator;

    [SerializeField]
    private ParticleSystem fireEffect;

    [SerializeField]
    private float aimSmoothing = 2f;

    private Vector3 currentMouseWorld;

    [SerializeField]
    private float aimRange = 20f;

    [SerializeField]
    private LayerMask aimLayerMask;

    [SerializeField]
    private float gravGunStrength = 5f;

    [SerializeField]
    private float gravEffectiveRange = 3f;

    [SerializeField]
    private LineRenderer gravBeam;

    [SerializeField]
    private float gravBeamWidth = 0.05f;

    [SerializeField]
    private float gravBeamDamping = 2f;

    private float currentGravWidth = 0f;

    private int magnetPairIndex = 0;

    private CharacterController controller;

    private CharacterCore character;

    private bool active = false;

    public bool IsAiming { get; private set; } = false;

    public bool AimValid { get; private set; } = false;

    public int AimDirection { get; private set; } = 1;

    private bool targetValid = false;

    private Camera mainCamera;

    private float currentLineWidth = 0f;

    private PhysicsObject gravObject;

    private GrappleNode currentGrappleNode = null;

    private Collider gravCollider;

    public bool GravActive { get; private set; } = false;

    private Vector3 mouseToWorld = Vector3.zero;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        character = GetComponent<CharacterCore>();

        controller.Controls.Player.PolarSwitch.performed += OnPolarSwitch;

        controller.Controls.Player.PolarFreeze.performed += OnPolarFreeze;

        controller.Controls.Player.Boost.performed += OnMagnetUpdate;

        controller.Controls.Player.Aim.performed += OnAim;

        controller.Controls.Player.Aim.canceled += OnAim;

        controller.Controls.Player.Fire.performed += OnFire;

        controller.Controls.Player.Recall.performed += OnRecall;

        controller.Controls.Player.PairSelect.performed += OnPairSelect;

        controller.Controls.Player.AltFire.performed += OnGravMode;

        controller.Controls.Player.AltFire.canceled += OnGravMode;

        foreach ( MagnetPair pair in magnetPairs )
        {
            pair.FirstMagnet.gameObject.SetActive( false );
            pair.SecondMagnet.gameObject.SetActive( false );
        }

        UIColorUpdate( CurrentPair.FirstMagnet.GetCurrentColor( CurrentPair.PolarState, CurrentPair.IsFrozen ) );

        mainCamera = Camera.main;
    }

    private void OnDisable()
    {
        controller.Controls.Player.PolarSwitch.performed -= OnPolarSwitch;

        controller.Controls.Player.PolarFreeze.performed -= OnPolarFreeze;

        controller.Controls.Player.Boost.performed -= OnMagnetUpdate;

        controller.Controls.Player.Aim.performed -= OnAim;

        controller.Controls.Player.Aim.canceled -= OnAim;

        controller.Controls.Player.Fire.performed -= OnFire;

        controller.Controls.Player.Recall.performed -= OnRecall;

        controller.Controls.Player.PairSelect.performed -= OnPairSelect;

        controller.Controls.Player.AltFire.performed -= OnGravMode;

        controller.Controls.Player.AltFire.canceled -= OnGravMode;
    }

    private void OnPairSelect( InputAction.CallbackContext context )
    {
        SwitchActivePair( Mathf.RoundToInt( context.ReadValue<float>() ) );
    }

    private void OnAim( InputAction.CallbackContext context )
    {
        if ( context.performed )
        {
            EnableAim();
        }
        else
        {
            CancelAim();

            CancelGravMode();
        }
    }

    private void OnGravMode( InputAction.CallbackContext context )
    {
        if ( context.performed )
        {
            TryEnterGravMode();
        }
        else
        {
            CancelGravMode();
        }
    }


    private void OnFire( InputAction.CallbackContext context )
    {
        if ( !IsAiming || !targetValid )
            return;

        if ( CurrentPair.ActiveMagnets < 2 )
        {
            FireMagnet();
        }
        else
        {
            for ( int i = 0; i < magnetPairs.Count; i++ )
            {
                if ( magnetPairs[i].ActiveMagnets < 2 )
                {
                    SwitchActivePair( i );

                    FireMagnet();

                    break;
                }
            }
        }
    }


    private void OnRecall( InputAction.CallbackContext context )
    {
        RecallCurrentPair();
    }


    private void OnPolarSwitch( InputAction.CallbackContext context )
    {
        if ( CurrentPair.IsFrozen )
            return;

        switch ( CurrentPair.PolarState )
        {
            case PolarState.Pull:
                CurrentPair.PolarState = PolarState.Push;
                UIColorUpdate( CurrentPair.FirstMagnet.GetCurrentColor( CurrentPair.PolarState, CurrentPair.IsFrozen ) );
                CurrentPair.FirstMagnet.TogglePolarState( CurrentPair.PolarState, CurrentPair.IsActive );
                CurrentPair.SecondMagnet.TogglePolarState( CurrentPair.PolarState, CurrentPair.IsActive );
                break;
            case PolarState.Push:
                CurrentPair.PolarState = PolarState.Pull;
                UIColorUpdate( CurrentPair.FirstMagnet.GetCurrentColor( CurrentPair.PolarState, CurrentPair.IsFrozen ) );
                CurrentPair.FirstMagnet.TogglePolarState( CurrentPair.PolarState, CurrentPair.IsActive );
                CurrentPair.SecondMagnet.TogglePolarState( CurrentPair.PolarState, CurrentPair.IsActive );
                break;
        }
    }

    private void OnPolarFreeze( InputAction.CallbackContext context ) //End magnet activity and freeze pairing
    {
        ToggleFreeze();
    }

    private void OnMagnetUpdate( InputAction.CallbackContext context ) //Whether magnets are active or not
    {
        SetPairActive( !CurrentPair.IsActive );
    }

    //-------------INPUT END--------------

    private void EnableAim()
    {
        aimLine.enabled = true;

        IsAiming = true;

        CalculateMouseToWorld();

        currentMouseWorld = mouseToWorld;
    }

    private void CancelAim()
    {
        aimLine.enabled = false;

        currentLineWidth = 0f;

        throwIndicator.Stop();

        IsAiming = false;

        gravObject = null;

        gravCollider = null;
    }

    private void TryEnterGravMode()
    {
        if ( IsAiming )
        {
            Vector3 spawnToMouse = ( mouseToWorld - magnetArmOrigin.position );

            if ( Physics.Raycast( magnetArmOrigin.position, spawnToMouse.normalized, out RaycastHit hit, aimRange, aimLayerMask ) )
            {
                if ( hit.collider != gravCollider )
                {
                    gravCollider = hit.collider;

                    if ( gravCollider.attachedRigidbody != null )
                    {
                        gravObject = gravCollider.attachedRigidbody.GetComponent<PhysicsObject>();
                    }
                    else
                    {
                        gravObject = null;
                    }
                }
            }

            if ( gravObject != null )
            {
                GravActive = true;

                gravBeam.enabled = true;

                aimLine.enabled = false;

                currentLineWidth = 0f;

                throwIndicator.Stop();

                currentGrappleNode = gravObject.gameObject.GetComponent<GrappleNode>();

                if ( currentGrappleNode != null )
                {
                    currentGrappleNode.SetConnectedBody( character.Rigidbody );
                }
            }
        }
    }

    private void CancelGravMode()
    {
        if ( GravActive )
        {
            GravActive = false;

            if ( IsAiming )
                aimLine.enabled = true;

            gravBeam.enabled = false;

            if ( currentGrappleNode != null )
            {
                currentGrappleNode.SetConnectedBody( null );

                currentGrappleNode = null;
            }
        }
    }

    private bool CheckAimValid()
    {
        if ( !AimValid )
        {
            if ( aimLine.enabled )
            {
                aimLine.enabled = false;

                targetValid = false;

                throwIndicator.Stop();
            }

            return false;
        }
        else if ( AimValid && !aimLine.enabled )
        {
            EnableAim();

            throwIndicator.Play();
        }

        return true;
    }

    private void SwitchActivePair( int newPairIndex )
    {
        if ( CurrentPair.FirstMagnet.gameObject.activeInHierarchy )
            CurrentPair.FirstMagnet.TogglePair( CurrentPair.PolarState, false, CurrentPair.IsFrozen );
        if ( CurrentPair.SecondMagnet.gameObject.activeInHierarchy )
            CurrentPair.SecondMagnet.TogglePair( CurrentPair.PolarState, false, CurrentPair.IsFrozen );

        UIColorUpdate( Color.white );

        magnetPairIndex = newPairIndex;

        if ( CurrentPair.FirstMagnet.gameObject.activeInHierarchy )
            CurrentPair.FirstMagnet.TogglePair( CurrentPair.PolarState, true, CurrentPair.IsFrozen );
        if ( CurrentPair.SecondMagnet.gameObject.activeInHierarchy )
            CurrentPair.SecondMagnet.TogglePair( CurrentPair.PolarState, true, CurrentPair.IsFrozen );

        UIColorUpdate( CurrentPair.FirstMagnet.GetCurrentColor( CurrentPair.PolarState, CurrentPair.IsFrozen ) );
    }

    private void FireMagnet()
    {
        character.Animator.SetTrigger( "Fire" );

        Magnet nextMagnet = CurrentPair.GetNextMagnet( true );

        nextMagnet.gameObject.SetActive( true );

        nextMagnet.TogglePolarState( CurrentPair.PolarState, CurrentPair.IsActive );

        nextMagnet.transform.position = magnetSpawn.position;

        nextMagnet.transform.SetParent( null );

        nextMagnet.FireMagnet( ( throwIndicator.transform.position - magnetSpawn.position ).normalized, CurrentPair.ActiveMagnets == 2 );

        if ( fireEffect != null )
        {
            fireEffect.Play();
        }
    }

    private void RecallCurrentPair()
    {
        if ( CurrentPair.IsFrozen )
            ToggleFreeze();

        SetPairActive( false );

        CurrentPair.FirstMagnet.UIIndicatorColor( Color.white );
        CurrentPair.SecondMagnet.UIIndicatorColor( Color.white );

        CurrentPair.FirstMagnet.transform.SetParent( null );
        CurrentPair.SecondMagnet.transform.SetParent( null );

        CurrentPair.FirstMagnet.gameObject.SetActive( false );
        CurrentPair.SecondMagnet.gameObject.SetActive( false );

        CurrentPair.ActiveMagnets = 0;
    }

    private void UIColorUpdate( Color toColor )
    {
        CurrentPair.uiPairIndicator.DOColor( toColor * 0.5f, 0.5f );
    }

    private void ToggleFreeze()
    {
        if ( !CurrentPair.FirstMagnet.gameObject.activeInHierarchy || !CurrentPair.FirstMagnet.gameObject.activeInHierarchy )
            return;

        CurrentPair.IsFrozen = !CurrentPair.IsFrozen;

        UIColorUpdate( CurrentPair.FirstMagnet.GetCurrentColor( CurrentPair.PolarState, CurrentPair.IsFrozen ) );

        if ( CurrentPair.IsFrozen )
        {
            CurrentPair.Tether = CurrentPair.FirstMagnet.CurrentReferenceBody.gameObject.AddComponent<FixedJoint>();
            CurrentPair.Tether.connectedBody = CurrentPair.SecondMagnet.CurrentReferenceBody;

            CurrentPair.FirstMagnet.FreezeMagnet( true, PolarState.Pull );
            CurrentPair.SecondMagnet.FreezeMagnet( true, PolarState.Pull );

            SetPairActive( false );
        }
        else
        {
            if ( CurrentPair.Tether != null )
                Destroy( CurrentPair.Tether );

            CurrentPair.FirstMagnet.FreezeMagnet( false, CurrentPair.PolarState );
            CurrentPair.SecondMagnet.FreezeMagnet( false, CurrentPair.PolarState );
        }
    }

    private void SetPairActive( bool active )
    {
        if ( !CurrentPair.FirstMagnet.gameObject.activeInHierarchy )
            return;

        CurrentPair.IsActive = active;

        if ( !CurrentPair.IsFrozen )
        {
            CurrentPair.FirstMagnet.ToggleEffect( CurrentPair.PolarState, CurrentPair.IsActive );
            CurrentPair.SecondMagnet.ToggleEffect( CurrentPair.PolarState, CurrentPair.IsActive );
        }
    }

    private void CalculateMouseToWorld()
    {
        mouseToWorld = (Vector3)controller.Controls.Player.MousePosition.ReadValue<Vector2>();

        mouseToWorld.z = -mainCamera.transform.position.z;

        mouseToWorld = mainCamera.ScreenToWorldPoint( mouseToWorld );

        mouseToWorld.z = transform.position.z;
    }



    private void Update()
    {
        if ( GravActive )
        {
            CalculateMouseToWorld();

            currentMouseWorld = Vector3.Lerp( currentMouseWorld, mouseToWorld, aimSmoothing * Time.deltaTime );

            AimDirection = ( ( gravObject.transform.position - transform.position ).x > 0 ) ? 1 : -1;

            currentGravWidth = Mathf.Lerp( gravBeamWidth / 4, gravBeamWidth, 0.5f * Mathf.Sin( Mathf.Deg2Rad * Time.timeSinceLevelLoad * 270f ) + 0.5f );

            gravBeam.startWidth = currentGravWidth;

            gravBeam.endWidth = currentGravWidth;

            gravBeam.SetPosition( 0, magnetSpawn.position );
            gravBeam.SetPosition( 1, gravObject.GetCOM() );

            launcherAimRotation = Quaternion.LookRotation( ( gravObject.GetCOM() - launcherHelper.position ).normalized, Vector3.up ) * Quaternion.Euler( launcherRotOffset );
        }
        else if ( IsAiming )
        {
            CalculateMouseToWorld();

            currentMouseWorld = Vector3.Lerp( currentMouseWorld, mouseToWorld, aimSmoothing * Time.deltaTime );

            AimValid = ( currentMouseWorld - transform.position ).sqrMagnitude >= minimumAimDistance * minimumAimDistance;

            if ( !CheckAimValid() )
                return;

            Vector3 spawnToMouse = ( currentMouseWorld - magnetArmOrigin.position );

            AimDirection = ( ( currentMouseWorld - transform.position ).x > 0 ) ? 1 : -1;

            currentLineWidth = Mathf.Lerp( currentLineWidth, aimLineWidth, aimLineWidthDamping * Time.deltaTime );

            aimLine.startWidth = currentLineWidth;

            aimLine.endWidth = currentLineWidth;

            if ( Physics.Raycast( magnetArmOrigin.position, spawnToMouse.normalized, out RaycastHit hit, aimRange, aimLayerMask ) )
            {
                //if ( hit.collider != gravCollider )
                //{
                //    gravCollider = hit.collider;

                //    if ( gravCollider.attachedRigidbody != null )
                //    {
                //        gravObject = gravCollider.attachedRigidbody.GetComponent<PhysicsObject>();
                //    }
                //    else
                //    {
                //        gravObject = null;
                //    }
                //}

                AimValid = ( hit.point - magnetArmOrigin.position ).sqrMagnitude >= minimumAimDistance * minimumAimDistance;

                if ( !CheckAimValid() )
                    return;

                if ( !throwIndicator.isEmitting )
                    throwIndicator.Play();

                throwIndicator.transform.position = hit.point;

                aimLine.SetPosition( 0, magnetSpawn.position );
                aimLine.SetPosition( 1, hit.point );

                launcherAimRotation = Quaternion.LookRotation( ( currentMouseWorld - launcherHelper.position ).normalized, Vector3.up ) * Quaternion.Euler( launcherRotOffset );

                targetValid = true;
            }
            else
            {
                if ( throwIndicator.isEmitting )
                    throwIndicator.Stop();

                aimLine.SetPosition( 0, magnetSpawn.position );
                aimLine.SetPosition( 1, magnetSpawn.position + spawnToMouse.normalized * 20f );

                launcherAimRotation = Quaternion.LookRotation( ( currentMouseWorld - launcherHelper.position ).normalized, Vector3.up ) * Quaternion.Euler( launcherRotOffset );

                targetValid = false;
            }
        }
    }

    private void LateUpdate()
    {
        if ( ( IsAiming && AimValid ) || GravActive )
        {
            launcherHelper.rotation = launcherAimRotation;

            neckRotation = neckHelper.localEulerAngles;

            if ( GravActive )
            {
                neckRotation.z = Mathf.Clamp( -AimDirection * Vector3.SignedAngle( AimDirection * Vector3.right, gravObject.transform.position - transform.position, Vector3.forward ), -40f, 25f );
            }
            else
            {
                neckRotation.z = Mathf.Clamp( -AimDirection * Vector3.SignedAngle( AimDirection * Vector3.right, currentMouseWorld - transform.position, Vector3.forward ), -40f, 25f );
            }

            neckHelper.localEulerAngles = neckRotation;

        }
    }

    private void FixedUpdate()
    {
        foreach ( MagnetPair pair in magnetPairs )
        {
            if ( pair.IsFrozen || !pair.IsActive )
                continue;

            pair.FirstMagnet.ApplyPolarForce( pair.PolarState );
            pair.SecondMagnet.ApplyPolarForce( pair.PolarState );
        }

        if ( GravActive )
        {
            float currentStrength = Mathf.Clamp( gravGunStrength * ( 1 / ( ( gravObject.transform.position - transform.position ).sqrMagnitude / ( gravEffectiveRange * gravEffectiveRange ) ) ), 0, gravGunStrength );

            Vector3 gravDirection = ( currentMouseWorld - gravObject.transform.position );

            if ( gravDirection.sqrMagnitude > 1 )
                gravDirection.Normalize();

            gravObject.ApplyExternalForce( gravDirection * currentStrength );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<MagnetResetBarrier>() != null )
        {
            int currentIndex = magnetPairIndex;

            for ( int i = 0; i < magnetPairs.Count; i++ )
            {
                magnetPairIndex = i;

                RecallCurrentPair();
            }

            magnetPairIndex = currentIndex;
        }
    }

    [System.Serializable]
    public class MagnetPair
    {
        public Magnet FirstMagnet;
        public Magnet SecondMagnet;
        public int ActiveMagnets = 0;
        public bool IsFrozen = false;
        public bool IsActive = false;
        public PolarState PolarState = PolarState.Pull;
        public Image uiPairIndicator;
        public FixedJoint Tether;

        public Magnet GetNextMagnet( bool consume = false )
        {
            int currentMagnetCount = ActiveMagnets;

            if ( consume )
                ActiveMagnets++;

            switch ( currentMagnetCount )
            {
                case 0:
                    return FirstMagnet;
                case 1:
                    return SecondMagnet;
            }

            return null;
        }

        public void Reset()
        {
            FirstMagnet.gameObject.SetActive( false );
            SecondMagnet.gameObject.SetActive( false );

            ActiveMagnets = 0;
        }
    }
}

public enum PolarState
{
    Push,
    Pull,
}
