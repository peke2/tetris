﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

	Tetrimino   m_tetrimino;

	//public GameObject  tetrisBlock;
	Stage       m_stage;
	InputBase   m_input;
	Generator   m_generator;

	const int DROP_INTERVAL = 15;   //	落下間隔(フレーム数)
	const int FIX_INTERVAL = 30;   //	固定までの間隔(フレーム数)
	int m_drop_interval;
	int m_fix_interval;

	int m_stage_w;
	int m_stage_h;

	int m_drop_serial;

	void Awake()
	{
		m_stage_w = 10;
		m_stage_h = 20;
		m_stage = new Stage(m_stage_w, m_stage_h);

		m_tetrimino = new Tetrimino();
		initTetriminoStartPosition();

		int min_index = 0;
		int max_index = m_tetrimino.getTypeMax();

		m_generator = new Generator(min_index, max_index);
		m_drop_serial = 0;
	}


	// Use this for initialization
	void Start ()
	{
		m_input = new InputPad();
		m_drop_interval = DROP_INTERVAL;
		m_fix_interval = 0;

		//	落下するブロックのIDを更新
		updateDropTypeId();
	}
	
	// Update is called once per frame
	void Update () {
		m_input.update();

		int input_bit = m_input.getButtonBit();
		int input_edge_bit = m_input.getButtonEdgeBit();
		movement(input_bit, input_edge_bit);
	}

	/**
	 *	落下するブロックのIDを更新
	 */
	void updateDropTypeId()
	{
		int type_id = m_generator.getIndex(m_drop_serial);
		m_tetrimino.setTypeId(type_id);
		m_drop_serial++;
	}

	//	初期座標をセット
	void initTetriminoStartPosition()
	{
		m_tetrimino.setPos(m_stage_w/2, m_stage_h);
	}


	void movement(int input_bit, int input_edge_bit)
	{
		if( (input_edge_bit & InputBase.MASK_BUTTON_A) != 0 )
		{
			m_tetrimino.incrementTypeOffset();
		}

		//	現在の落下ブロックで領域判定用のデータを作る
		Tetrimino.Pattern pat = m_tetrimino.getPattern();
		//int[] patArea = new int[pat.w*pat.h];
		Stage.BlockInfo[] patArea = new Stage.BlockInfo[pat.w*pat.h];
		for(int y = 0; y<pat.h; y++)
		{
			for(int x = 0; x<pat.w; x++)
			{
				int index = x + y * pat.w;
				Stage.BlockInfo binfo = new Stage.BlockInfo();
				if('1' == pat.pat[index])
				{
					binfo.state = Stage.BLOCK_STATE.EXISTS;
					binfo.color_index = m_tetrimino.getColorIndex();
				}
				else
				{
					binfo.state = Stage.BLOCK_STATE.NONE;
					binfo.color_index = 0;
				}
				patArea[index] = binfo;
			}
		}

		//	入力に合わせて座標移動
		int posx, posy;
		int add_x;
		posx = m_tetrimino.getPosX();
		posy = m_tetrimino.getPosY();

		add_x = 0;
		if((input_edge_bit & InputBase.MASK_LEFT) != 0)
		{
			add_x = -1;
		}
		else if((input_edge_bit & InputBase.MASK_RIGHT) != 0)
		{
			add_x = 1;
		}

		//	移動先にブロックが無ければ座標を更新
		if( false == m_stage.existsBlockOnArea(posx+add_x, posy, patArea, pat.w, pat.h) )
		{
			posx += add_x;
		}

		//	下を押している間は追加で落下間隔を減らす
		if( (input_bit & InputBase.MASK_DOWN) != 0 )
		{
			m_drop_interval--;
		}

		m_drop_interval--;
		if( 0 >= m_drop_interval )
		{
			m_drop_interval = DROP_INTERVAL;

			//	移動先にブロックが無ければ落下
			if(false == m_stage.existsBlockOnArea(posx, posy-1, patArea, pat.w, pat.h))
			{
				posy -= 1;
			}
			else
			{
				//	盤面に置く
				m_stage.placeBlocks(posx, posy, patArea, pat.w, pat.h);
				
				//	落下ブロックは開始位置へ
				initTetriminoStartPosition();

				//フラグでもセットして座標設定をパスした方が早いかも？
				posx = m_tetrimino.getPosX();
				posy = m_tetrimino.getPosY();

				//	落下ブロックを更新
				updateDropTypeId();

				//	盤面で1ライン埋まった行を取得
				List<int> completedLine = m_stage.getCompletedLines();
				//	対象の行を削除
				m_stage.eraseLines(completedLine);
			}
		}

		m_tetrimino.setPos(posx, posy);
	}


	public Stage getStage()
	{
		return m_stage;
	}

	public Tetrimino getTetrimino()
	{
		return m_tetrimino;
	}
}
