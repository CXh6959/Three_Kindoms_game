using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
			Destroy(图鉴列表对象.GetChild(i).gameObject);
		}
		for (int i = 0; i < list.Count; i++)
		{
			神将图鉴条目 条目 = list[i];
			GameObject item = Instantiate(图鉴条目预制体, 图鉴列表对象);
			Text label = item.GetComponentInChildren<Text>();
			if (label != null)
			{
				label.text = 条目.名字 + (条目.已拥有 ? " [已拥有]" : "");
			}
			Button button = item.GetComponent<Button>();
			if (button == null)
			{
				button = item.GetComponentInChildren<Button>();
			}
			if (button != null)
			{
				string 神将名 = 条目.名字;
				button.onClick.AddListener(() => 选择神将(神将名));
			}
		}
		if (string.IsNullOrEmpty(当前神将名) && list.Count > 0)
		{
			选择神将(list[0].名字);
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
		bool 可合成 = !全局将领库.是否号令类神君王(神将名);
		if (合成按钮 != null)
		{
			合成按钮.gameObject.SetActive(可合成);
		}
		if (升阶按钮 != null)
		{
			升阶按钮.gameObject.SetActive(可合成);
		}
	}

	public void 点击合成()
	{
		if (!string.IsNullOrEmpty(当前神将名))
		{
			将神坛系统.合成(当前神将名);
			刷新图鉴();
		}
	}

	public void 点击升阶()
	{
		if (!string.IsNullOrEmpty(当前神将名))
		{
			将神坛系统.升阶(当前神将名);
			刷新图鉴();
		}
	}
}
