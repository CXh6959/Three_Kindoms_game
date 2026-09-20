using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 玩家数据结构;

public class 新增功能入口 : MonoBehaviour
{
	private static readonly Color 背景色 = new Color32(22, 24, 27, 246);
	private static readonly Color 面板色 = new Color32(39, 43, 48, 255);
	private static readonly Color 按钮色 = new Color32(76, 83, 91, 255);
	private static readonly Color 强调色 = new Color32(184, 139, 62, 255);
	private static readonly Color 文字色 = new Color32(242, 239, 231, 255);
	private static readonly Color 次要文字色 = new Color32(184, 188, 193, 255);

	private GameObject 界面根对象;
	private GameObject 面板对象;
	private GameObject 退出确认对象;
	private RectTransform 安全区域对象;
	private RectTransform 内容对象;
	private Text 标题文本;
	private Text 状态文本;
	private Font 界面字体;
	private string 当前神将名;
	private int 神将页码;
	private int 上次屏幕宽度;
	private int 上次屏幕高度;
	private Rect 上次安全区域;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void 创建常驻入口()
	{
		if (FindObjectOfType<新增功能入口>() != null)
		{
			return;
		}
		GameObject host = new GameObject("新增功能入口");
		DontDestroyOnLoad(host);
		host.AddComponent<新增功能入口>();
	}

	private void Awake()
	{
		SceneManager.sceneLoaded += 场景加载完成;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= 场景加载完成;
	}

	private void 场景加载完成(Scene scene, LoadSceneMode mode)
	{
		StopAllCoroutines();
		if (界面根对象 != null)
		{
			Destroy(界面根对象);
			界面根对象 = null;
		}
		面板对象 = null;
		退出确认对象 = null;
		安全区域对象 = null;
		内容对象 = null;
		标题文本 = null;
		状态文本 = null;
		if (scene.buildIndex >= 0)
		{
			StartCoroutine(延迟创建界面(scene.buildIndex == 1));
		}
	}

	private IEnumerator 延迟创建界面(bool 创建功能面板)
	{
		yield return null;
		创建界面(创建功能面板);
	}

	private void 创建界面(bool 创建功能面板)
	{
		界面字体 = 查找界面字体();
		确保事件系统();

		界面根对象 = new GameObject("新增功能画布", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		界面根对象.transform.SetParent(transform, false);
		Canvas canvas = 界面根对象.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 32000;
		CanvasScaler scaler = 界面根对象.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.matchWidthOrHeight = 0.5f;

		安全区域对象 = 创建拉伸对象("安全区域", 界面根对象.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		应用安全区域();
		float 退出按钮横坐标 = 创建功能面板 ? -270f : -96f;
		Button exit = 创建按钮("退出游戏", 安全区域对象, "退出", new Vector2(150f, 58f), new Vector2(退出按钮横坐标, -44f), new Vector2(1f, 1f), 按钮色, 26);
		exit.onClick.AddListener(打开退出确认);
		创建退出确认界面();
		if (!创建功能面板)
		{
			return;
		}

		Button entry = 创建按钮("功能入口", 安全区域对象, "功能", new Vector2(150f, 58f), new Vector2(-96f, -44f), new Vector2(1f, 1f), 强调色, 26);
		entry.onClick.AddListener(() => 打开面板());

		面板对象 = 创建拉伸对象("新增功能遮罩", 界面根对象.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
		面板对象.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.62f);
		Button 遮罩按钮 = 面板对象.AddComponent<Button>();
		遮罩按钮.transition = Selectable.Transition.None;
		遮罩按钮.onClick.AddListener(关闭面板);

		RectTransform panel = 创建固定对象("新增功能面板", 面板对象.transform, new Vector2(1200f, 760f), Vector2.zero, new Vector2(0.5f, 0.5f));
		panel.gameObject.AddComponent<Image>().color = 背景色;
		Button 阻止穿透 = panel.gameObject.AddComponent<Button>();
		阻止穿透.transition = Selectable.Transition.None;

		RectTransform header = 创建拉伸对象("标题栏", panel, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -72f), Vector2.zero);
		header.gameObject.AddComponent<Image>().color = 面板色;
		标题文本 = 创建文本("标题", header, "轮回功能", 32, TextAnchor.MiddleLeft, 文字色);
		RectTransform 标题矩形 = 标题文本.rectTransform;
		标题矩形.offsetMin = new Vector2(28f, 0f);
		标题矩形.offsetMax = new Vector2(-90f, 0f);
		Button close = 创建按钮("关闭", header, "X", new Vector2(60f, 48f), new Vector2(-42f, -36f), new Vector2(1f, 1f), 按钮色, 26);
		close.onClick.AddListener(关闭面板);

		RectTransform tabs = 创建拉伸对象("功能导航", panel, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(16f, 68f), new Vector2(220f, -88f));
		创建导航按钮(tabs, "概况", 0, 显示概况);
		创建导航按钮(tabs, "将神坛", 1, 显示将神坛);
		创建导航按钮(tabs, "元宝抽奖", 2, 显示抽奖);
		创建导航按钮(tabs, "成就", 3, 显示成就);

		内容对象 = 创建拉伸对象("功能内容", panel, Vector2.zero, Vector2.one, new Vector2(236f, 68f), new Vector2(-18f, -88f));
		状态文本 = 创建文本("状态", panel, "", 23, TextAnchor.MiddleLeft, 次要文字色);
		状态文本.rectTransform.anchorMin = Vector2.zero;
		状态文本.rectTransform.anchorMax = new Vector2(1f, 0f);
		状态文本.rectTransform.offsetMin = new Vector2(236f, 8f);
		状态文本.rectTransform.offsetMax = new Vector2(-18f, 60f);

		面板对象.SetActive(false);
	}

	private void 创建退出确认界面()
	{
		退出确认对象 = 创建拉伸对象("退出确认遮罩", 界面根对象.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
		退出确认对象.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
		Button 遮罩按钮 = 退出确认对象.AddComponent<Button>();
		遮罩按钮.transition = Selectable.Transition.None;
		遮罩按钮.onClick.AddListener(关闭退出确认);

		RectTransform panel = 创建固定对象("退出确认面板", 退出确认对象.transform, new Vector2(620f, 300f), Vector2.zero, new Vector2(0.5f, 0.5f));
		panel.gameObject.AddComponent<Image>().color = 背景色;
		Button 阻止穿透 = panel.gameObject.AddComponent<Button>();
		阻止穿透.transition = Selectable.Transition.None;

		Text title = 创建文本("退出标题", panel, "退出游戏？", 38, TextAnchor.MiddleCenter, 文字色);
		title.rectTransform.anchorMin = new Vector2(0f, 0.42f);
		title.rectTransform.anchorMax = Vector2.one;
		title.rectTransform.offsetMin = new Vector2(24f, 0f);
		title.rectTransform.offsetMax = new Vector2(-24f, -12f);

		Button cancel = 创建按钮("取消退出", panel, "取消", new Vector2(220f, 68f), new Vector2(-125f, 62f), new Vector2(0.5f, 0f), 按钮色, 28);
		cancel.onClick.AddListener(关闭退出确认);
		Button confirm = 创建按钮("确认退出", panel, "退出", new Vector2(220f, 68f), new Vector2(125f, 62f), new Vector2(0.5f, 0f), 强调色, 28);
		confirm.onClick.AddListener(确认退出游戏);
		退出确认对象.SetActive(false);
	}

	private void 创建导航按钮(RectTransform parent, string label, int index, UnityEngine.Events.UnityAction action)
	{
		Button button = 创建按钮(label, parent, label, new Vector2(204f, 62f), new Vector2(102f, -40f - index * 74f), new Vector2(0.5f, 1f), 按钮色, 25);
		button.onClick.AddListener(action);
	}

	private void Update()
	{
		if (安全区域对象 != null && (上次屏幕宽度 != Screen.width || 上次屏幕高度 != Screen.height || 上次安全区域 != Screen.safeArea))
		{
			应用安全区域();
		}
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		if (退出确认对象 != null && 退出确认对象.activeSelf)
		{
			关闭退出确认();
		}
		else if (面板对象 != null && 面板对象.activeSelf)
		{
			关闭面板();
		}
		else
		{
			打开退出确认();
		}
	}

	private void 打开面板()
	{
		面板对象.SetActive(true);
		显示概况();
	}

	private void 关闭面板()
	{
		if (面板对象 != null)
		{
			面板对象.SetActive(false);
		}
	}

	private void 打开退出确认()
	{
		if (退出确认对象 == null)
		{
			return;
		}
		退出确认对象.transform.SetAsLastSibling();
		退出确认对象.SetActive(true);
	}

	private void 关闭退出确认()
	{
		if (退出确认对象 != null)
		{
			退出确认对象.SetActive(false);
		}
	}

	private static void 确认退出游戏()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private void 应用安全区域()
	{
		if (安全区域对象 == null || Screen.width <= 0 || Screen.height <= 0)
		{
			return;
		}
		Rect safeArea = Screen.safeArea;
		安全区域对象.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
		安全区域对象.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
		安全区域对象.offsetMin = Vector2.zero;
		安全区域对象.offsetMax = Vector2.zero;
		上次屏幕宽度 = Screen.width;
		上次屏幕高度 = Screen.height;
		上次安全区域 = safeArea;
	}

	private void 显示概况()
	{
		清空内容("轮回概况");
		if (!玩家数据可用())
		{
			显示状态("玩家数据尚未初始化，请从开始游戏场景进入游戏。");
			return;
		}
		玩家数据 玩家 = 获取玩家();
		轮回进度类 进度 = 获取进度();
		string info = "当前轮回：" + 轮回系统.当前轮回数 +
			"\n元宝：" + 玩家.财产信息.元宝.ToString("0") +
			"\n将神珠：" + 进度.将神珠数量 +
			"\n神将碎片：" + 进度.神将碎片数量 +
			"\n剩余国家：" + (全局变量.所有国家列表 == null ? 0 : 全局变量.所有国家列表.Count);
		Text summary = 创建文本("概况数据", 内容对象, info, 31, TextAnchor.UpperLeft, 文字色);
		summary.rectTransform.anchorMin = new Vector2(0f, 0.45f);
		summary.rectTransform.anchorMax = new Vector2(0.48f, 0.95f);
		summary.rectTransform.offsetMin = new Vector2(20f, 0f);
		summary.rectTransform.offsetMax = new Vector2(-10f, 0f);

		Button next = 创建按钮("下一轮回", 内容对象, "进入下一轮回", new Vector2(330f, 72f), new Vector2(-190f, 155f), new Vector2(1f, 0.5f), 强调色, 28);
		next.interactable = 轮回系统.是否可进入下一轮回();
		next.onClick.AddListener(() =>
		{
			if (!轮回系统.进入下一轮回())
			{
				显示状态("尚未统一天下，无法进入下一轮回。");
			}
		});

		Button dungeon = 创建按钮("轮回副本", 内容对象, "自动编队挑战副本", new Vector2(330f, 72f), new Vector2(-190f, 55f), new Vector2(1f, 0.5f), 按钮色, 27);
		dungeon.interactable = 轮回副本系统.是否开启();
		dungeon.onClick.AddListener(自动挑战副本);

		string 下一轮回状态 = next.interactable ? "已统一天下，可以进入下一轮回。" : "统一天下后可进入下一轮回。";
		string 副本状态 = dungeon.interactable ? "轮回副本已开启。" : "第50轮回开启轮回副本。";
		显示状态(下一轮回状态 + "  " + 副本状态);
	}

	private void 自动挑战副本()
	{
		玩家数据 玩家 = 获取玩家();
		if (玩家 == null)
		{
			显示状态("玩家数据不存在。");
			return;
		}
		List<将领信息> 队伍 = new List<将领信息>();
		for (int i = 0; i < 玩家.封地信息表.Count && 队伍.Count < 5; i++)
		{
			封地信息 封地 = 玩家.封地信息表[i];
			if (封地 == null || 封地.将领信息表 == null)
			{
				continue;
			}
			for (int j = 0; j < 封地.将领信息表.Count && 队伍.Count < 5; j++)
			{
				将领信息 将领 = 封地.将领信息表[j];
				if (将领 != null && 将领.详细信息 != null && 将领.将领配兵 != null && 将领.详细信息.状态 == 0.0 && 将领.将领配兵.数量 > 0.0)
				{
					队伍.Add(将领);
				}
			}
		}
		if (队伍.Count == 0)
		{
			显示状态("没有空闲且已配兵的将领可用于副本。");
			return;
		}
		bool success = 轮回副本系统.发起副本(队伍);
		显示状态(success ? "轮回副本出征成功，已派出 " + 队伍.Count + " 名将领。" : "副本出征失败，可能已有副本队伍在行军。");
	}

	private void 显示将神坛()
	{
		清空内容("将神坛");
		if (!玩家数据可用())
		{
			显示状态("玩家数据尚未初始化。");
			return;
		}
		List<神将图鉴条目> list = 将神坛系统.获取图鉴();
		if (list.Count == 0)
		{
			显示状态("神将数据尚未初始化。");
			return;
		}
		int pageCount = Mathf.Max(1, Mathf.CeilToInt(list.Count / 10f));
		神将页码 = Mathf.Clamp(神将页码, 0, pageCount - 1);
		int start = 神将页码 * 10;
		int end = Mathf.Min(start + 10, list.Count);
		for (int i = start; i < end; i++)
		{
			神将图鉴条目 entry = list[i];
			string label = entry.名字 + (entry.已拥有 ? "  已拥有" : "");
			Button button = 创建按钮("神将_" + entry.名字, 内容对象, label, new Vector2(370f, 48f), new Vector2(195f, -30f - (i - start) * 55f), new Vector2(0f, 1f), entry.名字 == 当前神将名 ? 强调色 : 按钮色, 22);
			string 神将名 = entry.名字;
			button.onClick.AddListener(() =>
			{
				当前神将名 = 神将名;
				显示将神坛();
			});
		}

		Button prev = 创建按钮("上一页", 内容对象, "<", new Vector2(72f, 48f), new Vector2(48f, 32f), Vector2.zero, 按钮色, 28);
		prev.interactable = 神将页码 > 0;
		prev.onClick.AddListener(() => { 神将页码--; 显示将神坛(); });
		Button next = 创建按钮("下一页", 内容对象, ">", new Vector2(72f, 48f), new Vector2(342f, 32f), Vector2.zero, 按钮色, 28);
		next.interactable = 神将页码 < pageCount - 1;
		next.onClick.AddListener(() => { 神将页码++; 显示将神坛(); });

		if (string.IsNullOrEmpty(当前神将名))
		{
			当前神将名 = list[0].名字;
		}
		神将图鉴条目 selected = list.Find(item => item.名字 == 当前神将名);
		if (selected == null)
		{
			selected = list[0];
			当前神将名 = selected.名字;
		}
		Text name = 创建文本("神将名称", 内容对象, selected.名字, 36, TextAnchor.UpperLeft, 文字色);
		name.rectTransform.anchorMin = new Vector2(0f, 0.82f);
		name.rectTransform.anchorMax = new Vector2(1f, 1f);
		name.rectTransform.offsetMin = new Vector2(440f, 0f);
		name.rectTransform.offsetMax = new Vector2(-20f, -12f);
		string detail = 将神坛系统.获取合成材料说明(selected.名字) + "\n\n" + 将神坛系统.获取当前阶位说明(selected.名字);
		Text description = 创建文本("神将说明", 内容对象, detail, 24, TextAnchor.UpperLeft, 次要文字色);
		description.rectTransform.anchorMin = new Vector2(0f, 0.25f);
		description.rectTransform.anchorMax = new Vector2(1f, 0.82f);
		description.rectTransform.offsetMin = new Vector2(440f, 0f);
		description.rectTransform.offsetMax = new Vector2(-20f, 0f);

		if (selected.可合成)
		{
			Button synthesize = 创建按钮("合成神将", 内容对象, "合成", new Vector2(210f, 62f), new Vector2(-250f, 38f), new Vector2(1f, 0f), 强调色, 26);
			synthesize.onClick.AddListener(() =>
			{
				神将合成结果 result = 将神坛系统.合成(当前神将名);
				显示将神坛();
				显示状态(result.消息);
			});
			Button advance = 创建按钮("神将升阶", 内容对象, "升阶", new Vector2(210f, 62f), new Vector2(-20f, 38f), new Vector2(1f, 0f), 按钮色, 26);
			advance.onClick.AddListener(() =>
			{
				神将合成结果 result = 将神坛系统.升阶(当前神将名);
				显示将神坛();
				显示状态(result.消息);
			});
		}
		轮回进度类 progress = 获取进度();
		显示状态("将神珠 " + progress.将神珠数量 + "  神将碎片 " + progress.神将碎片数量);
	}

	private void 显示抽奖()
	{
		清空内容("元宝抽奖");
		if (!玩家数据可用())
		{
			显示状态("玩家数据尚未初始化。");
			return;
		}
		刷新抽奖余额();
		Text rules = 创建文本("抽奖概率", 内容对象, "单次消耗 5000 元宝\n将神珠 1%\n神将碎片 5%\n商城道具 94%", 32, TextAnchor.MiddleCenter, 文字色);
		rules.rectTransform.anchorMin = new Vector2(0.08f, 0.36f);
		rules.rectTransform.anchorMax = new Vector2(0.92f, 0.88f);
		rules.rectTransform.offsetMin = Vector2.zero;
		rules.rectTransform.offsetMax = Vector2.zero;
		Button draw = 创建按钮("抽奖", 内容对象, "抽奖", new Vector2(300f, 76f), new Vector2(0f, 72f), new Vector2(0.5f, 0f), 强调色, 30);
		draw.onClick.AddListener(() =>
		{
			抽奖结果 result = 元宝抽奖系统.抽奖();
			显示状态(result.成功 ? "获得 " + result.奖励名称 : result.奖励名称);
			刷新抽奖余额();
		});
	}

	private void 刷新抽奖余额()
	{
		玩家数据 玩家 = 获取玩家();
		轮回进度类 进度 = 获取进度();
		if (玩家 != null)
		{
			显示状态("元宝 " + 玩家.财产信息.元宝.ToString("0") + "  将神珠 " + 进度.将神珠数量 + "  神将碎片 " + 进度.神将碎片数量);
		}
	}

	private void 显示成就()
	{
		清空内容("成就");
		if (!玩家数据可用())
		{
			显示状态("玩家数据尚未初始化。");
			return;
		}
		成就系统.检查轮回成就();
		for (int i = 0; i < 全局成就库.属性表.Count; i++)
		{
			成就定义 definition = 全局成就库.属性表[i];
			成就信息 progress = 获取进度().获取成就进度(definition.ID);
			float y = -38f - i * 76f;
			Text label = 创建文本("成就_" + definition.ID, 内容对象, definition.名字 + "  " + definition.描述 + "  奖励：" + definition.奖励内容, 24, TextAnchor.MiddleLeft, progress.已完成 ? 文字色 : 次要文字色);
			label.rectTransform.anchorMin = new Vector2(0f, 1f);
			label.rectTransform.anchorMax = new Vector2(1f, 1f);
			label.rectTransform.sizeDelta = new Vector2(-220f, 62f);
			label.rectTransform.anchoredPosition = new Vector2(-90f, y);
			string buttonText = progress.已领取 ? "已领取" : (progress.已完成 ? "领取" : "未完成");
			Button claim = 创建按钮("领取_" + definition.ID, 内容对象, buttonText, new Vector2(160f, 54f), new Vector2(-92f, y), new Vector2(1f, 1f), progress.已完成 && !progress.已领取 ? 强调色 : 按钮色, 23);
			claim.interactable = progress.已完成 && !progress.已领取;
			string achievementId = definition.ID;
			claim.onClick.AddListener(() =>
			{
				bool success = 成就系统.领取成就(achievementId);
				显示成就();
				显示状态(success ? "成就奖励领取成功。" : "成就奖励领取失败。");
			});
		}
		显示状态("当前轮回：" + 轮回系统.当前轮回数);
	}

	private void 清空内容(string title)
	{
		标题文本.text = title;
		for (int i = 内容对象.childCount - 1; i >= 0; i--)
		{
			GameObject child = 内容对象.GetChild(i).gameObject;
			child.SetActive(false);
			Destroy(child);
		}
		状态文本.text = "";
	}

	private void 显示状态(string message)
	{
		if (状态文本 != null)
		{
			状态文本.text = message ?? "";
		}
	}

	private static bool 玩家数据可用()
	{
		return 全局变量.所有玩家数据表 != null && 全局变量.本机身份 >= 0 && 全局变量.本机身份 < 全局变量.所有玩家数据表.Count;
	}

	private static 玩家数据 获取玩家()
	{
		return 玩家数据可用() ? 全局变量.所有玩家数据表[全局变量.本机身份] : null;
	}

	private static 轮回进度类 获取进度()
	{
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 轮回进度类.读取();
		}
		全局变量.轮回进度.修复旧数据();
		return 全局变量.轮回进度;
	}

	private Font 查找界面字体()
	{
		Text[] existingTexts = FindObjectsOfType<Text>();
		for (int i = 0; i < existingTexts.Length; i++)
		{
			if (existingTexts[i].font != null)
			{
				return existingTexts[i].font;
			}
		}
		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		return font != null ? font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
	}

	private static void 确保事件系统()
	{
		if (FindObjectOfType<EventSystem>() != null)
		{
			return;
		}
		GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
		DontDestroyOnLoad(eventSystem);
	}

	private RectTransform 创建固定对象(string name, Transform parent, Vector2 size, Vector2 position, Vector2 anchor)
	{
		GameObject go = new GameObject(name, typeof(RectTransform));
		RectTransform rect = go.GetComponent<RectTransform>();
		rect.SetParent(parent, false);
		rect.anchorMin = anchor;
		rect.anchorMax = anchor;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.sizeDelta = size;
		rect.anchoredPosition = position;
		return rect;
	}

	private RectTransform 创建拉伸对象(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
	{
		GameObject go = new GameObject(name, typeof(RectTransform));
		RectTransform rect = go.GetComponent<RectTransform>();
		rect.SetParent(parent, false);
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = offsetMin;
		rect.offsetMax = offsetMax;
		return rect;
	}

	private Button 创建按钮(string name, Transform parent, string label, Vector2 size, Vector2 position, Vector2 anchor, Color color, int fontSize)
	{
		RectTransform rect = 创建固定对象(name, parent, size, position, anchor);
		Image image = rect.gameObject.AddComponent<Image>();
		image.color = color;
		Button button = rect.gameObject.AddComponent<Button>();
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
		colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
		colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.75f);
		button.colors = colors;
		Text text = 创建文本("文本", rect, label, fontSize, TextAnchor.MiddleCenter, 文字色);
		text.rectTransform.offsetMin = new Vector2(8f, 4f);
		text.rectTransform.offsetMax = new Vector2(-8f, -4f);
		return button;
	}

	private Text 创建文本(string name, Transform parent, string value, int fontSize, TextAnchor alignment, Color color)
	{
		RectTransform rect = 创建拉伸对象(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		Text text = rect.gameObject.AddComponent<Text>();
		text.font = 界面字体;
		text.text = value;
		text.fontSize = fontSize;
		text.alignment = alignment;
		text.color = color;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Truncate;
		text.resizeTextForBestFit = true;
		text.resizeTextMinSize = Mathf.Max(14, fontSize - 8);
		text.resizeTextMaxSize = fontSize;
		text.raycastTarget = false;
		return text;
	}
}
