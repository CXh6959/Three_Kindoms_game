using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace 玩家数据结构
{
	/// <summary>
	/// 跨轮回（NG+）持久化数据。独立于普通存档，落盘于 persistentDataPath/轮回进度.txt。
	/// 普通存档会被"进入下一轮回"清空，但这些数据需保留，故单独存放。
	/// </summary>
	public class 轮回进度类
	{
		// 当前轮回数（第 N 轮回），初始为 1
		public int 轮回数 = 1;

		// 将神珠 / 神将碎片为联网校验的合成材料计数，按需求三.4"无需放入宝库中"，故不放背包
		public int 将神珠数量;

		public int 神将碎片数量;

		// 已合成神将名单（每个神将只能合成一次，需求三.2）
		public List<string> 已合成神将 = new List<string>();

		// 已获得神将名单。成就奖励也写入这里，防止奖励与合成重复获得。
		public List<string> 已获得神将 = new List<string>();

		// 已解锁的轮回系列将；系列编号为1-10。
		public List<int> 已解锁系列将 = new List<int>();

		public List<神将阶位信息> 神将阶位表 = new List<神将阶位信息>();

		// 成就进度（永久解锁，跨轮回保留，需求八）
		public List<成就信息> 成就进度表 = new List<成就信息>();

		// 远程配置发放元宝的幂等版本号（需求七.2 服务器发放元宝，蓝奏云式软校验）
		public long 上次发放版本;

		// 联网元宝余额。-1 表示旧存档/首次运行，首次初始化时采用玩家默认余额。
		public double 元宝余额 = -1.0;

		// 远程材料账本版本，避免旧配置覆盖新数据。
		public long 材料版本;

		public static string 存档路径
		{
			get
			{
				return Application.persistentDataPath + "/轮回进度.txt";
			}
		}

		public static 轮回进度类 读取()
		{
			try
			{
				if (File.Exists(存档路径))
				{
					string text = File.ReadAllText(存档路径);
					轮回进度类 data = JsonConvert.DeserializeObject<轮回进度类>(text);
					if (data != null)
					{
						data.修复旧数据();
						return data;
					}
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogWarning("读取轮回进度失败: " + e.Message);
			}
			return new 轮回进度类();
		}

		public void 修复旧数据()
		{
			if (轮回数 < 1)
			{
				轮回数 = 1;
			}
			if (已合成神将 == null)
			{
				已合成神将 = new List<string>();
			}
			if (已获得神将 == null)
			{
				已获得神将 = new List<string>();
			}
			if (已解锁系列将 == null)
			{
				已解锁系列将 = new List<int>();
			}
			if (成就进度表 == null)
			{
				成就进度表 = new List<成就信息>();
			}
			if (神将阶位表 == null)
			{
				神将阶位表 = new List<神将阶位信息>();
			}
			for (int i = 1; i <= 10 && i <= 轮回数; i++)
			{
				if (!已解锁系列将.Contains(i))
				{
					已解锁系列将.Add(i);
				}
			}
		}

		public void 保存()
		{
			try
			{
				string text = JsonConvert.SerializeObject(this);
				File.WriteAllText(存档路径, text);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogWarning("保存轮回进度失败: " + e.Message);
			}
		}

		// 成就进度查询（不存在则自动创建一条空记录）
		public 成就信息 获取成就进度(string 成就ID)
		{
			int count = 成就进度表.Count;
			for (int i = 0; i < count; i++)
			{
				if (成就进度表[i] != null && 成就进度表[i].成就ID == 成就ID)
				{
					return 成就进度表[i];
				}
			}
			成就信息 info = new 成就信息();
			info.成就ID = 成就ID;
			成就进度表.Add(info);
			return info;
		}

		public bool 是否已合成神将(string 名字)
		{
			修复旧数据();
			return 已合成神将.Contains(名字);
		}

		public void 记录已合成神将(string 名字)
		{
			修复旧数据();
			if (!已合成神将.Contains(名字))
			{
				已合成神将.Add(名字);
				保存();
			}
		}

		public bool 是否已获得神将(string 名字)
		{
			修复旧数据();
			return 已获得神将.Contains(名字);
		}

		public void 记录已获得神将(string 名字)
		{
			修复旧数据();
			if (!已获得神将.Contains(名字))
			{
				已获得神将.Add(名字);
				保存();
			}
		}

		public bool 是否解锁系列将(int 系列编号)
		{
			修复旧数据();
			return 系列编号 >= 1 && 系列编号 <= 10 && 轮回数 >= 系列编号;
		}

		public int 获取神将阶位(string 神将名)
		{
			修复旧数据();
			for (int i = 0; i < 神将阶位表.Count; i++)
			{
				if (神将阶位表[i] != null && 神将阶位表[i].神将名 == 神将名)
				{
					return Mathf.Clamp(神将阶位表[i].阶位, 0, 3);
				}
			}
			return 0;
		}

		public void 设置神将阶位(string 神将名, int 阶位)
		{
			修复旧数据();
			阶位 = Mathf.Clamp(阶位, 0, 3);
			for (int i = 0; i < 神将阶位表.Count; i++)
			{
				if (神将阶位表[i] != null && 神将阶位表[i].神将名 == 神将名)
				{
					神将阶位表[i].阶位 = 阶位;
					return;
				}
			}
			神将阶位表.Add(new 神将阶位信息 { 神将名 = 神将名, 阶位 = 阶位 });
		}
	}

	public class 神将阶位信息
	{
		public string 神将名;
		public int 阶位;
	}
}
