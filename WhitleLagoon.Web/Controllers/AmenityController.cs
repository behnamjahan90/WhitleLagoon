using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhitleLagoon.Web.ViewModels;

namespace WhitleLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //If we have another navigation property inside the villa we use .ThenInclude(u=>u.xxx)...
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties:"Villa");
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new() 
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }

            _unitOfWork.Amenity.Add(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The amenity has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u=> u.Id == amenityId)
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
                amenityVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(amenityVM);
            }

            _unitOfWork.Amenity.Update(amenityVM.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The amenity has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (amenityVM.Amenity == null) return RedirectToAction("Error", "Home");

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            Amenity? obj = _unitOfWork.Amenity.Get(x => x.Id == amenityVM.Amenity.Id);
            if (obj is not null)
            {
                _unitOfWork.Amenity.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The amenity could not be deleted.";
            return View();
        }
    }
}
