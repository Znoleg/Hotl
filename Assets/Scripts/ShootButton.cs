using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButton : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    private PlayerCharacter _playerCharacter;
    private bool _isPressed;

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (_isPressed)
        {
            _playerCharacter.AttackByButton();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    private void Start()
    {
        _playerCharacter = PlayerCharacter.Instance;
    }
}
