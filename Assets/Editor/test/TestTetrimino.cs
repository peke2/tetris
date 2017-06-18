using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestTetrimino
{
	[Test]
	public void testPattern()
	{
		Tetrimino.Pattern pat;
		
		pat = new Tetrimino.Pattern(3, 3, "000010001");
		Assert.AreEqual(1, pat.rect.x0);
		Assert.AreEqual(1, pat.rect.y0);
		Assert.AreEqual(2, pat.rect.x1);
		Assert.AreEqual(2, pat.rect.y1);

		pat = new Tetrimino.Pattern(3, 3, "010010110");
		Assert.AreEqual(0, pat.rect.x0);
		Assert.AreEqual(0, pat.rect.y0);
		Assert.AreEqual(1, pat.rect.x1);
		Assert.AreEqual(2, pat.rect.y1);

		pat = new Tetrimino.Pattern(2, 4, "01010101");
		Assert.AreEqual(1, pat.rect.x0);
		Assert.AreEqual(0, pat.rect.y0);
		Assert.AreEqual(1, pat.rect.x1);
		Assert.AreEqual(3, pat.rect.y1);

		pat = new Tetrimino.Pattern(2, 2, "1111");
		Assert.AreEqual(0, pat.rect.x0);
		Assert.AreEqual(0, pat.rect.y0);
		Assert.AreEqual(1, pat.rect.x1);
		Assert.AreEqual(1, pat.rect.y1);

		pat = new Tetrimino.Pattern(5, 3, "111111000111110");
		Assert.AreEqual(0, pat.rect.x0);
		Assert.AreEqual(0, pat.rect.y0);
		Assert.AreEqual(4, pat.rect.x1);
		Assert.AreEqual(2, pat.rect.y1);
	}

	[Test]
	public void testCalcOffset()
	{
		Tetrimino.Pattern beforePat;
		Tetrimino.Pattern afterPat;

		beforePat = new Tetrimino.Pattern(3, 3, "010010011");
		afterPat = new Tetrimino.Pattern(3, 3, "001111000");

		/*
		Before
		□■□
		□■□
		□■■
		
		After
		□□■
		■■■
		□□□
		↑ここが他のブロックに刺さる

		チェック時にずらして判定
		→□□■
		→■■■
		→□□□
		 */


		List<Tetrimino.Pos> offsetList;
		offsetList = Tetrimino.calcOffset(beforePat, afterPat);
		Assert.AreEqual(1, offsetList.Count);
		Assert.AreEqual(1, offsetList[0].x);
		Assert.AreEqual(0, offsetList[0].y);
	}


	[Test]
	public void testCalcOffsetMulti()
	{
		Tetrimino.Pattern beforePat;
		Tetrimino.Pattern afterPat;

		beforePat = new Tetrimino.Pattern(4, 4, "0100010001000100");
		afterPat = new Tetrimino.Pattern(4, 4, "0000000011110000");

		/*
		Before
		□■□□
		□■□□
		□■□□
		□■□□
		
		After
		□□□□
		□□□□
		■■■■
		□□□□
		 */


		List<Tetrimino.Pos> offsetList;
		offsetList = Tetrimino.calcOffset(beforePat, afterPat);
		Assert.AreEqual(3, offsetList.Count);
		Assert.AreEqual(1, offsetList[0].x);
		Assert.AreEqual(0, offsetList[0].y);
		Assert.AreEqual(-1, offsetList[1].x);
		Assert.AreEqual(0, offsetList[1].y);
		Assert.AreEqual(-2, offsetList[2].x);
		Assert.AreEqual(0, offsetList[2].y);
	}

	[Test]
	public void testCalcOffsetMultiVertical()
	{
		Tetrimino.Pattern beforePat;
		Tetrimino.Pattern afterPat;

		beforePat = new Tetrimino.Pattern(4, 4, "0000000011110000");
		afterPat = new Tetrimino.Pattern(4, 4, "0100010001000100");

		/*
		Before
		□□□□
		□□□□
		■■■■
		□□□□
		
		After
		□■□□
		□■□□
		□■□□
		□■□□
		 */


		List<Tetrimino.Pos> offsetList;
		offsetList = Tetrimino.calcOffset(beforePat, afterPat);
		Assert.AreEqual(3, offsetList.Count);
		Assert.AreEqual(0, offsetList[0].x);
		Assert.AreEqual(1, offsetList[0].y);
		Assert.AreEqual(0, offsetList[1].x);
		Assert.AreEqual(2, offsetList[1].y);
		Assert.AreEqual(0, offsetList[2].x);
		Assert.AreEqual(-1, offsetList[2].y);
	}

}