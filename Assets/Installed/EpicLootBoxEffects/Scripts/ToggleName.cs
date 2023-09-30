using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleName : MonoBehaviour {

	public GameObject[] nameEffectText;

	void Start () {
		for(int i = 0; i < 4; i++) {
			nameEffectText [i].SetActive (true);
		}
	}

	public void ToggleChanged (bool isHide) {
		for(int i = 0; i < 4; i++) {
			nameEffectText [i].SetActive (!isHide);
		}
	}
}