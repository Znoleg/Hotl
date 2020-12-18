using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UIManager : MonoBehaviour
{
    [SerializeField] private GroupedUI[] _connectedUIElements;
    private Vector2 _canvasResoltion;
    
    public static UIManager Instance { get; private set; }
    public GroupedUI ActiveUI { get; private set; }
    public Vector2 CanvasToRealResolutionScale { get; private set; }

    public void TurnOnElement(GroupedUI element)
    {
        TurnOffElements(element);
        
        ActiveUI = element;
    }

    private void TurnOffElements(GroupedUI exceptThis = null)
    {
        foreach (GroupedUI ui in _connectedUIElements)
        {
            if (ui == exceptThis) continue;
            ui.Deactivate();
        }
    }

    private void Awake()
    {
        Instance = this;
        GroupedUI.OnUIOpen += TurnOnElement;
    }

    private void Start()
    {
        _canvasResoltion = GetComponent<CanvasScaler>().referenceResolution;
        CanvasToRealResolutionScale = new Vector2(Screen.width / _canvasResoltion.x, Screen.height / _canvasResoltion.y);
    }

    private void OnDestroy()
    {
        GroupedUI.OnUIOpen -= TurnOnElement;
    }
}
