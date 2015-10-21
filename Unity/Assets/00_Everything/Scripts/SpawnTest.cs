using UnityEngine;
using System.Collections;

public class SpawnTest : MonoBehaviour {

    public GameObject beatShieldPrefab;

	void Start ()
    {
        StartCoroutine("SpawnBeats");
	}
	
	void Update ()
    {
	
	}

    IEnumerator SpawnBeats()
    {
        float secsPerBeat = GameObject.Find("BPM").GetComponent<BPM>().GetSecsPerBeat();

        int[] patterns =
        {
            1, 1, 1, 2,
            0, 0, 0, 0
        };

        int beatCount = 0;

        while (true)
        {
            var pattern = patterns[beatCount % patterns.Length];

            if (pattern == 1)
            {
                var deltaAngle = Mathf.PI / 3.0f;
                var angle = deltaAngle * (beatCount % patterns.Length);
                var spawnRadius = 0.5f;

                var spawnPos = transform.position + Vector3.forward * Mathf.Cos(angle) * spawnRadius + Vector3.up * Mathf.Sin(angle) * spawnRadius;

                var obj = GameObject.Instantiate(beatShieldPrefab, spawnPos, transform.rotation);
            }

            beatCount += 1;

            yield return new WaitForSeconds(secsPerBeat);

            //yield return null;
        }
    }
}
