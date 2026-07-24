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
	}

	public void 全军撤退()
	{
		战斗系统脚本对象.全军撤退 = true;
	}

	public void 显示兵力()
	{
		开始显示兵力 = true;
	}

	private void FixedUpdate()
	{
		if (开始显示兵力)
		{
			long time = TIME.getTime() - 战斗系统脚本对象.创建时间;
			战斗时间.text = TIME.ToTimeFormat(time);
			攻方兵力文本.text = 战斗系统脚本对象.攻方兵力.ToString();
			守方兵力文本.text = 战斗系统脚本对象.守方兵力.ToString();
		}
	}
}
