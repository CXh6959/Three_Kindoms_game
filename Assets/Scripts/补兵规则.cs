using System;
using 玩家数据结构;

public static class 补兵规则
{
	public static double 补满(封地信息 封地, 将领信息 将领)
	{
		if (封地 == null || 将领 == null || !封地.将领信息表.Contains(将领) ||
			将领.详细信息 == null || 将领.详细信息.状态 != 0.0 || 将领.将领配兵 == null ||
			将领.将领属性 == null || 将领.将领属性.最终属性 == null) return 0.0;
		将领配兵 配兵 = 将领.将领配兵;
		配兵.记录当前兵种();
		double 兵种ID = 配兵.ID > 0.0 ? 配兵.ID : 配兵.上次兵种ID;
		if (全局兵种库.查询指定ID的索引(兵种ID) < 0) return 0.0;
		double 缺兵 = 将领.将领属性.最终属性.统兵 - 配兵.数量;
		if (double.IsNaN(缺兵) || double.IsInfinity(缺兵) || 缺兵 <= 0.0) return 0.0;
		double 已补充 = 0.0;
		foreach (闲兵信息 闲兵 in 封地.闲兵信息表)
		{
			if (闲兵.ID != 兵种ID || double.IsNaN(闲兵.数量) || double.IsInfinity(闲兵.数量) || 闲兵.数量 <= 0.0) continue;
			double 数量 = Math.Min(缺兵, 闲兵.数量);
			配兵.ID = 兵种ID;
			配兵.数量 += 数量;
			闲兵.数量 -= 数量;
			已补充 += 数量;
			缺兵 -= 数量;
			if (缺兵 <= 0.0) break;
		}
		配兵.记录当前兵种();
		if (已补充 > 0.0) 将领.详细信息.剩余兵力 = 配兵.数量;
		return 已补充;
	}

	public static int 全部补满(玩家数据 玩家)
	{
		int 人数 = 0;
		if (玩家 == null) return 人数;
		玩家.计算最终属性();
		foreach (封地信息 封地 in 玩家.封地信息表)
			foreach (将领信息 将领 in 封地.将领信息表)
				if (补满(封地, 将领) > 0.0) 人数++;
		return 人数;
	}
}
