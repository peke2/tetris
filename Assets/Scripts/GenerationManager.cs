using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	世代管理
 */
public class GenerationManager
{

    const int GENERATION_NUMS = 20;     //	1世代の数
    const int MAX_PLAY_NUMS = 20;       //	最大プレイ数

    const int PROC_COUNT_PER_FRAME = 20; //	1フレームの処理回数

    const int INHERIT_BOUND = 120;      //	交叉する間隔

    int m_generation;   //	今の世代
    int m_play_count;   //	世代のプレイカウント

    List<List<int>> m_currentGenInputBitList;   //	現在の世代の入力情報格納リスト
    List<List<int>> m_nextGenInputBitList;      //	次世代の入力情報格納リスト

    List<List<Tetris.ErasedLineInfo>> m_erasedLineInfoList;   //  消去したラインの情報リスト

    List<Tetris> m_tetrisList;

    delegate State StateProcess();

    StateProcess m_stateProc;

    public enum State
    {
        None,   //	状態無し
        Init,   //	初期化
        Play,   //	プレイ中
        Change, //	交代
        ToNext, //	次の世代へ
    }


    public GenerationManager()
    {
        //	世代の操作情報保持リストを初期化
        m_currentGenInputBitList = initGenerationList(false);
        m_nextGenInputBitList = initGenerationList(true);   //	参照されるため、空の情報を追加する

        m_erasedLineInfoList = new List<List<Tetris.ErasedLineInfo>>();

        //	初期化から開始
        m_stateProc = procStateInit;
    }

    public State update()
    {
        //全部ゲームオーバーか？
        //必要な個体数の評価が済んだか？
        //次の世代を生成

        State state;
        state = m_stateProc();
        return state;
    }

    State procStateBlank()
    {
        return State.None;
    }

    /**
	 * 初期化
	 */
    State procStateInit()
    {
        m_generation = 0;
        m_play_count = 0;

        m_tetrisList = createGeneration();
        m_stateProc = procStateUpdate;

        return State.Init;
    }

    /**
	 * 更新
	 */
    State procStateUpdate()
    {
        for (int i = 0; i < PROC_COUNT_PER_FRAME; i++)
        {
            foreach (Tetris tetris in m_tetrisList)
            {
                tetris.update();
            }
        }

        if (true == isAllGameOver())
        {
            m_stateProc = procStateChange;
        }

        return State.Play;
    }

    /**
	 * 盤面の変更
	 */
    State procStateChange()
    {
        m_play_count++;
        int nums = m_play_count * MAX_PLAY_NUMS;

        //	操作情報を保持
        pushGenerationInfoList();

        //	1世代分の実行数に達したら次の世代へ
        if (GENERATION_NUMS <= nums)
        {
            //	操作の遺伝
            inheritGeneration();

            m_play_count = 0;
            m_generation++;
        }

        m_tetrisList = createGeneration();
        m_stateProc = procStateUpdate;

        return State.Change;
    }

    /**
	 *	世代管理リストを初期化
	 */
    List<List<int>> initGenerationList(bool isAddEmpty = true)
    {
        List<List<int>> list = new List<List<int>>();

        if (true == isAddEmpty)
        {
            for (int i = 0; i < GENERATION_NUMS; i++)
            {
                //	操作用データ(世代交代していないので空のデータ)
                list.Add(new List<int>());
            }
        }
        return list;
    }


    /**
	 *	世代を作成
	 */
    List<Tetris> createGeneration()
    {
        int index_offset;

        index_offset = m_play_count * MAX_PLAY_NUMS;

        List<Tetris> tetrisList = new List<Tetris>();

        List<int> bitList;
        Tetris tetris;

        //	自動
        for (int i = 0; i < MAX_PLAY_NUMS; i++)
        {
            bitList = m_nextGenInputBitList[i + index_offset];
            if (0 == bitList.Count) bitList = null;
            tetris = Tetris.CreateGameAutoPlay(bitList);
            tetrisList.Add(tetris);
        }

        return tetrisList;
    }


    /**
	 *	現在の世代の情報を残す
	 */
    void pushGenerationInfoList()
    {
        foreach (Tetris tetris in m_tetrisList)
        {
            InputAuto inp;
            inp = (InputAuto)tetris.getInput();

            List<int> list = new List<int>(inp.getInputBitList());

            //[todo] クラスの参照先が無い場合の対応を入れる
            //ここに来るのは自動入力だけの予定なので放置でも構わない
            //ユーザーの入力には参照位置の管理は含まれないのでキャストしたらメソッドは無いはず
            int index = inp.getListIndex();

            //	元の入力情報よりも参照位置が前ならば、後ろの情報をカット
            if (list.Count > index)
            {
                list.RemoveRange(index, list.Count - index);
            }

            m_currentGenInputBitList.Add(list);

            //  消去情報も合わせて保持
            m_erasedLineInfoList.Add(tetris.getErasedLineInfoList());
        }
    }

    //  ソート用
    class SortInfo
    {
        public List<int> inputList { get; private set; }
        public List<Tetris.ErasedLineInfo> erasedLineList { get; private set; }

        int m_score;
        public int Score { get { return m_score; } }

        public SortInfo(List<int> input, List<Tetris.ErasedLineInfo> erased)
        {
            inputList = input;
            erasedLineList = erased;

            calcScore();
        }

        void calcScore()
        {
            int total = 0;
            foreach (Tetris.ErasedLineInfo info in erasedLineList)
            {
                total += info.LineCount;
            }

            m_score = total;
        }

        /**
         * 残す部分を指定して入力情報リストの末尾をカット
         */
        public void cutInputListTail(int headnum)
        {
            inputList = inputList.GetRange(0, headnum);
        }
    }

    /**
	 *	世代を引き継ぐ
	 */
    void inheritGeneration()
    {
        //List<List<int>> list = new List<List<int>>();
        List<SortInfo> list = new List<SortInfo>();


        //	現在の入力情報で最大のカウントを取得
        int max_line_count = 0;
        for (int i = 0; i < GENERATION_NUMS; i++)
        {
            int count = m_currentGenInputBitList[i].Count;
            if (count > max_line_count) max_line_count = count;
        }

        //	現在の入力情報を取得
        for (int i = 0; i < GENERATION_NUMS; i++)
        {
            SortInfo sinfo = new SortInfo(m_currentGenInputBitList[i], m_erasedLineInfoList[i]);
            list.Add(sinfo);
        }

        //	スコアの大きい順にリストをソート
        list.Sort((SortInfo a, SortInfo b) => b.Score - a.Score);

        //  最終消去ラインまでの手順を残す
        int last_erased_line = 0;
        int line_count = list[0].erasedLineList.Count;
        if (line_count > 0)
        {
            last_erased_line = list[0].erasedLineList[line_count - 1].FrameCount;
        }

        Debug.Log("---- 世代[" + m_generation + "] ----");
        Debug.Log("最大スコア=" + list[0].Score);
        Debug.Log("最終消去ライン=" + last_erased_line);

        //	操作リストをクリア
        m_currentGenInputBitList.Clear();
        m_nextGenInputBitList.Clear();

        m_erasedLineInfoList.Clear();

        int bound = INHERIT_BOUND;
        List<List<int>> calcResultList;

        //  最終消去位置までの手順があるならばそこまでは残す
        if (last_erased_line > 0)
        {
            bound = last_erased_line;
            //  最終ラインまで残して後は消す
            list[0].cutInputListTail(bound);
        }

        //	1位と2位、2位と3位、3位と4位、4位と5位で交叉
        //	1位の動きが残りすぎるので他の順位も残るようにする
        //	この先の選択で重複するけど、1位以外の発生が増えるので良いかも？
        for (int i = 0; i < 4; i++)
        {
            calcResultList = calcCrossover(list[i].inputList, list[i + 1].inputList, bound);
            m_nextGenInputBitList.Add(calcResultList[0]);
        }

        //	1位の突然変異
        //	10%の部分を変異
        m_nextGenInputBitList.Add(mutation(list[0].inputList, 10));

        //	ランダムで選択した2つを交叉 → 4つ
        //	1位は除く
        List<int> comb = new List<int>();
        List<List<int>> combList;
        for (int i = 1; i < GENERATION_NUMS; i++)
        {
            comb.Add(i);
        }
        combList = GenerationManager.createCombination(comb, 2);    //	2つの組み合わせを列挙

        for (int i = 0; i < 4; i++)
        {
            int index;
            index = Random.Range(0, combList.Count);
            List<int> targetIndices = combList[index];
            combList.RemoveAt(index);

            calcResultList = calcCrossover(list[targetIndices[0]].inputList, list[targetIndices[1]].inputList, bound);
            m_nextGenInputBitList.Add(calcResultList[0]);   //	残すのは交叉後の1つだけ
                                                            //m_nextGenInputBitList.Add(calcResultList[1]);
        }

        //	ランダムで選択したものを突然変異 → 1つ
        //	1位以外
        //	5%の部分を変異
        m_nextGenInputBitList.Add(mutation(list[Random.Range(1, list.Count)].inputList, 5));

        //	ランダムで5つ選択してそのまま次へ持ち越し
        List<int> topRemovedList = new List<int>();
        List<int> randomSelected;

        //	1位は他と交叉しているので、そのまま次に残さない
        for (int i = 1; i < list.Count; i++)
        {
            topRemovedList.Add(i);
        }
        randomSelected = selectRandom(topRemovedList, 5);

        foreach (int i in randomSelected)
        {
            m_nextGenInputBitList.Add(new List<int>(list[i].inputList));
        }

        //	残り5つは最初から
        for (int i = 0; i < 5; i++)
        {
            m_nextGenInputBitList.Add(new List<int>());
        }

        Debug.Assert(m_nextGenInputBitList.Count == GENERATION_NUMS);
    }


    /**
	 *	指定のリストどうしを交叉
	 */
    static public List<List<int>> calcCrossover(List<int> listA, List<int> listB, int bound=600)
	{
		List<List<int>> list = new List<List<int>>();

		//	出力先を作る
		list.Add(new List<int>());
		list.Add(new List<int>());

		//	基準は短い方
		int step, tail;
		if(listA.Count < listB.Count)
		{
			step = listA.Count;
			tail = listB.Count;
		}
		else
		{
			step = listB.Count;
			tail = listA.Count;
		}

		//	お互いの内容を入れ替えながらリストに追加
		int a, b;
		int index;

		for(int i=0; i<step; i++)
		{
			index = (i/bound) & 1;

			if(listA.Count > i)
			{
				a = listA[i];
				list[index].Add(a);
			}

			if(listB.Count > i)
			{
				b = listB[i];
				list[index^1].Add(b);
			}
		}

		//	残りは交叉せずに、そのまま残す
		for(int i=step; i<tail; i++)
		{
			if(listA.Count > i)
			{
				list[0].Add(listA[i]);
			}

			if(listB.Count > i)
			{
				list[1].Add(listB[i]);
			}
		}

		return list;
	}

	/**
	 *	指定のリストを突然変異
	 */
	static public List<int> mutation(List<int> listA, int rate)
	{
		List<int> list = new List<int>();

		//	突然変異する箇所を選出
		int mutation_count;
		mutation_count = (int)(listA.Count * (float)rate / 100.0f);

		List<int> indexList = new List<int>();
		int ref_index = 0;

		for(int i=0; i<mutation_count; i++)
		{
			//	重複があるかもしれないが、あまり厳密に考えない
			indexList.Add(Random.Range(0, listA.Count));
		}
		indexList.Sort();

		int[] inputPatterns = InputAuto.getInputPatterns();
		int bit;
		for(int i=0; i<listA.Count; i++)
		{
			if((indexList.Count>ref_index) && (i==indexList[ref_index]))
			{
				bit = inputPatterns[Random.Range(0,inputPatterns.Length)];
				ref_index++;
			}
			else
			{
				bit = listA[i];
			}
			list.Add(bit);
		}

		return list;
	}

	/**
	 *	組み合わせ
	 */
	static public List<List<int>> createCombination(List<int> list, int r)
	{
		List<int> comb = new List<int>();
		List<List<int>> combList = new List<List<int>>();

		createCombinationRecursive(list, r, comb, combList, 1);

		return combList;
	}


	/**
	 *	組み合わせの再帰処理
	 */
	static public void createCombinationRecursive(List<int> list, int r, List<int> comb, List<List<int>> combList, int depth = 1)
	{
		//	指定されたリストから先頭を取り除いて要素を順番に取り出す	
		List<int> otherList = new List<int>(list);

		while(otherList.Count>0)
		{
			//	要素を取り出す
			int elem = otherList[0];
			otherList.RemoveAt(0);

			//	組み合わせに追加
			comb.Add(elem);
			if(depth < r)
			{
				//	組み合わせ数に満たない場合は次の要素へ
				createCombinationRecursive(otherList, r, comb, combList, depth+1);
			}
			else
			{
				//	完成した組み合わせを結果に追加
				combList.Add(new List<int>(comb));
			}
			//	組み合わせから末尾を除く
			comb.RemoveAt(comb.Count-1);
		}
	}


	/**
	 *	指定されたリストからランダムで要素を選択(重複無し)
	 */
	static public List<int> selectRandom(List<int> list, int num)
	{
		List<int> workList = new List<int>(list);
		List<int> selected = new List<int>();

		for(int i = 0; i<num; i++)
		{
			//	参照元が無ければ終了
			if(0 == workList.Count) break;

			//	選択したら重複しないよう参照元から除去
			int index;
			index = Random.Range(0, workList.Count);
			selected.Add(workList[index]);
			workList.RemoveAt(index);
		}
		return selected;
	}

	/**
	 *	全てゲームオーバーか？
	 */
	bool isAllGameOver()
	{
		foreach(Tetris tetris in m_tetrisList)
		{
			if(false == tetris.isGameOver()) return false;
		}

		return true;
	}


	public List<Tetris> getTetrisList()
	{
		return m_tetrisList;
	}


	public int getGeneration()
	{
		return m_generation;
	}
	public int getPlayCount()
	{
		return m_play_count;
	}
}	
