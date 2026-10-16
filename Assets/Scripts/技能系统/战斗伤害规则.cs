using UnityEngine;
using 玩家数据结构;

public static class 战斗伤害规则
{
	public static string 获取普攻类型(将领信息 攻击者)
	{
		if (攻击者 == null || 攻击者.将领属性 == null || 攻击者.将领属性.初始属性 == null ||
			攻击者.将领属性.初始属性.系列 != "神将") return "normal";
		技能系统.技能信息 技能 = 全局技能库.获取将领技能(攻击者.将领属性.初始属性.名字);
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

	public static bool NPC免疫伤害(string 名字, string 类型)
	{
		return (名字 == "机关兽" && 类型 == "normal") ||
			(名字 == "朱雀" && (类型 == "normal" || 类型 == "spell")) ||
			(名字 == "玄武" && (类型 == "normal" || 类型 == "physical")) ||
			((名字 == "青龙" || 名字 == "白虎") && (类型 == "normal" || 类型 == "spell")) ||
			(名字 == "蚩尤" && 类型 != "fire");
	}
}
