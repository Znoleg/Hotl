using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private MinimapUI _minimapUI;

    private Camera _usedCamera;

    private Vector3 _touchStart;
    private const float TouchZoomSpeed = 0.1f;
    private const float _zoomOutMin = 6f;
    private const float _zoomOutMax = 16f;

    private void Start()
    {
        if (!_player) _player = PlayerCharacter.Instance.transform;
        
        _usedCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!_minimapUI.gameObject.activeSelf)
        {
            var newPos = _player.position;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
        else 
        { 
            if (Input.GetMouseButtonDown(0))
            {
                _touchStart = _usedCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = _touchStart - _usedCamera.ScreenToWorldPoint(Input.mousePosition);
                _usedCamera.transform.position += direction;
            }

            if (Input.touchSupported)
            {
                if (Input.touchCount == 2)
                {

                    // get current touch positions
                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                    // get offset value
                    float deltaDistance = oldTouchDistance - currentTouchDistance;
                    Zoom(deltaDistance, TouchZoomSpeed);
                }
            }

            else Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        
    }

    private void Zoom(float increment)
    {
        _usedCamera.orthographicSize = Mathf.Clamp(_usedCamera.orthographicSize - increment, _zoomOutMin, _zoomOutMax);
    }

    private void Zoom(float deltaMagnitudeDiff, float speed)
    {
        _usedCamera.orthographicSize += deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        _usedCamera.orthographicSize = Mathf.Clamp(_usedCamera.orthographicSize, _zoomOutMin, _zoomOutMax);
    }
}
