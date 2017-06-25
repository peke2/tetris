using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager: MonoBehaviour {

	List<Tetris>    m_tetrisList;
	GenerationManager m_generationManager;

	bool m_isTetrisUpdated;

	void Awake()
	{
		m_tetrisList = new List<Tetris>();

		//Tetris tetris;

		//	プレイ用
		//tetris = Tetris.CreateGamePlay();
		//m_tetrisList.Add(tetris);

		//	自動
		m_generationManager = new GenerationManager();
	}

	// Use this for initialization
	void Start()
	{
		//施策
		//1世代20候補
		//全部終了
		//上位の操作情報を収集

		//評価
		//ドロップがどこま進んだか
		//選択
		//個体を2つ選択→交叉
		//1つ→突然変異
		//確率は1%くらい
		//1つ→そのまま

		//操作
		//交叉(組み換え)
		//突然変異

		m_isTetrisUpdated = false;


	}

	// Update is called once per frame
	void Update()
	{
		GenerationManager.State state;
		state = m_generationManager.update();

		if(GenerationManager.State.Init == state)
		{
			m_tetrisList = m_generationManager.getTetrisList();
			m_isTetrisUpdated = true;
		}
		else if(GenerationManager.State.Change == state)
		{
			int gen, cnt;
			gen = m_generationManager.getGeneration();
			cnt = m_generationManager.getPlayCount();
			Debug.Log("世代["+gen+"] カウント["+cnt+"]");

			//初期化と同じ処理…
			m_tetrisList = m_generationManager.getTetrisList();
			m_isTetrisUpdated = true;
		}
	}

	public List<Tetris> tetrisList
	{
		get { return m_tetrisList; }
	}

	public bool isTetrisUpdated()
	{
		return m_isTetrisUpdated;
	}
	public void setTetrisUpdated(bool isUpdated)
	{
		m_isTetrisUpdated = isUpdated;
	}
}
