using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour 
{
	[SerializeField] private Joystick controlJoy;
	private PlayerCharacter _player;
	private PlayerMovement pm;
	private bool followPlayer = true;
	private Vector3 mousePos;
	private Camera cam;
	private const float lookAheadRange = 7f;

	public bool FollowPlayer => followPlayer;

	public void SetFollowPlayer(bool val)
	{
		followPlayer = val;
	}

	public void Refresh()
    {
		CamFollowPlayer();
    }

	private void Start () 
	{

			_player = PlayerCharacter.Instance;
			pm = _player.GetComponent<PlayerMovement>();
			cam = Camera.main;
			transform.position = new Vector3(pm.transform.position.x, pm.transform.position.y, transform.position.z);

	}
	
	private void Update () {
		CamFollowPlayer();
		//LookAhead();
	}

	private void CamFollowPlayer()
	{
		if (!_player) return;
		
		Vector3 newPos = new Vector3 (_player.transform.position.x, _player.transform.position.y, this.transform.position.z);
		transform.position = newPos;
	}

	private void LookAhead()
	{
		if ((new Vector2(transform.position.x, transform.position.y) 
			- new Vector2(_player.transform.position.x, _player.transform.position.y))
				.magnitude >= lookAheadRange) return;
		Vector3 camPos = cam.ScreenToWorldPoint(new Vector3(controlJoy.Horizontal,controlJoy.Vertical));
		camPos.z = -10;
		Vector3 dir = camPos - transform.position;
		if (_player.GetComponent<SpriteRenderer> ().isVisible  == true) {
			transform.Translate(dir*2*Time.deltaTime);
		}
	}
}
