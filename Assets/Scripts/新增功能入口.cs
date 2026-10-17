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
	private GameObject 退出入口对象;
	private GameObject 将神坛入口对象;
	private GameObject 成就入口对象;
	private GameObject 游戏设置界面对象;
	private GameObject 成就信息布局对象;
	private GameObject 校场布局对象;
	private readonly List<Canvas> 场景弹窗画布 = new List<Canvas>();
	private RectTransform 安全区域对象;
	private RectTransform 内容对象;
	private RectTransform 将神坛入口矩形;
	private RectTransform 功能面板矩形;
	private RectTransform 退出面板矩形;
	private Text 标题文本;
	private Text 状态文本;
	private Font 界面字体;
	private string 当前神将名;
	private int 神将页码;
	private int 上次屏幕宽度;
	private int 上次屏幕高度;
	private Rect 上次安全区域;
	private float 下次入口状态刷新时间;

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
		退出入口对象 = null;
		将神坛入口对象 = null;
		成就入口对象 = null;
		游戏设置界面对象 = null;
		成就信息布局对象 = null;
		校场布局对象 = null;
		场景弹窗画布.Clear();
		安全区域对象 = null;
		内容对象 = null;
		将神坛入口矩形 = null;
		功能面板矩形 = null;
		退出面板矩形 = null;
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
		创建退出确认界面();
		if (!创建功能面板)
		{
			return;
		}

		游戏设置界面对象 = 查找场景对象("游戏设置界面");
		成就信息布局对象 = 查找场景对象("成就信息布局");
		校场布局对象 = 查找场景对象("校场布局");
		缓存场景弹窗画布();
		隐藏旧成就页签();

		创建设置退出入口();
		创建基地将神坛入口();

		Button achievementEntry = 创建按钮("轮回成就入口", 安全区域对象, "轮回成就", new Vector2(230f, 62f), new Vector2(-145f, 82f), new Vector2(1f, 0f), 强调色, 25);
		achievementEntry.onClick.AddListener(打开成就);
		成就入口对象 = achievementEntry.gameObject;
		设置对象显示状态(退出入口对象, false);
		设置对象显示状态(将神坛入口对象, false);
		成就入口对象.SetActive(false);

		面板对象 = 创建拉伸对象("新增功能遮罩", 界面根对象.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
		面板对象.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.62f);
		Button 遮罩按钮 = 面板对象.AddComponent<Button>();
		遮罩按钮.transition = Selectable.Transition.None;
		遮罩按钮.onClick.AddListener(关闭面板);

		RectTransform panel = 创建固定对象("新增功能面板", 面板对象.transform, new Vector2(1200f, 760f), Vector2.zero, new Vector2(0.5f, 0.5f));
		功能面板矩形 = panel;
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
		创建导航按钮(tabs, "战场", 4, 显示战场);
		创建导航按钮(tabs, "道具获取", 5, 显示道具获取);

		内容对象 = 创建拉伸对象("功能内容", panel, Vector2.zero, Vector2.one, new Vector2(236f, 68f), new Vector2(-18f, -88f));
		状态文本 = 创建文本("状态", panel, "", 23, TextAnchor.MiddleLeft, 次要文字色);
		状态文本.rectTransform.anchorMin = Vector2.zero;
		状态文本.rectTransform.anchorMax = new Vector2(1f, 0f);
		状态文本.rectTransform.offsetMin = new Vector2(236f, 8f);
		状态文本.rectTransform.offsetMax = new Vector2(-18f, 60f);
		适配弹窗尺寸();

		面板对象.SetActive(false);
		绑定战场入口();
	}

	private void 创建退出确认界面()
	{
		退出确认对象 = 创建拉伸对象("退出确认遮罩", 界面根对象.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
		退出确认对象.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
		Button 遮罩按钮 = 退出确认对象.AddComponent<Button>();
		遮罩按钮.transition = Selectable.Transition.None;
		遮罩按钮.onClick.AddListener(关闭退出确认);

		RectTransform panel = 创建固定对象("退出确认面板", 退出确认对象.transform, new Vector2(620f, 300f), Vector2.zero, new Vector2(0.5f, 0.5f));
		退出面板矩形 = panel;
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
		适配弹窗尺寸();
	}

	private void 创建导航按钮(RectTransform parent, string label, int index, UnityEngine.Events.UnityAction action)
	{
		Button button = 创建按钮(label, parent, label, new Vector2(204f, 62f), new Vector2(0f, -40f - index * 74f), new Vector2(0.5f, 1f), 按钮色, 25);
		button.onClick.AddListener(action);
	}

	private void Update()
	{
		bool 屏幕发生变化 = false;
		if (安全区域对象 != null && (上次屏幕宽度 != Screen.width || 上次屏幕高度 != Screen.height || 上次安全区域 != Screen.safeArea))
		{
			应用安全区域();
			屏幕发生变化 = true;
		}
		if (屏幕发生变化 || Time.unscaledTime >= 下次入口状态刷新时间)
		{
			下次入口状态刷新时间 = Time.unscaledTime + 0.1f;
			刷新入口显示状态();
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

	private void 刷新入口显示状态()
	{
		bool 显示退出 = 游戏设置界面对象 != null && 游戏设置界面对象.activeInHierarchy && !存在其他弹窗(游戏设置界面对象) && !面板是否打开();
		bool 显示成就 = 成就信息布局对象 != null && 成就信息布局对象.activeInHierarchy && !存在其他弹窗(成就信息布局对象) && !面板是否打开();
		bool 显示将神坛 = 校场布局对象 != null && 校场布局对象.activeInHierarchy && !存在其他弹窗(校场布局对象) && !显示退出 && !显示成就 && !面板是否打开();
		if (显示将神坛)
		{
			更新将神坛入口位置();
		}
		设置对象显示状态(退出入口对象, 显示退出);
		设置对象显示状态(成就入口对象, 显示成就);
		设置对象显示状态(将神坛入口对象, 显示将神坛);
	}

	private void 打开面板()
	{
		面板对象.SetActive(true);
		显示概况();
	}

	private void 打开将神坛()
	{
		面板对象.SetActive(true);
		显示将神坛();
	}

	private void 打开成就()
	{
		面板对象.SetActive(true);
		显示成就();
	}

	private bool 面板是否打开()
	{
		return (面板对象 != null && 面板对象.activeSelf) || (退出确认对象 != null && 退出确认对象.activeSelf);
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
		适配弹窗尺寸();
	}

	private void 适配弹窗尺寸()
	{
		if (安全区域对象 == null)
		{
			return;
		}
		// 遮罩仍覆盖屏幕，面板按安全区域等比缩小，保持各分区的相对尺寸。
		Canvas.ForceUpdateCanvases();
		适配单个弹窗(功能面板矩形);
		适配单个弹窗(退出面板矩形);
	}

	private void 适配单个弹窗(RectTransform panel)
	{
		if (panel == null || 安全区域对象.rect.width <= 0f || 安全区域对象.rect.height <= 0f)
		{
			return;
		}
		float scale = Mathf.Min(1f, Mathf.Min(安全区域对象.rect.width / (panel.sizeDelta.x + 40f), 安全区域对象.rect.height / (panel.sizeDelta.y + 40f)));
		panel.localScale = Vector3.one * scale;
		panel.position = 安全区域对象.TransformPoint(安全区域对象.rect.center);
	}

	private void 创建设置退出入口()
	{
		if (游戏设置界面对象 == null)
		{
			return;
		}
		Transform list = 查找子对象(游戏设置界面对象.transform, "设置列表布局");
		Transform first = list == null ? null : list.Find("音乐开");
		Transform template = list == null ? null : list.Find("难度高");
		if (first == null || template == null)
		{
			Debug.LogError("无法创建退出图标：设置列表缺少音乐开或难度高选项。");
			return;
		}
		退出入口对象 = Instantiate(template.gameObject, list, false);
		退出入口对象.name = "设置_退出游戏";
		RectTransform rect = 退出入口对象.GetComponent<RectTransform>();
		RectTransform firstRect = first.GetComponent<RectTransform>();
		rect.anchorMin = firstRect.anchorMin;
		rect.anchorMax = firstRect.anchorMax;
		rect.pivot = firstRect.pivot;
		rect.sizeDelta = firstRect.sizeDelta;
		GridLayoutGroup grid = list.GetComponent<GridLayoutGroup>();
		if (grid != null)
		{
			grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			grid.constraintCount = 6;
			退出入口对象.transform.SetSiblingIndex(6);
		}
		else
		{
			rect.anchoredPosition = firstRect.anchoredPosition + new Vector2(0f, -(firstRect.rect.height + 45f));
		}
		Text[] labels = 退出入口对象.GetComponentsInChildren<Text>(true);
		for (int i = 0; i < labels.Length; i++)
		{
			labels[i].text = "退出游戏";
			labels[i].font = 界面字体;
			labels[i].resizeTextForBestFit = true;
			labels[i].resizeTextMinSize = 12;
			labels[i].resizeTextMaxSize = 16;
			labels[i].raycastTarget = false;
		}
		Button[] buttons = 退出入口对象.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			// 替换整个事件，清除克隆项原来的持久化难度切换回调。
			buttons[i].onClick = new Button.ButtonClickedEvent();
			buttons[i].onClick.AddListener(打开退出确认);
			Navigation navigation = buttons[i].navigation;
			navigation.mode = Navigation.Mode.None;
			buttons[i].navigation = navigation;
		}
	}

	private void 创建基地将神坛入口()
	{
		if (校场布局对象 == null)
		{
			return;
		}
		Button entry = 创建图片按钮("将神坛入口", 校场布局对象.transform.parent, "将神坛", Resources.Load<Texture2D>("界面图片/将神坛入口"), new Vector2(174f, 198f), Vector2.zero, new Vector2(0.5f, 0.5f), 22);
		entry.onClick.AddListener(打开将神坛);
		将神坛入口对象 = entry.gameObject;
		将神坛入口矩形 = entry.GetComponent<RectTransform>();
		更新将神坛入口位置();
	}

	private void 更新将神坛入口位置()
	{
		if (校场布局对象 != null && 将神坛入口矩形 != null)
		{
			// 与校场共用基地坐标系，位置对应文档中校场左下方的空地。
			将神坛入口矩形.localPosition = 校场布局对象.transform.localPosition + new Vector3(-180f, -110f, 0f);
		}
	}

	private static Transform 查找子对象(Transform root, string name)
	{
		Transform[] children = root.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < children.Length; i++)
		{
			if (children[i].name == name) return children[i];
		}
		return null;
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

		string 下一轮回状态 = next.interactable ? "已统一天下，可以进入下一轮回。" : "统一天下后可进入下一轮回。";
		string 副本状态 = 轮回副本系统.是否开启() ? "轮回副本已开启。" : "第50轮回开启轮回副本。";
		显示状态(下一轮回状态 + "  " + 副本状态);
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
		int pageCount = list.Count;
		神将页码 = Mathf.Clamp(神将页码, 0, pageCount - 1);
		神将图鉴条目 selected = list[神将页码];
		当前神将名 = selected.名字;

		RectTransform portraitArea = 创建拉伸对象("神将立绘区", 内容对象, Vector2.zero, new Vector2(0.43f, 1f), new Vector2(12f, 78f), new Vector2(-18f, -12f));
		portraitArea.gameObject.AddComponent<RectMask2D>();
		RectTransform portraitRect = 创建拉伸对象("神将立绘", portraitArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		Image portrait = portraitRect.gameObject.AddComponent<Image>();
		portrait.sprite = 全局将领库.获取神将全身图(selected.名字);
		portrait.preserveAspect = true;
		portrait.raycastTarget = false;

		RectTransform pageArea = 创建拉伸对象("神将翻页区", 内容对象, Vector2.zero, new Vector2(0.43f, 0f), new Vector2(12f, 8f), new Vector2(-18f, 64f));
		Button prev = 创建按钮("上一页", pageArea, "<", new Vector2(64f, 48f), new Vector2(32f, 0f), new Vector2(0f, 0.5f), 按钮色, 28);
		prev.interactable = 神将页码 > 0;
		prev.onClick.AddListener(() => { 神将页码--; 显示将神坛(); });
		Button next = 创建按钮("下一页", pageArea, ">", new Vector2(64f, 48f), new Vector2(-32f, 0f), new Vector2(1f, 0.5f), 按钮色, 28);
		next.interactable = 神将页码 < pageCount - 1;
		next.onClick.AddListener(() => { 神将页码++; 显示将神坛(); });
		Text page = 创建文本("页码", pageArea, (神将页码 + 1) + " / " + pageCount, 22, TextAnchor.MiddleCenter, 次要文字色);
		page.rectTransform.offsetMin = new Vector2(76f, 0f);
		page.rectTransform.offsetMax = new Vector2(-76f, 0f);

		RectTransform details = 创建拉伸对象("神将详情区", 内容对象, new Vector2(0.45f, 0f), Vector2.one, new Vector2(8f, 8f), new Vector2(-12f, -12f));
		Text name = 创建文本("神将名称", details, selected.名字, 36, TextAnchor.MiddleLeft, 文字色);
		name.rectTransform.anchorMin = new Vector2(0f, 1f);
		name.rectTransform.offsetMin = new Vector2(0f, -60f);
		name.rectTransform.offsetMax = Vector2.zero;
		Text ownership = 创建文本("拥有状态", details, (selected.已拥有 ? "已拥有" : "未拥有") + "  " + new[] { "初阶", "玄阶", "地阶", "天阶" }[Mathf.Clamp(selected.阶位, 0, 3)], 24, TextAnchor.MiddleLeft, 次要文字色);
		ownership.rectTransform.anchorMin = new Vector2(0f, 1f);
		ownership.rectTransform.offsetMin = new Vector2(0f, -100f);
		ownership.rectTransform.offsetMax = new Vector2(0f, -64f);
		RectTransform description = 创建神将说明滚动区(details);
		添加神将说明段落(description, "基础属性", "基础属性", 文字色);
		添加神将说明段落(description, "属性数值", "武力 1000  智力 1000\n统帅 2000", 次要文字色);
		添加神将说明段落(description, "合成标题", "合成材料", 文字色);
		添加神将说明段落(description, "合成材料", 将神坛系统.获取合成材料说明(selected.名字), 次要文字色);
		添加神将说明段落(description, "技能标题", "技能介绍", 文字色);
		添加神将说明段落(description, "技能介绍", 将神坛系统.获取当前阶位说明(selected.名字), 次要文字色);

		RectTransform actions = 创建拉伸对象("神将操作区", details, Vector2.zero, new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, 64f));
		HorizontalLayoutGroup actionLayout = actions.gameObject.AddComponent<HorizontalLayoutGroup>();
		actionLayout.spacing = 20f;
		actionLayout.childControlWidth = true;
		actionLayout.childControlHeight = true;
		actionLayout.childForceExpandWidth = true;
		actionLayout.childForceExpandHeight = true;
		Button synthesize = 创建按钮("合成神将", actions, "合成", Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), 强调色, 26);
		synthesize.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
		synthesize.interactable = selected.可合成 && !selected.已拥有;
		synthesize.onClick.AddListener(() =>
		{
			神将合成结果 result = 将神坛系统.合成(当前神将名);
			显示将神坛();
			显示状态(result.消息);
		});
		Button advance = 创建按钮("神将升阶", actions, selected.阶位 >= 3 ? "已达天阶" : "升阶", Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), 按钮色, 26);
		advance.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
		advance.interactable = selected.已拥有 && selected.阶位 < 3;
		advance.onClick.AddListener(() =>
		{
			神将合成结果 result = 将神坛系统.升阶(当前神将名);
			显示将神坛();
			显示状态(result.消息);
		});
		Canvas.ForceUpdateCanvases();
		description.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
		轮回进度类 progress = 获取进度();
		显示状态("将神珠 " + progress.将神珠数量 + "  神将碎片 " + progress.神将碎片数量);
	}

	private RectTransform 创建神将说明滚动区(RectTransform parent)
	{
		return 创建垂直滚动区(parent, "神将说明滚动区", new Vector2(0f, 84f), new Vector2(0f, -116f));
	}

	private RectTransform 创建垂直滚动区(RectTransform parent, string name, Vector2 min, Vector2 max)
	{
		RectTransform root = 创建拉伸对象(name, parent, Vector2.zero, Vector2.one, min, max);
		root.gameObject.AddComponent<Image>().color = Color.clear;
		ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();
		scroll.horizontal = false;
		scroll.vertical = true;
		scroll.movementType = ScrollRect.MovementType.Clamped;
		scroll.scrollSensitivity = 30f;
		RectTransform viewport = 创建拉伸对象("说明可视区域", root, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-20f, 0f));
		viewport.gameObject.AddComponent<RectMask2D>();
		RectTransform content = 创建拉伸对象("说明内容", viewport, new Vector2(0f, 1f), Vector2.one, Vector2.zero, Vector2.zero);
		content.pivot = new Vector2(0.5f, 1f);
		VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
		layout.spacing = 12f;
		layout.padding = new RectOffset(0, 8, 4, 12);
		layout.childControlWidth = true;
		layout.childControlHeight = true;
		layout.childForceExpandWidth = true;
		layout.childForceExpandHeight = false;
		ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
		fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		scroll.viewport = viewport;
		scroll.content = content;

		RectTransform bar = 创建拉伸对象("说明滚动条", root, new Vector2(1f, 0f), Vector2.one, new Vector2(-12f, 0f), Vector2.zero);
		bar.gameObject.AddComponent<Image>().color = 面板色;
		Scrollbar scrollbar = bar.gameObject.AddComponent<Scrollbar>();
		scrollbar.direction = Scrollbar.Direction.BottomToTop;
		RectTransform handle = 创建拉伸对象("滑块", bar, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
		Image handleImage = handle.gameObject.AddComponent<Image>();
		handleImage.color = 按钮色;
		scrollbar.handleRect = handle;
		scrollbar.targetGraphic = handleImage;
		scroll.verticalScrollbar = scrollbar;
		scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
		return content;
	}

	private void 添加神将说明段落(RectTransform parent, string name, string value, Color color)
	{
		Text text = 创建文本(name, parent, value, 24, TextAnchor.UpperLeft, color);
		text.resizeTextForBestFit = false;
		text.supportRichText = false;
		text.lineSpacing = 1.15f;
		text.gameObject.AddComponent<LayoutElement>().minHeight = 30f;
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
		Font font = Resources.Load<Font>("等级数字");
		if (font != null)
		{
			return font;
		}
		Text[] existingTexts = FindObjectsOfType<Text>();
		for (int i = 0; i < existingTexts.Length; i++)
		{
			if (existingTexts[i].font != null)
			{
				return existingTexts[i].font;
			}
		}
		return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
	}

	private void 隐藏旧成就页签()
	{
		string[] 旧页签 = { "修身", "治国", "齐家", "平天下" };
		for (int i = 0; i < 旧页签.Length; i++)
		{
			GameObject tab = 查找场景对象(旧页签[i]);
			if (tab != null)
			{
				tab.SetActive(false);
			}
		}
	}

	private void 缓存场景弹窗画布()
	{
		场景弹窗画布.Clear();
		Canvas[] canvases = FindObjectsOfType<Canvas>(true);
		Scene activeScene = SceneManager.GetActiveScene();
		for (int i = 0; i < canvases.Length; i++)
		{
			Canvas canvas = canvases[i];
			if (canvas != null && canvas.gameObject.scene == activeScene && canvas.isRootCanvas && canvas.sortingOrder >= 2 && canvas.sortingOrder <= 4 && !canvas.gameObject.activeInHierarchy)
			{
				场景弹窗画布.Add(canvas);
			}
		}
	}

	private bool 存在其他弹窗(GameObject 当前界面)
	{
		for (int i = 0; i < 场景弹窗画布.Count; i++)
		{
			Canvas canvas = 场景弹窗画布[i];
			if (canvas == null || !canvas.gameObject.activeInHierarchy)
			{
				continue;
			}
			bool 属于当前界面 = 当前界面 != null && (canvas.transform.IsChildOf(当前界面.transform) || 当前界面.transform.IsChildOf(canvas.transform));
			if (!属于当前界面)
			{
				return true;
			}
		}
		return false;
	}

	private void 绑定战场入口()
	{
		GameObject entry = 查找场景对象("主界面_战场");
		Button button = entry == null ? null : entry.GetComponent<Button>();
		if (button == null) return;
		// 场景中的原战场回调目标为空，只替换这个入口，不修改将领等其他面板。
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(() =>
		{
			面板对象.SetActive(true);
			显示战场();
		});
	}

	private void 显示战场()
	{
		清空内容("战场");
		Text summary = 创建文本("副本信息", 内容对象, "轮回副本\n机关兽 · 蚩尤\n\n通关奖励\n轮回装备、神将碎片", 28, TextAnchor.UpperLeft, 文字色);
		summary.rectTransform.offsetMin = new Vector2(24f, 160f);
		summary.rectTransform.offsetMax = new Vector2(-24f, -36f);
		Button dungeon = 创建按钮("战场轮回副本", 内容对象, "轮回副本", new Vector2(280f, 72f), new Vector2(-160f, 88f), new Vector2(0.5f, 0f), 强调色, 28);
		dungeon.onClick.AddListener(显示轮回副本);
		Button reports = 创建按钮("战场军情", 内容对象, "查看军情", new Vector2(280f, 72f), new Vector2(160f, 88f), new Vector2(0.5f, 0f), 按钮色, 28);
		reports.onClick.AddListener(() =>
		{
			GameObject target = 查找场景对象("军情界面UI");
			if (target == null) return;
			关闭面板();
			target.SetActive(true);
		});
		显示状态(轮回副本系统.是否开启() ? "轮回副本已解锁" : "第50轮回解锁，当前第" + 轮回系统.当前轮回数 + "轮回");
	}

	private void 显示道具获取()
	{
		清空内容("道具获取");
		RectTransform 输入区域 = 创建固定对象("领取码输入框", 内容对象, new Vector2(420f, 68f), new Vector2(0f, 120f), new Vector2(0.5f, 0.5f));
		Image 背景 = 输入区域.gameObject.AddComponent<Image>();
		背景.color = 面板色;
		Text 输入文本 = 创建文本("领取码", 输入区域, "", 28, TextAnchor.MiddleLeft, 文字色);
		输入文本.rectTransform.offsetMin = new Vector2(18f, 8f);
		输入文本.rectTransform.offsetMax = new Vector2(-18f, -8f);
		InputField 输入框 = 输入区域.gameObject.AddComponent<InputField>();
		输入框.targetGraphic = 背景;
		输入框.textComponent = 输入文本;
		输入框.contentType = InputField.ContentType.Alphanumeric;
		输入框.characterLimit = 6;
		Text 预览 = 创建文本("领取内容", 内容对象, "", 28, TextAnchor.MiddleCenter, 文字色);
		预览.rectTransform.anchorMin = 预览.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		预览.rectTransform.sizeDelta = new Vector2(600f, 80f);
		预览.rectTransform.anchoredPosition = Vector2.zero;
		输入框.onValueChanged.AddListener(value =>
		{
			string[] 名单 = GM名将领取.解析(value);
			预览.text = 名单 == null ? "无效领取码" : string.Join("、", 名单) + "（各1名）";
		});
		输入框.text = "GM1001";
		Button 领取 = 创建按钮("领取名将", 内容对象, "领取", new Vector2(240f, 68f), new Vector2(0f, -120f), new Vector2(0.5f, 0.5f), 强调色, 28);
		领取.onClick.AddListener(() =>
		{
			string 消息;
			GM名将领取.领取(输入框.text, out 消息);
			显示状态(消息);
		});
		显示状态("");
	}

	private void 显示轮回副本()
	{
		清空内容("战场 · 轮回副本");
		if (!玩家数据可用())
		{
			显示状态("玩家数据尚未初始化。");
			return;
		}
		if (!轮回副本系统.是否开启())
		{
			创建文本("副本未解锁", 内容对象, "第50轮回解锁\n当前第" + 轮回系统.当前轮回数 + "轮回", 32, TextAnchor.MiddleCenter, 文字色);
			显示状态("通关奖励：轮回装备、神将碎片");
			return;
		}
		if (全局变量.军情列表.Exists(item => item.战场类型 == 2))
		{
			显示战场();
			显示状态("已有副本队伍，可在军情中查看进度。");
			return;
		}
		List<将领信息> selected = new List<将领信息>();
		RectTransform list = 创建垂直滚动区(内容对象, "副本选将", new Vector2(12f, 96f), new Vector2(-12f, -12f));
		Button start = 创建按钮("副本开始挑战", 内容对象, "开始挑战", new Vector2(260f, 64f), new Vector2(-148f, 40f), new Vector2(0.5f, 0f), 强调色, 26);
		start.interactable = false;
		start.onClick.AddListener(() =>
		{
			if (轮回副本系统.发起副本(selected))
			{
				显示战场();
				显示状态("副本出征成功，可在军情中查看进度。");
			}
			else
			{
				显示轮回副本();
				显示状态("出征失败，将领状态已变化或已有副本队伍。");
			}
		});
		Button back = 创建按钮("返回战场", 内容对象, "返回战场", new Vector2(260f, 64f), new Vector2(148f, 40f), new Vector2(0.5f, 0f), 按钮色, 26);
		back.onClick.AddListener(显示战场);
		List<将领信息> generals = 轮回副本系统.获取可出征将领(全局变量.本机身份);
		foreach (将领信息 general in generals)
		{
			RectTransform row = 创建固定对象("副本将领" + general.ID, list, new Vector2(600f, 76f), Vector2.zero, new Vector2(0.5f, 1f));
			Image background = row.gameObject.AddComponent<Image>();
			background.color = 面板色;
			LayoutElement element = row.gameObject.AddComponent<LayoutElement>();
			element.minHeight = element.preferredHeight = 76f;
			RectTransform box = 创建固定对象("勾选框", row, new Vector2(28f, 28f), new Vector2(24f, 0f), new Vector2(0f, 0.5f));
			box.gameObject.AddComponent<Image>().color = 按钮色;
			RectTransform mark = 创建固定对象("选中标记", box, new Vector2(18f, 18f), Vector2.zero, new Vector2(0.5f, 0.5f));
			Image markImage = mark.gameObject.AddComponent<Image>();
			markImage.color = 强调色;
			Text label = 创建文本("将领信息", row, general.将领属性.初始属性.名字 + "  " + general.将领属性.成长点数.等级.ToString("0") + "级\n兵力 " + general.将领配兵.数量.ToString("0"), 22, TextAnchor.MiddleLeft, 文字色);
			label.resizeTextForBestFit = false;
			label.rectTransform.offsetMin = new Vector2(48f, 4f);
			label.rectTransform.offsetMax = new Vector2(-12f, -4f);
			Toggle toggle = row.gameObject.AddComponent<Toggle>();
			toggle.targetGraphic = background;
			toggle.graphic = markImage;
			toggle.isOn = false;
			toggle.onValueChanged.AddListener(value =>
			{
				if (value) selected.Add(general);
				else selected.Remove(general);
				start.interactable = selected.Count > 0;
				显示状态("已选择 " + selected.Count + " 位将领");
			});
		}
		if (generals.Count == 0) 添加神将说明段落(list, "无可出征将领", "暂无空闲且已配兵的将领", 次要文字色);
		显示状态("可出征 " + generals.Count + " 位将领，已选择 0 位");
		Canvas.ForceUpdateCanvases();
		list.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
	}

	private static GameObject 查找场景对象(string name)
	{
		Transform[] transforms = FindObjectsOfType<Transform>(true);
		Scene activeScene = SceneManager.GetActiveScene();
		for (int i = 0; i < transforms.Length; i++)
		{
			if (transforms[i] != null && transforms[i].gameObject.scene == activeScene && transforms[i].name == name)
			{
				return transforms[i].gameObject;
			}
		}
		return null;
	}

	private static void 设置对象显示状态(GameObject target, bool active)
	{
		if (target != null && target.activeSelf != active)
		{
			target.SetActive(active);
		}
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
		go.layer = parent.gameObject.layer;
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
		go.layer = parent.gameObject.layer;
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

	private Button 创建图片按钮(string name, Transform parent, string label, Texture2D texture, Vector2 size, Vector2 position, Vector2 anchor, int fontSize = 25)
	{
		RectTransform rect = 创建固定对象(name, parent, size, position, anchor);
		Image image = rect.gameObject.AddComponent<Image>();
		if (texture != null)
		{
			image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
		}
		image.preserveAspect = true;
		Button button = rect.gameObject.AddComponent<Button>();
		button.targetGraphic = image;

		float labelHeight = fontSize + 12f;
		RectTransform labelBackground = 创建拉伸对象("标题背景", rect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(4f, 3f), new Vector2(-4f, labelHeight));
		labelBackground.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f, 0.88f);
		Text text = 创建文本("文本", labelBackground, label, fontSize, TextAnchor.MiddleCenter, 文字色);
		text.rectTransform.offsetMin = new Vector2(6f, 2f);
		text.rectTransform.offsetMax = new Vector2(-6f, -2f);
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
		text.resizeTextMinSize = Mathf.Min(fontSize, Mathf.Max(10, fontSize - 8));
		text.resizeTextMaxSize = fontSize;
		text.raycastTarget = false;
		return text;
	}
}
