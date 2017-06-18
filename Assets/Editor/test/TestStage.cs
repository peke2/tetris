using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

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


	/**
	 *	テスト用のブロックを配置
	 */
	void setTestBlocks(int w, int h, int color, Stage stg, int[] blocks)
	{
		//	配置指定でブロックが無い場合は、配置先を変更しないので別途盤面をクリアしておく
		stg.clearAllBlocks();

		Stage.BlockInfo[] area = new Stage.BlockInfo[w * h];

		for(int y=0; y<h; y++)
		{
			for(int x=0; x<w; x++)
			{
				int index;
				index = x + y * w;
				area[index] = new Stage.BlockInfo();

				if(1 == blocks[index])
				{
					area[index].state = Stage.BLOCK_STATE.EXISTS;
					area[index].color_index = color;
				}
				else
				{
					area[index].state = Stage.BLOCK_STATE.NONE;
					area[index].color_index = 0;
				}
			}
		}

		//	指定されたブロックの領域を配置
		stg.placeBlocks(0, 0, area, w, h);
	}


	[Test]
	public void 	getCompletedLines()
	{
		Stage stg = new Stage(6,10);

		int block_w = 6;
		int block_h = 10;
		Stage.BlockInfo[] area;

		int[] blocks = new int[]{
			1,1,0,0,1,0,
			1,0,0,0,0,0,
			0,1,0,0,0,0,
			0,0,1,0,0,0,
			0,0,0,1,0,0,
			0,0,0,0,1,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			1,0,0,1,1,1,
		};

		setTestBlocks(block_w, block_h, 3, stg, blocks);

		List<int> completedLines;
		completedLines = stg.getCompletedLines();
		//	完了したライン無し
		Assert.AreEqual(0, completedLines.Count);

		blocks = new int[]{
			//	内部での配置は左下(0,0)を起点に行われる
			//	(指定したデータは上下逆になる)
			1,0,1,1,1,1,
			1,1,1,1,1,1,	//	完成したライン
			0,1,0,0,0,0,
			0,0,1,0,0,0,
			1,1,1,1,1,1,	//	完成したライン
			0,0,0,0,1,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			1,1,1,1,1,1,	//	完成したライン
		};

		int block_color = 3;
		setTestBlocks(block_w, block_h, block_color, stg, blocks);

		completedLines = stg.getCompletedLines();

		Assert.AreEqual(3, completedLines.Count);

		int[] indices = new int[]{1,4,9};
		int i=0;
		foreach(int n in completedLines)
		{
			Assert.AreEqual(indices[i], n);
			i++;
		}

	}


	/**
	 *	指定されたブロックの状態と盤面を比較
	 */
	//	比較するブロックの色は、ひとまず1色のみにしておく
	//	「ブランク=0」それ以外は指定の色とみなす
	void compareBlocks(int block_w, int block_h, int[] blocks, int block_color, Stage.BlockInfo[,] binfos)
	{
		for(int y=0; y<block_h; y++)
		{
			for(int x=0; x<block_w; x++)
			{
				//	消えたラインの分、一番上のラインは何もなくなる
				Stage.BlockInfo binfo = binfos[x, y];
				int block = blocks[x+y*block_w];

				Assert.IsTrue(((block==1) && (binfo.state==Stage.BLOCK_STATE.EXISTS))
				 || ((block==0) && (binfo.state==Stage.BLOCK_STATE.NONE)), "["+x+","+y+"] is not match"
				);
				Assert.IsTrue(((block==1) && (binfo.color_index==block_color))
				 || ((block==0) && (binfo.color_index==0)), "["+x+","+y+"] is not match"
				);
			}
		}

	}


	[Test]
	public void eraseLines()
	{
		Stage stg = new Stage(6,10);

		int block_w = 6;
		int block_h = 10;
		Stage.BlockInfo[] area;
		int[] blocks;

		blocks = new int[]{
			//	内部での配置は左下(0,0)を起点に行われる
			//	(指定したデータは上下逆になる)
			1,0,1,1,1,1,
			1,1,1,1,1,1,	//	完成したライン
			0,1,0,0,0,0,
			0,0,1,0,0,0,
			1,1,1,1,1,1,	//	完成したライン
			0,0,0,0,1,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			1,1,1,1,1,1,	//	完成したライン
		};

		int block_color = 3;
		setTestBlocks(block_w, block_h, block_color, stg, blocks);

		List<int> eraseLines = new List<int>();

		//	1行消去(最終行)
		eraseLines.Add(9);
		stg.eraseLines(eraseLines);

		Stage.BlockInfo[,] binfos = stg.getBlockInfo();
		Stage.BlockInfo binfo;

		int[] expected_blocks = new int[]{
			1,0,1,1,1,1,
			1,1,1,1,1,1,
			0,1,0,0,0,0,
			0,0,1,0,0,0,
			1,1,1,1,1,1,
			0,0,0,0,1,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			0,0,0,0,0,0,
		};
		compareBlocks(block_w, block_h, expected_blocks, block_color, binfos);

		//	複数行の消去
		eraseLines.Clear();
		eraseLines.Add(0);
		eraseLines.Add(2);
		stg.eraseLines(eraseLines);
		expected_blocks = new int[]{
			1,1,1,1,1,1,
			0,0,1,0,0,0,
			1,1,1,1,1,1,
			0,0,0,0,1,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
		};
		compareBlocks(block_w, block_h, expected_blocks, block_color, binfos);

		//	複数行の消去
		eraseLines.Clear();
		eraseLines.Add(0);
		eraseLines.Add(2);
		eraseLines.Add(3);
		stg.eraseLines(eraseLines);
		expected_blocks = new int[]{
			0,0,1,0,0,0,
			0,0,0,0,0,1,
			1,0,1,0,1,0,
			0,1,0,1,0,1,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
			0,0,0,0,0,0,
		};
		compareBlocks(block_w, block_h, expected_blocks, block_color, binfos);
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
