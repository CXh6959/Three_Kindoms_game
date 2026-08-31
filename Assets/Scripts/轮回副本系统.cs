using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

public static class 轮回副本系统
{
	public static bool 是否开启()
	{
		return 轮回系统.当前轮回数 >= 50;
	}

	public static 装备属性库类 生成轮回装备()
	{
		if (!是否开启() || 全局装备库.属性表 == null || 全局装备库.属性表.Count == 0)
		{
			return null;
		}
		List<装备属性库类> 候选 = new List<装备属性库类>();
		for (int i = 0; i < 全局装备库.属性表.Count; i++)
		{
			装备属性库类 item = 全局装备库.属性表[i];
			if (item != null && !string.IsNullOrEmpty(item.类型))
			{
				候选.Add(item);
			}
		}
		if (候选.Count == 0)
		{
			return null;
		}
		装备属性库类 原装备 = 候选[Random.Range(0, 候选.Count)];
		int 轮回 = 轮回系统.当前轮回数;
		装备属性库类 result = new 装备属性库类("轮回" + 轮回 + "·" + 原装备.名称, 原装备.类型, Mathf.Min(99, 轮回), 原装备.基础值 * (1.0 + 轮回 * 0.05));
		result.是否轮回装备 = true;
		result.来源轮回 = 轮回;
		result.是否可交易 = true;
		return result;
	}

	public static List<将领信息> 生成副本守军()
	{
		List<将领信息> result = new List<将领信息>();
		if (!是否开启())
		{
			return result;
		}
		int 轮回 = 轮回系统.当前轮回数;
		int 机关兽数量 = Mathf.Clamp(3 + 轮回 / 10, 3, 10);
		for (int i = 0; i < 机关兽数量; i++)
		{
			将领信息 机关兽 = 特殊城池系统.创建副本NPC("机关兽", 1);
			if (机关兽 != null) result.Add(机关兽);
		}
		将领信息 蚩尤 = 特殊城池系统.创建副本NPC("蚩尤", 1);
		if (蚩尤 != null) result.Add(蚩尤);
		for (int j = 0; j < result.Count; j++)
		{
			result[j].详细信息.坑位颜色 = 1.0;
			result[j].详细信息.状态 = 1.0;
		}
		return result;
	}

	public static bool 发起副本(List<将领信息> 出征将领, int 玩家索引 = -1)
	{
		if (!是否开启() || 出征将领 == null || 出征将领.Count == 0)
		{
			return false;
		}
		int index = 玩家索引 < 0 ? 全局变量.本机身份 : 玩家索引;
		if (全局变量.所有玩家数据表 == null || index < 0 || index >= 全局变量.所有玩家数据表.Count)
		{
			return false;
		}
		for (int i = 0; i < 全局变量.军情列表.Count; i++)
		{
			if (全局变量.军情列表[i].战场类型 == 2)
			{
				return false;
			}
		}
		List<将领信息> list = new List<将领信息>();
		for (int j = 0; j < 出征将领.Count; j++)
		{
			将领信息 item = 出征将领[j];
			if (item == null || item.详细信息 == null || item.将领配兵 == null || item.将领配兵.数量 <= 0.0)
			{
				continue;
			}
			item.详细信息.身份 = index;
			item.详细信息.坑位颜色 = 0.0;
			item.详细信息.状态 = 1.0;
			list.Add(item);
		}
		if (list.Count == 0)
		{
			return false;
		}
		军情信息 info = new 军情信息();
		info.战场类型 = 2;
		info.坐标x = -1;
		info.坐标y = -1;
		info.到达时间 = TIME.getTime() + 10;
		info.队列将领列表 = list;
		全局变量.军情列表.Add(info);
		return true;
	}

	public static bool 通关副本(int 玩家索引 = -1)
	{
		if (!是否开启())
		{
			return false;
		}
		int index = 玩家索引 < 0 ? 全局变量.本机身份 : 玩家索引;
		if (全局变量.所有玩家数据表 == null || index < 0 || index >= 全局变量.所有玩家数据表.Count)
		{
			return false;
		}
		装备属性库类 装备 = 生成轮回装备();
		if (装备 == null)
		{
			return false;
		}
		全局变量.所有玩家数据表[index].背包装备列表.添加指定装备数据到背包(装备, Random.Range(2, 5));
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 玩家数据结构.轮回进度类.读取();
		}
		全局变量.轮回进度.神将碎片数量++;
		全局变量.轮回进度.保存();
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息("副本通关，获得可交易轮回装备：" + 装备.名称 + "，神将碎片×1");
		}
		return true;
	}
}

public static class 副本系统
{
	public static bool 发起副本(List<将领信息> 出征将领, int 玩家索引 = -1)
	{
		return 轮回副本系统.发起副本(出征将领, 玩家索引);
	}

	public static List<将领信息> 生成副本守军()
	{
		return 轮回副本系统.生成副本守军();
	}

	public static bool 通关副本(int 玩家索引 = -1)
	{
		return 轮回副本系统.通关副本(玩家索引);
	}
}
