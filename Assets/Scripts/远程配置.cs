using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using 玩家数据结构;

/// <summary>
/// 蓝奏云式只读远程配置（需求七：服务器发放元宝 / 将神珠·神将碎片数量联网校验）。
/// 复用现有 UnityWebRequest 模式（见 开始界面背景动画.cs 的验证状态），但：
///   1) 使用独立地址 全局变量.远程配置地址，绝不改动 全局变量.url（避免触发删档验证）；
///   2) 是全局只读配置、非按账号账本，本地仍可篡改——符合"软校验"定位。
/// 阶段四由登录流程在 拉取配置() 完成后调用 尝试发放元宝()。
/// </summary>
public class 远程配置
{
	public static long 元宝发放额度;

	public static long 发放版本号;

	public static int 将神珠校验值;

	public static int 神将碎片校验值;

	public static bool 已加载;

	// 约定的远程文件格式（每行 "键=值"）：
	//   元宝发放=10000
	//   发放版本=5
	//   将神珠校验=999
	//   神将碎片校验=9999
	public static IEnumerator 拉取配置(string 地址)
	{
		if (string.IsNullOrEmpty(地址))
		{
			yield break;
		}
		using (UnityWebRequest req = UnityWebRequest.Get(地址))
		{
			yield return req.SendWebRequest();
			if (req.result == UnityWebRequest.Result.Success)
			{
				解析(req.downloadHandler.text);
				已加载 = true;
			}
			else
			{
				UnityEngine.Debug.LogWarning("远程配置拉取失败: " + req.error);
			}
		}
	}

	private static void 解析(string 文本)
	{
		try
		{
			string[] lines = 文本.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();
				int eq = line.IndexOf('=');
				if (eq <= 0)
				{
					continue;
				}
				string key = line.Substring(0, eq).Trim();
				string val = line.Substring(eq + 1).Trim();
				long v;
				if (!long.TryParse(val, out v))
				{
					continue;
				}
				switch (key)
				{
					case "元宝发放":
						元宝发放额度 = v;
						break;
					case "发放版本":
						发放版本号 = v;
						break;
					case "将神珠校验":
						将神珠校验值 = (int)v;
						break;
					case "神将碎片校验":
						神将碎片校验值 = (int)v;
						break;
				}
			}
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogWarning("远程配置解析失败: " + e.Message);
		}
	}

	// 幂等发放元宝：仅当远程"发放版本号"大于本地"上次发放版本"时发放一次
	public static void 尝试发放元宝()
	{
		if (!已加载 || 发放版本号 <= 0 || 元宝发放额度 <= 0)
		{
			return;
		}
		if (全局变量.轮回进度 == null || 发放版本号 <= 全局变量.轮回进度.上次发放版本)
		{
			return;
		}
		int 本机身份 = 全局变量.本机身份;
		if (本机身份 < 0 || 本机身份 >= 全局变量.所有玩家数据表.Count)
		{
			return;
		}
		全局变量.所有玩家数据表[本机身份].财产信息.元宝 += 元宝发放额度;
		全局变量.轮回进度.上次发放版本 = 发放版本号;
		全局变量.轮回进度.保存();
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息("领取元宝 " + 元宝发放额度);
		}
	}
}
