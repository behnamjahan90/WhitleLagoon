using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Infrastructure.Data
{
    public class DbInitilizer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitilizer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "adminb@dotnetmastery.com",
                        Email = "adminb@dotnetmastery.com",
                        Name = "Behnam Jahangiri",
                        NormalizedUserName = "ADMINB@DOTNETMASTERY.COM",
                        NormalizedEmail = "ADMINB@DOTNETMASTERY.COM",
                        PhoneNumber = "1234567890",
                    }, "83H711bbwwgg@").GetAwaiter().GetResult();//we use these methods when we have not in task method to get rid of await syntax

                    ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "adminb@dotnetmastery.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }               
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
    }
}
