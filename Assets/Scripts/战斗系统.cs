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

	private GameObject 坑位模型;

	public List<将领配兵> 消灭敌军列表 = new List<将领配兵>();

	public List<将领配兵> 损失兵力列表 = new List<将领配兵>();

	public 城池信息库类 被攻击的城池;

	public bool 等待销毁战场;

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
		if (战场类型 == 1)
		{
			城墙信息显示.text = 被攻击的城池.城墙.ToString();
		}
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
		if (!开始检测战斗结果)
		{
			return;
		}
		if (战场类型 == 1)
		{
			城墙信息显示.text = 被攻击的城池.城墙.ToString();
			double 城墙 = 被攻击的城池.城墙;
			double num = 被攻击的城池.获取城墙上限();
			double num2 = 0.0;
			if (城墙 > 0.0)
			{
				num2 = 城墙 / num;
			}
			float num3 = (float)num2;
			城墙血条对象.localPosition = new Vector2(3f * num3, -0.15f);
		}
		double num4 = 0.0;
		if (守方兵力 <= 0.0 || 攻方兵力 <= 0.0)
		{
			if (全军撤退)
			{
				if (守方兵力 <= 0.0 && 攻方兵力 <= 0.0)
				{
					if (战场类型 == 1)
					{
						UnityEngine.Debug.Log("城池战斗撤退");
						战斗结束 = true;
						被攻击的城池.正在交战 = false;
					}
					else
					{
						UnityEngine.Debug.Log("山贼战斗撤退");
						战斗结束 = true;
					}
				}
			}
			else if (战场类型 == 1)
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
		守方要渲染的编队将领列表 = null;
		攻方要渲染的编队将领列表 = null;
		伤害显示缓存表 = null;
		开始检测战斗结果 = false;
		UnityEngine.Object.Destroy(base.gameObject.transform.parent.parent.parent.parent.parent.gameObject, 0.2f);
	}
}
