using UnityEngine;
using 玩家数据结构;

public static class 战斗伤害规则
{
	public static string 获取普攻类型(将领信息 攻击者)
	{
		if (攻击者 == null || 攻击者.将领属性 == null || 攻击者.将领属性.初始属性 == null ||
			攻击者.将领属性.初始属性.系列 != "神将") return "normal";
		技能系统.技能信息 技能 = 将领身份规则.获取技能(攻击者);
		if (技能 == null) return "normal";
		if (技能.伤害类型 == "物理攻击") return "physical";
		if (技能.伤害类型 == "法术攻击")
		{
			return 攻击者.将领配兵 != null && 攻击者.将领配兵.ID == 305.0 ? "fire" : "spell";
		}
		return "normal";
	}

	public static Color 获取显示颜色(string 类型)
	{
		if (类型 == "physical") return new Color32(74, 163, 255, 255);
		if (类型 == "spell" || 类型 == "fire") return new Color32(255, 82, 82, 255);
		return Color.white;
	}

	public static double 限制伤害(double 伤害, double 剩余)
	{
		if (double.IsNaN(伤害) || double.IsNaN(剩余) || 伤害 <= 0.0 || 剩余 <= 0.0) return 0.0;
		return System.Math.Min(伤害, 剩余);
	}

	public static void 结算经验(将领信息 攻击者, 将领信息 受击者, double 实际伤害, double 兵种攻击)
	{
		if (攻击者 == null || 受击者 == null || 实际伤害 <= 0.0 ||
			攻击者.详细信息.坑位颜色 == 受击者.详细信息.坑位颜色) return;
		int owner = (int)攻击者.详细信息.身份;
		if (owner != 全局变量.本机身份 || owner < 0 || owner >= 全局变量.所有玩家数据表.Count) return;
		double 加成 = 全局变量.所有玩家数据表[owner].获取指定状态加成("将领经验");
		double 原等级 = 攻击者.将领属性.成长点数.等级;
		攻击者.将领获取经验值(实际伤害 * 兵种攻击 * 0.5 * (1.0 + 加成 / 100.0));
		if (攻击者.将领属性.成长点数.等级 != 原等级) 全局变量.所有玩家数据表[owner].计算最终属性();
	}

	public static bool NPC免疫伤害(string 名字, string 类型)
	{
		return (名字 == "机关兽" && 类型 == "normal") ||
			(名字 == "朱雀" && (类型 == "normal" || 类型 == "spell")) ||
			(名字 == "玄武" && (类型 == "normal" || 类型 == "physical")) ||
			((名字 == "青龙" || 名字 == "白虎") && (类型 == "normal" || 类型 == "spell")) ||
			(名字 == "蚩尤" && 类型 != "fire");
	}
}
