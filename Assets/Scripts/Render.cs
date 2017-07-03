using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	このクラスは勝手にStageManagerを参照して情報を取得・描画する
 *	他のクラスからはこのクラスを触らない
 */
public class Render : MonoBehaviour {

	public GameObject tetrisBlock;
	public GameObject boardBlank;
	public GameObject gameoverMark;

	class Resource
	{
		Tetris  m_tetris;

		GameObject[] m_blocks;
		GameObject[] m_minoBlocks;
		GameObject   m_gameoverMark;

		Vector2 m_offset;

		public Resource(Tetris tetris, GameObject tetrisBlock, GameObject gameoverMark, Vector2[] blockUvList, Vector2[] blankUvList)
		{
			Stage stage = tetris.getStage();

			m_tetris = tetris;

			int w = stage.getBoardWidth();
			int h = stage.getBoardHeight();

			m_blocks = new GameObject[w*h];

			//	現状では落下ドロップは4個固定なので直値
			m_minoBlocks = new GameObject[4];
			for(int i = 0; i<4; i++)
			{
				m_minoBlocks[i] = Instantiate(tetrisBlock);
				//m_minoBlocks[i].GetComponent<SpriteRenderer>().sortingOrder = LAYER_DROP;
				MeshQuad mesh = m_minoBlocks[i].GetComponent<MeshQuad>();
				mesh.updateUv(blockUvList);
			}

			//	盤面の描画オブジェクトをあらかじめ用意
			for(int i=0; i<w*h; i++)
			{
				m_blocks[i] = Instantiate(tetrisBlock);
				//m_minoBlocks[i].GetComponent<SpriteRenderer>().sortingOrder = LAYER_DROP;
				MeshQuad mesh = m_blocks[i].GetComponent<MeshQuad>();
				mesh.updateUv(blankUvList);
			}

			//	ゲームオーバー表示
			m_gameoverMark = Instantiate(gameoverMark);
			m_gameoverMark.GetComponent<SpriteRenderer>().sortingOrder = LAYER_GAMEOVER;
		}

		public Vector2 offset
		{
			set { m_offset = new Vector2(value.x, value.y); }
			get { return m_offset; }
		}

		public Tetris tetris
		{
			get { return m_tetris; }
		}
		public GameObject[] blocks
		{
			get { return m_blocks; }
		}
		public GameObject[] minoBlocks
		{
			get { return m_minoBlocks; }
		}
		public GameObject gameoverMark
		{
			get	{ return m_gameoverMark; }
		}

		/**
		 *	状態をリセット
		 */
		public void reset(Tetris tetris)
		{
			//	盤面の情報を入れ替える
			m_tetris = tetris;

			//	盤面のオブジェクトは破棄
			int w = m_tetris.getStage().getBoardWidth();
			int h = m_tetris.getStage().getBoardHeight();

			for(int i=0; i<w*h; i++)
			{
				GameObject obj = m_blocks[i];
				if( null != obj )
				{
					GameObject.Destroy(obj);
				}
			}

			foreach(GameObject obj in m_minoBlocks)
			{
				GameObject.Destroy(obj);
			}

			//	非表示
			GameObject.Destroy(m_gameoverMark);
		}


		/**
		 *	削除
		 */
		public void destroyObject()
		{
			//	盤面オブジェクトの破棄
			int w = m_tetris.getStage().getBoardWidth();
			int h = m_tetris.getStage().getBoardHeight();

			for(int i=0; i<w*h; i++)
			{
				GameObject obj = m_blocks[i];
				if(null != obj)
				{
					GameObject.Destroy(obj);
				}
			}

			//	落下ブロックの破棄
			foreach(GameObject obj in m_minoBlocks)
			{
				GameObject.Destroy(obj);
			}

			//	ゲームオーバー表示の破棄
			GameObject.Destroy(m_gameoverMark);
		}
	}


	public GameObject   gameOverText;
	GameObject          m_gameOverText;

	List<Resource>    m_resourcelist;

	const int NUM_OF_HORIZONTAL = 5;	//	横に並べる数

	const int LAYER_BOAR		= 0;
	const int LAYER_DROP		= 1;
	const int LAYER_GAMEOVER	= 2;

	Color[] m_colorList = new Color[]
	{
		new Color(1,1,1,1),

		new Color(1,0.8f,0.2f,1),
		new Color(0.2f,0.2f,1,1),
		new Color(0.2f,1,0.2f,1),
		new Color(1,0.2f,1,1),
		new Color(0.2f,1,1,1),
		new Color(1,0.2f,0.2f,1),
		new Color(1,1,0.2f,1),
	};

	//	ブロックの参照先UV
	//	テクスチャに対する直値なので後々どうかと思うけど、今回はとりあえずやるだけやる
	Vector2[] m_blockUvList = new Vector2[]
	{
		new Vector2(0.25f, 1.0f),
		new Vector2(0.50f, 1.0f),
		new Vector2(0.25f, 0.75f),
		new Vector2(0.50f, 0.75f),
	};
	Vector2[] m_blankUvList = new Vector2[]
	{
		new Vector2(0.50f, 1.0f),
		new Vector2(0.75f, 1.0f),
		new Vector2(0.50f, 0.75f),
		new Vector2(0.75f, 0.75f),
	};


	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		//	盤面の更新があった？
		StageManager stgMngr = GetComponent<StageManager>();
		if(true == stgMngr.isTetrisUpdated())
		{
			if( null != m_resourcelist )
			{
				foreach(Resource res in m_resourcelist)
				{
					res.destroyObject();
				}
			}

			//	リソースを初期化
			initResource();

			//	リソース更新完了を知らせる
			stgMngr.setTetrisUpdated(false);
		}

		if(null != m_resourcelist)
		{
			foreach(Resource res in m_resourcelist)
			{
				Tetris tetris = res.tetris;
				drawBoard(res);
				drawTetrimino(res);

				updateGameOverState(res);
			}
		}
	}

	/**
	 *	描画リソースを初期化
	 */
	public void initResource()
	{
		StageManager stgMngr = GetComponent<StageManager>();

		m_resourcelist = new List<Resource>();

		int board_size_w = Tetris.BOARD_SIZE_W + 1;    //	「+1」は隙間
		int board_size_h = Tetris.BOARD_SIZE_H + 5;    //	落下ブロックと隙間で「+5」
		int index = 0;

		foreach(Tetris tetris in stgMngr.tetrisList)
		{
			Resource res = new Resource(tetris, tetrisBlock, gameoverMark, m_blockUvList, m_blankUvList);
			res.gameoverMark.SetActive(false);

			//	描画のオフセットを設定
			res.offset = new Vector2(board_size_w * (index % NUM_OF_HORIZONTAL), board_size_h * (index / NUM_OF_HORIZONTAL));

			m_resourcelist.Add(res);

			index++;
		}
	}


	void updateGameOverState(Resource res)
	{
		Tetris tetris = res.tetris;

		if(false == tetris.isGameOver()) return;

		Vector2 offset = res.offset;

		res.gameoverMark.SetActive(true);
		Stage stage = res.tetris.getStage();
		res.gameoverMark.transform.position = new Vector3(stage.getBoardWidth()/2+offset.x, stage.getBoardHeight()/2+offset.y, 0);
	}


	/**
	 *	盤面の描画
	 */
	void drawBoard(Resource res)
	{
		Stage stage = res.tetris.getStage();
		int	w = stage.getBoardWidth();
		int h = stage.getBoardHeight();
		int h_margin = stage.getBoardHeightMargin();
		GameObject[] blocks = res.blocks;
		Stage.BlockInfo[,]  board = stage.getBlockInfo();
		Vector2 offset = res.offset;

		for(int y=0; y<h; y++)
		{
			for(int x=0; x<w; x++)
			{
				int index = x + y*w;
				bool isVisible = false;
				/*
				if(null != blocks[index])
				{
					GameObject.Destroy(blocks[index]);
					blocks[index] = null;
				}*/

				GameObject block = blocks[index];
				if(Stage.BLOCK_STATE.EXISTS == board[x, y].state)
				{
					//block = Instantiate(tetrisBlock);
					//block.GetComponent<Renderer>().material.color = m_colorList[board[x,y].color_index];

					MeshQuad mesh = block.GetComponent<MeshQuad>();
					Color[] colors = new Color[]
					{
						m_colorList[board[x,y].color_index],
						m_colorList[board[x,y].color_index],
						m_colorList[board[x,y].color_index],
						m_colorList[board[x,y].color_index],
					};
					mesh.updateUv(m_blockUvList);
					mesh.updateColor(colors);

					//blocks[index] = block;
					isVisible = true;
				}
				else if( Stage.BLOCK_STATE.NONE == board[x, y].state )
				{
					//	ブランクはマージン以外の領域に描画
					if( y<h-h_margin )
					{
						//	block = Instantiate(boardBlank);
						//	blocks[index] = block;
						MeshQuad mesh = block.GetComponent<MeshQuad>();
						Color[] colors = new Color[]
						{
							new Color(1,1,1,1),
							new Color(1,1,1,1),
							new Color(1,1,1,1),
							new Color(1,1,1,1),
						};
						mesh.updateUv(m_blankUvList);
						mesh.updateColor(colors);
						isVisible = true;
					}
					
					//	マージンを超えたブランクは非表示
				}
				if( true == isVisible)
				{
					block.transform.position = new Vector3(x + offset.x, y + offset.y, 0);
					block.SetActive(true);
				}
			}
		}
	}

	/**
	 *	落下ブロックの描画
	 */
	void drawTetrimino(Resource res)
	{
		Tetrimino tetrimino = res.tetris.getTetrimino();
		GameObject[] blocks = res.minoBlocks;

		Tetrimino.Pattern pat = tetrimino.getPattern();

		int num_blocks = 0;
		int index = 0;

		Vector2 offset = res.offset;

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
					float posx = x + base_x + offset.x;
					float posy = y + base_y + offset.y;
					GameObject block = blocks[num_blocks];
					block.transform.position = new Vector3(posx, posy, 0);
					//blk.GetComponent<Renderer>().material.color = m_colorList[tetrimino.getColorIndex()];

					int color_index = tetrimino.getColorIndex();
					MeshQuad mesh = block.GetComponent<MeshQuad>();
					Color[] colors = new Color[]
					{
						m_colorList[color_index],
						m_colorList[color_index],
						m_colorList[color_index],
						m_colorList[color_index],
					};
					mesh.updateUv(m_blockUvList);
					mesh.updateColor(colors);

					num_blocks++;
				}

				index++;
			}
		}
	}

}
