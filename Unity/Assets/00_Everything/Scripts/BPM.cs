using UnityEngine;
using System.Collections;

public class BPM : MonoBehaviour {

    public int bpm = 100;

    public float GetSecsPerBeat()
    {
        return 60.0f / bpm;
    }

	void Start () 
    {
	}
	
	void Update () 
    {
	}
}
