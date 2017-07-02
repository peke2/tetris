using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestGenerationManager
{

	[Test]
	//	何もセットしなければ出力は無し
	public void calcCrossoverEmptyInput()
	{
		List<int> a, b;
		a = new List<int>();
		b = new List<int>();

		List<List<int>> list;
		list = GenerationManager.calcCrossover(a, b, 1);
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual(0, list[0].Count);
		Assert.AreEqual(0, list[1].Count);
	}

	[Test]
	//	同じサイズのリストの交叉
	public void calcCrossoverSameSize()
	{
		List<int> a, b;
		a = new List<int>();
		b = new List<int>();

		for(int i = 0; i<10; i++)
		{
			a.Add(i);
			b.Add(10+i);
		}


		List<List<int>> list;
		list = GenerationManager.calcCrossover(a, b, 1);
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual(10, list[0].Count);
		Assert.AreEqual(10, list[1].Count);

		int[] expectedA = new int[]{ 0,11, 2,13, 4,15, 6,17, 8,19};
		int[] expectedB = new int[]{10, 1,12, 3,14, 5,16, 7,18, 9};
		for(int i = 0; i<10; i++)
		{
			Assert.AreEqual(expectedA[i], list[0][i]);
			Assert.AreEqual(expectedB[i], list[1][i]);
		}
	}

	[Test]
	//	同じサイズのリストの交叉(交叉幅を複数に)
	public void calcCrossoverSameSizeMultiBound()
	{
		List<int> a, b;
		a = new List<int>();
		b = new List<int>();

		for(int i = 0; i<10; i++)
		{
			a.Add(i);
			b.Add(10+i);
		}


		List<List<int>> list;
		list = GenerationManager.calcCrossover(a, b, 3);
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual(10, list[0].Count);
		Assert.AreEqual(10, list[1].Count);

		int[] expectedA = new int[]{ 0, 1, 2,13,14,15, 6, 7, 8,19};
		int[] expectedB = new int[]{10,11,12, 3, 4, 5,16,17,18, 9};
		for(int i = 0; i<10; i++)
		{
			Assert.AreEqual(expectedA[i], list[0][i]);
			Assert.AreEqual(expectedB[i], list[1][i]);
		}
	}

	[Test]
	//	異なるサイズのリストの交叉(交叉幅を複数に)
	public void calcCrossoverDifferentSizeMultiBound()
	{
		List<int> a, b;
		a = new List<int>();
		b = new List<int>();

		for(int i = 0; i<10; i++)
		{
			a.Add(i);
		}
		for(int i = 0; i<16; i++)
		{
			b.Add(10+i);
		}

		List<List<int>> list;
		list = GenerationManager.calcCrossover(a, b, 3);
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual(10, list[0].Count);
		Assert.AreEqual(16, list[1].Count);

		//	サイズが違っていた場合、あふれた分は交叉対象にならない
		int[] expectedA = new int[]{ 0, 1, 2,13,14,15, 6, 7, 8,19};
		int[] expectedB = new int[]{10,11,12, 3, 4, 5,16,17,18, 9,20,21,22,23,24,25};
		for(int i = 0; i<10; i++)
		{
			Assert.AreEqual(expectedA[i], list[0][i]);
		}
		for(int i = 0; i<16; i++)
		{
			Assert.AreEqual(expectedB[i], list[1][i]);
		}
	}

	[Test]
	//	突然変異無し
	public void mutationNoChange()
	{
		List<int> listA = new List<int>();
		for(int i = 0; i<100; i++)
		{
			listA.Add(i); ;
		}

		List<int> resultList;
		resultList = GenerationManager.mutation(listA, 0);

		for(int i = 0; i<100; i++)
		{
			Assert.AreEqual(listA[i], resultList[i]);
		}
	}

	[Test]
	//	突然変異
	public void mutation()
	{
		List<int> listA = new List<int>();
		for(int i = 0; i<100; i++)
		{
			listA.Add(i); ;
		}

		List<int> resultList;
		resultList = GenerationManager.mutation(listA, 10);

		int changed_count = 0;
		for(int i = 0; i<100; i++)
		{
			if(listA[i] != resultList[i])
			{
				changed_count++;
			}
		}

		//	多くても指定確率分の回数、少なくても1回は変更が行われる
		Assert.IsTrue(changed_count>0 && changed_count<=10, "変更回数="+changed_count);
	}

	[Test]
	public void createCombination()
	{
		List<int> list;
		List<List<int>> combList;
		list = new List<int>();

		list.Add(1);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		combList = GenerationManager.createCombination(list, 2);

		Assert.AreEqual(6, combList.Count);

		list.Clear();
		for(int i = 1; i<20; i++)
		{
			list.Add(i);
		}
		combList = GenerationManager.createCombination(list, 2);

		Assert.AreEqual(171, combList.Count);

	}

	[Test]
	public void createCombinationLess()
	{
		List<int> list;
		List<List<int>> combList;
		list = new List<int>();

		//	組み合わせる数が要素よりも大きい場合も正常に動く
		list.Add(1);
		list.Add(2);
		combList = GenerationManager.createCombination(list, 3);

		Assert.AreEqual(0, combList.Count);
	}


	[Test]
	public void selectRandom()
	{
		List<int> selected;
		List<int> list = new List<int>();

		for(int i=0; i<10; i++)
		{
			list.Add(i);
		}
		//	選択された要素は指定数のみ
		selected = GenerationManager.selectRandom(list, 3);
		Assert.AreEqual(3, selected.Count);

		//	重複していない
		Assert.IsTrue( (selected[0]!=selected[1]) && (selected[1]!=selected[2]) && (selected[2]!=selected[0]));
	}

	[Test]
	public void selectRandomOver()
	{
		List<int> selected;
		List<int> list = new List<int>();

		for(int i = 0; i<10; i++)
		{
			list.Add(i);
		}
		selected = GenerationManager.selectRandom(list, 15);

		//	参照元を超えて選択されない
		Assert.AreEqual(10, selected.Count);
	}
}
