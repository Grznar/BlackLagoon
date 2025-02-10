using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    
    public class VillaController : Controller
    {

        private readonly ApplicationDbContext _db;

       public  VillaController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Villa> villas = _db.Villas.ToList();

            
            return View(villas);
        }
    }
}
