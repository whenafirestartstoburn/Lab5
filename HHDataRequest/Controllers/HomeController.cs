using System.Web.Mvc;
using HHDataRequest.Models;
using HHDataRequest.Service;

namespace HHDataRequest.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHhVacanciesService _service = new HhVacanciesService();
		private const int defaultBigSalary = 120000;
		private const int defaultLowSalary = 15000;

		public ActionResult Index()
		{
			HhDataModel model = _service.GetVacancies(defaultBigSalary, defaultLowSalary);
			return View(model);
		}
	}
}