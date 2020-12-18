using UnityEngine;
using UnityEngine.UI;

public class UIFunctionalityController : MonoBehaviour
{
    public static UIFunctionalityController Instance { get; private set; }
    public bool CurrentStatus { get; private set; }

    [SerializeField] private Button[] _buttons;
    [SerializeField] private FixedJoystick[] _joysticks;

    public void SetActive(bool status)
    {   
        foreach (Button button in _buttons)
        {
            button.enabled = status;
        }
        foreach (FixedJoystick joystick in _joysticks)
        {
            joystick.enabled = status;
        }

        CurrentStatus = status;
    }

    private void Awake()
    {
        Instance = this;
    }


}
