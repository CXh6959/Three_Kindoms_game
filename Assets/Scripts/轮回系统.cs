using UnityEngine;
using UnityEngine.SceneManagement;
using 玩家数据结构;

public static class 轮回系统
{
	public static int 当前轮回数
	{
		get
		{
			if (全局变量.轮回进度 == null)
			{
				全局变量.轮回进度 = 轮回进度类.读取();
			}
			全局变量.轮回进度.修复旧数据();
			return 全局变量.轮回进度.轮回数;
		}
	}

	public static bool 是否可进入下一轮回()
	{
		return 全局变量.所有国家列表 != null && 全局变量.所有国家列表.Count == 1;
	}

	public static double 获取义军属性倍率()
	{
		return 当前轮回数 <= 1 ? 1.0 : Mathf.Pow(1.5f, 当前轮回数 - 1);
	}

	public static bool 进入下一轮回()
	{
		if (!是否可进入下一轮回())
		{
			显示提示("尚未统一天下，无法进入下一轮回");
			return false;
		}
		if (全局变量.轮回进度 == null)
		{
			全局变量.轮回进度 = 轮回进度类.读取();
		}
		全局变量.轮回进度.轮回数++;
		全局变量.轮回进度.修复旧数据();
		全局变量.轮回进度.保存();
		成就系统.检查轮回成就();
		初始化脚本.初始化游戏数据();
		SceneManager.LoadScene(1);
		return true;
	}

	private static void 显示提示(string 文本)
	{
		if (全局变量.提示类 != null)
		{
			全局变量.提示类.显示信息(文本);
		}
		else
		{
			Debug.Log(文本);
		}
	}
}

public class 轮回系统脚本 : MonoBehaviour
{
	public void 进入下一轮回按钮()
	{
		轮回系统.进入下一轮回();
	}
}
