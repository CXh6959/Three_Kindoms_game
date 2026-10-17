using System;
using System.Globalization;

public static class 战斗数值格式
{
	public static string 显示(double 数值)
	{
		if (double.IsNaN(数值) || 数值 <= 0.0) return "0";
		if (double.IsInfinity(数值)) return "--";
		if (数值 >= 1e12) return 数值.ToString("0.##E+0", CultureInfo.InvariantCulture);
		if (数值 >= 1e8) return (数值 / 1e8).ToString("0.##", CultureInfo.InvariantCulture) + "亿";
		if (数值 >= 1e4) return (数值 / 1e4).ToString("0.##", CultureInfo.InvariantCulture) + "万";
		return Math.Ceiling(数值).ToString("0", CultureInfo.InvariantCulture);
	}
}
