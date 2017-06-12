using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino{

	public struct Pattern
	{
		int		m_w, m_h;
		string	m_pat;

		public Pattern(int w, int h, string pat)
		{
			m_w = w;
			m_h = h;
			m_pat = pat;
		}

		public int w { get { return m_w; } }
		public int h { get { return m_h; } }
		public string pat { get { return m_pat; } }
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
	
	int m_type_id;

	public Tetrimino ()
	{
		m_pattern_id = 0;
		setPos(0, 0);
	}
	
	public int getTypeMax()
	{
		return m_typeTable.Length-1;
	}

	public void setTypeId(int id)
	{
		Debug.Assert(0<=id && getTypeMax()>=id);

		m_type_id = id;

		//	外部から直接パターンIDを指定すると齟齬が生じるが、現状ではそこまで厳密にしない
		setPatternId(m_typeTable[m_type_id].pattern_id);
	}


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

}
