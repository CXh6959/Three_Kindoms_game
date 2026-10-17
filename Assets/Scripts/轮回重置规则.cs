using System.Collections.Generic;
using 玩家数据结构;

public static class 轮回重置规则
{
	public static Dictionary<int, List<将领信息>> 提取俘虏(玩家数据 玩家)
	{
		Dictionary<int, List<将领信息>> result = new Dictionary<int, List<将领信息>>();
		if (玩家 == null) return result;
		HashSet<将领信息> 已保留 = new HashSet<将领信息>();
		for (int i = 0; i < 玩家.封地信息表.Count; i++)
		{
			List<将领信息> 列表 = new List<将领信息>();
			foreach (将领索引 索引 in 玩家.封地信息表[i].俘虏信息表)
			{
				if (索引.第几个玩家 < 0 || 索引.第几个玩家 >= 全局变量.所有玩家数据表.Count) continue;
				玩家数据 原主 = 全局变量.所有玩家数据表[索引.第几个玩家];
				返回将领索引 位置 = 原主.获取指定ID标识的将领索引(索引.将领ID标识);
				if (位置.第几个封地 < 0) continue;
				将领信息 将领 = 原主.封地信息表[位置.第几个封地].将领信息表[位置.第几个将领];
				if (将领.详细信息.状态 == 3.0 && 将领.详细信息.俘虏玩家 == 玩家.基础信息.ID && 已保留.Add(将领)) 列表.Add(将领);
			}
			result.Add(i, 列表);
		}
		return result;
	}

	public static void 恢复俘虏(玩家数据 玩家, Dictionary<int, List<将领信息>> 保留俘虏)
	{
		玩家数据 系统玩家 = 全局变量.所有玩家数据表[2];
		foreach (KeyValuePair<int, List<将领信息>> entry in 保留俘虏)
		{
			foreach (将领信息 将领 in entry.Value)
			{
				将领.ID = 系统玩家.将领ID标识++;
				将领.详细信息.身份 = 2.0;
				将领.详细信息.状态 = 3.0;
				将领.详细信息.俘虏玩家 = 玩家.基础信息.ID;
				系统玩家.封地信息表[0].将领信息表.Add(将领);
				玩家.封地信息表[entry.Key].添加一个俘虏到列表(new 将领索引(2, 将领.ID));
			}
		}
	}

	public static void 加入初始汉国(玩家数据 玩家)
	{
		if (玩家 == null || 玩家.基础信息 == null) return;
		国家信息库类 汉国 = 全局方法类.获取指定名字的国家("汉");
		if (汉国 == null) return;
		玩家.基础信息.国家 = "汉";
		玩家.基础信息.官阶 = "平民";
		if (!汉国.成员列表.Contains(玩家.基础信息.ID)) 汉国.成员列表.Add(玩家.基础信息.ID);
	}
}
