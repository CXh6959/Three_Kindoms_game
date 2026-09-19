using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 兵营脚本 : MonoBehaviour
{
	public GameObject 标题列表对象;

	public Image 头像对象;

	public Text 名字等级对象;

	public GameObject 兵种列表对象;

	public 调整数量脚本 调整招募数量脚本对象;

	public int 第几个玩家;

	public int 第几个封地;

	public int 第几个建筑;

	public int 兵营类型 = 4;

	public void 刷新显示()
	{
		确保特殊兵种条目();
		标题列表对象.transform.GetChild(兵营类型 - 4).GetComponent<Toggle>().isOn = true;
		建筑信息 建筑 = 全局变量.所有玩家数据表[第几个玩家].封地信息表[第几个封地].建筑信息表[第几个建筑];
		int 头像索引 = 建筑.获取建筑头像索引();
		头像对象.sprite = 全局变量.书院头像资源表[头像索引];
		名字等级对象.text = 建筑.获取建筑等级文本();

		bool 特殊兵种已开启 = 轮回系统.当前轮回数 >= 10;
		if (兵种列表对象.transform.childCount > 4)
		{
			兵种列表对象.transform.GetChild(4).gameObject.SetActive(特殊兵种已开启);
		}
		int 显示兵种数量 = 特殊兵种已开启 && 兵种列表对象.transform.childCount > 4 ? 5 : 4;
		for (int i = 0; i < 显示兵种数量; i++)
		{
			int 兵种ID = (兵营类型 - 3) * 100 + i + 1;
			兵种属性库类 兵种 = 全局兵种库.查询指定ID的数据(兵种ID);
			if (兵种 == null)
			{
				continue;
			}

			Transform 条目 = 兵种列表对象.transform.GetChild(i);
			int 图片索引 = 全局兵种库.查询指定兵种的图片(兵种.名称);
			if (图片索引 >= 0 && 全局变量.所有兵种图片资源表 != null && 图片索引 < 全局变量.所有兵种图片资源表.Length)
			{
				条目.GetChild(1).GetComponent<Image>().sprite = 全局变量.所有兵种图片资源表[图片索引];
			}

			double 已有数量 = 全局变量.所有玩家数据表[第几个玩家].获取指定兵种ID总数(兵种ID);
			Transform 名称区域 = 条目.GetChild(2);
			名称区域.GetComponent<Text>().text = 兵种.名称;
			GameObject 数量文本 = 名称区域.GetChild(0).gameObject;
			GameObject 未解锁文本 = 名称区域.GetChild(1).gameObject;
			bool 已解锁 = i == 4 || 建筑.等级 > i * 3;
			数量文本.SetActive(已解锁);
			未解锁文本.SetActive(!已解锁);
			条目.GetChild(15).gameObject.SetActive(!已解锁);
			if (已解锁)
			{
				数量文本.GetComponent<Text>().text = "(数量" + 已有数量.ToString() + ")";
			}

			条目.GetChild(4).GetComponent<Text>().text = 兵种.攻击力.ToString();
			条目.GetChild(6).GetComponent<Text>().text = 兵种.防御力.ToString();
			条目.GetChild(8).GetComponent<Text>().text = 兵种.生命值.ToString();
			条目.GetChild(10).GetComponent<Text>().text = 兵种.攻击速度.ToString();
			条目.GetChild(12).GetComponent<Text>().text = 兵种.移动速度.ToString();
			条目.GetChild(14).GetComponent<Text>().text = 兵种.占用人口.ToString();
		}
	}

	private void 确保特殊兵种条目()
	{
		if (兵种列表对象 == null || 轮回系统.当前轮回数 < 10 || 兵种列表对象.transform.childCount >= 5 || 兵种列表对象.transform.childCount < 4)
		{
			return;
		}
		Transform source = 兵种列表对象.transform.GetChild(3);
		GameObject item = Instantiate(source.gameObject, 兵种列表对象.transform);
		item.name = "特殊兵种";
		RectTransform itemRect = item.GetComponent<RectTransform>();
		RectTransform sourceRect = source.GetComponent<RectTransform>();
		RectTransform previousRect = 兵种列表对象.transform.GetChild(2).GetComponent<RectTransform>();
		if (itemRect != null && sourceRect != null && previousRect != null && 兵种列表对象.GetComponent<LayoutGroup>() == null)
		{
			itemRect.anchoredPosition = sourceRect.anchoredPosition + (sourceRect.anchoredPosition - previousRect.anchoredPosition);
		}
		item.SetActive(true);
		if (item.transform.childCount > 16)
		{
			item.transform.GetChild(16).gameObject.SetActive(false);
		}
	}

	public void 招募选中兵种()
	{
		int 选中兵种ID = 0;
		int 可选兵种数量 = 轮回系统.当前轮回数 >= 10 && 兵种列表对象.transform.childCount > 4 ? 5 : 4;
		for (int i = 0; i < 可选兵种数量; i++)
		{
			if (兵种列表对象.transform.GetChild(i).GetChild(16).gameObject.activeSelf)
			{
				选中兵种ID = (兵营类型 - 3) * 100 + i + 1;
				UnityEngine.Debug.Log("选中ID" + 选中兵种ID.ToString());
				break;
			}
		}
		if (选中兵种ID == 0)
		{
			return;
		}
		调整招募数量脚本对象.第几个封地 = 第几个封地;
		调整招募数量脚本对象.第几个玩家 = 第几个玩家;
		调整招募数量脚本对象.调整类型 = 1;
		调整招募数量脚本对象.兵种ID = 选中兵种ID;
		调整招募数量脚本对象.显示调整界面();
		调整招募数量脚本对象.显示说明文本();
	}
}
