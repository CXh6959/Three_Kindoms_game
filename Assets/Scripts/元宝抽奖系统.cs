using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 抽奖结果
{
	public bool 成功;
	public string 奖励名称;
	public int 将神珠数量;
	public int 神将碎片数量;
	public double 剩余元宝;
}

public static class 元宝抽奖系统
{
	public const double 单次消耗 = 5000.0;

	public static 抽奖结果 抽奖(int 玩家索引 = -1)
	{
		抽奖结果 result = new 抽奖结果();
		玩家数据 玩家 = 获取玩家(玩家索引);
		if (玩家 == null || 玩家.财产信息 == null || 玩家.财产信息.元宝 < 单次消耗)
		{
			result.奖励名称 = "元宝不足";
			return result;
		}
		玩家.财产信息.元宝 -= 单次消耗;
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 轮回进度类.读取();
		}
		全局变量.轮回进度.元宝余额 = 玩家.财产信息.元宝;
		int roll = Random.Range(0, 10000);
		if (roll < 100)
		{
			全局变量.轮回进度.将神珠数量++;
			result.将神珠数量 = 1;
			result.奖励名称 = "将神珠";
		}
		else if (roll < 600)
		{
			全局变量.轮回进度.神将碎片数量++;
			result.神将碎片数量 = 1;
			result.奖励名称 = "神将碎片";
		}
		else
		{
			商品属性类 商品 = 获取随机可用商品();
			if (商品 != null)
			{
				玩家.背包道具列表.添加道具(商品.道具名, 1);
				result.奖励名称 = 商品.道具名;
			}
			else
			{
				result.奖励名称 = "商城道具";
			}
		}
		result.成功 = true;
		result.剩余元宝 = 玩家.财产信息.元宝;
		全局变量.轮回进度.保存();
		显示提示("抽奖获得: " + result.奖励名称);
		return result;
	}

	private static 商品属性类 获取随机可用商品()
	{
		List<商品属性类> 商品列表 = 全局商城库.获取全部商品();
		List<商品属性类> 可用列表 = new List<商品属性类>();
		for (int i = 0; i < 商品列表.Count; i++)
		{
			if (商品列表[i] != null && 全局道具库.获取指定名字的道具(商品列表[i].道具名) != null)
			{
				可用列表.Add(商品列表[i]);
			}
		}
		return 可用列表.Count == 0 ? null : 可用列表[Random.Range(0, 可用列表.Count)];
	}

	private static 玩家数据 获取玩家(int 玩家索引)
	{
		int index = 玩家索引 < 0 ? 全局变量.本机身份 : 玩家索引;
		if (全局变量.所有玩家数据表 == null || index < 0 || index >= 全局变量.所有玩家数据表.Count)
		{
			return null;
		}
		return 全局变量.所有玩家数据表[index];
	}

	private static void 显示提示(string 文本)
	{
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息(文本);
		}
	}
}

public class 元宝抽奖脚本 : MonoBehaviour
{
	public Text 结果文本;
	public Text 元宝文本;

	public void 点击抽奖()
	{
		抽奖结果 result = 元宝抽奖系统.抽奖();
		if (结果文本 != null)
		{
			结果文本.text = result.成功 ? "获得 " + result.奖励名称 : result.奖励名称;
		}
		刷新余额();
	}

	public void 刷新余额()
	{
		if (元宝文本 != null && 全局变量.所有玩家数据表 != null && 全局变量.本机身份 >= 0 && 全局变量.本机身份 < 全局变量.所有玩家数据表.Count)
		{
			元宝文本.text = 全局变量.所有玩家数据表[全局变量.本机身份].财产信息.元宝.ToString();
		}
	}
}
