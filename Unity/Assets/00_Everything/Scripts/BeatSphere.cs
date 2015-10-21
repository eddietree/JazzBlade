using UnityEngine;
using System.Collections;

public class BeatSphere : MonoBehaviour {

	void Start () 
    {
        StartCoroutine("HeadTowardsPlayer");
	}
	
	void Update () {
	
	}

    IEnumerator HeadTowardsPlayer()
    {
        float secsPerBeat = GameObject.Find("BPM").GetComponent<BPM>().GetSecsPerBeat();

        yield return new WaitForSeconds(secsPerBeat);

        var cam = Camera.main;
        var dst = cam.transform.position;
        var src = transform.position;
        var dist = ( dst - src).magnitude;
        
        var timeLeft = secsPerBeat * 4.0f;
        var numFrames = timeLeft / Time.deltaTime;
        var stepPerFrame = dist / numFrames;

        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, dst, stepPerFrame);

            yield return null;
        }
    }
}
