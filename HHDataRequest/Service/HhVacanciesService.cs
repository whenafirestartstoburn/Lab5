using RestSharp;
using HHDataRequest.Models;
using Newtonsoft.Json.Linq;

namespace HHDataRequest.Service
{
	public class HhVacanciesService : IHhVacanciesService
	{
		private const string HhApiHost = "https://api.hh.ru";
		private const string HhApiVacanciesResource = "/vacancies";
		private const int VacanciesPerPage = 100;
		private const int VacanciesFirstPage = 0;
		private const string UserAgentHeader = "HhGetVacanciesService/1.0 (mealtime @inbox.ru)";
		private const string VacanciesQueryFormat = "{0}?page={1}&per_page={2}";
		private const string VacancyDetailsQueryFormat = "{0}/{1}";

		private readonly IRestClient _client = new RestClient(HhApiHost);

		public HhDataModel GetVacancies(int bigSalary, int lowSalary)
		{
			HhDataModel model = new HhDataModel(bigSalary, lowSalary);
			IRestResponse response = RequestVacancies(VacanciesFirstPage);
			int pagesCount = (int) JObject.Parse(response.Content)["pages"];
			JArray vacancies = JObject.Parse(response.Content)["items"] as JArray;
			for (int i = VacanciesFirstPage; i < pagesCount; i++)
			{
				foreach (JToken vacancy in vacancies)
				{
					if (vacancy["salary"].Type == JTokenType.Null)
						continue;
					JToken salaryFrom = vacancy["salary"]["from"];
					JToken salaryTo = vacancy["salary"]["to"];
					JToken salaryCurr = vacancy["salary"]["currency"];
					JTokenType salaryFromType = salaryFrom.Type;
					JTokenType salaryToType = salaryTo.Type;
					double salary = -1D;
					if ((string)salaryCurr != "RUR")
						continue;
					else if (salaryFromType != JTokenType.Null && salaryToType != JTokenType.Null)
						salary = ((double) salaryFrom + (double) salaryTo) / 2;
					else if (salaryFromType == JTokenType.Null && salaryToType != JTokenType.Null)
						salary = (double) salaryTo;
					else if (salaryFromType != JTokenType.Null && salaryToType == JTokenType.Null)
						salary = (double) salaryFrom;
					if (salary > bigSalary)
					{
						model.ProfessionsWithBigSalary.Add((string)vacancy["name"]);
						JToken vacancyDetails = JObject.Parse(RequestVacancyDetails((string)vacancy["id"]).Content);
						JArray keySkills = vacancyDetails["key_skills"] as JArray;
						if(keySkills.HasValues)
						{
							foreach(JToken keySkill in keySkills)
							{
								model.SkillsForBigSalary.Add((string) keySkill["name"]);
							}
						}
					} else if (salary > 0 && salary < lowSalary)
					{
						model.ProfessionsWithLowSalary.Add((string)vacancy["name"]);
						JToken vacancyDetails = JObject.Parse(RequestVacancyDetails((string)vacancy["id"]).Content);
						JArray keySkills = vacancyDetails["key_skills"] as JArray;
						if (keySkills.HasValues)
						{
							foreach (JToken keySkill in keySkills)
							{
								model.SkillsForLowSalary.Add((string)keySkill["name"]);
							}
						}
					}
				}
				response = RequestVacancies(VacanciesFirstPage + i + 1);
				vacancies = JObject.Parse(response.Content)["items"] as JArray;
			}
			return model;
		}

		private IRestResponse RequestVacancies(int page)
		{
			IRestRequest request = new RestRequest(string.Format(VacanciesQueryFormat, HhApiVacanciesResource, page, VacanciesPerPage), Method.GET);
			request.AddHeader("User-Agent", UserAgentHeader);
			request.AddParameter("only_with_salary", "true");
			return _client.Execute(request);
		}

		private IRestResponse RequestVacancyDetails(string id)
		{
			IRestRequest request = new RestRequest(string.Format(VacancyDetailsQueryFormat, HhApiVacanciesResource, id), Method.GET);
			request.AddHeader("User-Agent", UserAgentHeader);
			return _client.Execute(request);
		}
	}
}