using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour {

	public static SFXManager S;

	public List<Sound> sounds;

	public AudioClip soundEffect;
	public List<string> playList;

	private AudioSource src;
	private AudioSource[] sources;

	private static int numChannels = 10;
	private int currMusicID = -1;

	void Awake() {
		if(S == null)
		{
			S = this;
		}
		else
		{
			//Application.loadedLevelName;
			
			GameObject camObj = GameObject.Find("Main Camera");
			Camera cam = camObj.GetComponent("Camera") as Camera;
			S.transform.parent = cam.gameObject.transform;

			if(this != S)
				Destroy(this.gameObject);

			return;
		}
		
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		S.sources = new AudioSource[numChannels];

		for (int i = 0; i < numChannels; i++) {
			S.sources[i] = this.gameObject.AddComponent ("AudioSource") as AudioSource;
			S.sources[i].volume = 1;
			S.sources[i].bypassEffects = true;
			S.sources[i].bypassListenerEffects = true;
			S.sources[i].bypassReverbZones = true;
			S.sources[i].pitch = 1;
			S.sources[i].rolloffMode = AudioRolloffMode.Linear;
			S.sources[i].pan = 0;
		}
		
		AddSound ("grenade", "grenade");
		AddSound ("EndFX", "EndFX");
		AddSound ("NoDamage", "LOZ_Shield"); 
		AddSound ("Laser1", "Laser1");
		AddSound ("Laser2", "Laser2");	
		AddSound ("EndCredits", "EndCredits");
		AddSound ("Warning", "Warning");
		AddSound ("Capture", "Capture");
		AddSound ("Boss", "Boss");
		AddSound ("Title", "Title", true);
		AddSound ("Theme", "Theme", true);

		playSound ("Theme");
	}

	// Gets an instance of the sound manager
	static public SFXManager getManager() {
		GameObject obj = GameObject.Find ("SFXManager");
		SFXManager manager = obj.GetComponent ("SFXManager") as SFXManager;
		return manager;
	}

	// Plays sound "name"
	public void playSound(string name) {
		S.playList.Add (name);
	}

	// Adds a sound that can then be played with playSound("name");
	private void AddSound(string name, string resource, bool loop = false, int nextMusic = -1) {
		Sound sfx = new Sound ();
		sfx.soundName = name;
		sfx.clip = Resources.Load (resource) as AudioClip;
		sfx.loop = loop;
		S.sounds.Add (sfx);
	}
	
	// Update is called once per frame
	void Update () {
		while (S.playList.Count > 0) { // Play all sounds in the playList
			foreach (Sound snd in sounds) { // Finds the right AudioClip to load
				if (snd.soundName == S.playList[0]) {

					// Finds first available channel
					for (int i = 0; i < numChannels; i++) {
						if (!S.sources[i].isPlaying) {
							S.sources[i].clip = snd.clip;
							S.sources[i].Play();
							S.sources[i].loop = snd.loop;
							
							if (snd.loop) {
								S.currMusicID = i;
							}
							
							break;
						}
					}

					// Finds first available channel
					/*for (int i = 0; i < numChannels; i++) {
						if (!sources[i].isPlaying) {
							sources[i].clip = snd.clip;
							sources[i].Play();
							break;
						}
					}*/

					S.playList.RemoveAt(0);
					break;
				}
			}
		}
	}
}
