using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlay : MonoBehaviour {
    public AudioClip[] soundtrack;
    int currentSong = 2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<AudioSource>().isPlaying == false) {
            currentSong++;
            if (currentSong >= soundtrack.Length) currentSong = 0;
            GetComponent<AudioSource>().clip = soundtrack[currentSong];
            GetComponent<AudioSource>().Play();
        }
	}
}
