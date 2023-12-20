using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhitleLagoon.Web.ViewModels;

namespace WhitleLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;

        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;

        }
        public IActionResult Index()
        {
            //If we have another navigation property inside the villa we use .ThenInclude(u=>u.xxx)...
            var villaNumbers = _villaNumberService.GetAllVillaNumbers("Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new() 
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
             
            //ViewData["VillaList"] = list;    ViewData is a dictionary and you will need a casting when calling in ui
            //ViewBag.VillaList = list; // ViewBag is dynamic type variable and you don't need casting while calling.
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj) 
        {
            //more efficient way to check an item exists in database
            bool roomNumberExists = _villaNumberService.CheckVillaNumberExists(obj.VillaNumber.Villa_Number);

            if (!ModelState.IsValid || roomNumberExists)
            {
                var errorMsg = "";
                if (roomNumberExists)
                    errorMsg = "The Villa number already exists.";
                else
                {
                    errorMsg = "The Villa number could not be created";
                }

                TempData["error"] = errorMsg;
                obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
            _villaNumberService.CreateVillaNumber(obj.VillaNumber);
            TempData["success"] = "The villa number has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (villaNumberVM.VillaNumber == null) return RedirectToAction("Error","Home");

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "The Villa number could not be created";
                villaNumberVM.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(villaNumberVM);
            }

            _villaNumberService.UpdateVillaNumber(villaNumberVM.VillaNumber);
            TempData["success"] = "The villa number has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (villaNumberVM.VillaNumber == null) return RedirectToAction("Error", "Home");

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? obj = _villaNumberService.GetVillaNumberById(villaNumberVM.VillaNumber.Villa_Number);
            if (obj is not null)
            {
                _villaNumberService.DeleteVillaNumber(obj.Villa_Number);
                TempData["success"] = "The villa number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa number could not be deleted.";
            return View();
        }
    }
}
