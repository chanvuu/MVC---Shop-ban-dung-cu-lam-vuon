using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Respositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ApplicationUser applicationUser)
        {
            var userDB = _context.ApplicationUsers.FirstOrDefault(x => x.Id == applicationUser.Id);
            if (userDB != null)
            {
                userDB.Name = applicationUser.Name;
                userDB.Address = applicationUser.Address;
                userDB.City = applicationUser.City;
                userDB.State = applicationUser.State;
                userDB.PinCode = applicationUser.PinCode;
            }
        }
    }
}
