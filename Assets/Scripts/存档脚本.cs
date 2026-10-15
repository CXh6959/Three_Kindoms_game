using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 存档脚本 : MonoBehaviour
{
	public GameObject 存档列表对象;

	private bool 正在写入;
	private readonly HashSet<int> 备份恢复槽位 = new HashSet<int>();
	private readonly HashSet<int> 损坏槽位 = new HashSet<int>();

	public void 刷新存档列表()
	{
		读取所有存档();
		显示所有存档();
	}

	public void 读取所有存档()
	{
		全局变量.所有存档列表.Clear();
		备份恢复槽位.Clear();
		损坏槽位.Clear();
		for (int slot = 1; slot <= 6; slot++)
		{
			string path = 获取存档路径(slot);
			存档信息库类 data;
			if (尝试读取(path, slot, out data))
			{
				全局变量.所有存档列表.Add(data);
			}
			else if (尝试读取(path + ".bak", slot, out data) || 尝试读取(path + ".tmp", slot, out data))
			{
				全局变量.所有存档列表.Add(data);
				备份恢复槽位.Add(slot);
			}
			else if (File.Exists(path) || File.Exists(path + ".bak") || File.Exists(path + ".tmp"))
			{
				损坏槽位.Add(slot);
			}
		}
	}

	private string 获取存档路径(int slot)
	{
		return Path.Combine(Application.persistentDataPath, "存档" + slot + ".txt");
	}

	private static bool 存档有效(存档信息库类 data, int slot)
	{
		return data != null && data.ID == slot && data.国家列表 != null && data.城池列表 != null
			&& data.玩家列表 != null && data.玩家列表.Count >= 3 && data.玩家列表[0] != null
			&& data.玩家列表[0].基础信息 != null && data.玩家列表.TrueForAll(player => player != null && player.封地信息表 != null);
	}

	private static bool 尝试读取(string path, int slot, out 存档信息库类 data)
	{
		data = null;
		if (!File.Exists(path)) return false;
		try
		{
			data = JsonConvert.DeserializeObject<存档信息库类>(File.ReadAllText(path));
			return 存档有效(data, slot);
		}
		catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is JsonException)
		{
			UnityEngine.Debug.LogWarning("存档读取失败：" + Path.GetFileName(path) + "，" + error.Message);
			return false;
		}
	}

	public void 显示所有存档()
	{
		if (存档列表对象 == null) return;
		for (int i = 0; i < Mathf.Min(6, 存档列表对象.transform.childCount); i++)
		{
			Text component = 存档列表对象.transform.GetChild(i).GetChild(1).GetComponent<Text>();
			int slot = i + 1;
			存档信息库类 data = 全局变量.所有存档列表.Find(item => item.ID == slot);
			if (data != null)
			{
				component.text = "存档" + slot + "：" + data.玩家列表[0].基础信息.名字 + "<" + data.玩家列表[0].基础信息.等级.ToString() + "级>   " + data.玩家列表[0].基础信息.国家 + "   " + TIME.转时间格式(data.存档时间) + (备份恢复槽位.Contains(slot) ? "（备份恢复）" : "");
			}
			else
			{
				component.text = "存档" + slot + (损坏槽位.Contains(slot) ? "：<文件损坏>" : "：<空记录>");
			}
		}
	}

	public void 读取指定存档(int 第几个存档)
	{
		if (第几个存档 < 1 || 第几个存档 > 6) return;
		读取所有存档();
		存档信息库类 data = 全局变量.所有存档列表.Find(item => item.ID == 第几个存档);
		if (data == null)
		{
			显示提示(损坏槽位.Contains(第几个存档) ? "存档损坏，未找到有效备份。" : "此槽位没有存档。");
			显示所有存档();
			return;
		}
		if (存档有效(data, 第几个存档))
		{
			UnityEngine.Debug.Log("读取存档" + 第几个存档.ToString() + "成功!");
			全局变量.所有国家列表 = data.国家列表;
			全局变量.所有玩家数据表 = data.玩家列表;
			全局变量.所有城池列表 = data.城池列表;
			全局变量.军情列表.Clear();
			全局变量.战场列表.Clear();
			初始化脚本.修复系统名将();
			int count = 全局变量.所有玩家数据表.Count;
			for (int i = 0; i < count; i++)
			{
				全局变量.所有玩家数据表[i].重置将领状态();
			}
			所有城池界面脚本.重置城池状态();
			SceneManager.LoadScene(1);
		}
	}

	public void 写入指定存档(int 第几个存档)
	{
		if (正在写入 || 第几个存档 < 1 || 第几个存档 > 6) return;
		存档信息库类 data = new 存档信息库类
		{
			ID = 第几个存档,
			存档时间 = TIME.getTime(),
			国家列表 = 全局变量.所有国家列表,
			城池列表 = 全局变量.所有城池列表,
			玩家列表 = 全局变量.所有玩家数据表
		};
		if (!存档有效(data, 第几个存档))
		{
			显示提示("游戏数据尚未准备好，无法保存。");
			return;
		}
		正在写入 = true;
		try
		{
			Directory.CreateDirectory(Application.persistentDataPath);
			string path = 获取存档路径(第几个存档);
			string temporary = path + ".tmp";
			string contents = JsonConvert.SerializeObject(data);
			File.WriteAllText(temporary, contents);
			if (File.Exists(path))
			{
				存档信息库类 previous;
				// 损坏的主文件不能覆盖已有的有效备份。
				if (尝试读取(path, 第几个存档, out previous)) File.Copy(path, path + ".bak", true);
				try { File.Replace(temporary, path, null); }
				catch (PlatformNotSupportedException)
				{
					File.Copy(temporary, path, true);
					File.Delete(temporary);
				}
			}
			else File.Move(temporary, path);
			刷新存档列表();
			显示提示("保存存档" + 第几个存档 + "成功!");
		}
		catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is JsonException)
		{
			UnityEngine.Debug.LogError("保存存档失败：" + error.Message);
			显示提示("保存失败，请检查剩余存储空间。");
		}
		finally { 正在写入 = false; }
	}

	private static void 显示提示(string message)
	{
		if (全局变量.提示类 != null) 全局变量.提示类.显示信息(message);
		else UnityEngine.Debug.Log(message);
	}
}
