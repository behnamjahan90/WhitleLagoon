﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IVillaRepository Villa {  get; private set; }

        public IVillaNumberRepository VillaNumber { get; private set; }
        public IAmenityRepository Amenity { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Villa = new VillaRepository(_db);
            VillaNumber = new VillaNumberRepository(_db);
            Amenity = new AmenityRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}