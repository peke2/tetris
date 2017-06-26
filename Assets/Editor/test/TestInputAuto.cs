using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestInputAuto {

	[Test]
	//	何もセットしなければ入力内容は無し
	public void construct()
	{
		InputAuto input;
		input = new InputAuto();

		List<int> bitList;
		bitList = input.getInputBitList();
		Assert.AreEqual(0, bitList.Count);
	}

	[Test]
	//	初期状態をセットしたらそのまま参照可能
	public void constructWithParam()
	{
		List<int>   list = new List<int>();

		list.Add(5);
		list.Add(13);
		list.Add(7);
		list.Add(1);
		list.Add(8);
		list.Add(2);

		InputAuto input;
		input = new InputAuto(list);

		List<int> bitList;
		bitList = input.getInputBitList();
		Assert.AreEqual(6, bitList.Count);

		Assert.AreEqual(5, bitList[0]);
		Assert.AreEqual(13, bitList[1]);
		Assert.AreEqual(7, bitList[2]);
		Assert.AreEqual(1, bitList[3]);
		Assert.AreEqual(8, bitList[4]);
		Assert.AreEqual(2, bitList[5]);
	}

	[Test]
	//	何もセットしない状態で入力ビットを取得
	public void update()
	{
		InputAuto input;
		input = new InputAuto();

		//	内容は空
		List<int> list;
		list = input.getInputBitList();
		Assert.AreEqual(0, list.Count);

		//	更新をすると内容が追加される
		for(int i=0; i<256; i++)
		{
			input.update();
		}

		list = input.getInputBitList();
		Assert.AreEqual(256, list.Count);

		foreach(int bit in list)
		{
			if(0>bit || 42<bit)
			{
				Assert.Fail("bit out of range=["+bit+"]");
			}
		}
	}

	[Test]
	//	初期パラメータをセットした状態での更新
	public void updateWithInitParam()
	{
		List<int>   list = new List<int>();

		int[] inputBits = new int[]{6,3,2,9,11,1,7};
		int default_count = inputBits.Length;

		foreach(int n in inputBits)
		{
			list.Add(n);
		}

		InputAuto input;
		input = new InputAuto(list);

		List<int> bitList;
		bitList = input.getInputBitList();
		Assert.AreEqual(default_count, bitList.Count);

		int bit;

		for(int i=0; i<default_count; i++)
		{
			input.update();
			bit = input.getButtonBit();
			Assert.AreEqual(inputBits[i], bit);
		}

		//	この時点ではリストは増えていない
		bitList = input.getInputBitList();
		Assert.AreEqual(default_count, bitList.Count);

		//	範囲外のアクセスでリストにデータが追加される
		input.update();
		bit = input.getButtonBit();
		Assert.IsTrue(0<=bit && 42>=bit, "bit=["+bit+"]");

		bitList = input.getInputBitList();
		Assert.AreEqual(default_count+1, bitList.Count);

		//	もう一度
		input.update();
		bit = input.getButtonBit();
		Assert.IsTrue(0<=bit && 42>=bit, "bit=["+bit+"]");

		bitList = input.getInputBitList();
		Assert.AreEqual(default_count+2, bitList.Count);	//	2回目なのでデータは2つ追加された

		//	既存の部分は変わらない
		for(int i=0; i<default_count; i++)
		{
			Assert.AreEqual(inputBits[i], bitList[i]);
		}

	}

}
