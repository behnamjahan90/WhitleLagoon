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
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //If we have another navigation property inside the villa we use .ThenInclude(u=>u.xxx)...
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties:"Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new() 
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

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
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }

            _unitOfWork.VillaNumber.Add(obj.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa number has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u=> u.Villa_Number == villaNumberId)
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
                villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(villaNumberVM);
            }

            _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa number has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };
            //Villa? villa = _db.Villas.Find(villaId);
            if (villaNumberVM.VillaNumber == null) return RedirectToAction("Error", "Home");

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? obj = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            if (obj is not null)
            {
                _unitOfWork.VillaNumber.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa number could not be deleted.";
            return View();
        }
    }
}
