using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour {

	public List<Sound> sounds;

	public AudioClip soundEffect;
	public List<string> playList;

	private AudioSource src;
	private AudioSource[] sources;

	private static int numChannels = 10;

	// Use this for initialization
	void Start () {
		sources = new AudioSource[numChannels];

		for (int i = 0; i < numChannels; i++) {
			sources[i] = this.gameObject.AddComponent ("AudioSource") as AudioSource;
			sources[i].volume = 1;
			sources[i].bypassEffects = true;
			sources[i].bypassListenerEffects = true;
			sources[i].bypassReverbZones = true;
			sources[i].pitch = 1;
			sources[i].rolloffMode = AudioRolloffMode.Linear;
			sources[i].pan = 0;
		}
		
		AddSound ("grenade", "grenade");
		AddSound ("Laser1", "Laser1");
		AddSound ("Laser2", "Laser2");

		
		
		AddSound ("EndCredits", "EndCredits");
		AddSound ("Warning", "Warning");
		AddSound ("Capture", "Capture");
		AddSound ("Boss", "Boss");
		AddSound ("Title", "Title");

		playSound ("Boss");
	}

	// Gets an instance of the sound manager
	static public SFXManager getManager() {
		GameObject obj = GameObject.Find ("SFXManager");
		SFXManager manager = obj.GetComponent ("SFXManager") as SFXManager;
		return manager;
	}

	// Plays sound "name"
	public void playSound(string name) {
		playList.Add (name);
	}

	// Adds a sound that can then be played with playSound("name");
	private void AddSound(string name, string resource) {
		Sound sfx = new Sound ();
		sfx.soundName = name;
		sfx.clip = Resources.Load (resource) as AudioClip;
		sounds.Add (sfx);
	}
	
	// Update is called once per frame
	void Update () {
		while (playList.Count > 0) { // Play all sounds in the playList
			foreach (Sound snd in sounds) { // Finds the right AudioClip to load
				if (snd.soundName == playList[0]) {

					// Finds first available channel
					for (int i = 0; i < numChannels; i++) {
						if (!sources[i].isPlaying) {
							sources[i].clip = snd.clip;
							sources[i].Play();
							break;
						}
					}

					playList.RemoveAt(0);
					break;
				}
			}
		}
	}
}
