using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAuto : InputBase
{
	int[]   m_auto_bit_array;
	int     m_array_index;

	public InputAuto(int[] bitarray=null)
	{
		if(null == bitarray)
		{
			initBitStream();
		}
		else
		{
			m_auto_bit_array = bitarray;
		}
	}

	void initBitStream()
	{
		int total_frames = 60*60*5; //[todo] 入力データのサイズは要見直し → 現状5分
		m_auto_bit_array = new int[total_frames];

		//	入力の組み合わせ
		int[] patterns = new int[]
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

		//[todo] パターンは外部から与えるか内部で管理するか決める
		//入力に依存したものなので、インスタンス作成後に取り出して外部から使えば良い？
		//ランダムはシードとか気にしないでおく
		for(int i=0; i<total_frames; i++)
		{
			int index = Random.Range(0, patterns.Length);
			m_auto_bit_array[i] = patterns[index];
		}
	}


	public override void update()
	{
		//	前のフレームの状態を残す
		m_input_bit_prev = m_input_bit;

		if(m_array_index >= m_auto_bit_array.Length)
		{
			m_input_bit = 0;
		}
		else
		{
			m_input_bit = m_auto_bit_array[m_array_index];
			m_array_index++;
		}
	}
}
