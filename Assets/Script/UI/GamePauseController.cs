using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        Time.timeScale = 0.0f;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnDisable() {
        Time.timeScale = 1.0f;
    }
}
