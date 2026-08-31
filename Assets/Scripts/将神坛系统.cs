using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 神将图鉴条目
{
	public string 名字;
	public int ID;
	public bool 可合成;
	public bool 已拥有;
	public int 阶位;
}

public class 神将合成结果
{
	public bool 成功;
	public string 消息;
}

public static class 将神坛系统
{
	private static readonly string[] 神将名单 =
	{
		"神·曹操", "神·刘备", "神·孙权", "神·献帝", "神·司马懿", "神·诸葛亮",
		"神·陆逊", "神·关羽", "神·吕布", "神·周瑜", "神·荀彧", "神·徐庶",
		"神·二乔", "神·郭嘉", "神·庞统", "神·鲁肃", "神·貂蝉", "神·姜维",
		"神·甄姬", "神·华佗"
	};

	public static List<神将图鉴条目> 获取图鉴(int 玩家索引 = -1)
	{
		确保进度();
		List<神将图鉴条目> result = new List<神将图鉴条目>();
		玩家数据 玩家 = 获取玩家(玩家索引);
		for (int i = 0; i < 神将名单.Length; i++)
		{
			将领属性库类 定义 = 全局将领库.查询指定名字的将领数据(神将名单[i]);
			if (定义 == null)
			{
				continue;
			}
			神将图鉴条目 item = new 神将图鉴条目();
			item.名字 = 定义.名字;
			item.ID = (int)定义.ID;
			item.可合成 = !全局将领库.是否号令类神君王(item.名字);
			item.已拥有 = 玩家 != null && 玩家.获取指定名字将领数量(item.名字) > 0;
			item.阶位 = 获取神将阶位(玩家, item.名字);
			result.Add(item);
		}
		return result;
	}

	public static string 获取合成材料说明(string 神将名)
	{
		string[] 材料 = 获取合成材料(神将名);
		if (材料 == null)
		{
			return 全局将领库.是否号令类神君王(神将名) ? "号令类神君王：通过成就获得" : "不可合成";
		}
		return (神将名 == "神·二乔" ? "大乔×10，小乔×10" : 材料[0] + "×10") + "，将神珠×1";
	}

	public static string 获取当前阶位说明(string 神将名, int 玩家索引 = -1)
	{
		玩家数据 玩家 = 获取玩家(玩家索引);
		int 阶位 = 获取神将阶位(玩家, 神将名);
		技能信息 技能 = 全局技能库.获取技能(神将名);
		if (技能 == null)
		{
			return "暂无技能说明";
		}
		return 技能.获取阶位说明(阶位);
	}

	public static 神将合成结果 合成(string 神将名, int 玩家索引 = -1)
	{
		确保进度();
		神将合成结果 result = new 神将合成结果();
		玩家数据 玩家 = 获取玩家(玩家索引);
		if (玩家 == null)
		{
			return 失败(result, "玩家数据不存在");
		}
		if (轮回系统.当前轮回数 < 10)
		{
			return 失败(result, "第10轮回开启神将合成");
		}
		if (!全局将领库.是否神将(神将名) || 全局将领库.是否号令类神君王(神将名))
		{
			return 失败(result, "该神将不可合成");
		}
		if (玩家.获取指定名字将领数量(神将名) > 0 || 全局变量.轮回进度.是否已合成神将(神将名))
		{
			return 失败(result, "每个神将只能合成一次");
		}
		if (全局变量.轮回进度.将神珠数量 < 1)
		{
			return 失败(result, "将神珠不足");
		}
		string[] 材料 = 获取合成材料(神将名);
		if (材料 == null || 玩家.获取指定名字将领数量(材料[0]) < 10 || (材料.Length > 1 && 玩家.获取指定名字将领数量(材料[1]) < 10))
		{
			return 失败(result, "合成名将材料不足");
		}
		if (材料.Length == 1)
		{
			玩家.消耗指定名字将领(材料[0], 10);
		}
		else
		{
			玩家.消耗指定名字将领(材料[0], 10);
			玩家.消耗指定名字将领(材料[1], 10);
		}
		全局变量.轮回进度.将神珠数量--;
		将领属性库类 定义 = 全局将领库.查询指定名字的将领数据(神将名);
		玩家.添加指定ID的将领到列表((int)定义.ID);
		设置神将阶位(玩家, 神将名, 0);
		全局变量.轮回进度.记录已合成神将(神将名);
		全局变量.轮回进度.记录已获得神将(神将名);
		全局变量.轮回进度.保存();
		return 成功(result, "合成成功：" + 神将名);
	}

	public static 神将合成结果 升阶(string 神将名, int 玩家索引 = -1)
	{
		确保进度();
		神将合成结果 result = new 神将合成结果();
		玩家数据 玩家 = 获取玩家(玩家索引);
		if (玩家 == null || !全局将领库.是否神将(神将名) || 全局将领库.是否号令类神君王(神将名) || 玩家.获取指定名字将领数量(神将名) <= 0)
		{
			return 失败(result, "尚未拥有该神将");
		}
		int 当前阶位 = 获取神将阶位(玩家, 神将名);
		if (当前阶位 >= 3)
		{
			return 失败(result, "神将已达天阶");
		}
		int 所需轮回 = (当前阶位 + 1) * 10 + 10;
		if (轮回系统.当前轮回数 < 所需轮回)
		{
			return 失败(result, "第" + 所需轮回 + "轮回开启下一阶");
		}
		string[] 升阶材料 = 获取合成材料(神将名);
		string 对应名将 = 升阶材料 == null ? "" : 升阶材料[0];
		if (升阶材料 != null && 升阶材料.Length > 1 && 玩家.获取指定名字将领数量(升阶材料[1]) >= 10)
		{
			对应名将 = 升阶材料[1];
		}
		bool 使用名将 = 对应名将 != "" && 玩家.获取指定名字将领数量(对应名将) >= 10;
		if (使用名将)
		{
			玩家.消耗指定名字将领(对应名将, 10);
		}
		else if (全局变量.轮回进度.神将碎片数量 >= 10)
		{
			全局变量.轮回进度.神将碎片数量 -= 10;
		}
		else
		{
			return 失败(result, "升阶材料不足，需要10名对应名将或10枚神将碎片");
		}
		设置神将阶位(玩家, 神将名, 当前阶位 + 1);
		全局变量.轮回进度.保存();
		return 成功(result, "升阶成功：" + 神将名);
	}

	public static bool 授予神将(string 神将名, int 玩家索引 = -1)
	{
		确保进度();
		玩家数据 玩家 = 获取玩家(玩家索引);
		将领属性库类 定义 = 全局将领库.查询指定名字的将领数据(神将名);
		if (玩家 == null || 定义 == null || !全局将领库.是否号令类神君王(神将名))
		{
			return false;
		}
		if (玩家.获取指定名字将领数量(神将名) > 0 || 全局变量.轮回进度.是否已获得神将(神将名))
		{
			全局变量.轮回进度.记录已获得神将(神将名);
			return false;
		}
		if (!玩家.添加指定ID的将领到列表((int)定义.ID))
		{
			return false;
		}
		设置神将阶位(玩家, 神将名, 0);
		全局变量.轮回进度.记录已获得神将(神将名);
		return true;
	}

	private static string[] 获取合成材料(string 神将名)
	{
		if (!全局将领库.是否神将(神将名) || 全局将领库.是否号令类神君王(神将名))
		{
			return null;
		}
		if (神将名 == "神·二乔")
		{
			return new string[] { "大乔", "小乔" };
		}
		return new string[] { 神将名.Substring(2) };
	}

	private static void 确保进度()
	{
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 轮回进度类.读取();
		}
		全局变量.轮回进度.修复旧数据();
	}

	private static int 获取神将阶位(玩家数据 玩家, string 神将名)
	{
		if (玩家 == null)
		{
			return 0;
		}
		for (int i = 0; i < 玩家.封地信息表.Count; i++)
		{
			for (int j = 0; j < 玩家.封地信息表[i].将领信息表.Count; j++)
			{
				将领信息 将领 = 玩家.封地信息表[i].将领信息表[j];
				if (将领 != null && 将领.将领属性 != null && 将领.将领属性.初始属性.名字 == 神将名)
				{
					return Mathf.Clamp(将领.阶位, 0, 3);
				}
			}
		}
		return 0;
	}

	private static void 设置神将阶位(玩家数据 玩家, string 神将名, int 阶位)
	{
		for (int i = 0; i < 玩家.封地信息表.Count; i++)
		{
			for (int j = 0; j < 玩家.封地信息表[i].将领信息表.Count; j++)
			{
				将领信息 将领 = 玩家.封地信息表[i].将领信息表[j];
				if (将领 != null && 将领.将领属性 != null && 将领.将领属性.初始属性.名字 == 神将名)
				{
					将领.阶位 = Mathf.Clamp(阶位, 0, 3);
				}
			}
		}
		if (全局变量.轮回进度 != null)
		{
			全局变量.轮回进度.设置神将阶位(神将名, 阶位);
		}
		玩家.计算最终属性();
	}

	private static 玩家数据 获取玩家(int 玩家索引)
	{
		int index = 玩家索引 < 0 ? 全局变量.本机身份 : 玩家索引;
		if (全局变量.所有玩家数据表 == null || index < 0 || index >= 全局变量.所有玩家数据表.Count)
		{
			return null;
		}
		return 全局变量.所有玩家数据表[index];
	}

	private static 神将合成结果 成功(神将合成结果 result, string message)
	{
		result.成功 = true;
		result.消息 = message;
		显示提示(message);
		return result;
	}

	private static 神将合成结果 失败(神将合成结果 result, string message)
	{
		result.成功 = false;
		result.消息 = message;
		显示提示(message);
		return result;
	}

	private static void 显示提示(string message)
	{
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息(message);
		}
	}
}

public class 将神坛脚本 : MonoBehaviour
{
	public Transform 图鉴列表对象;
	public GameObject 图鉴条目预制体;
	public Text 技能说明;
	public Text 材料说明;
	public Button 合成按钮;
	public Button 升阶按钮;
	private string 当前神将名;

	public void 刷新图鉴()
	{
		List<神将图鉴条目> list = 将神坛系统.获取图鉴();
		if (图鉴列表对象 == null || 图鉴条目预制体 == null)
		{
			return;
		}
		for (int i = 图鉴列表对象.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(图鉴列表对象.GetChild(i).gameObject);
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject item = UnityEngine.Object.Instantiate(图鉴条目预制体, 图鉴列表对象);
			Text label = item.GetComponentInChildren<Text>();
			if (label != null)
			{
				label.text = list[j].名字 + (list[j].已拥有 ? " [已拥有]" : "");
			}
		}
	}

	public void 选择神将(string 神将名)
	{
		当前神将名 = 神将名;
		if (技能说明 != null)
		{
			技能说明.text = 将神坛系统.获取当前阶位说明(神将名);
		}
		if (材料说明 != null)
		{
			材料说明.text = 将神坛系统.获取合成材料说明(神将名);
		}
	}

	public void 点击合成()
	{
		将神坛系统.合成(当前神将名);
		刷新图鉴();
	}

	public void 点击升阶()
	{
		将神坛系统.升阶(当前神将名);
		刷新图鉴();
	}
}
