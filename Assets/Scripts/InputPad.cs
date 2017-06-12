using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPad : InputBase {


	// Use this for initialization
	public InputPad ()
	{
	}


	public override void update()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		bool a = Input.GetButtonDown("Fire1");

		m_input_bit_prev = m_input_bit;
		m_input_bit = 0;

		if( Mathf.Abs(h) > 0.2f )
		{
			if(h > 0)	m_input_bit |= MASK_RIGHT;
			else		m_input_bit |= MASK_LEFT;
		}

		if(Mathf.Abs(v) > 0.2f)
		{
			if(v > 0)	m_input_bit |= MASK_UP;
			else		m_input_bit |= MASK_DOWN;
		}

		if(true == a) m_input_bit |= MASK_BUTTON_A;

	}

	public override int getButtonBit()
	{
		return m_input_bit;
	}

	public override int getButtonEdgeBit()
	{
		return (m_input_bit ^ m_input_bit_prev) & m_input_bit;
	}
}
