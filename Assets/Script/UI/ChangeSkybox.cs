using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public List<Material> skyboxMaterial = new List<Material>();
    public float timeBeforeChange;
    public int currentSkyboxIndex;
    public bool isSet;

    private void Awake() {
        currentSkyboxIndex = 0;
        isSet = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSet){
            RenderSettings.skybox = skyboxMaterial[currentSkyboxIndex];
            isSet = false;
            StartCoroutine(WaitUntilChange());
        }
    }

    IEnumerator WaitUntilChange(){
        yield return new WaitForSeconds(timeBeforeChange);
        currentSkyboxIndex = ChangeIndex();
        isSet = true;
    }

    int ChangeIndex(){
        if (currentSkyboxIndex >= skyboxMaterial.Count - 1)
            return 0;
        return currentSkyboxIndex + 1;
    }
}
