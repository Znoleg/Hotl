using UnityEngine;

public class GunPickup : MonoBehaviour, IInteractable
{
    public bool CanInteract(Character character) => character is PlayerCharacter; 

    public void Interact(Character character)
    {
        
    }
}

