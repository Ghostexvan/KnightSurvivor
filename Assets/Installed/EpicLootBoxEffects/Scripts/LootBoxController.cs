using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxController : MonoBehaviour {

	public int idIcon;
	public int idEffect;
	public bool isOpened;

	public GameObject[] IconPrefabs;
	public GameObject[] EffectPrefabs;
	public GameObject[] DesFxObjs;
	public GameObject[] DesIconObjs;
	private GameObject Lootbox;

	public Text effectsText;
	public Text nameEffectText;

	void Start () {
		idEffect += 1;
		idIcon += 1;
		effectsText.text = "Type       " + idEffect + " / 25";
		nameEffectText.text = EffectPrefabs [idEffect].gameObject.name;
		SetupVfx ();
		isOpened = false;
	}

	private void OnMouseDown (){
		if (!isOpened) {
			StartCoroutine(PlayFx());
		}
	}

	IEnumerator PlayFx() {
		isOpened = true;
		idEffect = Mathf.Clamp(idEffect, 1, 25);
		effectsText.text = "Type       " + idEffect + " / 25";
		yield return new WaitForSeconds(0.2f);
		Destroy (Lootbox);
		Lootbox = Instantiate (IconPrefabs [2], this.transform.position, this.transform.rotation);
		yield return new WaitForSeconds(0.1f);
		Instantiate (EffectPrefabs [idEffect], this.transform.position, this.transform.rotation);
		CameraShake.myCameraShake.ShakeCamera (0.3f, 0.1f);
	}

	IEnumerator PlayIcon() {
		DesIconObjs = GameObject.FindGameObjectsWithTag("Icon");

		foreach(GameObject DesIconObj in DesIconObjs)
			Destroy(DesIconObj.gameObject);
		
		yield return new WaitForSeconds(0.1f);
		Lootbox = Instantiate (IconPrefabs [1], this.transform.position, this.transform.rotation);
	}

	public void ChangedFx (int i) {
		ResetVfx ();
		idEffect = idEffect + i;
		idEffect = Mathf.Clamp(idEffect, 1, 25);
		effectsText.text = "Type       " + idEffect + " / 25";
		nameEffectText.text = EffectPrefabs [idEffect].gameObject.name;
		//StartCoroutine(PlayIcon());
	}

	public void SetupVfx () {
		Lootbox = Instantiate (IconPrefabs [1], this.transform.position, this.transform.rotation);
	}

	public void PlayAllVfx (){
		if (!isOpened) {
			StartCoroutine(PlayFx());
		}
	}

	public void ResetVfx () {
		DesFxObjs = GameObject.FindGameObjectsWithTag("Effects");
	
		foreach(GameObject DesFxObj in DesFxObjs)
				Destroy(DesFxObj.gameObject);
		isOpened = false;

		DesIconObjs = GameObject.FindGameObjectsWithTag("Icon");
	
		foreach(GameObject DesIconObj in DesIconObjs)
			Destroy(DesIconObj.gameObject);
		StartCoroutine(PlayIcon());
	}
}