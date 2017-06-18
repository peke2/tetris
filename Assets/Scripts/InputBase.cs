using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBase {

	public const int BIT_UP = 0;
	public const int BIT_DOWN = 1;
	public const int BIT_LEFT = 2;
	public const int BIT_RIGHT = 3;
	public const int BIT_BUTTON_A = 4;
	public const int BIT_BUTTON_B = 5;
	public const int BIT_BUTTON_C = 6;

	public const int MASK_UP = (1<<BIT_UP);
	public const int MASK_DOWN = (1<<BIT_DOWN);
	public const int MASK_LEFT = (1<<BIT_LEFT);
	public const int MASK_RIGHT = (1<<BIT_RIGHT);
	public const int MASK_BUTTON_A = (1<<BIT_BUTTON_A);
	public const int MASK_BUTTON_B = (1<<BIT_BUTTON_B);
	public const int MASK_BUTTON_C = (1<<BIT_BUTTON_C);

	protected int m_input_bit;
	protected int m_input_bit_prev;

	// Use this for initialization
	public InputBase ()
	{
		m_input_bit = 0;
		m_input_bit_prev = 0;
	}


	public virtual void update()
	{

	}

	public virtual int getButtonBit()
	{
		return 0;
	}

	public virtual int getButtonEdgeBit()
	{
		return 0;
	}

}
