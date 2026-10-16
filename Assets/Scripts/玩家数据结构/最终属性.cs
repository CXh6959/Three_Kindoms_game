namespace 玩家数据结构
{
	public class 最终属性
	{
		public double 武力;

		public double 攻击;

		public double 智力;

		public double 防御;

		public double 统帅;

		public double 统兵;

		public double 生命值;

		public double 体力上限;

		public double 统帅加成;

		public long 统帅加成剩余时间;

		public void 修复无效数值()
		{
			武力 = 有效数值(武力);
			智力 = 有效数值(智力);
			统帅 = 有效数值(统帅);
			攻击 = 有效数值(攻击);
			防御 = 有效数值(防御);
			统兵 = 有效数值(统兵);
			生命值 = 有效数值(生命值);
			体力上限 = 有效数值(体力上限);
		}

		private static double 有效数值(double value)
		{
			// 正常高属性不截断；阻止损坏数据把 NaN/Infinity 传入血条和战斗运算。
			return double.IsNaN(value) || double.IsInfinity(value) || value < 0.0 ? 0.0 : value;
		}

		public void 增加持续时间()
		{
			long num = 3600L;
			if (统帅加成剩余时间 == 0L)
			{
				统帅加成剩余时间 = TIME.getTime();
			}
			if (统帅加成剩余时间 <= TIME.getTime())
			{
				统帅加成剩余时间 = TIME.getTime() + num;
			}
			else
			{
				统帅加成剩余时间 += num;
			}
		}

		public long 获取统帅加成剩余时间()
		{
			if (统帅加成剩余时间 > 0)
			{
				long num = 统帅加成剩余时间 - TIME.getTime();
				if (num > 0)
				{
					return num;
				}
			}
			return 0L;
		}
	}
}
