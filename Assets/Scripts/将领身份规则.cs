using 玩家数据结构;

public static class 将领身份规则
{
	// 显示名字可修改，原始定义 ID 和系列才决定技能、合成资格。
	public static 将领属性库类 获取定义(将领信息 将领)
	{
		if (将领 == null || 将领.将领属性 == null || 将领.将领属性.初始属性 == null) return null;
		初始属性 属性 = 将领.将领属性.初始属性;
		将领属性库类 定义 = 全局将领库.查询指定ID的将领数据(属性.ID);
		return 定义 != null && 定义.系列 == 属性.系列 ? 定义 : null;
	}

	public static bool 匹配名字(将领信息 将领, string 名字)
	{
		将领属性库类 定义 = 获取定义(将领);
		return 定义 != null && 定义.名字 == 名字;
	}

	public static bool 是保留名字(string 名字)
	{
		将领属性库类 定义 = 全局将领库.查询指定名字的将领数据((名字 ?? "").Trim());
		return 定义 != null && 定义.ID >= 10.0;
	}

	public static 技能系统.技能信息 获取技能(将领信息 将领)
	{
		将领属性库类 定义 = 获取定义(将领);
		return 定义 != null && 定义.系列 == "神将" ? 全局技能库.获取将领技能(定义.名字) : null;
	}
}
