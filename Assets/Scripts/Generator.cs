using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator
{
	int m_min_index;
	int m_max_index;
	const int SEED_VALUE = 45678;
	int[] m_table;

	public Generator(int min_index, int max_index)
	{
		m_min_index = min_index;
		m_max_index = max_index;

		//	512で一周
		initTable(512);
	}

	/**
	 *	テーブルを初期化
	 */
	private void initTable(int size)
	{
		m_table = new int[size];

		//他の箇所でランダムを使う場合を考えると、ここでシードをセットするのはどうなのか？
		Random.InitState(SEED_VALUE);

		for(int i = 0; i<size; i++)
		{
			//m_table[i] = Random.Range(m_min_index, m_max_index+1);
			m_table[i] = i % ((m_max_index+1) - m_min_index) + m_min_index;	//	順番
		}
	}

	/**
	 *	対象のシリアルの生成インデックスを取得 
	 */
	public int getIndex(int serial)
	{
		int index = serial % m_table.Length;
		return	m_table[index];
	}

}
