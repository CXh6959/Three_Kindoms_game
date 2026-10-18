using UnityEngine;
using UnityEngine.UI;

public static class 建筑等级显示
{
	private static Font 字体;

	public static void 刷新(Text 文本, int 等级)
	{
		if (文本 == null) return;
		if (字体 == null) 字体 = Resources.Load<Font>("等级数字");
		文本.font = 字体;
		文本.fontSize = 8;
		文本.resizeTextForBestFit = true;
		文本.resizeTextMinSize = 5;
		文本.resizeTextMaxSize = 8;
		文本.alignment = TextAnchor.MiddleCenter;
		文本.horizontalOverflow = HorizontalWrapMode.Overflow;
		文本.verticalOverflow = VerticalWrapMode.Truncate;
		文本.color = new Color(0.12f, 0.16f, 0.13f, 1f);
		文本.raycastTarget = false;
		文本.text = 等级.ToString();
	}
}
