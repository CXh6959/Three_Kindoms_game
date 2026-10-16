using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

public static class 系统名将调度
{
	// 包括尚未渲染的援军，避免提前结束战斗后名将永久卡在出战状态。
	private static readonly Dictionary<将领信息, 战斗系统> 出战名将 = new Dictionary<将领信息, 战斗系统>();

	public static bool 是无归属名将(将领信息 将领)
	{
		return 将领 != null && 将领.详细信息 != null && 将领.详细信息.身份 == 2.0 &&
			将领.将领属性 != null && 将领.将领属性.初始属性 != null && 将领.将领属性.初始属性.系列 == "名将";
	}

	public static void 修复名将池(bool 重置出战状态 = false)
	{
		if (全局变量.所有玩家数据表 == null || 全局变量.所有玩家数据表.Count <= 2) return;
		玩家数据 系统玩家 = 全局变量.所有玩家数据表[2];
		if (系统玩家 == null || 系统玩家.封地信息表 == null) return;
		if (重置出战状态) 出战名将.Clear();
		if (系统玩家.封地信息表.Count == 0) 系统玩家.封地信息表.Add(new 封地信息());
		HashSet<string> 已有名字 = new HashSet<string>();
		全局变量.所有名将列表.Clear();
		foreach (玩家数据 玩家 in 全局变量.所有玩家数据表)
		{
			if (玩家 == null || 玩家.封地信息表 == null) continue;
			foreach (封地信息 封地 in 玩家.封地信息表)
			{
				if (封地 == null || 封地.将领信息表 == null) continue;
				foreach (将领信息 将领 in 封地.将领信息表)
				{
					if (将领 == null || 将领.将领属性 == null || 将领.将领属性.初始属性 == null) continue;
					已有名字.Add(将领.将领属性.初始属性.名字);
					if (玩家 != 系统玩家) continue;
					系统玩家.将领ID标识 = System.Math.Max(系统玩家.将领ID标识, 将领.ID + 1);
					if (!系统名将数据.是否附表名将(将领.将领属性.初始属性.名字)) continue;
					将领.将领属性.初始属性.系列 = "名将";
					将领.详细信息.身份 = 2.0;
					if (重置出战状态 && 将领.详细信息.状态 == 1.0) 将领.详细信息.状态 = 0.0;
					全局变量.所有名将列表.Add(将领);
				}
			}
		}
		foreach (将领属性库类 定义 in 全局将领库.属性表)
		{
			if (定义 == null || 定义.系列 != "名将" || !系统名将数据.是否附表名将(定义.名字) || !已有名字.Add(定义.名字)) continue;
			将领信息 将领 = new 将领信息();
			将领.生成将领数据(定义);
			将领.将领获取经验值(将领.获取升级需要经验(99.0));
			将领.详细信息.忠诚 = 60.0;
			将领.ID = 系统玩家.将领ID标识++;
			系统玩家.封地信息表[0].将领信息表.Add(将领);
			全局变量.所有名将列表.Add(将领);
		}
		系统玩家.计算最终属性();
	}

	public static List<将领信息> 生成驻防(城池信息库类 城池, 战斗系统 战场)
	{
		List<将领信息> result = new List<将领信息>();
		if (城池 == null || (城池.规模 != 3 && 城池.规模 != 4) || 战场 == null || 全局变量.所有玩家数据表.Count <= 2) return result;
		foreach (封地信息 封地 in 全局变量.所有玩家数据表[2].封地信息表)
		{
			foreach (将领信息 将领 in 封地.将领信息表)
			{
				if (!是无归属名将(将领) || 将领.详细信息.状态 != 0.0 || 出战名将.ContainsKey(将领)) continue;
				// 每名可用名将独立判定，不限制为整座城只出现一名。
				if (Random.Range(0, 100) >= 20) continue;
				将领.将领配兵.ID = 104.0;
				将领.将领配兵.数量 = 将领.将领属性.最终属性.统兵;
				将领.详细信息.坑位颜色 = 1.0;
				将领.详细信息.状态 = 1.0;
				出战名将.Add(将领, 战场);
				result.Add(将领);
			}
		}
		return result;
	}

	public static void 释放战场(战斗系统 战场)
	{
		List<将领信息> 待释放 = new List<将领信息>();
		foreach (KeyValuePair<将领信息, 战斗系统> entry in 出战名将)
		{
			if (ReferenceEquals(entry.Value, 战场)) 待释放.Add(entry.Key);
		}
		foreach (将领信息 将领 in 待释放)
		{
			if (是无归属名将(将领) && 将领.详细信息.状态 == 1.0) 将领.详细信息.状态 = 0.0;
			出战名将.Remove(将领);
		}
	}

	public static bool 尝试俘获(将领信息 攻击者, 将领信息 被击败)
	{
		if (!是无归属名将(被击败) || 被击败.详细信息.剩余兵力 > 0.0 || 被击败.详细信息.状态 == 3.0 ||
			攻击者 == null || 攻击者.详细信息 == null || 攻击者.详细信息.身份 != 全局变量.本机身份) return false;
		玩家数据 玩家 = 全局变量.所有玩家数据表[全局变量.本机身份];
		返回将领索引 出征索引 = 玩家.获取指定ID标识的将领索引(攻击者.ID);
		返回将领索引 名将索引 = 全局变量.所有玩家数据表[2].获取指定ID标识的将领索引(被击败.ID);
		if (出征索引.第几个封地 < 0 || 名将索引.第几个封地 < 0) return false;
		if (Random.Range(0, 100) >= 15)
		{
			被击败.详细信息.忠诚 = System.Math.Max(0.0, 被击败.详细信息.忠诚 - 1.0);
			return false;
		}
		被击败.详细信息.状态 = 3.0;
		被击败.详细信息.俘虏玩家 = 全局变量.本机身份;
		玩家.封地信息表[出征索引.第几个封地].添加一个俘虏到列表(new 将领索引(2, 被击败.ID));
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息("俘获名将：" + 被击败.将领属性.初始属性.名字 + "，关押在封地" + (出征索引.第几个封地 + 1));
		}
		return true;
	}
}
