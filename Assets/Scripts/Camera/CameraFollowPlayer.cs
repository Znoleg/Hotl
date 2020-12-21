using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Joystick _controlJoy;
    private PlayerCharacter _player;
    private Camera _cam;
    private const float _lookAheadRange = 7f;

    public void Refresh()
    {
        CamFollowPlayer();
    }

    private void Start()
    {
        _player = PlayerCharacter.Instance;
        _cam = Camera.main;
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        CamFollowPlayer();
        //LookAhead();
    }

    private void CamFollowPlayer()
    {
        if (!_player) return;

        Vector3 newPos = new Vector3(_player.transform.position.x, _player.transform.position.y, this.transform.position.z);
        transform.position = newPos;
    }

    private void LookAhead()
    {
        if ((new Vector2(transform.position.x, transform.position.y)
            - new Vector2(_player.transform.position.x, _player.transform.position.y))
                .magnitude >= _lookAheadRange) return;
        Vector3 camPos = _cam.ScreenToWorldPoint(new Vector3(_controlJoy.Horizontal, _controlJoy.Vertical));
        camPos.z = -10;
        Vector3 dir = camPos - transform.position;
        if (_player.GetComponent<SpriteRenderer>().isVisible == true)
        {
            transform.Translate(dir * 2 * Time.deltaTime);
        }
    }
}
