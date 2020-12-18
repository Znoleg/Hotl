using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private Texture2D _background;
    [SerializeField] private GUIStyle _textStyle;
    private Queue<string> _sentences = new Queue<string>();
    private Queue<Texture2D> _faces = new Queue<Texture2D>();
    private string _currentText;
    private Texture2D _currentFace;
    private bool _display;
    // private Button _toSwitchFrames;
    float originalWidth = 1920.0f; //turn these to floats to fix placement issue
    float originalHeight = 1080.0f;
    private const float _cooldown = 1f;
    private float? _currentCooldown = _cooldown;

    public static CutsceneManager Current { get; private set; }
    private void Start()
    {
        
        Current = this;
        //_toSwitchFrames.interactable = false;
    }

    

    public void StartCutscene(Dialogue[] dialogues)
    {
        _sentences.Clear();
        _faces.Clear();
        //_toSwitchFrames.interactable = true;

        for (int i = 0; i < dialogues.Length; i++)
        {
            _sentences.Enqueue(dialogues[i].text);
            _faces.Enqueue(dialogues[i].face);
        }

        Time.timeScale = 0f;
        _display = true;
        DisplayNextFrame();
        
    }

    public void DisplayNextFrame()
    {
        if (!_display) return;
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        _currentText = _sentences.Dequeue();
        _currentFace = _faces.Dequeue();
    }

    private void EndDialogue()
    {
        _display = false;
        _currentCooldown = null;
        Time.timeScale = 1f;
    }

    private void OnGUI()
    {
        GUI.depth = 0;
        //Vector3 scale = new Vector3();
        //scale.x = Screen.width / originalWidth;
        //scale.y = Screen.height / originalHeight;
        //scale.z = 1;
        var svMat = GUI.matrix;
        //GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        var width = Screen.width;
        var height = Screen.height;
        if (_display)
        {
            GUI.DrawTexture(new Rect(0, 0, width, height), _background);
            GUI.DrawTexture(new Rect(width - width / 3f, height - height / 1.2f, width / 3f, height / 2f), _currentFace);
            GUI.Box(new Rect(20, height - height / 3.8f, width / 1.2f, height / 3), _currentText, _textStyle);
        }
        GUI.matrix = svMat;
    }
    private void Update()
    {
        //if (_currentCooldown.HasValue) _currentCooldown -= Time.deltaTime;
        //if (_display && ((Input.touchCount > 0 && _currentCooldown <= 0) || Input.GetKeyDown(KeyCode.Space)))
        //{
        //    var input = Input.GetTouch(0);
        //    if (input. DisplayNextFrame();
        //    _currentCooldown = _cooldown;
        //}
        //foreach (var input in Input.touches)
        //    if (input.phase == TouchPhase.Began)
        //        DisplayNextFrame();
        if (Input.GetMouseButtonDown(0)) DisplayNextFrame();
        if (Input.GetKeyDown(KeyCode.Space)) DisplayNextFrame();
    }
}
