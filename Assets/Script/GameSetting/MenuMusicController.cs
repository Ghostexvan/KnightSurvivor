using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    [Serializable]
    public struct AudioInfo{
        //public string useInScene;
        public AudioClip audioClip;
    }

    public List<AudioInfo> audioInfos;
    public int index;
    public bool isGettingAudioClip;
    public AudioSource audioSource;

    void Awake()
    {
        index = -1;
        audioSource = GameObject.Find("GameSettings").GetComponent<AudioSource>();
        StartCoroutine(GetAudioClipToPlay());
    }

    // Update is called once per frame
    void Update()
    {
        if (isGettingAudioClip)
            return;

        if (!audioSource.isPlaying){
            StartCoroutine(GetAudioClipToPlay());
        }
    }

    public IEnumerator GetAudioClipToPlay(){
        isGettingAudioClip = true;
        int tempIndex;

        do {
            tempIndex = UnityEngine.Random.Range(0, audioInfos.Count);
        } while (index == tempIndex);
        index = tempIndex;

        audioSource.clip = audioInfos[index].audioClip;
        audioSource.Play();
        isGettingAudioClip = false;
        yield return null;
    }
}
