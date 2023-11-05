using System;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [Serializable]
    public struct resolutionInfo{
        public int width, height;
        public bool isFullscreen;
    }

    public resolutionInfo resolution;
    public float volume;
    public int frameRate;
    public bool isSet;
}
