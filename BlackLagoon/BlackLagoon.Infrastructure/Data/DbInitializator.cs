using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Infrastructure.Data
{
    public class DbInitializator : IDbInitializor
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public DbInitializator(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
           _userManager = userManager;
        }

        public async void Initializeta()
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
                   await  _userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        NameOfUser = "Admin",
                        PhoneNumber = "123456789",

                    }, "Heslo_123");
                    ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
