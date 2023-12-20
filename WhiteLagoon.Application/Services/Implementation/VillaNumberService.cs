using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckVillaNumberExists(int villa_Number)
        {
           return _unitOfWork.VillaNumber.Any(u => u.Villa_Number == villa_Number);
        }

        public void CreateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumber.Add(villaNumber);
            _unitOfWork.Save();
        }

        public bool DeleteVillaNumber(int id)
        {
            try
            {
                VillaNumber? obj = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == id);
                if (obj is not null)
                {
                    _unitOfWork.VillaNumber.Remove(obj);
                    _unitOfWork.Save();
                    return true;
                } 
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<VillaNumber> GetAllVillaNumbers(string? includeProperties=null)
        {
            if (!string.IsNullOrEmpty(includeProperties))
                return _unitOfWork.VillaNumber.GetAll(includeProperties: includeProperties);
            else
                return _unitOfWork.VillaNumber.GetAll();
        }

        public VillaNumber GetVillaNumberById(int id,string? includeProperties=null)
        {
            if (!string.IsNullOrEmpty(includeProperties))
                return _unitOfWork.VillaNumber.Get(u=>u.Villa_Number == id,includeProperties: includeProperties);
            else
                return _unitOfWork.VillaNumber.Get(u => u.Villa_Number == id);
        }

        public void UpdateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumber.Update(villaNumber);
            _unitOfWork.Save();
        }
    }
}
