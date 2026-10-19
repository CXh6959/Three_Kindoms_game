using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class 原生补兵按钮
{
	public static void 创建(Button 模板, Transform 操作栏, UnityAction 补兵)
	{
		if (模板 == null || 操作栏 == null || 操作栏.Find("批量补兵") != null) return;
		Button 按钮 = Object.Instantiate(模板, 操作栏, false);
		按钮.name = "批量补兵";
		RectTransform 布局 = 按钮.GetComponent<RectTransform>();
		布局.anchorMin = 布局.anchorMax = 布局.pivot = new Vector2(0.5f, 0.5f);
		布局.anchoredPosition = new Vector2(180f, -2.91f);
		布局.sizeDelta = new Vector2(97f, 39f);
		按钮.onClick = new Button.ButtonClickedEvent();
		按钮.onClick.AddListener(补兵);
		按钮.gameObject.SetActive(true);
	}
}
