using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMusicController : MonoBehaviour
{
    [Serializable]
    public struct AudioListInfo{
        public string name;
        public List<AudioClip> audioList;
    }

    public AudioSource audioSource;
    public bool isGettingAudioClip;
    public int index;
    public AudioListInfo bossBattle,
                         normal,
                         gameOver;
    public string currentAudioList;

    private void Awake() {
        audioSource = GameObject.Find("GameSettings").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckChangeGameState();
    }

    // Update is called once per frame
    void Update()
    {
        CheckChangeGameState();
    }

    public IEnumerator GetAudioClipToPlay(AudioListInfo clipList){
        isGettingAudioClip = true;
        int tempIndex;

        do {
            tempIndex = UnityEngine.Random.Range(0, clipList.audioList.Count);
        } while (index == tempIndex);
        index = tempIndex;
        
        audioSource.clip = clipList.audioList[index];
        audioSource.Play();
        isGettingAudioClip = false;
        yield return null;
    }

    public void ChangeAudioList(AudioListInfo clipList){
        if (currentAudioList != clipList.name){
            index = -1;
            currentAudioList = clipList.name;
            StartCoroutine(GetAudioClipToPlay(clipList));
        }
        
        if (!audioSource.isPlaying)
            ContinuePlayAudio();
    }

    public void ContinuePlayAudio(){
        if (audioSource.isPlaying){
            return;
        }

        StartCoroutine(GetAudioClipToPlay(GetCurrentAudioList()));
    }

    public AudioListInfo GetCurrentAudioList(){
        if (currentAudioList == bossBattle.name)
            return bossBattle;
        
        return normal;
    }

    public void CheckChangeGameState(){
        if (GameObject.Find("GameController").GetComponent<GameController>().isBossBattle){
            ChangeAudioList(bossBattle);
            return;
        }

        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath){
            ChangeAudioList(gameOver);
            return;
        }

        ChangeAudioList(normal);
    }
}
