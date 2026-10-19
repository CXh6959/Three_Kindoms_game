using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 选择出征将领 : MonoBehaviour
{
	private int 第几个玩家 = 全局变量.本机身份;

	private int 当前选中封地 = 全局变量.第几个封地;

	public int 山贼坐标x;

	public int 山贼坐标y;

	public int 城池坐标x;

	public int 城池坐标y;

	public GameObject 山贼信息对象;

	public GameObject 编队切换对象;

	public GameObject 编队将领对象;

	public Text 精准到达_时;

	public Text 精准到达_分;

	public Text 精准到达_秒;

	private List<GameObject> 所有列表将领对象 = new List<GameObject>();

	private List<返回将领索引> 已显示将领列表 = new List<返回将领索引>();

	private List<返回将领索引> 已选中将领列表 = new List<返回将领索引>();

	private 战斗系统 增援目标;
	private bool 增援模式;

	public void 打开增援(战斗系统 目标)
	{
		增援目标 = 目标;
		增援模式 = true;
		第几个玩家 = 全局变量.本机身份;
		当前选中封地 = 全局变量.第几个封地;
		已选中将领列表.Clear();
		所有列表将领对象.Clear();
		for (int i = 编队将领对象.transform.childCount - 1; i > 0; i--)
		{
			GameObject 旧条目 = 编队将领对象.transform.GetChild(i).gameObject;
			旧条目.SetActive(false);
			Destroy(旧条目);
		}
		gameObject.SetActive(true);
		编队切换对象.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
		if (目标.战场类型 == 1)
		{
			城池坐标x = 目标.坐标x;
			城池坐标y = 目标.坐标y;
			显示城池详情();
		}
		else if (目标.战场类型 == 0)
		{
			山贼坐标x = 目标.坐标x;
			山贼坐标y = 目标.坐标y;
			显示山贼详情();
			山贼信息对象.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = "";
		}
		else
		{
			山贼信息对象.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = 全局将领库.获取指定将领的头像("蚩尤");
			山贼信息对象.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "轮回副本";
			山贼信息对象.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "第" + 轮回系统.当前轮回数 + "轮回";
			山贼信息对象.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "";
			for (int i = 0; i < 3; i++) 山贼信息对象.transform.GetChild(2).GetChild(i).GetComponent<Text>().text = "";
			山贼信息对象.transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Text>().text = "攻方兵力:" + 战斗数值格式.显示(目标.攻方兵力) + "\n守方兵力:" + 战斗数值格式.显示(目标.守方兵力);
		}
		Font 字体 = 山贼信息对象.transform.GetChild(0).GetChild(1).GetComponent<Text>().font;
		替换图片文字(transform.Find("出征界面背景布局/标题栏背景/8 (21)"), "战场增援", 字体);
		替换图片文字(transform.Find("攻占城池界面操作/攻占/Image"), "增援", 字体);
		Transform 时间设置 = transform.Find("时间设置布局");
		if (时间设置 != null) 时间设置.gameObject.SetActive(false);
		Transform 切换 = transform.Find("切换封地");
		if (切换 != null) 切换.gameObject.SetActive(false);
		显示编队将领列表();
	}

	private static void 替换图片文字(Transform 位置, string 标题, Font 字体)
	{
		if (位置 == null) return;
		Image 原图 = 位置.GetComponent<Image>();
		if (原图 != null) 原图.enabled = false;
		GameObject 文本对象 = new GameObject("文字", typeof(RectTransform), typeof(Text));
		文本对象.layer = 位置.gameObject.layer;
		文本对象.transform.SetParent(位置, false);
		Text 文本 = 文本对象.GetComponent<Text>();
		文本.rectTransform.anchorMin = Vector2.zero;
		文本.rectTransform.anchorMax = Vector2.one;
		文本.rectTransform.sizeDelta = Vector2.zero;
		文本.text = 标题;
		文本.font = 字体;
		文本.fontSize = 20;
		文本.resizeTextForBestFit = true;
		文本.resizeTextMinSize = 12;
		文本.resizeTextMaxSize = 20;
		文本.alignment = TextAnchor.MiddleCenter;
		文本.color = new Color(0.94f, 0.83f, 0.36f, 1f);
		文本.raycastTarget = false;
	}

	private void OnDisable()
	{
		if (增援模式) Destroy(gameObject);
	}

	private void 提交增援()
	{
		List<将领信息> 队伍 = new List<将领信息>();
		玩家数据 玩家 = 全局变量.所有玩家数据表[第几个玩家];
		foreach (返回将领索引 索引 in 已选中将领列表)
		{
			if (索引.第几个封地 < 0 || 索引.第几个封地 >= 玩家.封地信息表.Count) continue;
			List<将领信息> 将领 = 玩家.封地信息表[索引.第几个封地].将领信息表;
			if (索引.第几个将领 >= 0 && 索引.第几个将领 < 将领.Count) 队伍.Add(将领[索引.第几个将领]);
		}
		bool 成功 = 战场增援规则.出征(增援目标, 队伍, 第几个玩家);
		全局变量.提示类.显示信息(成功 ? "增援已出发，10秒后抵达当前战场" : "增援失败，请检查战场和将领的兵力、体力、状态");
		已选中将领列表.Clear();
		显示编队将领列表();
	}

	public void 加入军情队列(int 战场类型, int 坐标x, int 坐标y, long 到达时间, List<将领信息> 要加入的将领列表)
	{
		军情信息 军情信息 = new 军情信息();
		军情信息.战场类型 = 战场类型;
		军情信息.坐标x = 坐标x;
		军情信息.坐标y = 坐标y;
		军情信息.到达时间 = 到达时间;
		军情信息.队列将领列表 = 要加入的将领列表;
		军情信息.已进入战场 = false;
		全局变量.军情列表.Add(军情信息);
	}

	public void 山贼_出征选中将领()
	{
		if (增援模式) { 提交增援(); return; }
		List<将领信息> list = new List<将领信息>();
		int count = 已选中将领列表.Count;
		for (int i = 0; i < count; i++)
		{
			返回将领索引 返回将领索引 = 已选中将领列表[i];
			全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领].详细信息.坑位颜色 = 0.0;
			全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领].详细信息.状态 = 1.0;
			list.Add(全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领]);
		}
		if (count != 0)
		{
			long 到达时间 = TIME.getTime() + 10;
			加入军情队列(0, 山贼坐标x, 山贼坐标y, 到达时间, list);
			已选中将领列表.Clear();
			显示编队将领列表();
			全局变量.提示类.显示信息("出征成功!");
		}
	}

	public void 城池_出征选中将领()
	{
		if (增援模式) { 提交增援(); return; }
		List<将领信息> list = new List<将领信息>();
		int count = 已选中将领列表.Count;
		for (int i = 0; i < count; i++)
		{
			返回将领索引 返回将领索引 = 已选中将领列表[i];
			全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领].详细信息.坑位颜色 = 0.0;
			全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领].详细信息.状态 = 1.0;
			list.Add(全局变量.所有玩家数据表[全局变量.本机身份].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领]);
		}
		if (count != 0)
		{
			long num = TIME.getTime() + 10;
			if (精准到达_时.text != "" && 精准到达_分.text != "" && 精准到达_秒.text != "")
			{
				DateTime dateTime = TIME.TimeStampToDateTime(num);
				num = TIME.DateTimeToTimeStamp(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, int.Parse(精准到达_时.text), int.Parse(精准到达_分.text), int.Parse(精准到达_秒.text)));
			}
			加入军情队列(1, 城池坐标x, 城池坐标y, num, list);
			已选中将领列表.Clear();
			显示编队将领列表();
			全局变量.提示类.显示信息("出征成功!");
		}
	}

	public void 副本_出征选中将领()
	{
		if (增援模式) { 提交增援(); return; }
		List<将领信息> list = new List<将领信息>();
		for (int i = 0; i < 已选中将领列表.Count; i++)
		{
			返回将领索引 返回将领索引 = 已选中将领列表[i];
			将领信息 将领 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[返回将领索引.第几个封地].将领信息表[返回将领索引.第几个将领];
			list.Add(将领);
		}
		if (轮回副本系统.发起副本(list, 第几个玩家))
		{
			已选中将领列表.Clear();
			显示编队将领列表();
			全局变量.提示类.显示信息("副本出征成功!");
		}
		else
		{
			全局变量.提示类.显示信息("副本出征失败，请选择已配兵的将领并确认当前没有副本队伍。");
		}
	}

	public void 批量补兵()
	{
		玩家数据 玩家 = 全局变量.所有玩家数据表[第几个玩家];
		玩家.计算最终属性();
		int 人数 = 0;
		// 未配兵条目不能勾选出征，因此按当前列表补兵，而不是只遍历已选中的队伍。
		foreach (返回将领索引 索引 in 已显示将领列表)
		{
			if (!增援模式 && 索引.第几个封地 != 当前选中封地) continue;
			封地信息 封地 = 玩家.封地信息表[索引.第几个封地];
			if (补兵规则.补满(封地, 封地.将领信息表[索引.第几个将领]) > 0.0) 人数++;
		}
		显示编队将领列表();
		全局变量.提示类.显示信息("已为" + 人数 + "名将领补兵");
	}

	public void 刷新山贼()
	{
		当前选中封地 = 全局变量.第几个封地;
		已选中将领列表.Clear();
		显示山贼详情();
		显示编队将领列表();
	}

	public void 刷新城池()
	{
		当前选中封地 = 全局变量.第几个封地;
		已选中将领列表.Clear();
		显示城池详情();
		显示编队将领列表();
	}

	private void 显示城池详情()
	{
		城池信息库类 城池信息库类 = 所有城池界面脚本.根据坐标获取指定城池(城池坐标x, 城池坐标y);
		山贼信息对象.transform.GetChild(0).GetChild(0).GetComponent<Image>()
			.sprite = 全局变量.城池规模头像资源表[城池信息库类.规模];
			山贼信息对象.transform.GetChild(0).GetChild(1).GetComponent<Text>()
				.text = 城池信息库类.名称 + "(" + 城池信息库类.获取规模名称() + "城" + 城池信息库类.坐标x.ToString() + "," + 城池信息库类.坐标y.ToString() + ")  国家:" + 城池信息库类.获取国家名字();
				山贼信息对象.transform.GetChild(0).GetChild(2).GetComponent<Text>()
					.text = "天赋:" + 城池信息库类.获取天赋类型名称() + "+" + 城池信息库类.天赋加成.ToString() + "%  封地:" + 城池信息库类.城池封地列表.Count.ToString() + "/" + 城池信息库类.获取封地上限().ToString();
					山贼信息对象.transform.GetChild(0).GetChild(3).GetComponent<Text>()
						.text = "城主:" + 城池信息库类.获取城主名字();
						山贼信息对象.transform.GetChild(2).GetChild(0).GetComponent<Text>()
							.text = "驻防将领:" + 城池信息库类.城池驻防列表.Count.ToString() + "/" + 城池信息库类.获取驻防上限().ToString();
							山贼信息对象.transform.GetChild(2).GetChild(1).GetComponent<Text>()
								.text = "道路:" + 城池信息库类.获取道路上限().ToString();
								山贼信息对象.transform.GetChild(2).GetChild(2).GetComponent<Text>()
									.text = "城墙:" + 城池信息库类.城墙.ToString();
									山贼信息对象.transform.GetChild(2).GetChild(3).GetChild(0)
										.GetComponent<Text>()
										.text = ((城池信息库类.规模 == 3 || 城池信息库类.规模 == 4)
											? "【名将协防】\n每名空闲无归属名将20%\n可同时多名，击败15%俘获\n出征封地【俘虏】页劝降"
											: "【名将协防】\n仅州城、都城出现\n本城不出现系统名将") + "\n【攻克奖励】\n战功 + " + 城池信息库类.战功.ToString() + "\n国库铜钱 + 100000\n国库粮食 + 200000";
									}

									private void 显示山贼详情()
									{
										山贼属性信息 山贼属性信息 = 附近山贼.获取指定坐标的山贼(山贼坐标x, 山贼坐标y);
										if (山贼属性信息 != null)
										{
											double 等级 = 山贼属性信息.等级;
											Image component = 山贼信息对象.transform.GetChild(0).GetChild(0).GetComponent<Image>();
											int num = (int)等级 - 1;
											component.sprite = 全局变量.山贼头像资源表[num];
											山贼信息对象.transform.GetChild(0).GetChild(1).GetComponent<Text>()
												.text = 等级.ToString() + "级山贼 (" + 山贼坐标x.ToString() + "," + 山贼坐标y.ToString() + ")";
												int count = 山贼属性信息.将领数据列表.Count;
												string text = "";
												double num2 = 0.0;
												for (int i = 0; i < count; i++)
												{
													double iD = 山贼属性信息.将领数据列表[i].将领配兵.ID;
													double 数量 = 山贼属性信息.将领数据列表[i].将领配兵.数量;
													num2 += 数量;
													int num3 = 全局兵种库.查询指定ID的索引(iD);
													text = ((num3 == -1) ? (text + "未知 " + 数量.ToString() + "\n") : (text + 全局兵种库.属性表[num3].名称 + " " + 数量.ToString() + "\n"));
												}
												山贼信息对象.transform.GetChild(0).GetChild(2).GetComponent<Text>()
													.text = "兵力:" + num2.ToString();
													string text2 = "奖励资源";
													if (山贼属性信息.掉落宝物 == 1.0)
													{
														text2 += "、宝物";
													}
													if (山贼属性信息.掉落宝箱 == 1.0)
													{
														text2 += "、宝箱";
													}
													if (山贼属性信息.掉落装备 == 1.0)
													{
														text2 += "、装备";
													}
													山贼信息对象.transform.GetChild(0).GetChild(3).GetComponent<Text>()
														.text = text2;
														山贼信息对象.transform.GetChild(2).GetChild(0).GetComponent<Text>()
															.text = "敌军等级:" + 山贼属性信息.将领数据列表[0].将领属性.成长点数.等级.ToString() + "级";
															山贼信息对象.transform.GetChild(2).GetChild(1).GetComponent<Text>()
																.text = "敌军规模:" + count.ToString() + "名";
																山贼信息对象.transform.GetChild(2).GetChild(3).GetChild(0)
																	.GetComponent<Text>()
																	.text = text;
																}
															}

															private void 显示编队将领列表()
															{
																已显示将领列表.Clear();
																int count = 全局变量.所有玩家数据表[第几个玩家].封地信息表.Count;
																int num = 0;
																for (int i = 0; i < count; i++)
																{
																	int count2 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表.Count;
																	for (int j = 0; j < count2; j++)
																	{
																		double 编队 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].详细信息.编队;
																		double iD2 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.初始属性.ID;
																		bool flag = false;
																		if (编队切换对象.transform.GetChild(0).GetComponent<Toggle>().isOn)
																		{
																			if (i == 当前选中封地 || 增援模式)
																			{
																				flag = true;
																			}
																		}
																		else if (编队切换对象.transform.GetChild(1).GetComponent<Toggle>().isOn)
																		{
																			if (编队 == 1.0)
																			{
																				flag = true;
																			}
																		}
																		else if (编队切换对象.transform.GetChild(2).GetComponent<Toggle>().isOn)
																		{
																			if (编队 == 2.0)
																			{
																				flag = true;
																			}
																		}
																		else if (编队切换对象.transform.GetChild(3).GetComponent<Toggle>().isOn)
																		{
																			if (编队 == 3.0)
																			{
																				flag = true;
																			}
																		}
																		else if (编队切换对象.transform.GetChild(4).GetComponent<Toggle>().isOn)
																		{
																			if (编队 == 4.0)
																			{
																				flag = true;
																			}
																		}
																		else if (编队切换对象.transform.GetChild(5).GetComponent<Toggle>().isOn && 编队 == 5.0)
																		{
																			flag = true;
																		}
																		if (!flag)
																		{
																			continue;
																		}
																		GameObject gameObject;
																		if (所有列表将领对象.Count <= num)
																		{
																			gameObject = UnityEngine.Object.Instantiate(编队将领对象.transform.GetChild(0).gameObject);
																			gameObject.transform.SetParent(编队将领对象.transform);
																			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
																			所有列表将领对象.Add(gameObject);
																		}
																		else
																		{
																			gameObject = 所有列表将领对象[num];
																		}
																		gameObject.gameObject.SetActive(value: true);
																		gameObject.transform.GetChild(8).gameObject.SetActive(value: false);
																		将领属性库类 将领属性库类 = 全局将领库.查询指定ID的将领数据(全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.初始属性.ID);
																		if (将领属性库类 != null)
																		{
																			gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>()
																				.sprite = 全局将领库.获取指定将领的头像(将领属性库类.名字);
																				Animator component = gameObject.transform.GetChild(1).GetChild(1).GetComponent<Animator>();
																				gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(value: false);
																				if (将领属性库类.头像特效 != 0.0)
																				{
																					gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(value: true);
																					component.SetInteger("特效类型", (int)将领属性库类.头像特效);
																				}
																			}
																			Text component2 = gameObject.transform.GetChild(2).GetComponent<Text>();
																			string 名字 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.初始属性.名字;
																			double 等级 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.成长点数.等级;
																			string text = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.初始属性.获取职业名字();
																			component2.text = 名字 + "(" + 等级.ToString() + "级" + text + ")";
																			Text component3 = gameObject.transform.GetChild(4).GetComponent<Text>();
																			double iD = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领配兵.ID;
																			if (iD != 0.0 && 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领配兵.数量 > 0.0)
																			{
																				double 数量 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领配兵.数量;
																				double 统兵 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.最终属性.统兵;
																				string text2 = "未知";
																				if (全局兵种库.查询指定ID的索引(iD) != -1)
																				{
																					text2 = 全局兵种库.属性表[全局兵种库.查询指定ID的索引(iD)].名称;
																					Image component4 = gameObject.transform.GetChild(3).GetComponent<Image>();
																					int num2 = 全局兵种库.查询指定兵种的图标(text2);
																					if (num2 != -1)
																					{
																						component4.sprite = 全局变量.所有兵种图标资源表[num2];
																					}
																				}
																				component3.text = 数量.ToString() + "/" + 统兵.ToString() + "(" + text2 + ")";
																			}
																			else
																			{
																				gameObject.transform.GetChild(8).gameObject.SetActive(value: true);
																				component3.text = "未配兵";
																			}
																			Text component5 = gameObject.transform.GetChild(6).GetComponent<Text>();
																			component5.text = 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].详细信息.剩余体力.ToString() + "/" + 全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].将领属性.最终属性.体力上限.ToString();
																			if (全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].详细信息.剩余体力 < 5.0)
																			{
																				gameObject.transform.GetChild(8).gameObject.SetActive(value: true);
																			}
																			if (全局变量.所有玩家数据表[第几个玩家].封地信息表[i].将领信息表[j].详细信息.状态 != 0.0)
																			{
																				gameObject.transform.GetChild(8).gameObject.SetActive(value: true);
																			}
																			if (i != 当前选中封地 && !增援模式)
																			{
																				gameObject.transform.GetChild(8).gameObject.SetActive(value: true);
																			}
																			gameObject.transform.GetChild(7).gameObject.SetActive(value: false);
																			if (!gameObject.transform.GetChild(8).gameObject.activeSelf)
																			{
																				if (gameObject.transform.GetChild(9).gameObject.activeSelf)
																				{
																					component2.color = 颜色类.GetColor("#49FBDA");
																					component3.color = 颜色类.GetColor("#2DBE5A");
																					component5.color = 颜色类.GetColor("#2DBE5A");
																				}
																				else
																				{
																					component2.color = 颜色类.GetColor("#C8C8C8");
																					component3.color = 颜色类.GetColor("#C8C8C8");
																					component5.color = 颜色类.GetColor("#C8C8C8");
																				}
																				bool flag2 = false;
																				for (int k = 0; k < 已选中将领列表.Count; k++)
																				{
																					if (已选中将领列表[k].第几个将领 == j && 已选中将领列表[k].第几个封地 == i)
																					{
																						flag2 = true;
																						break;
																					}
																				}
																				if (flag2)
																				{
																					gameObject.transform.GetChild(7).gameObject.SetActive(value: true);
																				}
																			}
																			已显示将领列表.Add(new 返回将领索引(i, j));
																			if (已选中将领列表.Count > 4 && !gameObject.transform.GetChild(7).gameObject.activeSelf)
																			{
																				gameObject.transform.GetChild(8).gameObject.SetActive(value: true);
																			}
																			num++;
																		}
																	}
																}

																public void 输出列表()
																{
																	for (int i = 0; i < 已选中将领列表.Count; i++)
																	{
																		UnityEngine.Debug.Log("已选：" + 已选中将领列表[i]?.ToString());
																	}
																	for (int j = 0; j < 已显示将领列表.Count; j++)
																	{
																		UnityEngine.Debug.Log("已显示" + 已显示将领列表[j]?.ToString());
																	}
																}

																private void 隐藏所有列表将领对象()
																{
																	已选中将领列表.Clear();
																	已显示将领列表.Clear();
																	int count = 所有列表将领对象.Count;
																	for (int i = 0; i < count; i++)
																	{
																		所有列表将领对象[i].gameObject.SetActive(value: false);
																	}
																}

																public void 勾选出征将领(int 勾选类型)
																{
																	int count = 所有列表将领对象.Count;
																	for (int i = 0; i < count; i++)
																	{
																		if (!所有列表将领对象[i].gameObject.activeSelf || 所有列表将领对象[i].transform.GetChild(8).gameObject.activeSelf)
																		{
																			continue;
																		}
																		bool flag = false;
																		if (所有列表将领对象[i].transform.GetChild(9).gameObject.activeSelf && 勾选类型 == 1)
																		{
																			flag = true;
																		}
																		if (!编队切换对象.transform.GetChild(0).GetComponent<Toggle>().isOn && 勾选类型 == 0)
																		{
																			flag = true;
																		}
																		if (!flag)
																		{
																			continue;
																		}
																		bool flag2 = true;
																		int count2 = 已选中将领列表.Count;
																		for (int j = 0; j < 已选中将领列表.Count; j++)
																		{
																			if (已选中将领列表[j].第几个将领 == 已显示将领列表[i].第几个将领 && 已选中将领列表[j].第几个封地 == 已显示将领列表[i].第几个封地)
																			{
																				已选中将领列表.RemoveAt(j);
																				flag2 = false;
																				break;
																			}
																		}
																		if (flag2 && count2 < 5)
																		{
																			已选中将领列表.Add(已显示将领列表[i]);
																		}
																	}
																	显示编队将领列表();
																}


																public void 切换编队()
																{
																	隐藏所有列表将领对象();
																	显示编队将领列表();
																	勾选出征将领(0);
																}
															}
