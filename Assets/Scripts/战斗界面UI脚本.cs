using UnityEngine;
using UnityEngine.UI;

public class 战斗界面UI脚本 : MonoBehaviour
{
	public GameObject 战斗地图对象;

	public Text 攻方兵力文本;

	public Text 守方兵力文本;

	public Text 战斗时间;

	private 战斗系统 战斗系统脚本对象;

	public GameObject 地图按钮对象;

	public GameObject 封地按钮对象;

	public bool 开始显示兵力;

	private float 下次刷新时间;

	private long 上次战斗时间 = long.MinValue;

	private double 上次攻方兵力 = double.NaN;

	private double 上次守方兵力 = double.NaN;

	public void 返回主界面()
	{
		全局变量.战斗地图相机.transform.SetParent(全局变量.战斗地图相机.transform.parent.parent.parent);
		全局变量.大地图相机.SetActive(value: true);
		全局变量.战斗地图相机.SetActive(value: false);
		全局变量.主界面UI对象.SetActive(value: true);
		全局变量.大地图布局对象.SetActive(value: true);
		战斗系统脚本对象.正在观战 = false;
		开始显示兵力 = false;
		base.gameObject.SetActive(value: false);
		地图按钮对象.SetActive(value: false);
		封地按钮对象.SetActive(value: true);
		UnityEngine.Debug.Log("返回");
	}

	public void 获取脚本对象()
	{
		战斗系统脚本对象 = 战斗地图对象 == null ? null : 战斗地图对象.GetComponentInChildren<战斗系统>(true);
		配置兵力文本(攻方兵力文本);
		配置兵力文本(守方兵力文本);
		上次战斗时间 = long.MinValue;
		上次攻方兵力 = double.NaN;
		上次守方兵力 = double.NaN;
		下次刷新时间 = 0f;
	}

	private static void 配置兵力文本(Text 文本)
	{
		if (文本 == null) return;
		文本.font = Resources.Load<Font>("等级数字");
		文本.fontSize = 8;
		文本.resizeTextForBestFit = true;
		文本.resizeTextMinSize = 5;
		文本.resizeTextMaxSize = 8;
		文本.alignment = TextAnchor.MiddleCenter;
		文本.horizontalOverflow = HorizontalWrapMode.Overflow;
		文本.verticalOverflow = VerticalWrapMode.Truncate;
		文本.raycastTarget = false;
		文本.rectTransform.sizeDelta = new Vector2(51f, 11f);
	}

	public void 全军撤退()
	{
		if (战斗系统脚本对象 != null) 战斗系统脚本对象.请求撤军();
	}

	public void 显示兵力()
	{
		开始显示兵力 = true;
		下次刷新时间 = 0f;
	}

	private void Update()
	{
		if (!开始显示兵力 || 战斗系统脚本对象 == null || Time.unscaledTime < 下次刷新时间)
		{
			return;
		}
		下次刷新时间 = Time.unscaledTime + 0.1f;
		long time = TIME.getTime() - 战斗系统脚本对象.创建时间;
		if (time != 上次战斗时间)
		{
			战斗时间.text = TIME.ToTimeFormat(time);
			上次战斗时间 = time;
		}
		if (战斗系统脚本对象.攻方兵力 != 上次攻方兵力)
		{
			if (攻方兵力文本 != null) 攻方兵力文本.text = 战斗数值格式.显示(战斗系统脚本对象.攻方兵力);
			上次攻方兵力 = 战斗系统脚本对象.攻方兵力;
		}
		if (战斗系统脚本对象.守方兵力 != 上次守方兵力)
		{
			if (守方兵力文本 != null) 守方兵力文本.text = 战斗数值格式.显示(战斗系统脚本对象.守方兵力);
			上次守方兵力 = 战斗系统脚本对象.守方兵力;
		}
	}
}
