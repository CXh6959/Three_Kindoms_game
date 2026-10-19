using System.Collections.Generic;
using 玩家数据结构;

public static class 战场增援规则
{
	public static bool 可以增援(战斗系统 战斗, int 玩家)
	{
		return 战斗 != null && !战斗.战斗结束 && !战斗.全军撤退 &&
			(战斗.攻身份 == 玩家 || 战斗.守身份 == 玩家);
	}

	public static bool 出征(战斗系统 战斗, List<将领信息> 将领列表, int 玩家)
	{
		if (!可以增援(战斗, 玩家) || 将领列表 == null || 将领列表.Count == 0 || 将领列表.Count > 5) return false;
		List<将领信息> 可用 = 轮回副本系统.获取可出征将领(玩家);
		HashSet<将领信息> 去重 = new HashSet<将领信息>();
		foreach (将领信息 将领 in 将领列表)
			if (!可用.Contains(将领) || 将领.详细信息.剩余体力 < 5.0 || !去重.Add(将领)) return false;
		int 阵营 = 战斗.攻身份 == 玩家 ? 0 : 1;
		foreach (将领信息 将领 in 将领列表)
		{
			将领.将领配兵.记录当前兵种();
			将领.详细信息.坑位颜色 = 阵营;
			将领.详细信息.状态 = 1.0;
		}
		全局变量.军情列表.Add(new 军情信息
		{
			战场类型 = 战斗.战场类型, 坐标x = 战斗.坐标x, 坐标y = 战斗.坐标y,
			到达时间 = TIME.getTime() + 10, 队列将领列表 = new List<将领信息>(将领列表),
			是战场增援 = true, 增援目标 = 战斗, 增援方 = 阵营
		});
		return true;
	}

	// 增援只投递给发令时的战场实例，不能在原战场结束后重开一场战斗。
	public static bool 处理军情(军情信息 军情)
	{
		战斗系统 战斗 = 军情.增援目标;
		if (战斗 == null || 战斗.战斗结束 || 战斗.全军撤退)
		{
			if (!军情.已进入战场)
				foreach (将领信息 将领 in 军情.队列将领列表)
					if (将领.详细信息.状态 == 1.0) 将领.详细信息.状态 = 0.0;
			return false;
		}
		if (!军情.已进入战场 && 军情.到达时间 <= TIME.getTime())
		{
			if (军情.增援方 == 0) 战斗.攻方要渲染的编队将领列表.Add(军情.队列将领列表);
			else 战斗.守方要渲染的编队将领列表.Add(军情.队列将领列表);
			军情.已进入战场 = true;
		}
		return true;
	}
}
