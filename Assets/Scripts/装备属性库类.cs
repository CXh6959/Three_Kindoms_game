public class 装备属性库类
{
	public string 名称;

	public string 类型;

	public double 等级;

	public double 基础值;

	public bool 是否轮回装备;

	public int 来源轮回;

	public bool 是否可交易;

	public 装备属性库类(string 名称1, string 类型1, double 等级1, double 基础值1)
	{
		名称 = 名称1;
		类型 = 类型1;
		等级 = 等级1;
		基础值 = 基础值1;
	}
}
