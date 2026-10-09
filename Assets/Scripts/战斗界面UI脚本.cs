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
		战斗系统脚本对象 = 战斗地图对象.transform.GetChild(0).GetChild(0).GetChild(0)
			.GetChild(0)
			.GetChild(0)
			.GetComponent<战斗系统>();
		上次战斗时间 = long.MinValue;
		上次攻方兵力 = double.NaN;
		上次守方兵力 = double.NaN;
		下次刷新时间 = 0f;
	}

	public void 全军撤退()
	{
		战斗系统脚本对象.全军撤退 = true;
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
			攻方兵力文本.text = 战斗系统脚本对象.攻方兵力.ToString();
			上次攻方兵力 = 战斗系统脚本对象.攻方兵力;
		}
		if (战斗系统脚本对象.守方兵力 != 上次守方兵力)
		{
			守方兵力文本.text = 战斗系统脚本对象.守方兵力.ToString();
			上次守方兵力 = 战斗系统脚本对象.守方兵力;
		}
	}
}
