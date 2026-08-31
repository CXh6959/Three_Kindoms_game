using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;
using 技能系统;

/// <summary>
/// 神/特殊NPC 主动技能战斗组件（阶段二.4 第1迭代：伤害+治疗）。
/// 自包含：挂在神/NPC将领对象上（由 将领功能.开始渲染_enqueue 仅对有技能的将添加），
/// 按怒气独立计时施放技能。非神将领不加此组件，原战斗逻辑完全不变。
/// 第2迭代待补（需轻改 将领功能.cs）：状态效果(冰冻/魅惑/无敌/武圣/连环/护盾/回春/复生/圣阳/清除)、
/// 单体/随机/前排目标选取、NPC公式中 X=轮回数 的缩放、号令类进场增益与减益。
/// </summary>
public class 技能战斗组件 : MonoBehaviour
{
	private 将领功能 将领;
	private 战斗系统 战斗;
	private 技能信息 技能;
	private double 怒气 = 50.0; // 起手半管怒气
	private float 结算计时;

	private void Start()
	{
		将领 = GetComponent<将领功能>();
		if (将领 == null || 将领.本将领信息 == null)
		{
			Destroy(this);
			return;
		}
		技能 = 全局技能库.获取将领技能(将领.本将领信息.将领属性.初始属性.名字);
		if (技能 == null)
		{
			Destroy(this);
			return;
		}
		// 战斗系统在将领对象往上第3级（同 将领功能.开始渲染_enqueue 的取法）
		Transform t = base.transform;
		for (int i = 0; i < 3; i++)
		{
			if (t == null)
			{
				break;
			}
			t = t.parent;
		}
		if (t != null)
		{
			战斗 = t.GetComponent<战斗系统>();
		}
	}

	private void Update()
	{
		if (将领 == null || 技能 == null || 战斗 == null)
		{
			return;
		}
		if (将领.本将领信息 == null || 将领.本将领信息.详细信息 == null)
		{
			return;
		}
		if (将领.本将领信息.详细信息.剩余兵力 <= 0.0)
		{
			return;
		}
		结算计时 += Time.deltaTime;
		if (结算计时 < 0.5f)
		{
			return;
		}
		结算计时 = 0f;
		怒气 += 6.0; // 被动回怒，约4秒满
		if (怒气 >= 100.0)
		{
			怒气 -= 100.0;
			施放技能();
		}
	}

	private void 施放技能()
	{
		int 类型 = 技能.类型;
		if (类型 == 0 || 类型 == 1 || 类型 == 2)
		{
			施放伤害();
		}
		else if (类型 == 3)
		{
			施放治疗();
		}
		// 类型4(增益)/5(负面)依赖状态系统，第2迭代实现
	}

	private void 施放伤害()
	{
		// 第1迭代：统一对敌方全体造成 基数攻击 × 倍率% 的伤害（真伤，不走普攻的攻防差）
		double 基数 = 将领.本将领信息.将领属性.最终属性.攻击;
		if (基数 <= 0.0)
		{
			基数 = 100.0;
		}
		double 伤害 = 基数 * 技能.倍率 / 100.0;
		List<将领功能> 敌方 = 获取存活(false);
		for (int i = 0; i < 敌方.Count; i++)
		{
			将领功能 e = 敌方[i];
			if (e != null && e.本将领信息 != null && e.本将领信息.详细信息 != null && e.本将领信息.详细信息.剩余兵力 > 0.0)
			{
				e.扣除血量(伤害); // 走既有管线，自动处理兵力/伤亡/胜负判定
			}
		}
		显示特效();
	}

	private void 施放治疗()
	{
		// 第1迭代：恢复我方全体，治疗量=兵种生命值基准 × 倍率%（华佗/甄姬等治疗类）
		double 比例 = 技能.倍率;
		if (比例 <= 0.0)
		{
			比例 = 20.0;
		}
		List<将领功能> 我方 = 获取存活(true);
		for (int i = 0; i < 我方.Count; i++)
		{
			将领功能 a = 我方[i];
			if (a == null || a.本将领信息 == null || a.本将领信息.详细信息 == null)
			{
				continue;
			}
			double 基准 = 1000.0;
			int idx = 全局兵种库.查询指定ID的索引(a.本将领信息.将领配兵.ID);
			if (idx != -1)
			{
				基准 = 全局兵种库.属性表[idx].生命值 * 3.0;
			}
			double 治疗量 = 基准 * 比例 / 100.0;
			a.本将领信息.详细信息.剩余兵力 = a.本将领信息.详细信息.剩余兵力 + 治疗量;
			a.本将领信息.将领配兵.数量 = a.本将领信息.详细信息.剩余兵力;
			if (a.本将领信息.详细信息.坑位颜色 == 0.0)
			{
				战斗.攻方兵力 += 治疗量;
			}
			else if (a.本将领信息.详细信息.坑位颜色 == 1.0)
			{
				战斗.守方兵力 += 治疗量;
			}
		}
		显示特效();
	}

	// 取同方(我方=true)或敌方(false)所有存活将领
	private List<将领功能> 获取存活(bool 我方)
	{
		List<将领功能> result = new List<将领功能>();
		if (战斗 == null || 将领 == null || 将领.本将领信息 == null || 将领.本将领信息.详细信息 == null)
		{
			return result;
		}
		double 我的颜色 = 将领.本将领信息.详细信息.坑位颜色;
		bool 取攻方 = (我方 && 我的颜色 == 0.0) || (!我方 && 我的颜色 == 1.0);
		GameObject 坑位 = 取攻方 ? 战斗.攻方坑位对象 : 战斗.守方坑位对象;
		if (坑位 == null)
		{
			return result;
		}
		int n = 坑位.transform.childCount;
		for (int i = 0; i < n; i++)
		{
			Transform slot = 坑位.transform.GetChild(i);
			if (slot.childCount > 0)
			{
				将领功能 g = slot.GetChild(0).GetComponent<将领功能>();
				if (g != null && g.本将领信息 != null && g.本将领信息.详细信息 != null && g.本将领信息.详细信息.剩余兵力 > 0.0)
				{
					result.Add(g);
				}
			}
		}
		return result;
	}

	private void 显示特效()
	{
		if (全局变量.打击特效pre != null)
		{
			GameObject fx = Instantiate(全局变量.打击特效pre);
			fx.transform.position = base.transform.position;
			Destroy(fx, 0.6f);
		}
	}
}
