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

	static public SFXManager getManager() {
		GameObject obj = GameObject.Find ("SFXManager");
		SFXManager manager = obj.GetComponent ("SFXManager") as SFXManager;
		return manager;
	}

	public void playSound(string name) {
		playList.Add (name);
	}

	// Use this for initialization
	void Start () {
		sources = new AudioSource[numChannels];

		for (int i = 0; i < numChannels; i++) {
			sources[i] = this.gameObject.AddComponent ("AudioSource") as AudioSource;
			sources[i].volume = 1;
		}

		//AddSound ("explosion", "takeoff");
		AddSound ("grenade", "grenade");
		AddSound ("EndFX", "EndFX");
		/*sources [0].clip = sounds [0].clip;
		sources [1].clip = sounds [1].clip;
		sources [0].Play ();
		sources [1].Play ();*/
		//playSound ("grenade");
		//playSound ("EndFX");
	}

	//Sound sfx;

	private void AddSound(string name, string resource) {
		Sound sfx = new Sound ();
		sfx.soundName = name;
		sfx.clip = Resources.Load (resource) as AudioClip;
		sounds.Add (sfx);
	}
	
	// Update is called once per frame
	void Update () {
		while (playList.Count > 0) {
			foreach (Sound snd in sounds) {
				if (snd.soundName == playList[0]) {

					for (int i = 0; i < numChannels; i++) {
						if (!sources[i].isPlaying) {
							sources[i].clip = snd.clip;
							sources[i].Play();
						}
					}
					//src.clip = snd.clip;
					//src.Play ();
					playList.RemoveAt(0);
					break;
				}
			}
			//print (playList[0]);
			//playList.RemoveAt(0);
			/*foreach (Sound snd in sounds) {
				if (snd.name == playList[0]) {
					src.clip = snd.clip;
					src.Play ();
					playList.RemoveAt(0);
					break;
				}
			}*/
		}
	}
}
