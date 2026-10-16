using System.Collections.Generic;
using 玩家数据结构;

public static class 成就系统
{
	public static void 检查轮回成就()
	{
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 轮回进度类.读取();
		}
		全局变量.轮回进度.修复旧数据();
		int 当前轮回 = 全局变量.轮回进度.轮回数;
		bool changed = false;
		for (int i = 0; i < 全局成就库.属性表.Count; i++)
		{
			成就定义 定义 = 全局成就库.属性表[i];
			if (定义 == null || 定义.触发类型 != "轮回" || 当前轮回 < 定义.阈值)
			{
				continue;
			}
			成就信息 进度 = 全局变量.轮回进度.获取成就进度(定义.ID);
			if (!进度.已完成)
			{
				进度.已完成 = true;
				进度.完成时间 = TIME.getTime();
				changed = true;
			}
		}
		if (changed)
		{
			全局变量.轮回进度.保存();
		}
	}

	public static List<成就定义> 获取已解锁成就()
	{
		检查轮回成就();
		List<成就定义> result = new List<成就定义>();
		for (int i = 0; i < 全局成就库.属性表.Count; i++)
		{
			成就定义 定义 = 全局成就库.属性表[i];
			成就信息 进度 = 全局变量.轮回进度.获取成就进度(定义.ID);
			if (进度.已完成)
			{
				result.Add(定义);
			}
		}
		return result;
	}

	public static bool 领取成就(string 成就ID)
	{
		检查轮回成就();
		成就定义 定义 = 全局成就库.获取成就定义(成就ID);
		if (定义 == null)
		{
			return false;
		}
		成就信息 进度 = 全局变量.轮回进度.获取成就进度(成就ID);
		if (!进度.已完成 || 进度.已领取)
		{
			return false;
		}
		bool 成功 = false;
		if (定义.奖励类型 == "将领")
		{
			成功 = 将神坛系统.授予神将(定义.奖励内容);
		}
		else if (定义.奖励类型 == "道具" && 定义.奖励内容 == "将神珠")
		{
			全局变量.轮回进度.将神珠数量 += (int)定义.奖励数量;
			成功 = true;
		}
		if (!成功)
		{
			return false;
		}
		进度.已领取 = true;
		全局变量.轮回进度.保存();
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息("已领取成就奖励: " + 定义.奖励内容);
		}
		return true;
	}
}
