using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro_MVC5._0.Data;
using MoviePro_MVC5._0.Models.Database;
using MoviePro_MVC5._0.Models.Settings;
using System.Linq;
using System.Threading.Tasks;


namespace MoviePro_MVC5._0.Services
{
    //Seeds users into the database at runtime
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings,ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task ManageDataAsync()
        {
            await UpdateDatabaseAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCollections();
        }

        private async Task UpdateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
        //Seeds Administrator roles
        private async Task SeedRolesAsync()
        {
            //Searches for roles in the database if any returns , else it seeds IdentityRole (adminRole)
            if (_dbContext.Roles.Any()) return;
            var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        //Creates new user if no users are previously recorded in the database
        private async Task SeedUsersAsync()
        {
            if (_dbContext.Users.Any()) return;
            var credentials = _appSettings.MovieProSettings.DefaultCredentials;
            //Creates new user as instance of IdentityUser
            var newUser = new IdentityUser()
            {
                Email=credentials.Email,
                UserName=credentials.Email,
                EmailConfirmed=true
            };
            //Seeds user and role 
            await _userManager.CreateAsync(newUser, credentials.Password);
            await _userManager.AddToRoleAsync(newUser, credentials.Role);


        }

        private async Task SeedCollections()
        {
            if (_dbContext.Collection.Any()) return;
            _dbContext.Add(new Collection()
            {
                Name=_appSettings.MovieProSettings.DefaultCollection.Name,
                Description=_appSettings.MovieProSettings.DefaultCollection.Description
            });
            await _dbContext.SaveChangesAsync();
        }

    }
}
