using UnityEngine;
using System.Collections;

public class CameraRotateEffect : MonoBehaviour {
	PlayerMovement pm;
	private Camera camera;
	float mod=0.1f;
	float zVal=0.0f;
	
	void Start () 
	{
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
		camera = GetComponent<Camera>();
	}

	void Update () 
	{
		if (camera && pm.IsMoving)
		{
			Vector3 rot = new Vector3 (0, 0, zVal);
			

            zVal += mod;
			var dot = camera.WorldToScreenPoint(rot);


			//if (dot.z >= 3.0f && dot.z < 10.0f)
			//{
			//    mod = -0.1f;
			//}
			//else if (dot.z < 357.0f && dot.z > 350.0f)
			//{
			//    mod = 0.1f;
			//}
			if (dot.z >= 15f) mod = -0.1f;
			if (dot.z <= 5f) mod = 0.1f;
            Debug.Log(dot);
			//Debug.Log("rotation: " + transform.rotation.z + "euler: " + transform.eulerAngles.z);
			//if (transform.eulerAngles.z >= 5) mod = -0.1f;
			//else if (transform.eulerAngles.z <= 355) mod = 0.1f;
			var vec = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, dot.z - 10);

			transform.eulerAngles = vec;

		}
	}
}
