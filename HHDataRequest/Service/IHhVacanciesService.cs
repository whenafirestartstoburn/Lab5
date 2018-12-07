using HHDataRequest.Models;

namespace HHDataRequest.Service
{
	public interface IHhVacanciesService
	{
		HhDataModel GetVacancies(int bigSalary, int lowSalary); 
	}
}
