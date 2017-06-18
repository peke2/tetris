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
		bool btn_a = Input.GetButtonDown("Fire1");
		bool btn_b = Input.GetButtonDown("Fire2");
		bool btn_c = Input.GetButtonDown("Fire3");

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

		if(true == btn_a) m_input_bit |= MASK_BUTTON_A;
		if(true == btn_b) m_input_bit |= MASK_BUTTON_B;
		if(true == btn_c) m_input_bit |= MASK_BUTTON_C;

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
