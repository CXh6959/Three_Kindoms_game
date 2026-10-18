using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 战斗系统 : MonoBehaviour
{
	public int 战场类型;

	public int 坐标x;

	public int 坐标y;

	public long 创建时间;

	public GameObject 战斗结果显示对象;

	public GameObject 伤害布局列表;

	public List<GameObject> 伤害显示缓存表 = new List<GameObject>();

	public List<List<将领信息>> 攻方要渲染的编队将领列表 = new List<List<将领信息>>();

	public List<List<将领信息>> 守方要渲染的编队将领列表 = new List<List<将领信息>>();

	public int 攻身份;

	public int 守身份 = 1;

	public double 攻方兵力;

	public double 守方兵力;

	public bool 开始检测战斗结果;

	public bool 战斗结束;

	public bool 正在观战;

	public bool 全军撤退;

	public GameObject 攻方坑位对象;

	public GameObject 守方坑位对象;

	public Text 城墙信息显示;

	public Transform 城墙血条对象;

	private double 上次显示城墙 = double.NaN;

	private GameObject 坑位模型;

	public List<将领配兵> 消灭敌军列表 = new List<将领配兵>();

	public List<将领配兵> 损失兵力列表 = new List<将领配兵>();

	public 城池信息库类 被攻击的城池;

	public bool 等待销毁战场;

	private void OnDestroy()
	{
		释放参战将领();
	}

	public void 请求撤军()
	{
		if (战斗结束 || 等待销毁战场) return;
		全军撤退 = true;
		开始检测战斗结果 = true;
	}

	private void 释放参战将领()
	{
		释放待上场队列(攻方要渲染的编队将领列表);
		释放待上场队列(守方要渲染的编队将领列表);
		foreach (将领功能 将领 in GetComponentsInChildren<将领功能>(true))
		{
			将领.StopAllCoroutines();
			将领信息 信息 = 将领.本将领信息;
			if (信息 != null && 信息.详细信息.状态 == 1.0) 信息.详细信息.状态 = 0.0;
		}
		系统名将调度.释放战场(this);
	}

	private static void 释放待上场队列(List<List<将领信息>> 队列)
	{
		if (队列 == null) return;
		foreach (List<将领信息> 编队 in 队列)
		{
			if (编队 == null) continue;
			foreach (将领信息 将领 in 编队)
				if (将领 != null && 将领.详细信息.状态 == 1.0) 将领.详细信息.状态 = 0.0;
		}
		队列.Clear();
	}

	private void Start()
	{
		渲染坑位();
		伤害显示缓存表.Clear();
		for (int i = 0; i < 30; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(全局变量.伤害显示pre);
			gameObject.transform.SetParent(伤害布局列表.transform);
			gameObject.transform.localPosition = new Vector2(0f, 0f);
			gameObject.SetActive(value: false);
			伤害显示缓存表.Add(gameObject);
		}
		被攻击的城池 = 所有城池界面脚本.根据坐标获取指定城池(坐标x, 坐标y);
		if (战场类型 == 1 && 被攻击的城池 != null && 城墙信息显示 != null)
		{
			城墙信息显示.font = Resources.Load<Font>("等级数字");
			城墙信息显示.fontSize = 28;
			城墙信息显示.resizeTextForBestFit = true;
			城墙信息显示.resizeTextMinSize = 16;
			城墙信息显示.resizeTextMaxSize = 28;
			城墙信息显示.alignment = TextAnchor.MiddleCenter;
			城墙信息显示.rectTransform.sizeDelta = new Vector2(220f, 48f);
			城墙信息显示.rectTransform.localScale = new Vector3(0.025f, 0.025f, 1f);
			城墙信息显示.rectTransform.anchoredPosition = new Vector2(0f, -1.2f);
			城墙信息显示.text = 战斗数值格式.显示(被攻击的城池.城墙);
		}
	}

	public double 扣除城墙(double 伤害, string 类型)
	{
		if (战斗结束 || 战场类型 != 1 || 被攻击的城池 == null) return 0.0;
		double 实际伤害 = 战斗伤害规则.限制伤害(伤害, 被攻击的城池.城墙);
		if (实际伤害 <= 0.0) return 0.0;
		被攻击的城池.城墙 -= 实际伤害;
		if (伤害显示缓存表 != null && 城墙血条对象 != null)
		{
			foreach (GameObject item in 伤害显示缓存表)
			{
				if (item == null || item.activeSelf) continue;
				item.SetActive(true);
				item.transform.position = 城墙血条对象.position;
				Text 文本 = item.GetComponentInChildren<Text>(true);
				if (文本 != null)
				{
					文本.text = "-" + 战斗数值格式.显示(实际伤害);
					文本.color = 战斗伤害规则.获取显示颜色(类型);
				}
				break;
			}
		}
		return 实际伤害;
	}

	private void 渲染坑位()
	{
		float num = 6.5f;
		float num2 = 3.5f;
		加载资源.头像资源();
		加载资源.预制体资源();
		float num3 = -35f;
		float y = 29.8f;
		int num4 = 0;
		int num5 = 0;
		if (战场类型 == 1)
		{
			num3 = -12f;
		}
		for (int i = 0; i < 15; i++)
		{
			坑位模型 = UnityEngine.Object.Instantiate(全局变量.红色坑位pre);
			坑位模型.transform.name = "坑位" + i.ToString();
			坑位模型.transform.SetParent(攻方坑位对象.transform);
			坑位模型.transform.localPosition = new Vector2(0f - (float)num4 * num, 0f - (float)num5 * num2);
			num5++;
			if (num5 >= 5)
			{
				num5 = 0;
				num4++;
				if (num4 >= 3)
				{
					num4 = 0;
				}
			}
		}
		攻方坑位对象.transform.localPosition = new Vector2(num3, y);
		num4 = 0;
		num5 = 0;
		for (int j = 0; j < 15; j++)
		{
			坑位模型 = UnityEngine.Object.Instantiate(全局变量.蓝色坑位pre);
			坑位模型.transform.name = "坑位" + j.ToString();
			坑位模型.transform.SetParent(守方坑位对象.transform);
			坑位模型.transform.localPosition = new Vector2(0f + (float)num4 * num, 0f - (float)num5 * num2);
			num5++;
			if (num5 >= 5)
			{
				num5 = 0;
				num4++;
				if (num4 >= 3)
				{
					num4 = 0;
				}
			}
		}
		守方坑位对象.transform.localPosition = new Vector2(num3 + 8f, y);
	}

	public void 记录击杀兵力信息(double 兵种ID, double 击杀数量)
	{
		将领配兵 将领配兵 = 消灭敌军列表.Find((将领配兵 t) => t.ID == 兵种ID);
		if (将领配兵 == null)
		{
			将领配兵 将领配兵2 = new 将领配兵();
			将领配兵2.ID = 兵种ID;
			将领配兵2.数量 = 击杀数量;
			消灭敌军列表.Add(将领配兵2);
		}
		else
		{
			将领配兵.数量 += 击杀数量;
		}
	}

	public void 记录损失兵力信息(double 兵种ID, double 损失数量)
	{
		将领配兵 将领配兵 = 损失兵力列表.Find((将领配兵 t) => t.ID == 兵种ID);
		if (将领配兵 == null)
		{
			将领配兵 将领配兵2 = new 将领配兵();
			将领配兵2.ID = 兵种ID;
			将领配兵2.数量 = 损失数量;
			损失兵力列表.Add(将领配兵2);
		}
		else
		{
			将领配兵.数量 += 损失数量;
		}
	}

	private void FixedUpdate()
	{
		if (!开始检测战斗结果 || 等待销毁战场)
		{
			return;
		}
		if (战场类型 == 1)
		{
			double 城墙 = 被攻击的城池.城墙;
			if (城墙 != 上次显示城墙)
			{
				if (城墙信息显示 != null) 城墙信息显示.text = 战斗数值格式.显示(城墙);
				double num = 被攻击的城池.获取城墙上限();
				double num2 = 城墙 > 0.0 && num > 0.0 ? 城墙 / num : 0.0;
				float num3 = (float)num2;
				if (城墙血条对象 != null) 城墙血条对象.localPosition = new Vector2(3f * Mathf.Clamp01(num3), -0.15f);
				上次显示城墙 = 城墙;
			}
		}
		double num4 = 0.0;
		if (全军撤退)
		{
			// 不依赖浮点兵力扣到零；撤军也必须释放尚未上场的编队。
			战斗结束 = true;
			if (战场类型 == 1 && 被攻击的城池 != null) 被攻击的城池.正在交战 = false;
		}
		else if (守方兵力 <= 0.0 || 攻方兵力 <= 0.0)
		{
			if (战场类型 == 1)
			{
				if (守方兵力 <= 0.0 && 被攻击的城池.城墙 <= 0.0)
				{
					UnityEngine.Debug.Log("城池战斗结束");
					被攻击的城池.攻下城池(攻身份);
					num4 = 被攻击的城池.获取攻打战功();
					战斗结束 = true;
					被攻击的城池.正在交战 = false;
				}
				else if (攻方兵力 <= 0.0)
				{
					UnityEngine.Debug.Log("城池战斗结束1");
					战斗结束 = true;
					被攻击的城池.正在交战 = false;
				}
			}
			else if (战场类型 == 2)
			{
				UnityEngine.Debug.Log("轮回副本战斗结束");
				if (守方兵力 <= 0.0)
				{
					轮回副本系统.通关副本(攻身份);
				}
				战斗结束 = true;
			}
			else
			{
				UnityEngine.Debug.Log("山贼战斗结束");
				if (守方兵力 <= 0.0)
				{
					UnityEngine.Debug.Log("山贼已被击败,刷新");
					附近山贼.重新生成指定山贼(坐标x, 坐标y);
				}
				战斗结束 = true;
			}
		}
		if (!战斗结束)
		{
			return;
		}
		等待销毁战场 = true;
		释放参战将领();
		double num5 = 0.0;
		double num6 = 0.0;
		double num7 = 0.0;
		double num8 = 0.0;
		int count = 消灭敌军列表.Count;
		for (int i = 0; i < count; i++)
		{
			兵种属性库类 兵种属性库类 = 全局兵种库.查询指定ID的数据(消灭敌军列表[i].ID);
			num5 += 兵种属性库类.攻击力 * 0.05 * 消灭敌军列表[i].数量;
			num6 += 兵种属性库类.攻击力 * 0.1 * 消灭敌军列表[i].数量;
			num7 += 兵种属性库类.攻击力 * 0.3 * 消灭敌军列表[i].数量;
			num8 += 兵种属性库类.攻击力 * 0.002 * 消灭敌军列表[i].数量;
		}
		double num9 = 全局变量.所有玩家数据表[全局变量.本机身份].获取指定状态加成("资源声望") / 100.0;
		num5 *= 1.0 + num9;
		num6 *= 1.0 + num9;
		num7 *= 1.0 + num9;
		if (num5 > 10000000.0)
		{
			num5 = 10000000.0;
		}
		if (num6 > 10000000.0)
		{
			num6 = 10000000.0;
		}
		if (num7 > 30000000.0)
		{
			num7 = 30000000.0;
		}
		全局变量.所有玩家数据表[全局变量.本机身份].基础信息.君主获得经验(Mathf.Floor((float)num5));
		全局变量.所有玩家数据表[全局变量.本机身份].财产信息.铜钱 = 全局变量.所有玩家数据表[全局变量.本机身份].财产信息.铜钱 + (double)Mathf.Floor((float)num6);
		全局变量.所有玩家数据表[全局变量.本机身份].财产信息.粮食 = 全局变量.所有玩家数据表[全局变量.本机身份].财产信息.粮食 + (double)Mathf.Floor((float)num7);
		全局变量.所有玩家数据表[全局变量.本机身份].财产信息.黄金 = 全局变量.所有玩家数据表[全局变量.本机身份].财产信息.黄金 + (double)Mathf.Floor((float)num8);
		全局变量.所有玩家数据表[全局变量.本机身份].基础信息.战功 = 全局变量.所有玩家数据表[全局变量.本机身份].基础信息.战功 + (double)Mathf.Floor((float)num4);
		全局变量.提示类.显示信息("战斗结束!\r\n声望+" + Mathf.Floor((float)num5).ToString() + "\r\n铜钱 + " + Mathf.Floor((float)num6).ToString() + "\r\n粮食 + " + Mathf.Floor((float)num7).ToString() + "\r\n黄金 + " + Mathf.Floor((float)num8).ToString());
		UnityEngine.Debug.Log("战斗结束!\r\n声望+" + Mathf.Floor((float)num5).ToString() + "\r\n铜钱 + " + Mathf.Floor((float)num6).ToString() + "\r\n粮食 + " + Mathf.Floor((float)num7).ToString() + "\r\n黄金 + " + Mathf.Floor((float)num8).ToString());
		if (正在观战)
		{
			全局变量.战斗地图相机.transform.SetParent(全局变量.战斗地图相机.transform.parent.parent.parent);
			全局变量.大地图相机.SetActive(value: true);
			全局变量.大地图布局对象.SetActive(value: true);
			全局变量.战斗地图相机.SetActive(value: false);
			全局变量.主界面UI对象.SetActive(value: true);
			全局变量.战斗界面UI对象.SetActive(value: false);
			战斗界面UI脚本 界面 = 全局变量.战斗界面UI对象.GetComponent<战斗界面UI脚本>();
			if (界面 != null) 界面.开始显示兵力 = false;
			正在观战 = false;
		}
		bool flag;
		do
		{
			flag = true;
			for (int j = 0; j < 全局变量.军情列表.Count; j++)
			{
				if (全局变量.军情列表[j].到达时间 <= TIME.getTime() && 全局变量.军情列表[j].坐标x == 坐标x && 全局变量.军情列表[j].坐标y == 坐标y && 全局变量.军情列表[j].战场类型 == 战场类型)
				{
					全局变量.军情列表.RemoveAt(j);
					flag = false;
				}
			}
		}
		while (!flag);
		UnityEngine.Debug.Log("军情清理完毕!");
		攻方兵力 = 0.0;
		守方兵力 = 0.0;
		开始检测战斗结果 = false;
		UnityEngine.Object.Destroy(base.gameObject.transform.parent.parent.parent.parent.parent.gameObject, 0.2f);
	}
}
