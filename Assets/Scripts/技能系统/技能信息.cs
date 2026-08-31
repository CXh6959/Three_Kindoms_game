using System;

namespace 技能系统
{
	/// <summary>
	/// 主动技能定义。神将与 NPC 特殊将领的技能数据来自《神将技能数据表》。
	/// 阶段二接入战斗引擎（将领功能.cs）时按此数据执行（怒气/状态效果/多段/AOE）。
	/// </summary>
	[Serializable]
	public class 技能信息
	{
		public double ID;            // 技能ID

		public string 神将名;        // 该技能归属的神将/NPC 名

		public string 名字;          // 技能名

		public string 说明;          // 完整公式/效果描述（原样保留百分数与 X=当前轮回数 变量）

		public int 类型;             // 0物理伤害 1法术伤害 2火焰伤害 3治疗 4增益 5负面/控制

		public double 倍率;          // 主要伤害/治疗倍率（百分比数值，如 3000 表示 3000%）

		public double 冷却秒;        // 冷却时间（秒）；怒气型技能填 0，由怒气机制驱动

		public string 特效预制体名;  // VFX 预制体名（缺省复用打击特效）
	}
}
