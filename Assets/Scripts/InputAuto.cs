using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAuto:InputBase
{
	List<int> m_autoBitList;
	int m_list_index;

	//	入力の組み合わせ
	static int[] m_patterns = new int[]
	{
		0,
		MASK_LEFT,
		MASK_LEFT | MASK_DOWN,
		MASK_LEFT | MASK_BUTTON_A,
		MASK_LEFT | MASK_DOWN | MASK_BUTTON_A,
		MASK_LEFT | MASK_BUTTON_B,
		MASK_LEFT | MASK_DOWN | MASK_BUTTON_B,

		MASK_RIGHT,
		MASK_RIGHT | MASK_DOWN,
		MASK_RIGHT | MASK_BUTTON_A,
		MASK_RIGHT | MASK_DOWN | MASK_BUTTON_A,
		MASK_RIGHT | MASK_BUTTON_B,
		MASK_RIGHT | MASK_DOWN | MASK_BUTTON_B,

		MASK_DOWN,
		MASK_DOWN | MASK_BUTTON_A,
		MASK_DOWN | MASK_BUTTON_B,

		MASK_BUTTON_A,
		MASK_BUTTON_B,
	};


	public InputAuto(List<int> bitList = null)
	{
		initInputBitList();

		if(null != bitList)
		{
			//	指定されたリストを適用
			m_autoBitList = bitList;
		}
	}

	public override void update()
	{
		//	前のフレームの状態を残す
		m_input_bit_prev = m_input_bit;
		//	現在の入力を取得
		m_input_bit = getInputBit();
	}

	/**
	 *	入力ビットを取得
	 */
	int getInputBit()
	{
		int index;
		int bit;

		if(m_list_index < m_autoBitList.Count)
		{
			//	参照可能ならば既存のリストから参照する
			bit = m_autoBitList[m_list_index];
			m_list_index++;
		}
		else
		{
			//	参照先が無ければ
			index = Random.Range(0, m_patterns.Length);
			bit = m_patterns[index];

			//	取得した情報は後で参照できるよう保持
			m_autoBitList.Add(bit);

			//	次の参照はリストの末尾(まだ参照先は無い)
			m_list_index = m_autoBitList.Count;
		}

		return bit;
	}

	/**
	 *	入力ビットリストを初期化
	 */
	public void initInputBitList()
	{
		m_list_index = 0;
		m_autoBitList = new List<int>();
	}

	/**
	 *	入力ビットリストを取得
	 */
	public override List<int> getInputBitList()
	{
		return m_autoBitList;
	}

	/**
	 *	参照先の位置を取得
	 */
	public int getListIndex()
	{
		return m_list_index;
	}


	/**
	 *	入力のパターンを取得する
	 */
	static public int[] getInputPatterns()
	{
		return m_patterns;
	}
}
