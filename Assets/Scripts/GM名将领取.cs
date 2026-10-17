using System;
using System.Collections.Generic;
using 玩家数据结构;

public static class GM名将领取
{
	public static string[] 解析(string 代码)
	{
		代码 = (代码 ?? "").Trim().ToUpperInvariant();
		int 编号;
		if (代码.Length != 6 || !代码.StartsWith("GM", StringComparison.Ordinal) || !int.TryParse(代码.Substring(2), out 编号)) return null;
		return 将神坛系统.获取GM对应原将(编号);
	}

	public static bool 领取(string 代码, out string 消息)
	{
		string[] 名单 = 解析(代码);
		消息 = "无效领取码";
		if (名单 == null || 全局变量.所有玩家数据表 == null || 全局变量.本机身份 < 0 || 全局变量.本机身份 >= 全局变量.所有玩家数据表.Count) return false;
		玩家数据 玩家 = 全局变量.所有玩家数据表[全局变量.本机身份];
		if (玩家.封地信息表 == null || 玩家.封地信息表.Count == 0)
		{
			消息 = "请先建立封地";
			return false;
		}
		if (玩家.获取将领总数() + 名单.Length > 玩家.基础信息.将领数上限)
		{
			消息 = "将领容量不足";
			return false;
		}
		List<将领信息> 新将领 = new List<将领信息>();
		foreach (string 名字 in 名单)
		{
			将领属性库类 定义 = 全局将领库.查询指定名字的将领数据(名字);
			if (定义 == null || (定义.系列 != "名将" && 定义.系列 != "君王"))
			{
				消息 = "原将数据不存在：" + 名字;
				return false;
			}
			将领信息 将领 = new 将领信息();
			将领.生成将领数据(定义);
			新将领.Add(将领);
		}
		foreach (将领信息 将领 in 新将领) 玩家.添加将领信息到列表(0, 将领);
		消息 = "已获得：" + string.Join("、", 名单) + "（各1名）";
		return true;
	}
}
