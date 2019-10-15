using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicGround : MonoBehaviour {

	public char[] charsArray;
	public Material[] materialsArray;

	public GameObject quad;
	Material m_Material;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setArt(char letter)
	{
		quad.GetComponent<Renderer>().material = materialsArray[getIndexFromCharsArray(letter)];
	}

	public int getIndexFromCharsArray(char letter)
	{
		for (int i = 0; i < charsArray.Length; i++)
		{
			if (charsArray[i].Equals(letter))
			{
				return i;
			}
		}
		return 0;
	}
}
