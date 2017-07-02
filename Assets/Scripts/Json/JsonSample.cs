using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JsonSample : MonoBehaviour {

	//[Serializable]	ドキュメントでは使えていたが、実際には使えない(5.6だから？)
	class MyData
	{
		public int m_index;
		public string m_name;
		public List<List<int>> m_list;	//	ListのListは無理だった
		public List<string> m_greeting;
		public float[] m_numbers;

		public MyData()
		{
			m_index = 91;
			m_name = "Json変換 テスト";
			m_list = new List<List<int>>();

			List<int> l = new List<int>();
			l.Add(3);
			l.Add(7);
			l.Add(11);
			m_list.Add(l);
		/*
			l.Clear();
			l.Add(13);
			l.Add(17);
			l.Add(19);
			l.Add(23);
			m_list.Add(l);
			*/

			m_greeting = new List<string>();
			m_greeting.Add("Good morning");
			m_greeting.Add("Hello");

			m_numbers = new float[]
			{
				120.3f, 456.657f, 0.123f
			};
		}
	}

	class DataStruct
	{
		public int m_serial;
		public List<int> m_list;
		public DataStruct()
		{
			m_list = new List<int>();
			m_list.Add(453);
			m_list.Add(37);
			m_list.Add(43);

			m_serial = 179;
		}
	}

	class DataTable
	{
		public int m_num_structs;
		public List<DataStruct> m_list;	//	内容を new で作らなければいけないプロパティは出力できない？
										//	リスト、配列自体は大丈夫そうな感じ

		public DataTable()
		{
			m_list = new List<DataStruct>();

			DataStruct ds;
			ds = new DataStruct();
			m_list.Add(ds);

			m_num_structs = 1;
		}
	}


	// Use this for initialization
	void Start () {
		MyData data = new MyData();
		string json = JsonUtility.ToJson(data);
		Debug.Log(json);

		DataTable table = new DataTable();
		json = JsonUtility.ToJson(table);
		Debug.Log(json);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
