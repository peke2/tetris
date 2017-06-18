using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino{

	public class Pattern
	{
		int		m_w, m_h;
		string	m_pat;

		public struct Rect
		{
			public int x0, y0;
			public int x1, y1;
		}
		
		Rect m_rect;

		public Pattern(int w, int h, string pat)
		{
			m_w = w;
			m_h = h;
			m_pat = pat;

			calcRect();
		}

		public int w { get { return m_w; } }
		public int h { get { return m_h; } }
		public string pat { get { return m_pat; } }
		public Rect rect { get { return m_rect; } }


		private void calcRect()
		{
			int min_x, min_y;
			int max_x, max_y;

			min_x = int.MaxValue;
			min_y = int.MaxValue;
			max_x = 0;
			max_y = 0;

			for(int y=0; y<m_h; y++)
			{
				for(int x=0; x<m_w; x++)
				{
					int index = x + y * m_w;
					if( '1' == m_pat[index] )
					{
						if(x < min_x) min_x = x;
						else if(x > max_x) max_x = x;
						if(y < min_y) min_y = y;
						else if(y > max_y) max_y = y;
					}
				}
			}

			m_rect.x0 = min_x;
			m_rect.y0 = min_y;
			m_rect.x1 = max_x;
			m_rect.y1 = max_y;
		}

	}

	int m_pattern_id;

	int m_pos_x, m_pos_y;

	public void setPos(int x, int y)
	{
		m_pos_x = x;
		m_pos_y = y;
	}

	public int getPosX() { return m_pos_x; }
	public int getPosY() { return m_pos_y; }

	//	描画時に「下から順番」に描画されるので、実際に表示される際のパターンは上下逆になる
	//	データの判定を考える際に注意！
	Pattern[] patternTable = new Pattern[]{
		//--------
		new Pattern(3,3,"010010011"),
		new Pattern(3,3,"001111000"),
		new Pattern(3,3,"110010010"),
		new Pattern(3,3,"000111100"),
		
		//--------
		new Pattern(3,3,"010010110"),
		new Pattern(3,3,"000111001"),
		new Pattern(3,3,"011010010"),
		new Pattern(3,3,"000100111"),
		
		//--------
		new Pattern(3,3,"110011000"),
		new Pattern(3,3,"010110100"),
		new Pattern(3,3,"000110011"),
		new Pattern(3,3,"001011010"),
		
		//--------
		new Pattern(3,3,"011110000"),
		new Pattern(3,3,"100110010"),
		new Pattern(3,3,"000011110"),
		new Pattern(3,3,"010011001"),
		
		//--------
		new Pattern(3,3,"010111000"),
		new Pattern(3,3,"010110010"),
		new Pattern(3,3,"000111010"),
		new Pattern(3,3,"010011010"),
		
		//--------
		new Pattern(4,4,"0100010001000100"),
		new Pattern(4,4,"0000000011110000"),
		new Pattern(4,4,"0010001000100010"),
		new Pattern(4,4,"0000111100000000"),
		
		//--------
		new Pattern(2,2,"1111"),
	};

	//	種類の定義
	struct Type
	{
		int m_pattern_id;
		int m_num_patterns;
	
		public Type(int id, int num)
		{
			m_pattern_id = id;
			m_num_patterns = num;
		}

		public int pattern_id { get { return m_pattern_id; } }
		public int num_patterns { get { return m_num_patterns; } }
	}

	Type[] m_typeTable = new Type[]
	{
		new Type(0, 4),
		new Type(4, 4),
		new Type(8, 4),
		new Type(12, 4),
		new Type(16, 4),
		new Type(20, 4),
		new Type(24, 1),
	};
	
	int m_type_id;		//	パターンを種類で分類したときのID
	int m_type_offset;	//	タイプ内のオフセット

	public Tetrimino ()
	{
		m_pattern_id = 0;
		setPos(0, 0);
	}
	
	public int getTypeMax()
	{
		return m_typeTable.Length-1;
	}

	public int getTypeId()
	{
		return m_type_id;
	}

	public int getColorIndex()
	{
		//	色のインデックスは「0」が無色
		//	パターンIDの開始とは異なるので関数を経由して取得するようにする
		return m_type_id+1;
	}

	/**
	 *	種類のIDをセット
	 *	(パターンを種類として分類されたものを指定)
	 */
	public void setTypeId(int id)
	{
		Debug.Assert(0<=id && getTypeMax()>=id);

		m_type_id = id;
		m_type_offset = 0;	//	パターンがセットされたら初期状態にする

		//	外部から直接パターンIDを指定すると齟齬が生じるが、現状ではそこまで厳密にしない
		setPatternId(m_typeTable[m_type_id].pattern_id);
	}


	/**
	 *	種類のオフセットを加算(回転)
	 */
	public void addTypeOffset(int addition)
	{
		m_type_offset += addition;

		if( addition >= 0 )
		{
			if(m_typeTable[m_type_id].num_patterns <= m_type_offset)
			{
				m_type_offset = 0;
			}
		}
		else
		{
			if(0 > m_type_offset)
			{
				m_type_offset = m_typeTable[m_type_id].num_patterns - 1;
			}
		}

		setPatternId(m_typeTable[m_type_id].pattern_id + m_type_offset);
	}


	/**
	 *	パターンIDをセット
	 *	ブロックのパターンをIDで指定
	 */
	public void setPatternId(int id)
	{
		int max_id = getMaxPatternId();
		if(0 > id) id = 0;
		else if(max_id < id) id = max_id;

		m_pattern_id = id;
	}

	public int getPatternId()
	{
		return m_pattern_id;
	}

	public int getMaxPatternId()
	{
		int max_id = patternTable.Length-1;
		if(0 > max_id) max_id = 0;
		return max_id;
	}

	public Pattern getPattern()
	{
		return patternTable[m_pattern_id];
	}



	public struct Pos
	{
		public int x;
		public int y;

		public Pos(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	/**
	 *	パターン配置チェック用のオフセットを計算
	 */
	static public List<Pos> calcOffset(Pattern beforePat, Pattern afterPat)
	{
		Pattern.Rect beforeRect, afterRect;
		beforeRect = beforePat.rect;
		afterRect = afterPat.rect;

		List<Pos> offsetList = new List<Pos>();

		//	前の状態から広がる場合にずらしてチェックする
		if(afterRect.x0 < beforeRect.x0)
		{
			int cnt = Mathf.Abs(afterRect.x0 - beforeRect.x0);
			for(int i=1; i<=cnt; i++)
			{
				offsetList.Add(new Pos(i, 0));
			}
		}
		if(afterRect.x1 > beforeRect.x1)
		{
			int cnt = afterRect.x1 - beforeRect.x1;
			for(int i=1; i<=cnt; i++)
			{
				offsetList.Add(new Pos(-i, 0));
			}
		}

		if(afterRect.y0 < beforeRect.y0)
		{
			int cnt = Mathf.Abs(afterRect.y0 - beforeRect.y0);
			for(int i=1; i<=cnt; i++)
			{
				offsetList.Add(new Pos(0, i));
			}
		}
		if(afterRect.y1 > beforeRect.y1)
		{
			int cnt = afterRect.y1 - beforeRect.y1;
			for(int i=1; i<=cnt; i++)
			{
				offsetList.Add(new Pos(0, -i));
			}
		}

		return offsetList;
	}

}
