using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BeatSphere : MonoBehaviour {

	void Start () 
    {
        StartCoroutine("HeadTowardsPlayer");

        transform.DOPunchScale(Vector3.one * 0.1f, secsPerBeat * 0.5f);
	}
	
	void Update () 
    {
	}


    IEnumerator HeadTowardsPlayer()
    {
        float secsPerBeat = GameObject.Find("BPM").GetComponent<BPM>().GetSecsPerBeat();
        
        var cam = Camera.main;
        var dst = cam.transform.position;
        var src = transform.position;
        var dist = ( dst - src).magnitude;

        float delayNumBeats = 3.0f;
        

        yield return new WaitForSeconds(secsPerBeat * delayNumBeats);

        //transform.DOMove(dst, secsPerBeat * (4.0 - delayNumBeats));
        var timeLeft = secsPerBeat * (4-delayNumBeats);
        transform.DOMove(dst, timeLeft).SetEase( Ease.InBack);

        yield return new WaitForSeconds(secsPerBeat);
        Destroy(gameObject);
    }
}
