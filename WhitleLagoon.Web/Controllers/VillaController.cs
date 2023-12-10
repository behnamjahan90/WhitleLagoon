using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhitleLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;//this is used for retrieving image files from wwwroot and save there

        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
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

            if (obj.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);
                obj.ImageUrl = @"\images\VillaImage\" + fileName;
            }
            else
            {
                obj.ImageUrl = "https://placehold.co/600*400";
            }

            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "The villa has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _unitOfWork.Villa.Get(x => x.Id == villaId);
            //Villa? villa = _db.Villas.Find(villaId);
            if (villa == null) return RedirectToAction("Error","Home");


            return View(villa);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if(ModelState.IsValid && villa.Id>0)
            {

                if (villa.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    if (string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        var oldImageUrl = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImageUrl))
                        {
                            System.IO.File.Delete(oldImageUrl);
                        }
                    }
                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    villa.Image.CopyTo(fileStream);
                    villa.ImageUrl = @"\images\VillaImage\" + fileName;
                }

                _unitOfWork.Villa.Update(villa);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be updated.";
            return View(villa);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? villa = _unitOfWork.Villa.Get(x => x.Id == villaId);
            //Villa? villa = _db.Villas.Find(villaId);
            if (villa == null) return RedirectToAction("Error", "Home");
            return View(villa);
        }

        [HttpPost]
        public IActionResult Delete(Villa villa)
        {
            Villa? obj = _unitOfWork.Villa.Get(x => x.Id == villa.Id);
            if (obj is not null)
            {
                if (!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var oldImageUrl = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImageUrl))
                    {
                        System.IO.File.Delete(oldImageUrl);
                    }
                }

                _unitOfWork.Villa.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be deleted.";
            return View(villa);
        }
    }
}
