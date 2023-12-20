using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhitleLagoon.Web.ViewModels;

namespace WhitleLagoon.Web.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IVillaService _villaService;

        public AmenityController(IAmenityService amenityService, IVillaService villaService)
        {
            _amenityService = amenityService;
            _villaService = villaService;

        }
        public IActionResult Index()
        {
            //If we have another navigation property inside the villa we use .ThenInclude(u=>u.xxx)...
            var amenities = _amenityService.GetAllAmenities(includeproperties:"Villa");
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new() 
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
             
            //ViewData["VillaList"] = list;    ViewData is a dictionary and you will need a casting when calling in ui
            //ViewBag.VillaList = list; // ViewBag is dynamic type variable and you don't need casting while calling.
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj) 
        {
         
            if (!ModelState.IsValid)
            {
                TempData["error"] = "The Amenity could not be created";
                obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
            _amenityService.CreateAmenity(obj.Amenity);
            TempData["success"] = "The amenity has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _amenityService.GetAmenityById(amenityId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (amenityVM.Amenity == null) return RedirectToAction("Error","Home");

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "The amenity could not be created";
                amenityVM.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(amenityVM);
            }

            _amenityService.UpdateAmenity(amenityVM.Amenity);
            TempData["success"] = "The amenity has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _amenityService.GetAmenityById(amenityId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (amenityVM.Amenity == null) return RedirectToAction("Error", "Home");

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            if (_amenityService.DeleteAmenity(amenityVM.Amenity.Id))
            {
                TempData["success"] = "The amenity has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The amenity could not be deleted.";
            return View();
        }
    }
}
