using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager: MonoBehaviour {

	List<Tetris>    m_tetrisList;

	void Awake()
	{
		m_tetrisList = new List<Tetris>();

		Tetris tetris = Tetris.CreateGamePlay();
		m_tetrisList.Add(tetris);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		foreach(Tetris tetris in m_tetrisList)
		{
			tetris.update();
		}
	}


	public List<Tetris> tetrisList
	{
		get { return m_tetrisList; }
	}

}
