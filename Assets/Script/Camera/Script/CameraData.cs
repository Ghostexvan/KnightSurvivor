using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class CameraData : ScriptableObject
{
    public float angle,
                 step,
                 distance,
                 min_distance,
                 max_distance;
    
    public float GetDistanceAxisY(){
        return distance * (float) Math.Cos(ConvertToRadiant(angle));
    }

    public float GetDistanceAxisZ(){
        return distance * (float) Math.Sin(ConvertToRadiant(angle));
    }

    float ConvertToRadiant(float degree){
        return ((float) Math.PI / 180) * degree;
    }

    public void IncreaseDistance(){
        if (distance >= max_distance)
            return;

        distance += step;
    }

    public void DecreaseDistance(){
        if (distance <= min_distance)
            return;

        distance -= step;
    }
}
