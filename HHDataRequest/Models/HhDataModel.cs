using System;
using System.Collections.Generic;

namespace HHDataRequest.Models
{
	public class HhDataModel
	{
		public HhDataModel(int bigSalary, int lowSalary)
		{
			BigSalary = bigSalary;
			LowSalary = lowSalary;
			ProfessionsWithBigSalary = new List<String>();
			SkillsForBigSalary = new List<String>();
			ProfessionsWithLowSalary = new List<String>();
			SkillsForLowSalary = new List<String>();
		}

		public IList<String> ProfessionsWithBigSalary { get; private set; }
		public IList<String> SkillsForBigSalary { get; private set; }
		public IList<String> ProfessionsWithLowSalary { get; private set; }
		public IList<String> SkillsForLowSalary { get; private set; }
		public int BigSalary { get; private set; }
		public int LowSalary { get; private set; }
	}
}