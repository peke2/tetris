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


	delegate int GetInputPattern();

	GetInputPattern m_ControlProc;
	GetInputPattern m_ButtonProc;

	public InputAuto(List<int> bitList = null)
	{
		initInputBitList();

		if(null != bitList)
		{
			//	指定されたリストを適用
			m_autoBitList = bitList;
		}

		//	入力のタイプを組み合わせる
		int index;
		GetInputPattern[] controls = new GetInputPattern[]
		{
			getInputPatternLeft,
			getInputPatternRight,
			getInputPatternLess,
		};
		GetInputPattern[] buttons = new GetInputPattern[]
		{
            /*
			getInputPatternButtonA,
			getInputPatternButtonB,
			getInputPatternButtonLess,
            */
            getInputPatternButtonLess,
        };
		index = Random.Range(0, 3);
		m_ControlProc = controls[index];

		//index = Random.Range(0, 3);
        index = 0;
        m_ButtonProc = buttons[index];
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
			//	参照先が無ければランダムで入力を作成する
			//	index = Random.Range(0, m_patterns.Length);
			//	bit = m_patterns[index];
			bit = m_ControlProc();
			bit |= m_ButtonProc();

			//	取得した情報は後で参照できるよう保持
			m_autoBitList.Add(bit);

			//	次の参照はリストの末尾(まだ参照先は無い)
			m_list_index = m_autoBitList.Count;
		}

		return bit;
	}

    public override int getButtonEdgeBit()
    {
        return m_input_bit;
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

	/**
	 *	1～100の間で数を取得
	 */
	int getRate()
	{
		return Random.Range(1, 100+1);
	}

	//	左移動多め
	int getInputPatternLeft()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 10)
		{
			bit = MASK_LEFT;
		}
		else if(rate <= 15)
		{
			bit = MASK_RIGHT;
		}
		else
		{
			bit = 0;
		}

		return bit;
	}

	//	右移動多め
	int getInputPatternRight()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 10)
		{
			bit = MASK_RIGHT;
		}
		else if(rate <= 15)
		{
			bit = MASK_LEFT;
		}
		else
		{
			bit = 0;
		}

		return bit;
	}

	//	移動少なめ
	int getInputPatternLess()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 96)
		{
			bit = 0;
		}
		else if(rate <= 98)
		{
			bit = MASK_LEFT;
		}
		else
		{
			bit = MASK_RIGHT;
		}

		return bit;
	}

	//	回転A多め
	int getInputPatternButtonA()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 10)
		{
			bit = MASK_BUTTON_A;
		}
		else if(rate <= 15)
		{
			bit = MASK_BUTTON_B;
		}
		else
		{
			bit = 0;
		}

		return bit;
	}

	//	回転B多め
	int getInputPatternButtonB()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 10)
		{
			bit = MASK_BUTTON_B;
		}
		else if(rate <= 15)
		{
			bit = MASK_BUTTON_A;
		}
		else
		{
			bit = 0;
		}

		return bit;
	}

	//	回転少な目
	int getInputPatternButtonLess()
	{
		int rate = getRate();
		int bit = 0;

		if(rate <= 96)
		{
			bit = 0;
		}
		else if(rate <= 98)
		{
			bit = MASK_BUTTON_A;
		}
		else
		{
			bit = MASK_BUTTON_B;
		}

		return bit;
	}
}
