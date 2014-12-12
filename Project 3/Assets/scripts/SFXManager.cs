using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour {

	public List<Sound> sounds;

	public AudioClip soundEffect;
	public List<string> playList;

	private AudioSource src;
	private AudioSource[] sources;

	private static int numChannels = 256;
	private int currTrack = -1;
	private string nextTrack = "";
	private int currSrcID = -1; 

	void Awake() {

		print ("lv: " + Application.loadedLevelName);

		/*if(S == null)
		{
			S = this;
		}
		else
		{
			print ("lv: " + Application.loadedLevelName);
			//Application.loadedLevelName;
			
			GameObject camObj = GameObject.Find("Main Camera");
			Camera cam = camObj.GetComponent("Camera") as Camera;
			S.transform.parent = cam.gameObject.transform;

			if(this != S)
				Destroy(this.gameObject);

			return;
		}
		
		DontDestroyOnLoad(this.gameObject);*/
	}

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
		AddSound ("EndFX", "EndFX");
		AddSound ("NoDamage", "LOZ_Shield"); 
		AddSound ("Laser1", "Laser1");
		AddSound ("Laser2", "Laser2");	
		AddSound ("EndCredits", "EndCredits");
		AddSound ("Warning", "Warning");
		AddSound ("Capture", "Capture");
		AddSound ("Boss1", "Boss1", false, "Boss2");
		AddSound ("Boss2", "Boss2", true);//, true);
		AddSound ("Title", "Title", true);
		AddSound ("Theme", "Theme", true);


		if (Application.loadedLevelName == "SplashScreen") {
			playSound ("Title");
		}
		else if (Application.loadedLevelName == "dom-dev 1") {
			playSound ("Theme");
		}
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

	public void StopMusic() {
		sources [currSrcID].Stop ();
	}

	// Adds a sound that can then be played with playSound("name");
	private void AddSound(string name, string resource, bool loop = false, string nextMusic = "") {
		Sound sfx = new Sound ();
		sfx.soundName = name;
		sfx.clip = Resources.Load (resource) as AudioClip;
		sfx.loop = loop;
		sfx.nextMusic = nextMusic;
		sounds.Add (sfx);
	}

	// Finds first available channel
	private void playOnFirstSource(Sound snd) {
		for (int i = 0; i < numChannels; i++) {
			if (!sources[i].isPlaying) {
				sources[i].clip = snd.clip;
				sources[i].Play();
				sources[i].loop = snd.loop;
				
				if (snd.nextMusic != "") {
					currTrack = i;
					nextTrack = snd.nextMusic;
				}
				else {
					currTrack = -1;
				}

				if (snd.loop) {
					currSrcID = i;
				}

				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currTrack != -1) {
			if (!sources[currTrack].isPlaying) {
				print ("Next track: " + nextTrack);
				playSound(nextTrack);
				currTrack = -1;
			}
		}

		while (playList.Count > 0) { // Play all sounds in the playList
			foreach (Sound snd in sounds) { // Finds the right AudioClip to load
				if (snd.soundName == playList[0]) {
					playOnFirstSource(snd);
					playList.RemoveAt(0);
					break;
				}
			}
		}
	}
}
