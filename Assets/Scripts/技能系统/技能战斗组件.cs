using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;
using 技能系统;

public class 技能战斗组件 : MonoBehaviour
{
	private 将领功能 将领;
	private 战斗系统 战斗;
	private 技能信息 技能;
	private double 怒气;
	private bool 已执行进场效果;
	private float 无敌结束;
	private float 圣阳结束;
	private float 清障结束;
	private float 武圣结束;
	private float 无双结束;
	private float 冻结结束;
	private float 魅惑结束;
	private float 连环结束;
	private float 回春结束;
	private double 回春比例;
	private double 复生比例;
	private int 护盾层数;
	private float 反击倍率;
	private double 攻击加成比例;
	private double 防御加成比例;
	private double 减速比例;
	private float 攻击加成结束;
	private float 防御加成结束;

	private void Start()
	{
		将领 = GetComponent<将领功能>();
		if (将领 == null || 将领.本将领信息 == null || 将领.本将领信息.将领属性 == null)
		{
			Destroy(this);
			return;
		}
		技能 = 全局技能库.获取将领技能(将领.本将领信息.将领属性.初始属性.名字);
		战斗 = 获取战斗();
		怒气 = 0.0;
	}

	private void Update()
	{
		if (将领 == null)
		{
			return;
		}
		战斗 = 战斗 == null ? 获取战斗() : 战斗;
		if (!已执行进场效果 && 战斗 != null)
		{
			已执行进场效果 = true;
			执行进场效果();
		}
		if (回春结束 > Time.time && 将领.本将领信息.详细信息.剩余兵力 > 0.0)
		{
			将领.恢复兵力(将领.获取最大兵力() * 回春比例 * Time.deltaTime);
		}
	}

	public bool 攻击前尝试施放技能()
	{
		if (将领 == null || !将领.是否存活())
		{
			return true;
		}
		if (!是否可以行动())
		{
			return true;
		}
		if (魅惑结束 > Time.time)
		{
			魅惑攻击();
			return true;
		}
		if (技能 == null)
		{
			return false;
		}
		if (怒气 >= 100.0)
		{
			怒气 -= 100.0;
			施放技能();
			return true;
		}
		return false;
	}

	public bool 尝试特殊普通攻击()
	{
		if (!是特殊NPC() || !将领.是否存活())
		{
			return false;
		}
		if (!是否可以行动())
		{
			return true;
		}
		int x = 轮回系统.当前轮回数;
		string name = 将领.本将领信息.将领属性.初始属性.名字;
		if (name == "蚩尤")
		{
			对敌方单体伤害(20000.0 + x * 1000.0, "physical");
			使我方添加状态("圣阳", 99999f, 0.0);
		}
		else
		{
			string type = name == "玄武" ? "physical" : "spell";
			记录普通攻击伤害(2000.0 + x * 100.0, type);
			if (name == "机关兽")
			{
				降低敌方怒气(10.0);
				增加自身怒气(50.0);
			}
			else if (name == "朱雀")
			{
				降低敌方怒气(20.0 + x);
				增加自身怒气(100.0);
			}
			else if (name == "玄武")
			{
				治疗我方(0.10 + x / 100.0);
				增加自身怒气(100.0);
			}
			else if (name == "青龙")
			{
				对敌方添加随机状态("冰冻", Mathf.CeilToInt(5.0f + x / 10.0f), 100f, 0.0);
				增加自身怒气(100.0);
			}
			else if (name == "白虎")
			{
				对敌方清除随机增益(Mathf.CeilToInt(5.0f + x / 10.0f));
				增加自身怒气(100.0);
			}
		}
		return true;
	}

	public void 普通攻击后()
	{
		if (技能 == null || 是特殊NPC())
		{
			return;
		}
		增加自身怒气(25.0);
	}

	public bool 是否可以行动()
	{
		return 冻结结束 <= Time.time && 将领 != null && 将领.本将领信息 != null && 将领.本将领信息.详细信息.剩余兵力 > 0.0;
	}

	public float 获取攻速倍率()
	{
		if (连环结束 <= Time.time)
		{
			return 1f;
		}
		return (float)Mathf.Clamp((float)(1.0 - 减速比例), 0.05f, 1f);
	}

	public double 获取攻击倍率()
	{
		double result = 1.0 + 攻击加成比例;
		if (攻击加成结束 <= Time.time) result = 1.0;
		if (无双结束 > Time.time) result += 2.0;
		return result;
	}

	public double 获取防御倍率()
	{
		double result = 1.0 + 防御加成比例;
		if (防御加成结束 <= Time.time) result = 1.0;
		if (无双结束 > Time.time) result += 2.0;
		return result;
	}

	public double 处理伤害(double 伤害, string 伤害类型, bool 普通攻击, bool 技能攻击)
	{
		if (护盾层数 > 0)
		{
			护盾层数--;
			return 0.0;
		}
		string name = 将领.本将领信息.将领属性.初始属性.名字;
		if (无敌结束 > Time.time || (name == "机关兽" && 普通攻击) || (name == "朱雀" && (普通攻击 || 伤害类型 == "spell")) || (name == "玄武" && (普通攻击 || 伤害类型 == "physical")) || ((name == "青龙" || name == "白虎") && (普通攻击 || 伤害类型 == "spell")) || (name == "蚩尤" && 伤害类型 != "fire"))
		{
			return 0.0;
		}
		if (武圣结束 > Time.time)
		{
			伤害 *= 0.2;
		}
		if (连环结束 > Time.time && 伤害类型 == "fire")
		{
			伤害 *= 2.0;
		}
		int unitIndex = 全局兵种库.查询指定ID的索引(将领.本将领信息.将领配兵.ID);
		if (unitIndex != -1)
		{
			int unitId = (int)全局兵种库.属性表[unitIndex].ID;
			if (unitId == 205)
			{
				伤害 *= 伤害类型 == "fire" ? 2.0 : (伤害类型 == "physical" ? 0.5 : 1.0);
			}
			else if (unitId == 105)
			{
				伤害 *= 伤害类型 == "physical" ? 2.0 : (伤害类型 == "spell" ? 0.5 : 1.0);
			}
		}
		return Mathf.Max(0f, (float)伤害);
	}

	public void 受击后(将领功能 攻击者)
	{
		if (攻击者 != null && 武圣结束 > Time.time && 反击倍率 > 0f)
		{
			攻击者.扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 反击倍率 / 100.0, "physical");
		}
	}

	public bool 尝试复生()
	{
		if (复生比例 <= 0.0 || 回春结束 <= Time.time)
		{
			return false;
		}
		将领.本将领信息.详细信息.剩余兵力 = 将领.获取最大兵力() * 复生比例;
		将领.本将领信息.将领配兵.数量 = 将领.本将领信息.详细信息.剩余兵力;
		复生比例 = 0.0;
		return true;
	}

	public void 增加自身怒气(double 数值)
	{
		怒气 = Mathf.Clamp((float)(怒气 + 数值), 0f, 100f);
	}

	public void 减少自身怒气(double 数值)
	{
		if (圣阳结束 > Time.time || 是特殊NPC())
		{
			return;
		}
		怒气 = Mathf.Clamp((float)(怒气 - 数值), 0f, 100f);
	}

	private void 执行进场效果()
	{
		string name = 将领.本将领信息.将领属性.初始属性.名字;
		if (name == "神·曹操" || name == "神·孙权" || name == "神·献帝")
		{
			增加我方怒气(25.0, false);
		}
		else if (name == "神·刘备")
		{
			使我方添加状态("圣阳", 10f, 0.0);
		}
		else if (name == "蚩尤")
		{
			使我方添加护盾(10);
		}
	}

	private void 施放技能()
	{
		string name = 将领.本将领信息.将领属性.初始属性.名字;
		int rank = 将领.本将领信息.阶位;
		double value = 技能.获取阶位倍率(rank);
		if (name == "神·曹操")
		{
			增加我方怒气(value, false);
			降低敌方怒气(20.0);
			使我方添加状态("攻击", 10f, (rank + 1) * 10.0);
		}
		else if (name == "神·刘备")
		{
			增加我方怒气(value, false);
			降低敌方怒气(20.0);
			治疗我方((rank + 1) * 0.05);
		}
		else if (name == "神·孙权")
		{
			增加我方怒气(value, false);
			降低敌方怒气(20.0);
			使我方添加状态("防御", 10f, (rank + 1) * 10.0);
		}
		else if (name == "神·献帝")
		{
			增加我方怒气(value, false);
			降低敌方怒气(20.0);
			使我方添加状态("攻击", 10f, (rank + 1) * 5.0);
			使我方添加状态("防御", 10f, (rank + 1) * 5.0);
		}
		else if (name == "神·司马懿")
		{
			for (int i = 0; i < 6; i++) 对敌方全体伤害(value, "spell", false);
			增加自身怒气(50.0);
		}
		else if (name == "神·诸葛亮")
		{
			对敌方全体伤害(value, "spell", false);
			增加自身怒气((rank + 1) * 25.0);
			添加状态("无敌", 5f, 0.0);
		}
			else if (name == "神·陆逊")
			{
				对敌方添加随机状态("连环", rank == 0 ? 2 : rank == 1 ? 4 : rank == 2 ? 6 : 8, 10f, -1.0);
			对敌方全体伤害(value, "fire", false);
		}
		else if (name == "神·关羽")
		{
			对敌方竖排伤害(value, "physical", 3);
			添加状态("武圣", 99999f, rank == 0 ? 1000.0 : rank == 1 ? 4000.0 : rank == 2 ? 8000.0 : 12000.0);
		}
		else if (name == "神·吕布")
		{
			添加状态("无双", 10f, 200.0);
			对敌方全体伤害(value, "physical", false);
			增加自身怒气(50.0);
		}
		else if (name == "神·周瑜")
		{
			将领功能 target = 获取敌方().Count == 0 ? null : (rank < 2 ? 获取敌方()[Random.Range(0, 获取敌方().Count)] : 获取最高攻击敌人());
			if (target != null) target.扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * value / 100.0, "fire");
		}
		else if (name == "神·荀彧")
		{
			增加我方怒气(value, false);
			使我方添加状态("圣阳", 10f, 0.0);
		}
		else if (name == "神·徐庶")
		{
			清除我方负面();
			使我方添加状态("清障", value, 0.0);
		}
		else if (name == "神·二乔")
		{
			增加我方怒气(value, false);
			增加自身怒气((rank + 4) * 25.0);
		}
		else if (name == "神·郭嘉")
		{
			对敌方添加随机状态("冰冻", rank == 3 ? -1 : (int)value, (rank + 2) * 5f, 0.0);
		}
		else if (name == "神·庞统")
		{
			对敌方添加随机状态("连环", -1, 10f, value / 100.0);
		}
		else if (name == "神·鲁肃")
		{
			增加我方怒气(value, true);
			降低敌方怒气(rank == 0 ? 50 : rank == 1 ? 65 : rank == 2 ? 80 : 100);
		}
		else if (name == "神·貂蝉")
		{
			对敌方添加随机状态("魅惑", (int)value, rank == 0 ? 8f : rank == 1 ? 12f : rank == 2 ? 16f : 20f, 0.0);
		}
		else if (name == "神·姜维")
		{
			治疗我方(value / 100.0);
			使我方添加护盾(rank == 3 ? -1 : rank == 0 ? 3 : rank == 1 ? 7 : 11);
		}
		else if (name == "神·甄姬")
		{
			治疗我方(value / 100.0);
			对我方添加随机状态("回春", rank == 3 ? -1 : rank == 0 ? 3 : rank == 1 ? 7 : 11, 10f, (rank + 1) * 0.1);
		}
		else if (name == "神·华佗")
		{
			治疗我方(value / 100.0);
			对我方添加随机状态("复生", rank == 3 ? -1 : rank == 0 ? 3 : rank == 1 ? 7 : 11, 60f, (rank + 1) * 0.2);
		}
		else if (name == "机关兽") 对敌方全体伤害(5000.0 + 轮回系统.当前轮回数 * 500.0, "physical", false);
		else if (name == "朱雀") { 对敌方全体伤害(5000.0 + 轮回系统.当前轮回数 * 600.0, "fire", false); 使我方添加状态("圣阳", 99999f, 0.0); }
		else if (name == "玄武") { 玄武技能(); 使我方添加状态("圣阳", 99999f, 0.0); }
		else if (name == "青龙") { 对敌方前排伤害(5000.0 + 轮回系统.当前轮回数 * 800.0, "spell"); 使我方添加状态("圣阳", 99999f, 0.0); }
		else if (name == "白虎") { 对敌方全体伤害(5000.0 + 轮回系统.当前轮回数 * 600.0, "physical", false); 使我方添加状态("圣阳", 99999f, 0.0); }
		else if (name == "蚩尤") { 对敌方全体伤害(20000.0 + 轮回系统.当前轮回数 * 2000.0, "physical", false); 对敌方全体伤害(20000.0 + 轮回系统.当前轮回数 * 2000.0, "spell", false); }
	}

	private void 增加我方怒气(double value, bool 包含自身)
	{
		List<将领功能> list = 获取我方();
		for (int i = 0; i < list.Count; i++)
		{
			技能战斗组件 component = list[i].GetComponent<技能战斗组件>();
			if (component != null && (包含自身 || component != this)) component.增加自身怒气(value);
		}
	}

	private void 降低敌方怒气(double value)
	{
		List<将领功能> list = 获取敌方();
		for (int i = 0; i < list.Count; i++)
		{
			技能战斗组件 component = list[i].GetComponent<技能战斗组件>();
			if (component != null) component.减少自身怒气(value);
		}
	}

	private void 治疗我方(double 比例)
	{
		List<将领功能> list = 获取我方();
		for (int i = 0; i < list.Count; i++) list[i].恢复兵力(list[i].获取最大兵力() * 比例);
	}

	private void 对敌方全体伤害(double 倍率, string type, bool 普通攻击)
	{
		type = 转换伤害类型(type);
		List<将领功能> list = 获取敌方();
		for (int i = 0; i < list.Count; i++)
		{
			double damage = 将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0;
			if (普通攻击) list[i].扣除普通特殊伤害(damage, type);
			else list[i].扣除技能伤害(damage, type);
		}
	}

	private void 对敌方单体伤害(double 倍率, string type)
	{
		type = 转换伤害类型(type);
		List<将领功能> list = 获取敌方();
		if (list.Count > 0) list[Random.Range(0, list.Count)].扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0, type);
	}

	private void 对敌方竖排伤害(double 倍率, string type, int 次数)
	{
		type = 转换伤害类型(type);
		List<将领功能> list = 获取敌方();
		if (list.Count == 0) return;
		int column = 将领.transform.parent == null ? 0 : 将领.transform.parent.GetSiblingIndex() % 3;
		for (int k = 0; k < 次数; k++)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (获取列(list[i]) == column) list[i].扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0, type);
			}
		}
	}

	private void 对敌方前排伤害(double 倍率, string type)
	{
		type = 转换伤害类型(type);
		List<将领功能> list = 获取敌方();
		for (int i = 0; i < list.Count; i++) if (获取行(list[i]) == 0) list[i].扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0, type);
	}

	private void 玄武技能()
	{
		将领功能 target = 获取最高攻击敌人();
		if (target == null)
		{
			return;
		}
		double 倍率 = 5000.0 + 轮回系统.当前轮回数 * 1000.0;
		target.扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0, "physical");
		技能战斗组件 component = target.GetComponent<技能战斗组件>();
		if (component != null) component.添加状态("冰冻", 10f, 0.0);
	}

	private void 对敌方添加随机状态(string name, int count, float seconds, double value, bool 强制全部 = false)
	{
		添加随机状态(获取敌方(), name, count, seconds, value, 强制全部);
	}

	private void 对我方添加随机状态(string name, int count, float seconds, double value)
	{
		添加随机状态(获取我方(), name, count, seconds, value, false);
	}

	private void 添加随机状态(List<将领功能> list, string name, int count, float seconds, double value, bool 强制全部)
	{
		if (count < 0 || 强制全部) count = list.Count;
		List<将领功能> pool = new List<将领功能>(list);
		for (int i = 0; i < count && pool.Count > 0; i++)
		{
			int index = Random.Range(0, pool.Count);
			技能战斗组件 component = pool[index].GetComponent<技能战斗组件>();
			if (component != null) component.添加状态(name, seconds, value);
			pool.RemoveAt(index);
		}
	}

	public void 添加状态(string name, float seconds, double value)
	{
		if ((有状态("清障") || 有状态("圣阳")) && (name == "冰冻" || name == "魅惑" || name == "连环")) return;
		if (是特殊NPC() && (name == "冰冻" || name == "魅惑" || name == "连环" || name == "减速")) return;
		float end = seconds >= 99999f ? float.MaxValue : Time.time + seconds;
		if (name == "无敌") 无敌结束 = end;
		else if (name == "圣阳") 圣阳结束 = end;
		else if (name == "清障") 清障结束 = end;
		else if (name == "武圣") { 武圣结束 = end; 反击倍率 = (float)value; }
		else if (name == "无双") 无双结束 = end;
		else if (name == "冰冻") 冻结结束 = end;
		else if (name == "魅惑") 魅惑结束 = end;
		else if (name == "连环") { if (连环结束 <= Time.time) 减速比例 = 0.0; 连环结束 = end; if (value > 0.0 && value < 1.0) 减速比例 = Mathf.Max((float)减速比例, (float)value); }
		else if (name == "攻击") { if (攻击加成结束 <= Time.time) 攻击加成比例 = 0.0; 攻击加成比例 = Mathf.Max((float)攻击加成比例, (float)(value / 100.0)); 攻击加成结束 = end; }
		else if (name == "防御") { if (防御加成结束 <= Time.time) 防御加成比例 = 0.0; 防御加成比例 = Mathf.Max((float)防御加成比例, (float)(value / 100.0)); 防御加成结束 = end; }
		else if (name == "回春") { 回春结束 = end; 回春比例 = value; }
		else if (name == "复生") { 回春结束 = end; 复生比例 = value; }
		else if (name == "护盾") 护盾层数 = Mathf.Max(护盾层数, (int)value);
	}

	private void 使我方添加状态(string name, float seconds, double value)
	{
		List<将领功能> list = 获取我方();
		for (int i = 0; i < list.Count; i++)
		{
			技能战斗组件 component = list[i].GetComponent<技能战斗组件>();
			if (component != null) component.添加状态(name, seconds, value);
		}
	}

	private void 使我方添加护盾(int count)
	{
		List<将领功能> list = 获取我方();
		for (int i = 0; i < list.Count; i++)
		{
			技能战斗组件 component = list[i].GetComponent<技能战斗组件>();
			if (component != null) component.添加状态("护盾", 99999f, count < 0 ? 10 : 1);
		}
	}

	private void 清除我方负面()
	{
		List<将领功能> list = 获取我方();
		for (int i = 0; i < list.Count; i++)
		{
			技能战斗组件 component = list[i].GetComponent<技能战斗组件>();
			if (component != null) component.清除负面状态();
		}
	}

	private void 对敌方清除随机增益(int count)
	{
		List<将领功能> list = 获取敌方();
		for (int i = 0; i < count && list.Count > 0; i++)
		{
			int index = Random.Range(0, list.Count);
			技能战斗组件 component = list[index].GetComponent<技能战斗组件>();
			if (component != null) component.清除增益状态();
			list.RemoveAt(index);
		}
	}

	private void 冻结最高攻击敌人()
	{
		将领功能 target = 获取最高攻击敌人();
		if (target != null)
		{
			技能战斗组件 component = target.GetComponent<技能战斗组件>();
			if (component != null) component.添加状态("冰冻", 10f, 0.0);
		}
	}

	private void 魅惑攻击()
	{
		List<将领功能> list = 获取我方();
		if (list.Count == 0) return;
		将领功能 target = list[Random.Range(0, list.Count)];
		target.扣除技能伤害(将领.本将领信息.将领属性.最终属性.攻击 * 0.05, "physical");
	}

	private string 转换伤害类型(string type)
	{
		int index = 全局兵种库.查询指定ID的索引(将领.本将领信息.将领配兵.ID);
		if (index != -1 && 全局兵种库.属性表[index].ID == 305.0 && type == "spell")
		{
			return "fire";
		}
		return type;
	}

	private void 清除负面状态()
	{
		冻结结束 = 魅惑结束 = 连环结束 = 0f;
		减速比例 = 0.0;
	}

	private void 清除增益状态()
	{
		无敌结束 = 圣阳结束 = 武圣结束 = 无双结束 = 回春结束 = 0f;
		攻击加成比例 = 防御加成比例 = 0.0;
		攻击加成结束 = 防御加成结束 = 0f;
		护盾层数 = 0;
	}

	private bool 有状态(string name)
	{
		if (name == "清障") return 清障结束 > Time.time;
		if (name == "圣阳") return 圣阳结束 > Time.time;
		if (name == "连环") return 连环结束 > Time.time;
		return false;
	}

	private bool 是特殊NPC()
	{
		return 将领 != null && 将领.本将领信息 != null && 全局将领库.是否特殊NPC(将领.本将领信息.将领属性.初始属性.名字);
	}

	private void 记录普通攻击伤害(double 倍率, string type)
	{
		List<将领功能> list = 获取敌方();
		for (int i = 0; i < list.Count; i++) list[i].扣除普通特殊伤害(将领.本将领信息.将领属性.最终属性.攻击 * 倍率 / 100.0, type);
	}

	private 战斗系统 获取战斗()
	{
		Transform t = transform;
		for (int i = 0; i < 4 && t != null; i++, t = t.parent)
		{
			战斗系统 result = t.GetComponent<战斗系统>();
			if (result != null) return result;
		}
		return null;
	}

	private List<将领功能> 获取我方() { return 获取将领列表(true); }
	private List<将领功能> 获取敌方() { return 获取将领列表(false); }

	private List<将领功能> 获取将领列表(bool 我方)
	{
		List<将领功能> result = new List<将领功能>();
		if (战斗 == null || 将领 == null || 将领.本将领信息 == null) return result;
		bool 攻方 = (将领.本将领信息.详细信息.坑位颜色 == 0.0) == 我方;
		GameObject root = 攻方 ? 战斗.攻方坑位对象 : 战斗.守方坑位对象;
		if (root == null) return result;
		for (int i = 0; i < root.transform.childCount; i++)
		{
			Transform slot = root.transform.GetChild(i);
			if (slot.childCount == 0) continue;
			将领功能 item = slot.GetChild(0).GetComponent<将领功能>();
			if (item != null && item.本将领信息 != null && item.本将领信息.详细信息.剩余兵力 > 0.0) result.Add(item);
		}
		return result;
	}

	private 将领功能 获取最高攻击敌人()
	{
		List<将领功能> list = 获取敌方();
		将领功能 result = null;
		for (int i = 0; i < list.Count; i++) if (result == null || list[i].本将领信息.将领属性.最终属性.攻击 > result.本将领信息.将领属性.最终属性.攻击) result = list[i];
		return result;
	}

	private int 获取列(将领功能 item) { return item.transform.parent == null ? 0 : item.transform.parent.GetSiblingIndex() % 3; }
	private int 获取行(将领功能 item) { return item.transform.parent == null ? 0 : item.transform.parent.GetSiblingIndex() / 3; }
}
