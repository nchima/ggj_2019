using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	Dictionary<int, Level> loadedLevels = new Dictionary<int, Level>();

	public Character character; // This is a link to the prefab of the character in the scene
	public BasicGround ground; // Basic ground piece prefab
	public Acorn acorn; // acorn prefab
	public Berry berry; // berry prefab
	public Cave cave; // cave prefab
	public Level levelPrefab; // level prefab

	public List<BasicGround> groundPieces;
	public List<Acorn> acorns;
	public List<Berry> berries;
	public List<Cave> caves;
	public Animator vineAnimator;

	public int berriesCollected = 0;
	public int acornsCollected = 0;

	public string gameState = "startScreen";

	private int _currentLevel = 0;
	
	public AudioClip jumpSFX;
	public AudioClip landSFX;
	public AudioClip celebrationSFX;
	public AudioClip squawkFlapSFX;
	public AudioClip squawkFranticSFX;
	public AudioClip victorySFX;
    public AudioSource oneShotAudioSource;
	public AudioSource idleAudioSource;
	public AudioSource bearAudioSource;
	public AudioSource birdAudioSource;
	public AudioSource chordsAudioSource;
	public AudioSource sleepyAudioSource;

	public GameObject berryUI;
	public GameObject acornUI;

	Vector3 characterHomePosition = new Vector3(0.6f,-1.5f,0);

	// Use this for initialization
	void Start () {
		loadLevels();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("escape"))
		{
			loadedLevels[_currentLevel].hide();
			loadedLevels[0].show();
			_currentLevel = 0;
			resetCharacterAndUI();
			resetAudio();
		}
		if (Input.GetKeyDown("r"))
		{
			loadedLevels[_currentLevel].hide();
			loadedLevels[_currentLevel].show();
			resetCharacterAndUI();
			resetAudio();
		}
		if (Input.GetKeyDown("]"))
		{
			moveLevel(1);
		}
		
		if (Input.GetKeyDown("["))
		{
			moveLevel(-1);
		}
		if (Input.GetKeyDown("r"))
		{
			loadedLevels[_currentLevel].hide();
			loadedLevels[_currentLevel].show();
			resetCharacterAndUI();
			resetAudio();
		}

		if (gameState == "startScreen")
		{
			print("Loading Level '" + loadedLevels[_currentLevel].show() + "'");
			resetCharacterAndUI();
			gameState = "playing";
			resetAudio();
		}
	}

	void resetAudio()
	{
		idleAudioSource.Stop();
		chordsAudioSource.Stop();
		bearAudioSource.Stop();
		birdAudioSource.Stop();
		idleAudioSource.Play();
		chordsAudioSource.Play();
		bearAudioSource.Play();
		birdAudioSource.Play();
		idleAudioSource.volume = 0.5f;
		chordsAudioSource.volume = 0.5f;
		bearAudioSource.volume = 0.0f;
		birdAudioSource.volume = 0.0f;
	}

	void resetCharacterAndUI()
	{
		// Reset character
		character.m_Rigidbody.isKinematic = false;
		character.m_Rigidbody.velocity = new Vector3(0,0,0);
		character.m_Rigidbody.angularVelocity = new Vector3(0,0,0);
		character.gameObject.transform.position = characterHomePosition;
		character.gameObject.SetActive(true);
		character.canJump = false;
		character.usingBear = false;
		character.usingBird = false;

		// Reset UI
		berriesCollected = 0;
		acornsCollected = 0;
		acornUI.SetActive(false);
		berryUI.SetActive(false);
	}

	void loadLevels()
	{
		// If this goes true we just load from text at bottom of this file...
		bool loadInternalLevels = false;
		string[] files = null;
		try {
			files = Directory.GetFiles(@".", "*.whty", SearchOption.AllDirectories);
		} catch {
			loadInternalLevels = true;
		}
		
		int levelIndex = 0;
		
		if (loadInternalLevels){
			// We're going to use levels stored internnally, probably a web build running...
			for (int i=0; i<6; i++)
			{
				Level l = Instantiate(levelPrefab, new Vector3(0,0,0), Quaternion.identity);
				l.initLevel(""+i, this, true);
				loadedLevels.Add(levelIndex, l);
				levelIndex++;
			}

		} else {
			// Loop through the level files
			foreach (string filename in files)
			{
				Level l = Instantiate(levelPrefab, new Vector3(0,0,0), Quaternion.identity);
				l.initLevel(filename, this, false);
				loadedLevels.Add(levelIndex, l);
				levelIndex++;
			}
		}
	}

	public void showBerriesCollected()
	{
		berryUI.SetActive(true);
		berryUI.GetComponentsInChildren<TextMesh>()[0].text = "" + berriesCollected + "/" + berries.Count;
	}
	public void showAcornsCollected()
	{
		acornUI.SetActive(true);
		acornUI.GetComponentsInChildren<TextMesh>()[0].text = "" + acornsCollected + "/" + acorns.Count;
	}

	IEnumerator hideBerriesCollected()
	{
		yield return new WaitForSeconds(1.5f);
		berryUI.SetActive(false);
	}

	IEnumerator hideAcornsCollected()
	{
		yield return new WaitForSeconds(1.5f);
		acornUI.SetActive(false);
	}

	public void berryCollected()
	{
		berriesCollected++;
		showBerriesCollected();
		StartCoroutine(hideBerriesCollected());
	}

	public void acornCollected()
	{
		acornsCollected++;
		showAcornsCollected();
		StartCoroutine(hideAcornsCollected());
	}

	public void finishedLevel(Cave _cave)
	{
		if (_currentLevel == loadedLevels.Count-1){
			// THEY WIN THE GAME
		} else {
			// If they collected all of them, celebrate!
			if (acornsCollected == acorns.Count && acornsCollected != 0){
				showAcornsCollected();
			}
			if (berriesCollected == berries.Count && berriesCollected != 0){
				showBerriesCollected();
			}
			
			vineAnimator.SetTrigger("LevelTransition");

			StartCoroutine(loadNextLevel());
		}
	}

	IEnumerator loadNextLevel()
	{
		yield return new WaitForSeconds(5.0f);
		Cave c = (Cave)FindObjectOfType(typeof(Cave));
		c.front.SetActive(false);
		yield return new WaitForSeconds(1.0f);
		moveLevel(1);		
	}

	// Moves the player to a different level in the loop of loaded levels by change (usually 1 or -1)
	void moveLevel(int change)
	{
		loadedLevels[_currentLevel].hide();
		_currentLevel = (_currentLevel + change ) % loadedLevels.Count;
		if (_currentLevel < 0)
		{
			_currentLevel = loadedLevels.Count-1;
		}
		print("Loading Level '" + loadedLevels[_currentLevel].show() + "'");
		resetCharacterAndUI();
		resetAudio();
	}
	public Dictionary<string, string[]> internalLevelLines = new Dictionary<string, string[]>()
	{
		{"0", new string[]
			{
"Level Name: Tutorial1",
"Terrain:    c",
"Terrain: ──────"
			}
		},
		{"1", new string[]
			{
"Level Name: Tutorial2",
"Terrain: b",
"Terrain:  a a  c",
"Terrain: ────────"
			}
		},
		{"2", new string[]
			{
"Level Name: Tutorial3",
"Terrain:      a        a   c",
"Terrain: ─┐ ┌──┐a┌─┐a┌──┐ ┌──┐",
"Terrain: █┤ ├██└─┘█└─┘██└─┘██└   ",
"Terrain:  └B┘"
			}
		},
		{"3", new string[]
			{
"Level Name: Hidden Treasures",
"Terrain:                ┤B├ ",
"Terrain:                ┤b├          ab┤",
"Terrain:                ┤ ├         ┌──┤ a ",
"Terrain:                         a               c",
"Terrain: ─┐ ┌──┐a┌─┐ ┌──────┐a┌──┐ ┌────┐ ┌───────────",
"Terrain: █┤ ├██└─┘█┤a├██████└─┘██└─┘████┤ ├███████████",
"Terrain: █┤b├██████└─┘██████████████████└─┘███████████",
"Terrain: █└─┘█████████████████████████████████████████"
			}
		},
		{"4", new string[]
			{
"Level Name: Grand Canyon",
"Terrain:               b               b   ",
"Terrain: ─ ────             a    ┌──┐       a┌──┐",
"Terrain:   b    ┌──┐                                  c",
"Terrain:     a        ┌──┐   ┌──────────┐ ┌──────┐ ┌────",
"Terrain: ┌──┐     a          ├██████████┤ ├██████└─┘████  ",
"Terrain:   b    ┌──┐   a     ├██████████└B┘█████████████",
"Terrain:                     ├██████████████████████████",
"Terrain: aba                a├██████████████████████████",
"Terrain: ────────────────────┘██████████████████████████"
			}
		},
		{"5", new string[]
			{
"Level Name: Thanks",
"Terrain:    aaaaaaabbbaaaaaa",
"Terrain:─┐  abbaababaabaBaaa   c",
"Terrain:    aaabaaabbbbaaaab ┌──",
"Terrain:    aaabaBabbbbaabaa ├██",
"Terrain:    aaabaaabaabaaaba ├██",
"Terrain:    aaabaaaaabaaaaa c├██",
"Terrain: ███████████████████████",
			}
		}
	};
}
