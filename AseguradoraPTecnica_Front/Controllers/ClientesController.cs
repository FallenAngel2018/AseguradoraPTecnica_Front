using Microsoft.AspNetCore.Mvc;

namespace AseguradoraPTecnica_Front.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
