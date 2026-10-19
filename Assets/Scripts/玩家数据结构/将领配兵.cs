namespace 玩家数据结构
{
	public class 将领配兵
	{
		public double ID;

		public double 数量;

		public double 上次兵种ID;

		public void 记录当前兵种()
		{
			if (ID > 0.0 && 数量 > 0.0) 上次兵种ID = ID;
		}
	}
}
