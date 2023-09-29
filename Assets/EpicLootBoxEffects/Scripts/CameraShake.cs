using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public float shakeTimer;
	public float shakeAmount;
	public static CameraShake myCameraShake;
	private Vector3 startPos;

	void Awake () {
		myCameraShake = this;
		startPos = transform.position;
	}

	void Update () {
		if (shakeTimer >= 0) {
			Vector2 shakePos = Random.insideUnitCircle * shakeAmount;
			transform.position = new Vector3 (transform.position.x + (shakePos.x * 0.3f),
				transform.position.y + shakePos.y,
				transform.position.z);
			shakeTimer -= Time.deltaTime;
		} else {
			transform.position = startPos;
		}

	}

	public void ShakeCamera (float shakePwr, float shakeDur) {
		shakeAmount = shakePwr;
		shakeTimer = shakeDur;
	}
}