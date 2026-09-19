using UnityEngine;
using UnityEngine.UI;

public class 元宝抽奖脚本 : MonoBehaviour
{
	public Text 结果文本;
	public Text 元宝文本;

	public void 点击抽奖()
	{
		抽奖结果 result = 元宝抽奖系统.抽奖();
		if (结果文本 != null)
		{
			结果文本.text = result.成功 ? "获得 " + result.奖励名称 : result.奖励名称;
		}
		刷新余额();
	}

	public void 刷新余额()
	{
		if (元宝文本 != null && 全局变量.所有玩家数据表 != null && 全局变量.本机身份 >= 0 && 全局变量.本机身份 < 全局变量.所有玩家数据表.Count)
		{
			元宝文本.text = 全局变量.所有玩家数据表[全局变量.本机身份].财产信息.元宝.ToString("0");
		}
	}
}
