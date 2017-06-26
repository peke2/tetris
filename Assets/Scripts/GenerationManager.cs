using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	世代管理
 */
public class GenerationManager
{

	const int GENERATION_NUMS = 20;     //	1世代の数
	const int MAX_PLAY_NUMS = 20;       //	最大プレイ数

	const int PROC_COUNT_PER_FRAME = 4;	//	1フレームの処理回数

	int m_generation;   //	今の世代
	int m_play_count;   //	世代のプレイカウント

	List<List<int>> m_currentGenerationList;
	List<List<int>> m_nextGenerationList;

	List<Tetris> m_tetrisList;

	delegate State StateProcess();

	StateProcess    m_stateProc;

	public enum State
	{
		None,   //	状態無し
		Init,   //	初期化
		Play,   //	プレイ中
		Change, //	交代
		ToNext, //	次の世代へ
	}


	public GenerationManager()
	{
		//	世代の走査情報
		m_currentGenerationList = initGenerationList();
		m_nextGenerationList = initGenerationList();

		//	初期化から開始
		m_stateProc = procStateInit;
	}

	public State update()
	{
		//全部ゲームオーバーか？
		//必要な個体数の評価が済んだか？
		//次の世代を生成

		State state;
		state = m_stateProc();
		return state;
	}

	State procStateBlank()
	{
		return State.None;
	}

	/**
	 * 初期化
	 */
	State procStateInit()
	{
		m_generation = 0;
		m_play_count = 0;

		m_tetrisList = createGeneration();
		m_stateProc = procStateUpdate;

		return State.Init;
	}

	/**
	 * 更新
	 */
	State procStateUpdate()
	{
		for(int i=0; i<PROC_COUNT_PER_FRAME; i++)
		{
			foreach(Tetris tetris in m_tetrisList)
			{
				tetris.update();
			}
		}

		if( true == isAllGameOver() )
		{
			m_stateProc = procStateChange;
		}

		return State.Play;
	}

	/**
	 * 盤面の変更
	 */
	State procStateChange()
	{
		m_play_count++;
		int nums = m_play_count * MAX_PLAY_NUMS;

		//	1世代分の実行数に達したら次の世代へ
		if( GENERATION_NUMS <= nums )
		{
			m_play_count = 0;
			m_generation++;
		}

		m_tetrisList = createGeneration();
		m_stateProc = procStateUpdate;

		return State.Change;
	}

	/**
	 *	世代管理リストを初期化
	 */
	List<List<int>> initGenerationList()
	{
		List<List<int>> list = new List<List<int>>();
		for(int i = 0; i<GENERATION_NUMS; i++)
		{
			//	操作用データ(世代交代していないので空のデータ)
			list.Add(new List<int>());
		}
		return list;
	}


	/**
	 *	世代を作成
	 */
	List<Tetris> createGeneration()
	{
		int index_offset;

		index_offset = m_play_count * MAX_PLAY_NUMS;

		List<Tetris> tetrisList = new List<Tetris>();

		List<int> bitList;
		Tetris tetris;

		//	自動
		for(int i=0; i<MAX_PLAY_NUMS; i++)
		{
			bitList = m_currentGenerationList[i + index_offset];
			if(0 == bitList.Count) bitList = null;
			tetris = Tetris.CreateGameAutoPlay(bitList);
			tetrisList.Add(tetris);
		}

		return tetrisList;
	}


	/**
	 *	全てゲームオーバーか？
	 */
	bool isAllGameOver()
	{
		foreach(Tetris tetris in m_tetrisList)
		{
			if(false == tetris.isGameOver()) return false;
		}

		return true;
	}


	public List<Tetris> getTetrisList()
	{
		return m_tetrisList;
	}


	public int getGeneration()
	{
		return m_generation;
	}
	public int getPlayCount()
	{
		return m_play_count;
	}
}	
