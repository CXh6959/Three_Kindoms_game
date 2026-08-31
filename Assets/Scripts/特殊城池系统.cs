using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

public static class 特殊城池系统
{
	private static readonly string[] 四神兽 = { "朱雀", "玄武", "青龙", "白虎" };

	public static bool 是否特殊都城(城池信息库类 城池)
	{
		return 城池 != null && 城池.规模 == 4 && 轮回系统.当前轮回数 >= 10;
	}

	public static List<将领信息> 生成都城驻防(城池信息库类 城池)
	{
		List<将领信息> result = new List<将领信息>();
		if (!是否特殊都城(城池))
		{
			return result;
		}
		int 轮回 = 轮回系统.当前轮回数;
		int 机关兽数量 = 14;
		int 神兽数量 = 1;
		if (轮回 >= 20 && 轮回 < 30)
		{
			机关兽数量 = 13;
			神兽数量 = 2;
		}
		else if (轮回 >= 30 && 轮回 < 40)
		{
			机关兽数量 = 12;
			神兽数量 = 3;
		}
		else if (轮回 >= 40 && 轮回 < 50)
		{
			机关兽数量 = 11;
			神兽数量 = 4;
		}
		else if (轮回 >= 50)
		{
			机关兽数量 = 10;
			神兽数量 = 4;
		}
		int owner = 获取城主(城池);
		for (int i = 0; i < 机关兽数量; i++)
		{
			result.Add(创建NPC("机关兽", owner));
		}
		List<string> 神兽列表 = new List<string>(四神兽);
		for (int j = 0; j < 神兽数量; j++)
		{
			int index = Random.Range(0, 神兽列表.Count);
			result.Add(创建NPC(神兽列表[index], owner));
			神兽列表.RemoveAt(index);
		}
		if (轮回 >= 50)
		{
			result.Add(创建NPC("蚩尤", owner));
		}
		for (int k = 0; k < result.Count; k++)
		{
			result[k].详细信息.坑位颜色 = 1.0;
			result[k].详细信息.状态 = 1.0;
		}
		return result;
	}

	public static 将领信息 创建副本NPC(string 名字, int owner)
	{
		return 创建NPC(名字, owner);
	}

	private static 将领信息 创建NPC(string 名字, int owner)
	{
		将领属性库类 定义 = 全局将领库.查询指定名字的将领数据(名字);
		将领信息 result = new 将领信息();
		result.生成将领数据(定义);
		result.详细信息.身份 = owner;
		result.详细信息.剩余体力 = 100.0;
		result.详细信息.剩余兵力 = 1.0;
		result.将领属性.成长点数.等级 = 99.0;
		result.将领属性.最终属性.体力上限 = 100.0;
		double scale = 名字 == "机关兽" ? 5.0 : (名字 == "蚩尤" ? 100.0 : 10.0);
		result.将领属性.最终属性.武力 = 定义.武力 * scale;
		result.将领属性.最终属性.智力 = 定义.智力 * scale;
		result.将领属性.最终属性.统帅 = 定义.统帅 * scale;
		result.将领属性.最终属性.攻击 = result.将领属性.最终属性.武力;
		result.将领属性.最终属性.防御 = result.将领属性.最终属性.智力;
		result.将领属性.最终属性.统兵 = Mathf.Max(1f, (float)(定义.统帅 * 3.0 * (名字 == "蚩尤" ? 2.0 : 1.5)));
		result.将领配兵.ID = 名字 == "蚩尤" ? 107.0 : 106.0;
		result.将领配兵.数量 = result.将领属性.最终属性.统兵;
		return result;
	}

	private static int 获取城主(城池信息库类 城池)
	{
		国家信息库类 国家 = 全局方法类.获取指定名字的国家(城池.国家);
		return 国家 == null ? 1 : 国家.国王;
	}
}
