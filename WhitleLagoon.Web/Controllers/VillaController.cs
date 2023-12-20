using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;

namespace WhitleLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;

        }
        public IActionResult Index()
        {
            var villas = _villaService.GetAllVillas();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj) 
        {
            if (obj.Name==obj.Description) 
            {
                ModelState.AddModelError("name", "The Description cannot exactly match the Name");
            }
            if (!ModelState.IsValid)
            {
                TempData["error"] = "The villa could not be created.";
                return View();
            }
            _villaService.CreateVilla(obj);
            TempData["success"] = "The villa has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _villaService.GetVillaById(villaId);
            //Villa? villa = _db.Villas.Find(villaId);
            if (villa == null) return RedirectToAction("Error","Home");


            return View(villa);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if(ModelState.IsValid && villa.Id>0)
            {
                _villaService.UpdateVilla(villa);
                TempData["success"] = "The villa has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be updated.";
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _villaService.GetVillaById(villaId);
            //Villa? villa = _db.Villas.Find(villaId);
            if (villa == null) return RedirectToAction("Error", "Home");
            return View(villa);
        }

        [HttpPost]
        public IActionResult Delete(Villa villa)
        {
            if(_villaService.DeleteVilla(villa.Id))
            {
                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "The villa could not be deleted.";
            }

            return View(villa);
        }
    }
}
