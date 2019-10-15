using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Level : MonoBehaviour {


	string[] _lines;
	string _levelName;
	string _filename;
	bool _internalLevel;
	LevelManager _levelManager;
	System.DateTime fileLastChangedTime;

	public void initLevel(string filename, LevelManager lm, bool internalLevel) {
		_levelManager = lm;
		_internalLevel = internalLevel;
		if (_internalLevel)
		{
			_lines = _levelManager.internalLevelLines[filename];
		} else {
			_filename = filename;
			fileLastChangedTime = System.IO.File.GetLastWriteTime(filename);
			StartCoroutine(checkForFreshFile());
			_lines = System.IO.File.ReadAllLines(filename);
		}
	}


	IEnumerator checkForFreshFile()
	{
		yield return new WaitForSeconds(3.0f);
		if (fileLastChangedTime != System.IO.File.GetLastWriteTime(_filename))
		{
			fileLastChangedTime = System.IO.File.GetLastWriteTime(_filename);
			hide();
			initLevel(_filename, _levelManager, false);
			show();
		} else {
			StartCoroutine(checkForFreshFile());
		}
	}

	public string show()
	{
		int terrainLineIndex = 0;
		if (!_internalLevel){
			_lines = System.IO.File.ReadAllLines(_filename);
		}
		foreach (string line in _lines)
		{
			if (line.StartsWith("//")){
				continue;
			} else if (line.StartsWith("Level Name:")){
				_levelName = line.Substring(line.IndexOf(":")+2);
			} else if (line.StartsWith("Terrain:")){
				loadTerrain(line.Substring(line.IndexOf(":")+2), terrainLineIndex);
				terrainLineIndex++;
			}
		}
		return _levelName;
	}
	
	
	void loadTerrain(string terrain, int terrainLineIndex)
	{
		int index = 0;
		foreach (char letter in terrain)
		{
			if (letter == 'a') {
				_levelManager.acorns.Add(Instantiate(_levelManager.acorn, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity));
			} else if (letter == 'b') {
				_levelManager.berries.Add(Instantiate(_levelManager.berry, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity));
			} else if (letter == 'B') {
				_levelManager.berries.Add(Instantiate(_levelManager.berry, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity));
				BasicGround g = Instantiate(_levelManager.ground, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity);
				g.setArt('─');
				_levelManager.groundPieces.Add(g);
			} else if (letter == 'c') {
				_levelManager.caves.Add(Instantiate(_levelManager.cave, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity));
			} else if (letter != ' ' ) { 
				BasicGround g = Instantiate(_levelManager.ground, new Vector3(index*3.0f, -3.0f*terrainLineIndex, 0), Quaternion.identity);
				g.setArt(letter);
				_levelManager.groundPieces.Add(g);
			}
			index++;
		}
	}

	public void hide()
	{
		// Just destroy it all
		foreach (BasicGround g in _levelManager.groundPieces)
		{
			Destroy(g.gameObject);
		}
		_levelManager.groundPieces.Clear();
		foreach (Acorn a in _levelManager.acorns)
		{
			if (a!=null)
			{
				Destroy(a.gameObject);
			}
		}
		_levelManager.acorns.Clear();
		foreach (Berry b in _levelManager.berries)
		{
			if (b!=null)
			{
				Destroy(b.gameObject);
			}
		}
		_levelManager.berries.Clear();
		foreach (Cave c in _levelManager.caves)
		{
			Destroy(c.gameObject);
		}
		_levelManager.caves.Clear();
	}
}
