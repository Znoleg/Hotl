using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShootMode
{
    Button, Deviation, DoubleTap
}

public class JoystickPositionHandler : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _leftPositions = new List<RectTransform>();
    [SerializeField] private List<RectTransform> _rightPosition = new List<RectTransform>();
    [SerializeField] private GameObject _shootButtonUI;
    [SerializeField] Transform[] _positionsToIgnore;
    [SerializeField] private Joystick _leftJoy;
    [SerializeField] private Joystick _rightJoy;
    [SerializeField] private List<Animator> _animators = new List<Animator>();

    private ShootMode _shootMode = ShootMode.Deviation;

    private Vector2 _leftDefaultPos;
    private Vector2 _rightDefaultPos;

    private bool _wasTouched;

    private float _screenHeight;
    private float _screenWidth;

    private const float _ignoreMagnitude = 85f;

    private const float _zoneToShoot = 0.7f;
    private PlayerCharacter _player;

    private const float _cooldown = 2f;
    private float? _currentCooldown;

    private CoroutineObject _devastationRoutine;
    private CoroutineObject _doubleTapRoutine;

    private enum JoyPosition
    {
        Left, Right
    }

    public void ChangeShootMode(ShootMode shootMode)
    {
        _shootMode = shootMode;

        #region Turn Off All Modes
        //_devastationRoutine.Stop();
        //_doubleTapRoutine.Stop();
        //_shootButtonUI.SetActive(false);
        #endregion

        //TurnOnNeededMode();
    }

    public void ChangeShootMode(string shootMode)
    {
        ChangeShootMode((ShootMode)Enum.Parse(typeof(ShootMode), shootMode));
    }

    private bool PressedAtJoy(JoyPosition joyPosition, out Touch? touch)
    {
        touch = null;
        if (Input.touchCount > 0)
        {
            var input = Input.GetTouch(0);
            if (!(input.position.y < _screenHeight / 2)) return false;
            
            if (joyPosition == JoyPosition.Right && input.position.x > _screenWidth / 2)
            {
                touch = input;
                return true;
            }
            else if (joyPosition == JoyPosition.Left && input.position.x < _screenWidth / 2)
            {
                touch = input;
                return true;
            }
        }
        return false;

    }

    private void Awake()
    {
        _devastationRoutine = new CoroutineObject(this, CheckDeviationShootRoutine);
        _doubleTapRoutine = new CoroutineObject(this, CheckDoubleTapShootRoutine);
    }

    private void Start()
    {
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;
        
        _leftDefaultPos = _leftPositions[0].position;
        _rightDefaultPos = _rightPosition[0].position;
        
        _player = PlayerCharacter.Instance;

        TurnOnNeededMode();
    }


    private void FixedUpdate()
    {
        
        if (Input.touchCount > 0 && !_wasTouched)
        {
            _wasTouched = true;

            SetAnim(_wasTouched);
            var inputPos = Input.GetTouch(0).position;
            if (inputPos.y < _screenHeight / 2)
            {
                if (inputPos.x < _screenWidth / 2)
                {
                    //_uiHolderLeft.Launch(inputPos);
                    foreach (RectTransform rect in _leftPositions)
                    {
                        rect.transform.position = inputPos;
                    }
                }
                else
                {
                    foreach (var ignorePos in _positionsToIgnore)
                        if ((inputPos - (Vector2)ignorePos.position).magnitude < _ignoreMagnitude) return;
                    foreach (RectTransform rect in _rightPosition) rect.transform.position = inputPos;
                }
            }
        }
        else if (Input.touchCount == 0)
        {
            if (!_wasTouched) return;

            _wasTouched = false;
            SetAnim(_wasTouched);

            _leftPositions[0].position = _leftDefaultPos;
            _rightPosition[0].position = _rightDefaultPos;
        }
    }

    private void SetAnim(bool status)
    {
        foreach (Animator anim in _animators) anim.SetBool("isPlayerTouching", status);
    }

    private IEnumerator CheckDeviationShootRoutine()
    {
        while (true)
        {
            if (Mathf.Abs(_rightJoy.Vertical) >= _zoneToShoot || Mathf.Abs(_rightJoy.Horizontal) >= _zoneToShoot)
                _player.AttackByButton();

            yield return null;
        }
    }

    private IEnumerator CheckDoubleTapShootRoutine()
    {
        while (true)
        {
            if (!_currentCooldown.HasValue)
            {
                if (Input.GetMouseButtonDown(0) && PressedAtJoy(JoyPosition.Right, out var touch)) _currentCooldown = _cooldown;
            }
            else if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
                if (Input.GetMouseButtonDown(0) && PressedAtJoy(JoyPosition.Right, out var touch))
                {
                    _currentCooldown = null;
                    _player.AttackByButton();
                }
            }
            else if (_currentCooldown <= 0)
            {
                _currentCooldown = null;
            }
            
            yield return null;
        }
    }

    private void TurnOnNeededMode()
    {
        if (_shootMode == ShootMode.Deviation) _devastationRoutine.Start();
        else if (_shootMode == ShootMode.DoubleTap) _doubleTapRoutine.Start();
        else if (_shootMode == ShootMode.Button)
            _shootButtonUI.SetActive(true);
    }

    //private IEnumerator CheckDeviationShootRoutine()
    //{
    //    yield return null;
    //    if (Mathf.Abs(_rightJoy.Vertical) >= _zoneToShoot || Mathf.Abs(_rightJoy.Horizontal) >= _zoneToShoot)
    //        _player.AttackByButton();
    //}

    //private IEnumerator CheckDoubleTapShootRoutine()
    //{
    //    Touch? touch;
    //    Touch? touch2;
    //    if (PressedAtJoy(JoyPosition.Right, out touch))
    //    {
    //        if (PressedAtJoy(JoyPosition.Right, out touch2))
    //            if (touch2.Value.deltaTime - touch.Value.deltaTime < 1f)
    //                _player.AttackByButton();
    //    }
    //    yield return null;
    //}
}
