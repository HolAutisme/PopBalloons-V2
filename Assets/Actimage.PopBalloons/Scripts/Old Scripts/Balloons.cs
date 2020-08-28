using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloons : MonoBehaviour
{
    public static Balloons instance;
    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
