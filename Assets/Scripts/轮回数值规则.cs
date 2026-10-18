using System;
using 玩家数据结构;

public static class 轮回数值规则
{
	public static double NPC属性倍率(int 轮回数)
	{
		return 1.0 + (Math.Max(1, 轮回数) - 1.0) * 1.5;
	}

	// 只作用于刚生成/刚重算的基础结果，不能对同一结果重复叠乘。
	public static void 强化NPC属性(最终属性 属性, double 倍率)
	{
		属性.武力 *= 倍率;
		属性.智力 *= 倍率;
		属性.统帅 *= 倍率;
		属性.攻击 *= 倍率;
		属性.防御 *= 倍率;
		属性.生命值 *= 倍率;
		属性.体力上限 *= 倍率;
		属性.统兵 = Math.Floor(属性.统兵 * 倍率);
	}

	public static int 科技等级上限(int 科技索引, int 轮回数)
	{
		switch (科技索引)
		{
		case 6: case 7: case 8: case 10: case 11: case 12: case 13:
			return (int)Math.Min(int.MaxValue, 10L + Math.Max(1, 轮回数) - 1L);
		default:
			return 10;
		}
	}

	public static bool 可研究轮回科技(double 当前等级, int 轮回数, int 书院等级, int 书院上限)
	{
		return 当前等级 >= 0 && 当前等级 < 科技等级上限(6, 轮回数) &&
			书院等级 >= Math.Min(当前等级 + 1.0, 书院上限);
	}
}
