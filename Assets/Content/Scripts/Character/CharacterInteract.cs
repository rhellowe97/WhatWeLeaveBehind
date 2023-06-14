using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent( typeof( CharacterController ) )]
public class CharacterInteract : MonoBehaviour
{
    private HashSet<IInteractable> currentInteractables = new HashSet<IInteractable>();

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        controller.Controls.Player.Interact.performed += UseInteractable;
    }

    private void OnDisable()
    {
        controller.Controls.Player.Interact.performed -= UseInteractable;
    }

    private void UseInteractable( InputAction.CallbackContext context )
    {
        if ( currentInteractables.Count > 0 )
            currentInteractables.First().Interact();
    }

    private void OnTriggerEnter( Collider other )
    {
        MonoBehaviour[] componentList = other.GetComponents<MonoBehaviour>();

        foreach ( MonoBehaviour component in componentList )
        {
            IInteractable possibleInteractable = component as IInteractable;

            if ( possibleInteractable != null && !currentInteractables.Contains( possibleInteractable ) )
            {
                currentInteractables.Add( possibleInteractable );
            }
        }
    }

    private void OnTriggerExit( Collider other )
    {
        MonoBehaviour[] componentList = other.GetComponents<MonoBehaviour>();

        foreach ( MonoBehaviour component in componentList )
        {
            IInteractable possibleInteractable = component as IInteractable;

            if ( possibleInteractable != null && currentInteractables.Contains( possibleInteractable ) )
            {
                currentInteractables.Remove( possibleInteractable );
            }
        }
    }
}
