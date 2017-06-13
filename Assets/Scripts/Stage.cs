using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage {

	public enum BLOCK_STATE
	{
		NONE = 0,
		EXISTS,
	}

	public class BlockInfo
	{
		BLOCK_STATE m_state;
		int m_color_index;

		public BLOCK_STATE state
		{
			get { return m_state; }
			set { m_state = value; }
		}
		public int color_index
		{
			get { return m_color_index; }
			set { m_color_index = value; }
		}
	}

	//[todo]	盤面の管理方法をリストで行うか検討する
	//盤面は配列よりもリストで管理した方が、ライン消去時に高速で処理できる？
	BlockInfo[,] m_board;
	int m_board_w, m_board_h;
	int m_standby_h;	//	落下ブロックの待機分高さ割り増し

	public Stage(int w, int h)
	{
		m_board = new BlockInfo[w, h];
		for(int y = 0; y<h; y++)
		{
			for(int x = 0; x<w; x++)
			{
				BlockInfo binfo = new BlockInfo();
				binfo.state = BLOCK_STATE.NONE;
				//binfo.state = BLOCK_STATE.EXISTS;
				binfo.color_index = 0;
				m_board[x, y] = binfo;

			}
		}
		m_board_w = w;
		m_board_h = h;

		m_standby_h = 4;	//	とりあえずブロックの最大高さを直接指定
	}

	/**
	 *	完成した行の番号を取得 
	 */
	public List<int> getCompletedLines()
	{
		List<int>   list = new List<int>();


		for(int y = 0; y<m_board_h; y++)
		{
			bool    completed = true;
			for(int x = 0; x<m_board_w; x++)
			{
				if( BLOCK_STATE.NONE == m_board[x,y].state )
				{
					completed = false;
					break;
				}
			}

			if( true == completed )
			{
				list.Add(y);
			}
		}

		return list;
	}

	/**
	 *	指定された行を消す
	 */
	public void eraseLines(List<int> list)
	{
		//	ブロックを詰めるため上のラインから消していく
		list.Sort((a, b) => a-b);

		//	念のため同じ行は除く
		IEnumerable<int> eraseList = list.Distinct();

		int last_line_index = m_board_h-1;	//	最終ライン

		foreach(int index in eraseList)
		{
			for(int y=index; y<last_line_index; y++)	//	最後のラインの1つ手前まで
			{
				for(int x=0; x<m_board_w; x++)
				{
					BlockInfo   src, dst;
					src = m_board[x, y+1];
					dst = m_board[x, y];
					//	1つ上の行をコピー
					dst.state = src.state;
					dst.color_index = src.color_index;
				}
			}
			//	最終行をクリア
			for(int x=0; x<m_board_w; x++)
			{
				BlockInfo   info = m_board[x, last_line_index];
				info.state = BLOCK_STATE.NONE;
				info.color_index = 0;
			}

			//	1ライン消えるので、最終ラインも1つ下げる
			last_line_index--;
			
			//[todo]	最終ラインはあらかじめ保持しておく
			//毎回盤面全体の更新になるので、移動対象の最終ラインをあらかじめ保持しておくと良いかも
		}
	}

	/**
	 *	指定された位置にブロックはあるか？
	 */
	public bool existsBlock(int x, int y)
	{
		//	盤面に入る前のエリアではブロックの判定はしない
		if(true == isStandbyArea(x, y)) return false;
		
		//	盤面の範囲内でなければブロックが全部埋まっているイメージ
		if(false == isInside(x, y)) return true;

		//	盤面の指定位置にブロックがあるか？？
		if(m_board[x, y].state == BLOCK_STATE.EXISTS) return true;

		return false;
	}

	/**
	 *	指定された位置にブロックはあるか？
	 */
	public bool existsBlockOnArea(int base_x, int base_y, int[] area, int w, int h)
	{
		for(int y=0; y<h; y++)
		{
			for(int x=0; x<w; x++)
			{
				if( 0!=area[x+y*w] )
				{
					//	1つでもブロックがあれば終了
					if(true == existsBlock(base_x + x, base_y + y)) return true;
				}
			}
		}

		return false;
	}


	/**
	 *	指定された位置にブロックを配置する
	 */
	public void placeBlocks(int base_x, int base_y, int[] area, int w, int h)
	{
		for(int y = 0; y<h; y++)
		{
			for(int x = 0; x<w; x++)
			{
				int bx = base_x + x;
				int by = base_y + y;
				if( (0!=area[x+y*w]) && (true == isInside(bx,by)) )
				{
					m_board[bx, by].state = BLOCK_STATE.EXISTS;
				}
			}
		}
	}

	/**
	 *	
	 */
	//最終的にはこれに置き換える(ブロックの色指定のため)
	public void placeBlocks(int base_x, int base_y, BlockInfo[] area, int w, int h)
	{
		for(int y = 0; y<h; y++)
		{
			for(int x = 0; x<w; x++)
			{
				int bx = base_x + x;
				int by = base_y + y;
				BlockInfo binfo = area[x+y*w];
				if( (BLOCK_STATE.EXISTS==binfo.state) && (true == isInside(bx,by)) )
				{
					m_board[bx, by].state = binfo.state;
					m_board[bx, by].color_index = binfo.color_index;
				}
			}
		}
	}

	/**
	 *	盤面の範囲内の座標か？	
	 */
	public bool isInside(int x, int y)
	{
		if( (0<=x && m_board_w>x)
		 && (0<=y && m_board_h>y) ) return true;

		return false;
	}

	/**
	 *	待機領域内の座標か？
	 */
	public bool isStandbyArea(int x, int y)
	{
		if((0<=x && m_board_w>x)
		 && (m_board_h<=y && m_board_h+m_standby_h>y)) return true;

		return false;
	}

	/**
	 *	ブロックの状態を取得
	 */
	public BlockInfo[,] getBlockInfo()
	{
		return m_board;
	}

	/**
	 *	盤面のサイズを取得
	 */
	public int getBoardWidth()
	{
		return m_board_w;
	}
	public int getBoardHeight()
	{
		return m_board_h;
	}
}
