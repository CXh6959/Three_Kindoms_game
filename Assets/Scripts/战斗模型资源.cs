using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

public static class 战斗模型资源
{
	private static readonly Dictionary<string, Sprite> 缓存 = new Dictionary<string, Sprite>();

	public static string 获取资源名(将领信息 将领)
	{
		将领属性库类 定义 = 将领身份规则.获取定义(将领);
		if (定义 != null && 定义.系列 == "神将")
		{
			switch (定义.名字)
			{
			case "青龙": case "白虎": case "朱雀": case "玄武": case "蚩尤": return 定义.名字;
			}
		}
		if (将领 == null || 将领.将领配兵 == null) return null;
		switch ((int)将领.将领配兵.ID)
		{
		case 105: return "象骑兵";
		case 205: return "藤甲兵";
		case 305: return "火油兵";
		case 405: return "玄甲兵";
		default: return null;
		}
	}

	public static float 获取大小倍率(string 名字)
	{
		switch (名字)
		{
		case "蚩尤": return 2f;
		case "青龙": case "白虎": case "朱雀": case "玄武": return 1.5f;
		default: return 1f;
		}
	}

	public static GameObject 创建(将领信息 将领, Transform 父对象)
	{
		string 名字 = 获取资源名(将领);
		if (名字 == null) return null;
		Sprite 图片;
		if (!缓存.TryGetValue(名字, out 图片))
		{
			图片 = Resources.Load<Sprite>("战斗替换模型/" + 名字);
			缓存[名字] = 图片;
		}
		if (图片 == null) return null;
		GameObject 模型 = new GameObject(名字);
		模型.transform.SetParent(父对象, false);
		SpriteRenderer 渲染 = 模型.AddComponent<SpriteRenderer>();
		渲染.sprite = 图片;
		渲染.flipX = 将领.详细信息.坑位颜色 == 0.0;
		float 倍率 = 获取大小倍率(名字);
		模型.transform.localScale = new Vector3(倍率, 倍率, 1f);
		return 模型;
	}
}
