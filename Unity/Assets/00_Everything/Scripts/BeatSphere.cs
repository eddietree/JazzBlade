using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BeatSphere : MonoBehaviour 
{

    float PitchShift( int numSemitones )
    {
        return Mathf.Pow(1.05946f, numSemitones);
    }

	void Start () 
    {
        StartCoroutine("HeadTowardsPlayer");

        // scale pulsate
        float secsPerBeat = GameObject.Find("BPM").GetComponent<BPM>().GetSecsPerBeat();
        transform.DOPunchScale(Vector3.one * 0.1f, secsPerBeat * 0.5f);

        int[] semitones =
        {
            0, 4, 7, 11, 
            12, 12+4,
            -1,
        };

        // pitch
        var audioSrc = GetComponent<AudioSource>();
        var semitoneIndex = Random.RandomRange(0, semitones.Length);
        audioSrc.pitch = PitchShift(semitones[semitoneIndex]);
        audioSrc.Play();
	}
	
	void Update () {
	
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

        var audioSrc = GetComponent<AudioSource>();
        audioSrc.Play();

        transform.DOPunchScale(Vector3.one * 0.1f, secsPerBeat * 0.5f);
        yield return new WaitForSeconds(secsPerBeat);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        var collider = collision.collider;

        if (collider.name == "ShieldCollision")
        {
            var audioSrc = GetComponent<AudioSource>();
            audioSrc.pitch = 1.0f;
            audioSrc.Play();

            StopCoroutine("HeadTowardsPlayer");

            Debug.Log(collider.name);
            Destroy(gameObject);
        }
    }
}
