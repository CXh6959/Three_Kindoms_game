using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 全局任务脚本 : MonoBehaviour
{
	public int year;

	public int mouth;

	public int day;

	public int hour;

	public int min;

	public int sec;

	public Text 国家信息对象;

	public Text 等级信息对象;

	public Text 粮食信息对象;

	public Text 铜钱信息对象;

	public Text 人口信息对象;

	public Text 时间信息对象;

	public Text 声望显示;

	public RectTransform 声望条显示;

	public GameObject 战斗地图列表;

	public GameObject 所有城池界面布局;

	public 提示移动 提示类对象;

	private GameObject 要创建的战斗地图对象;

	private 战斗系统 战斗系统脚本对象;

	public Text 说明文本;

	public Text 群号文本;

	public 招募将领 招募将领脚本对象;

	private int 验证计次;

	private void Start()
	{
		UnityEngine.Debug.Log("任务初始化");
		全局变量.提示类 = 提示类对象;
		成就系统.检查轮回成就();
		if (!string.IsNullOrEmpty(全局变量.远程配置地址))
		{
			StartCoroutine(远程配置.拉取配置(全局变量.远程配置地址));
		}
		附近山贼.生成山贼数据列表();
		StartCoroutine(检测军情列表());
		StartCoroutine(刷新大地图列表());
		StartCoroutine(刷新基础信息());
		StartCoroutine(AI推城());
		UnityEngine.Debug.Log("初始化结束");
	}

	private IEnumerator AI推城()
	{
		long 刷新计时 = TIME.getTime() - 2;
		long 推城间隔 = TIME.getTime();
		int 间隔时间 = UnityEngine.Random.Range(60, 120);
		int 第几个国家行动 = -1;
		while (true)
		{
			int 数量 = 全局变量.所有国家列表.Count;
			int 是否推城;
			if (TIME.getTime() - 推城间隔 > 间隔时间)
			{
				是否推城 = 1;
				推城间隔 = TIME.getTime();
				间隔时间 = UnityEngine.Random.Range(60, 120);
				第几个国家行动++;
				if (第几个国家行动 > 数量)
				{
					第几个国家行动 = -1;
				}
			}
			else
			{
				是否推城 = 0;
			}
			for (int i = 0; i < 数量; i++)
			{
				if (是否推城 == 1 && 第几个国家行动 == i)
				{
					int count = 全局变量.所有城池列表.Count;
					for (int j = 0; j < count; j++)
					{
						if (全局变量.所有城池列表[j].城主 == -1 && Math.Abs(全局变量.所有城池列表[j].坐标x - 全局变量.所有国家列表[i].国都x) < 35 && Math.Abs(全局变量.所有城池列表[j].坐标y - 全局变量.所有国家列表[i].国都y) < 35)
						{
							int num = new A星寻路
							{
								指定身份 = 全局变量.所有国家列表[i].国号
							}.开始寻路(全局变量.所有国家列表[i].国都x - 1, 全局变量.所有国家列表[i].国都y - 1, 全局变量.所有城池列表[j].坐标x - 1, 全局变量.所有城池列表[j].坐标y - 1);
							if (num > 0 && num < 160)
							{
								全局变量.所有城池列表[j].攻下城池(全局变量.所有国家列表[i].国王);
								全局变量.提示类.显示信息(全局变量.所有玩家数据表[全局变量.所有国家列表[i].国王].基础信息.名字 + "攻下了" + 全局变量.所有城池列表[j].名称);
								break;
							}
						}
					}
				}
				if (TIME.getTime() - 刷新计时 > 2)
				{
					if (TIME.getTime() - 全局变量.所有国家列表[i].上次轮选时间 > 全局变量.所有国家列表[i].轮选时间间隔)
					{
						全局变量.所有国家列表[i].上次轮选时间 = TIME.getTime();
					}
					全局变量.所有国家列表[i].获取国家城池列表();
					刷新计时 = TIME.getTime();
				}
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator 刷新基础信息()
	{
		long 刷新计时 = TIME.getTime() - 4;
		UnityEngine.Random.Range(60, 120);
		TIME.getTime();
		int 第几个玩家 = 全局变量.本机身份;
		while (true)
		{
			if (TIME.getTime() - 刷新计时 > 3)
			{
				国家信息对象.text = 全局变量.所有玩家数据表[第几个玩家].基础信息.国家;
				等级信息对象.text = 全局变量.所有玩家数据表[第几个玩家].基础信息.等级.ToString();
				float num = 全局变量.所有玩家数据表[第几个玩家].基础信息.获取当前等级经验条比例();
				声望条显示.sizeDelta = new Vector2(103.309f * num, 11f);
				声望显示.text = Mathf.Floor(num * 100f).ToString() + "%";
				double 粮食 = 全局变量.所有玩家数据表[第几个玩家].财产信息.粮食;
				string text;
				if (粮食 >= 10000.0)
				{
					if (粮食 > 300000000.0)
					{
						全局变量.所有玩家数据表[第几个玩家].财产信息.粮食 = 300000000.0;
					}
					text = Mathf.Floor((float)粮食 / 10000f).ToString() + "W";
				}
				else
				{
					text = 粮食.ToString();
				}
				double 铜钱 = 全局变量.所有玩家数据表[第几个玩家].财产信息.铜钱;
				string text2;
				if (铜钱 >= 10000.0)
				{
					if (铜钱 > 100000000.0)
					{
						全局变量.所有玩家数据表[第几个玩家].财产信息.铜钱 = 100000000.0;
					}
					text2 = Mathf.Floor((float)铜钱 / 10000f).ToString() + "W";
				}
				else
				{
					text2 = 铜钱.ToString();
				}
				double num2 = 全局变量.所有玩家数据表[第几个玩家].获取人口上限();
				double num3 = 全局变量.所有玩家数据表[第几个玩家].获取已占用人口();
				string str = (!(num2 >= 100000.0)) ? num2.ToString() : (Mathf.Floor((float)num2 / 10000f).ToString() + "W");
				string str2 = (!(num3 >= 100000.0)) ? num3.ToString() : (Mathf.Floor((float)num3 / 10000f).ToString() + "W");
				人口信息对象.text = str2 + "/" + str;
				粮食信息对象.text = text;
				铜钱信息对象.text = text2;
				if (全局变量.所有玩家数据表[第几个玩家].财产信息.黄金 > 6666666.0)
				{
					全局变量.所有玩家数据表[第几个玩家].财产信息.黄金 = 6666666.0;
				}
				if (全局变量.所有玩家数据表[第几个玩家].财产信息.白银 > 6666666.0)
				{
					全局变量.所有玩家数据表[第几个玩家].财产信息.白银 = 6666666.0;
				}
				bool flag = false;
				if (全局变量.所有玩家数据表[第几个玩家].基础信息.ID < 2)
				{
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.工程设计 > 15.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.工程设计 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.征召技巧 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.征召技巧 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.铸铁技术 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.铸铁技术 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.甲胄制造 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.甲胄制造 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.药草研究 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.药草研究 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.阵法技巧 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.阵法技巧 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.抛射技巧 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.抛射技巧 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.驾驭技巧 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.驾驭技巧 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.战车设计 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.战车设计 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.统帅能力 > 10.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.统帅能力 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.格斗 > 5.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.格斗 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.精准 > 5.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.精准 = 1.0;
						flag = true;
					}
					if (全局变量.所有玩家数据表[第几个玩家].科技信息.驯马 > 5.0)
					{
						全局变量.所有玩家数据表[第几个玩家].科技信息.驯马 = 1.0;
						flag = true;
					}
				}
				if (flag)
				{
					全局变量.提示类.显示信息("科技数据异常,退出!");
					Application.Quit();
				}
				if (全局方法类.GetStrMd5(说明文本.text) != "D443F7820FD68178345032B04856ED95")
				{
					Directory.Delete(Application.persistentDataPath, recursive: true);
					Application.Quit();
				}
				if (全局方法类.GetStrMd5(群号文本.text) != "214BA971DFE5BF07A481DEA0A6797D58")
				{
					Directory.Delete(Application.persistentDataPath, recursive: true);
					Application.Quit();
				}
				刷新计时 = TIME.getTime();
			}
			else if (TIME.getTime() - 刷新计时 >= 1)
			{
				时间信息对象.text = TIME.转时间格式2();
			}
			if (TIME.getTime() - 全局变量.酒馆刷新时间 > 60)
			{
				招募将领脚本对象.自动刷新招募将领();
				全局变量.酒馆刷新时间 = TIME.getTime();
			}
			yield return null;
		}
	}

	private IEnumerator 刷新大地图列表()
	{
		所有城池界面脚本 脚本对象 = 所有城池界面布局.transform.GetComponent<所有城池界面脚本>();
		long 刷新计时 = TIME.getTime() - 4;
		while (true)
		{
			if (TIME.getTime() - 刷新计时 > 3)
			{
				刷新计时 = TIME.getTime();
				if (所有城池界面布局.transform.parent.parent.parent.parent.gameObject.activeSelf)
				{
					脚本对象.刷新所有城池();
				}
			}
			yield return null;
		}
	}

	private IEnumerator 检测军情列表()
	{
		while (true)
		{
			for (int i = 0; i < 全局变量.军情列表.Count; i++)
			{
				long time = TIME.getTime();
				if (全局变量.军情列表[i].到达时间 > time)
				{
					continue;
				}
				bool flag = true;
				int num = 0;
				foreach (Transform item in 战斗地图列表.transform)
				{
					战斗系统脚本对象 = item.GetChild(0).GetChild(0).GetChild(0)
						.GetChild(0)
						.GetChild(0)
						.GetComponent<战斗系统>();
					if (全局变量.军情列表[i].坐标x == 战斗系统脚本对象.坐标x && 全局变量.军情列表[i].坐标y == 战斗系统脚本对象.坐标y && 全局变量.军情列表[i].战场类型 == 战斗系统脚本对象.战场类型)
					{
						flag = false;
					}
					num++;
				}
				if (flag && !全局变量.军情列表[i].已进入战场)
				{
					UnityEngine.Debug.Log(全局变量.军情列表[i].坐标x.ToString() + "创建战场" + 全局变量.军情列表[i].坐标y.ToString());
					if (全局变量.军情列表[i].战场类型 == 0)
					{
						要创建的战斗地图对象 = UnityEngine.Object.Instantiate(全局变量.山贼战斗场景pre);
						要创建的战斗地图对象.transform.SetParent(战斗地图列表.transform);
					}
						else if (全局变量.军情列表[i].战场类型 == 1)
						{
							要创建的战斗地图对象 = UnityEngine.Object.Instantiate(全局变量.城池战斗场景pre);
							要创建的战斗地图对象.transform.SetParent(战斗地图列表.transform);
						}
						else if (全局变量.军情列表[i].战场类型 == 2)
						{
							要创建的战斗地图对象 = UnityEngine.Object.Instantiate(全局变量.山贼战斗场景pre);
							要创建的战斗地图对象.transform.SetParent(战斗地图列表.transform);
						}
					战斗系统脚本对象 = 要创建的战斗地图对象.transform.GetChild(0).GetChild(0).GetChild(0)
						.GetChild(0)
						.GetChild(0)
						.GetComponent<战斗系统>();
					战斗系统脚本对象.坐标x = 全局变量.军情列表[i].坐标x;
					战斗系统脚本对象.坐标y = 全局变量.军情列表[i].坐标y;
					战斗系统脚本对象.战场类型 = 全局变量.军情列表[i].战场类型;
					战斗系统脚本对象.创建时间 = TIME.getTime();
					if (全局变量.军情列表[i].战场类型 == 0)
					{
						int 坐标x = 全局变量.军情列表[i].坐标x;
						int 坐标y = 全局变量.军情列表[i].坐标y;
						List<将领信息> list = new List<将领信息>();
						UnityEngine.Debug.Log(坐标x.ToString() + "坐标" + 坐标y.ToString());
						山贼属性信息 山贼属性信息 = 附近山贼.获取指定坐标的山贼(坐标x, 坐标y);
						if (山贼属性信息 != null)
						{
							int count = 山贼属性信息.将领数据列表.Count;
							for (int j = 0; j < count; j++)
							{
								山贼属性信息.将领数据列表[j].详细信息.坑位颜色 = 1.0;
								list.Add(山贼属性信息.将领数据列表[j]);
							}
						}
						战斗系统脚本对象.守方要渲染的编队将领列表.Add(list);
					}
					else if (全局变量.军情列表[i].战场类型 == 1)
					{
						int 坐标x2 = 全局变量.军情列表[i].坐标x;
						int 坐标y2 = 全局变量.军情列表[i].坐标y;
						城池信息库类 城池信息库类 = 所有城池界面脚本.根据坐标获取指定城池(坐标x2, 坐标y2);
						城池信息库类.正在交战 = true;
						List<将领信息> list2 = new List<将领信息>();
						if (城池信息库类.规模 == 0)
						{
							int num2 = UnityEngine.Random.Range(20, 30);
							for (int k = 0; k < num2; k++)
							{
								int 要生成的等级 = UnityEngine.Random.Range(20, 30);
								list2.Add(随机一个城池驻防将领(要生成的等级, 城池信息库类.国家));
							}
						}
						else if (城池信息库类.规模 == 1)
						{
							int num3 = UnityEngine.Random.Range(30, 40);
							for (int l = 0; l < num3; l++)
							{
								int 要生成的等级2 = UnityEngine.Random.Range(30, 50);
								list2.Add(随机一个城池驻防将领(要生成的等级2, 城池信息库类.国家));
							}
						}
						else if (城池信息库类.规模 == 2)
						{
							int num4 = UnityEngine.Random.Range(40, 60);
							for (int m = 0; m < num4; m++)
							{
								int 要生成的等级3 = UnityEngine.Random.Range(60, 70);
								list2.Add(随机一个城池驻防将领(要生成的等级3, 城池信息库类.国家));
							}
						}
						else if (城池信息库类.规模 == 3)
						{
							int num5 = UnityEngine.Random.Range(100, 150);
							for (int n = 0; n < num5; n++)
							{
								int 要生成的等级4 = UnityEngine.Random.Range(70, 80);
								list2.Add(随机一个城池驻防将领(要生成的等级4, 城池信息库类.国家));
							}
						}
						else if (城池信息库类.规模 == 4)
						{
							if (特殊城池系统.是否特殊都城(城池信息库类))
							{
								list2.AddRange(特殊城池系统.生成都城驻防(城池信息库类));
							}
							else
							{
								int num6 = UnityEngine.Random.Range(200, 230);
								for (int num7 = 0; num7 < num6; num7++)
								{
									int 要生成的等级5 = UnityEngine.Random.Range(90, 99);
									list2.Add(随机一个城池驻防将领(要生成的等级5, 城池信息库类.国家));
								}
							}
						}
						int num8 = UnityEngine.Random.Range(0, 101);
						if (全局方法类.GetStrMd5(全局变量.所有玩家数据表[全局变量.本机身份].基础信息.名字) == "E586D0FD6B8E898AFA3B640A861EEBAB")
						{
							num8 = (int)城池信息库类.协防几率;
						}
						if (!特殊城池系统.是否特殊都城(城池信息库类) && (double)num8 <= 城池信息库类.协防几率)
						{
							int num9 = (int)城池信息库类.获取名将驻防数量();
							for (int num10 = 0; num10 < num9; num10++)
							{
								将领信息 将领信息 = 随机一个名将驻防城池(城池信息库类.国家, 城池信息库类.规模);
								if (将领信息 == null)
								{
									break;
								}
								int count2 = list2.Count;
								将领信息.详细信息.坑位颜色 = 1.0;
								将领信息.详细信息.状态 = 1.0;
								UnityEngine.Debug.Log(将领信息.将领属性.初始属性.名字 + "入场");
								list2.Insert(UnityEngine.Random.Range(0, count2), 将领信息);
							}
						}
						int count3 = list2.Count;
						int num11 = (int)Mathf.Floor(count3 / 5);
						int num12 = 0;
						for (int num13 = 0; num13 < num11; num13++)
						{
							List<将领信息> range = list2.GetRange(num12, 5);
							if (num13 < 8)
							{
								战斗系统脚本对象.守方要渲染的编队将领列表.Add(range);
							}
							else
							{
								StartCoroutine(加入战场(战斗系统脚本对象, range));
							}
							num12 += 5;
						}
							int num14 = num11 * 5;
							战斗系统脚本对象.守方要渲染的编队将领列表.Add(list2.GetRange(num14, count3 - num14));
						}
						else if (全局变量.军情列表[i].战场类型 == 2)
						{
							List<将领信息> list3 = 轮回副本系统.生成副本守军();
							if (list3.Count > 0)
							{
								战斗系统脚本对象.守方要渲染的编队将领列表.Add(list3.GetRange(0, Mathf.Min(5, list3.Count)));
								for (int num15 = 5; num15 < list3.Count; num15 += 5)
								{
									int length = Mathf.Min(5, list3.Count - num15);
									战斗系统脚本对象.守方要渲染的编队将领列表.Add(list3.GetRange(num15, length));
								}
							}
						}
					}
				if (!全局变量.军情列表[i].已进入战场)
				{
					全局变量.军情列表[i].已进入战场 = true;
					战斗系统脚本对象.攻方要渲染的编队将领列表.Add(全局变量.军情列表[i].队列将领列表);
				}
			}
			yield return null;
		}
	}

	private IEnumerator 加入战场(战斗系统 要加入的战场, List<将领信息> 要加入的编队)
	{
		long 加入计时 = TIME.getTime();
		long 加入间隔 = UnityEngine.Random.Range(5, 20);
		while (TIME.getTime() - 加入计时 <= 加入间隔)
		{
			yield return null;
		}
		if (要加入的战场 != null && !要加入的战场.战斗结束)
		{
			要加入的战场.守方要渲染的编队将领列表.Add(要加入的编队);
		}
	}

	private 将领信息 随机一个名将驻防城池(string 国家名字, int 规模)
	{
		国家信息库类 国家信息库类 = 全局方法类.获取指定名字的国家(国家名字);
		List<将领信息> list = (国家信息库类 == null) ? 全局变量.所有玩家数据表[2].封地信息表[0].将领信息表 : 全局变量.所有玩家数据表[国家信息库类.国王].封地信息表[0].将领信息表;
		List<将领信息> list2 = new List<将领信息>();
		int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i].详细信息.状态 != 0.0)
				{
					continue;
				}
				bool flag = true;
				if (!全局将领库.是否解锁系列将(list[i].将领属性.初始属性.系列))
				{
					flag = false;
				}
				if (list[i].将领属性.初始属性.系列 != "名将")
			{
				if (规模 < 3)
				{
					flag = false;
				}
				if (list[i].将领属性.初始属性.系列 == "君王" && 规模 < 4)
				{
					flag = false;
				}
			}
			if (flag)
			{
				list[i].将领配兵.ID = 104.0;
				list[i].将领配兵.数量 = list[i].将领属性.最终属性.统兵;
				list2.Add(list[i]);
			}
		}
		if (list2.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list2.Count);
			return list2[index];
		}
		return null;
	}

	private 将领信息 随机一个城池驻防将领(int 要生成的等级, string 国家名字)
	{
		int num = -1;
		num = (全局方法类.获取指定名字的国家(国家名字)?.国王 ?? 2);
		if (要生成的等级 > 90)
		{
			if (全局变量.难度 == 2)
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 40.0;
			}
			else
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 25.0;
			}
		}
		else if (要生成的等级 > 70)
		{
			if (全局变量.难度 == 2)
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 35.0;
			}
			else
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 15.0;
			}
		}
		else if (要生成的等级 > 50)
		{
			if (全局变量.难度 == 2)
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 25.0;
			}
			else
			{
				全局变量.所有玩家数据表[num].科技信息.统帅能力 = 5.0;
			}
		}
		else if (全局变量.难度 == 2)
		{
			全局变量.所有玩家数据表[num].科技信息.统帅能力 = 15.0;
		}
		else
		{
			全局变量.所有玩家数据表[num].科技信息.统帅能力 = 5.0;
		}
			int 当前轮回可用ID上限 = Mathf.Clamp(轮回系统.当前轮回数, 1, 3) * 2;
			将领属性库类 将领属性库类 = 全局将领库.查询指定ID的将领数据(UnityEngine.Random.Range(1, Mathf.Min(6, 当前轮回可用ID上限) + 1));
		if (将领属性库类 != null)
		{
			将领属性库类.获取随机属性();
			将领信息 将领信息 = new 将领信息();
			将领信息.生成将领数据(将领属性库类);
			将领信息.将领属性.初始属性.名字 = 随机姓名.生成随机姓名();
			将领信息.将领获取经验值(将领信息.获取升级需要经验(要生成的等级));
			将领信息.详细信息.坑位颜色 = 1.0;
			int count = 全局变量.所有玩家数据表[num].封地信息表[0].将领信息表.Count;
			全局变量.所有玩家数据表[num].封地信息表[0].将领信息表.Add(将领信息);
			全局变量.所有玩家数据表[num].计算最终属性();
			全局变量.所有玩家数据表[num].封地信息表[0].将领信息表.RemoveAt(count);
			int num2 = 100 * UnityEngine.Random.Range(1, 5);
			int num3 = UnityEngine.Random.Range(1, 5);
			num3 = ((要生成的等级 <= 50) ? UnityEngine.Random.Range(1, 4) : 4);
			if (num2 == 400 && num3 == 3)
			{
				num3 = 4;
			}
			int num4 = num2 + num3;
			将领信息.将领配兵.ID = num4;
			将领信息.将领配兵.数量 = 将领信息.将领属性.最终属性.统兵;
			全局变量.所有玩家数据表[num].科技信息.统帅能力 = 8.0;
			return 将领信息;
		}
		return null;
	}

	public void 调整普通难度()
	{
		全局变量.难度 = 1;
		int count = 全局变量.所有玩家数据表.Count;
		全局变量.所有玩家数据表[1].科技信息.升级全部科技(2);
		for (int i = 2; i < count; i++)
		{
			全局变量.所有玩家数据表[i].科技信息.升级全部科技(5);
		}
		全局变量.提示类.显示信息("已调整为普通难度!");
	}

	public void 调整挑战难度()
	{
		全局变量.难度 = 2;
		int count = 全局变量.所有玩家数据表.Count;
		全局变量.所有玩家数据表[1].科技信息.升级全部科技(5);
		for (int i = 2; i < count; i++)
		{
			全局变量.所有玩家数据表[i].科技信息.升级全部科技(15);
		}
		全局变量.提示类.显示信息("已调整为挑战难度!");
	}
}
