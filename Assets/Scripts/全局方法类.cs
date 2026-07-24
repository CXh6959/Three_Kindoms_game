using System;
using System.Security.Cryptography;
using System.Text;
using 玩家数据结构;

public class 全局方法类
{
	public static bool 删除指定国家(string 国家名字)
	{
		int count = 全局变量.所有国家列表.Count;
		for (int i = 0; i < count; i++)
		{
			if (全局变量.所有国家列表[i].国号 == 国家名字)
			{
				全局变量.所有国家列表.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public static int 获取指定国家的索引(string 国家名字)
	{
		int count = 全局变量.所有国家列表.Count;
		for (int i = 0; i < count; i++)
		{
			if (全局变量.所有国家列表[i].国号 == 国家名字)
			{
				return i;
			}
		}
		return -1;
	}

	public static 国家信息库类 获取指定ID的国家(int 指定ID)
	{
		int count = 全局变量.所有国家列表.Count;
		for (int i = 0; i < count; i++)
		{
			if (全局变量.所有国家列表[i].ID == 指定ID)
			{
				return 全局变量.所有国家列表[i];
			}
		}
		return null;
	}

	public static 国家信息库类 获取指定名字的国家(string 名字)
	{
		int count = 全局变量.所有国家列表.Count;
		for (int i = 0; i < count; i++)
		{
			if (全局变量.所有国家列表[i].国号 == 名字)
			{
				return 全局变量.所有国家列表[i];
			}
		}
		return null;
	}

	public static 玩家数据 获取指定名字的玩家(string 名字)
	{
		int count = 全局变量.所有玩家数据表.Count;
		for (int i = 0; i < count; i++)
		{
			if (全局变量.所有玩家数据表[i].基础信息.名字 == 名字)
			{
				return 全局变量.所有玩家数据表[i];
			}
		}
		return null;
	}

	public static string GetStrMd5(string ConvertString)
	{
		return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(ConvertString))).Replace("-", "");
	}
}
