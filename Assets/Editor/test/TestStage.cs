using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestStage {

	[Test]
	public void 	placeBlocks()
	{
		Stage stg = new Stage(6,10);

		int block_w = 6;
		int block_h = 10;
		Stage.BlockInfo[] area = new Stage.BlockInfo[block_w*block_h];

		int index;

		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				index = x + y * block_w;
				area[index] = new Stage.BlockInfo();
				area[index].state = Stage.BLOCK_STATE.EXISTS;
				area[index].color_index = index;	//	確認用にインデックスをそのまま入れておく
			}
		}

		Stage.BlockInfo[,] stgBinfo = stg.getBlockInfo();
		int blank_count = 0;

		//[todo]	このループ何とかする
		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				if( (stgBinfo[x,y].state == Stage.BLOCK_STATE.NONE) && (stgBinfo[x,y].color_index == 0) )
				{
					blank_count++;
				}
			}
		}

		Assert.AreEqual(block_w*block_h, blank_count);

		stg.placeBlocks(0,0,area, 6,10);

		int exists_count = 0;
		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				index = x + y * block_w;
				if( (stgBinfo[x,y].state == Stage.BLOCK_STATE.EXISTS) && (stgBinfo[x,y].color_index == index) )
				{
					exists_count++;
				}
			}
		}

		Assert.AreEqual(block_w*block_h, exists_count);

	}

	[Test]
	public void 	getCompletedLines()
	{
		Stage stg = new Stage(6,10);

		int block_w = 6;
		int block_h = 10;
		Stage.BlockInfo[] area = new Stage.BlockInfo[block_w*block_h];

		int index;

		int[] blocks = new int[]{
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
		};

		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				index = x + y * block_w;
				area[index] = new Stage.BlockInfo();

				if( 1 == blocks[index] )
				{
					area[index].state = Stage.BLOCK_STATE.EXISTS;
					area[index].color_index = 3;	//	とりあえず適当な色
				}
				else
				{
					area[index].state = Stage.BLOCK_STATE.NONE;
					area[index].color_index = 0;
				}
			}
		}
		/*
		Stage.BlockInfo[,] stgBinfo = stg.getBlockInfo();
		int blank_count = 0;

		//[todo]	このループ何とかする
		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				if( (stgBinfo[x,y].state == Stage.BLOCK_STATE.NONE) && (stgBinfo[x,y].color_index == 0) )
				{
					blank_count++;
				}
			}
		}

		Assert.AreEqual(block_w*block_h, blank_count);

		stg.placeBlocks(0,0,area, 6,10);

		int exists_count = 0;
		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				index = x + y * block_w;
				if( (stgBinfo[x,y].state == Stage.BLOCK_STATE.EXISTS) && (stgBinfo[x,y].color_index == index) )
				{
					exists_count++;
				}
			}
		}

		Assert.AreEqual(block_w*block_h, exists_count);
	*/
	}



	/*
	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator NewEditModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
	*/
}
