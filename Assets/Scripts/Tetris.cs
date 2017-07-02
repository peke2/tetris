using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	テトリスの制御
 */
public class Tetris
{
	Tetrimino   m_tetrimino;

	Stage       m_stage;
	InputBase   m_input;
	Generator   m_generator;

	public const int BOARD_SIZE_W = 10;	//	盤面の基本サイズ
	public const int BOARD_SIZE_H = 20;

	const int DROP_INTERVAL = 15;		//	落下間隔(フレーム数)
	const int DROP_INTERVAL_REDUCE = 2;	//	落下間隔の削減値
	const int FIX_INTERVAL = 30;		//	固定までの間隔(フレーム数)
	int m_drop_interval;
	//int m_fix_interval;	//固定までの時間だけど、今は使わないので外しておく

	int m_stage_w;
	int m_stage_h;

	int m_drop_serial;

	bool m_is_game_over;


	/**
	 *	通常のプレイ用インスタンスを作成
	 */
	public static Tetris CreateGamePlay()
	{
		Tetris tetris;
		Tetrimino tetrimino = new Tetrimino();

		int min_index = 0;
		int max_index = tetrimino.getTypeMax();

		Generator generator = new Generator(min_index, max_index);
		InputBase input = new InputPad();

		//落下ブロックは内部で固定でも良いのでは？
		//生成クラスでの範囲指定に、落下ブロックの情報が必要だから中で生成すると厳しい
		//ひとまずは外に出しておく
		//ファクトリで作成するので外部からは呼ぶだけだし
		tetris = new Tetris(BOARD_SIZE_W, BOARD_SIZE_H, tetrimino, generator, input);
		return tetris;
	}

	/**
	 *	自動プレイ用インスタンスを作成
	 */
	public static Tetris CreateGameAutoPlay(List<int> bitList=null)
	{
		Tetris tetris;
		Tetrimino tetrimino = new Tetrimino();

		int min_index = 0;
		int max_index = tetrimino.getTypeMax();

		Generator generator = new Generator(min_index, max_index);
		InputBase input = new InputAuto(bitList);

		//落下ブロックは内部で固定でも良いのでは？
		//生成クラスでの範囲指定に、落下ブロックの情報が必要だから中で生成すると厳しい
		//ひとまずは外に出しておく
		//ファクトリで作成するので外部からは呼ぶだけだし
		tetris = new Tetris(BOARD_SIZE_W, BOARD_SIZE_H, tetrimino, generator, input);
		return tetris;
	}

	public Tetris(int stage_w, int stage_h, Tetrimino tetrimino, Generator generator, InputBase input)
	{
		m_stage_w = stage_w;
		m_stage_h = stage_h;
		m_stage = new Stage(m_stage_w, m_stage_h);

		m_tetrimino = tetrimino;
		initTetriminoStartPosition();

		m_generator = generator;
		m_drop_serial = 0;

		m_input = input;
		m_drop_interval = DROP_INTERVAL;
		//m_fix_interval = 0;

		m_is_game_over = false;

		//	落下するブロックのIDを更新
		updateDropTypeId();
	}

	// Update is called once per frame
	public void update()
	{
		if(false ==  m_is_game_over)
		{
			m_input.update();

			int input_bit = m_input.getButtonBit();
			int input_edge_bit = m_input.getButtonEdgeBit();
			movement(input_bit, input_edge_bit);
		}
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

	/**
	 *	ゲームオーバーに相当する座標か？
	 */
	bool isGameOverPosition()
	{
		if(m_stage_h == m_tetrimino.getPosY()) return true;
		return false;
	}

	/**
	 *	判定用のブロック状態を作成
	 */
	Stage.BlockInfo[] createPatternArea(Tetrimino tetrimino)
	{
		Tetrimino.Pattern pat = tetrimino.getPattern();
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
					binfo.color_index = tetrimino.getColorIndex();
				}
				else
				{
					binfo.state = Stage.BLOCK_STATE.NONE;
					binfo.color_index = 0;
				}
				patArea[index] = binfo;
			}
		}
		return patArea;
	}


	/**
	 *	落下ブロックの移動
	 */
	void movement(int input_bit, int input_edge_bit)
	{
		Tetrimino.Pattern prevPat = m_tetrimino.getPattern();
		int rot_offset = 0;

		if((input_edge_bit & InputBase.MASK_BUTTON_A) != 0)
		{
			rot_offset = 1;
		}
		else if((input_edge_bit & InputBase.MASK_BUTTON_B) != 0)
		{
			rot_offset = -1;
		}

		if(rot_offset != 0)
		{
			m_tetrimino.addTypeOffset(rot_offset);
		}

		//	現在の落下ブロックで領域判定用のデータを作る
		Tetrimino.Pattern pat = m_tetrimino.getPattern();
		Stage.BlockInfo[] patArea = createPatternArea(m_tetrimino);

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
		if(false == m_stage.existsBlockOnArea(posx+add_x, posy, patArea, pat.w, pat.h))
		{
			posx += add_x;
		}
		else
		{
			//	回転があって、かつブロックが重なる場合のみ座標をずらして再試行する
			if(rot_offset != 0)
			{
				//	現在のブロックが入る箇所を探す
				List<Tetrimino.Pos> offsetList;
				offsetList = Tetrimino.calcOffset(prevPat, pat);
				bool is_rotated = false;

				foreach(Tetrimino.Pos offset in offsetList)
				{
					//	回転後の領域にブロックが無ければ回転は完了
					if(false == m_stage.existsBlockOnArea(posx+offset.x, posy+offset.y, patArea, pat.w, pat.h))
					{
						posx += offset.x;
						posy += offset.y;
						is_rotated = true;
						break;
					}
				}
				if(false == is_rotated)
				{
					//	逆方向に回転させて回転を取り消す
					m_tetrimino.addTypeOffset(-rot_offset);
					//	再度パターンを作り直す
					pat = m_tetrimino.getPattern();
					patArea = createPatternArea(m_tetrimino);
				}
			}
		}

		//	下を押している間は追加で落下間隔を減らす
		if((input_bit & InputBase.MASK_DOWN) != 0)
		{
			m_drop_interval -= DROP_INTERVAL_REDUCE;
		}

		m_drop_interval--;
		if(0 >= m_drop_interval)
		{
			m_drop_interval = DROP_INTERVAL;

			//	移動先にブロックが無ければ落下
			if(false == m_stage.existsBlockOnArea(posx, posy-1, patArea, pat.w, pat.h))
			{
				posy -= 1;
			}
			else
			{
				//	ブロックが重なる状態でゲームオーバーに相当する座標ならゲームオーバー
				if(true == isGameOverPosition())
				{
					m_is_game_over = true;
				}

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

	/**
	 *	ゲームオーバーか？
	 */
	public bool isGameOver()
	{
		return m_is_game_over;
	}

	/**
	 *	ドロップがどこまで進んだか？
	 */
	public int getDropSerial()
	{
		return m_drop_serial;
	}

	/**
	 *	入力を取得
	 */
	public InputBase getInput()
	{
		return m_input;
	}
}
