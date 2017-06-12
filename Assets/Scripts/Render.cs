using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Render : MonoBehaviour {

	public GameObject  tetrisBlock;
	public GameObject  boardBlank;
	GameObject[]  m_blocks;
	GameObject[]  m_minoBlocks;

	Stage m_stage;
	Tetrimino m_tetrimino;

	int m_test_wait_count = 0;


	// Use this for initialization
	void Start ()
	{
		StageManager    stgMngr = GetComponent<StageManager>();
		m_stage = stgMngr.getStage();
		int w = m_stage.getBoardWidth();
		int h = m_stage.getBoardHeight();
		m_blocks = new GameObject[w*h];

		m_tetrimino = stgMngr.getTetrimino();

		m_minoBlocks = new GameObject[4];
		for(int i = 0; i<4; i++)
		{
			m_minoBlocks[i] = Instantiate(tetrisBlock);
		}
	}

	// Update is called once per frame
	void Update () {
		drawBoard(m_stage);
		drawTetrimino(m_tetrimino, m_minoBlocks);
	}

	void drawBoard(Stage stage)
	{
		int	w = stage.getBoardWidth();
		int h = stage.getBoardHeight();
		Stage.BlockInfo[,]  board = stage.getBlockInfo();

		for(int y = 0; y<h; y++)
		{
			for(int x = 0; x<w; x++)
			{
				int index = x + y*w;
				if(null != m_blocks[index])
				{
					GameObject.Destroy(m_blocks[index]);
					m_blocks[index] = null;
				}

				GameObject block = null;
				if(Stage.BLOCK_STATE.EXISTS == board[x, y].state)
				{
					block = Instantiate(tetrisBlock);
					m_blocks[index] = block;
				}
				else if(Stage.BLOCK_STATE.NONE == board[x, y].state)
				{
					block = Instantiate(boardBlank);
					m_blocks[index] = block;
				}
				if(null != block)
				{
					block.transform.position = new Vector3(x, y, 0);
					block.SetActive(true);
				}
			}
		}
	}

	void drawTetrimino(Tetrimino tetrimino, GameObject[] blocks)
	{
		Tetrimino.Pattern pat = tetrimino.getPattern();

		int num_blocks = 0;
		int index = 0;

		int base_x, base_y;
		base_x = tetrimino.getPosX();
		base_y = tetrimino.getPosY();

		for(int y = 0; y<pat.h; y++)
		{
			for(int x = 0; x<pat.w; x++)
			{
				char c = pat.pat[index];

				if(c == '1')
				{
					float posx = x + base_x;
					float posy = y + base_y;
					blocks[num_blocks].transform.position = new Vector3(posx, posy, 0);
					num_blocks++;
				}

				index++;
			}
		}
		/*
		m_test_wait_count++;
		if(30 <= m_test_wait_count)
		{
			int pat_id = tetrimino.getPatternId();
			pat_id++;
			if(tetrimino.getMaxPatternId() < pat_id) pat_id = 0;
			tetrimino.setPatternId(pat_id);
			m_test_wait_count = 0;
		}*/

	}

}
