using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Data
{
    public class DBInitializer
    {
        public static void SeedDataBase(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            var config = services.GetRequiredService<IConfiguration>();

            InitializeIdentity(userManager, roleManager, config).Wait();

            if (!context.Horse.Any())
            {
                CreateHorses(context);
            }
        }

        public static void CreateHorses(AppDbContext context)
        {

            var horse = new Horse
            {
                Name = "Bella",
                DOB = new DateTime(2011, 06, 24),
                Sex = "Mare",
                Colour = "Grey",
                Heightcm = 144.272,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse);
          

            horse = new Horse
            {
                Name = "Star",
                DOB = new DateTime(2009, 04, 01),
                Sex = "Gelding",
                Colour = "Bay",
                Heightcm = 142.24,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse);       
 

            horse = new Horse
            {
                Name = "Tilly",
                DOB = new DateTime(2019, 07, 22),
                Sex = "Filly",
                Colour = "Chestnut",
                Heightcm = 152.4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse); 

            horse = new Horse
            {
                Name = "Apollo",
                DOB = new DateTime(2013, 05, 01),
                Sex = "Gelding",
                Colour = "Brown",
                Heightcm = 172.72,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse);       

            horse = new Horse
            {
                Name = "Oliver Twist",
                DOB = new DateTime(2006, 04, 17),
                Sex = "Gelding",
                Colour = "Chestnut",
                Heightcm = 152.4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse);           

            horse = new Horse
            {
                Name = "Polly",
                DOB = new DateTime(2008, 06, 14),
                Sex = "Mare",
                Colour = "Skewbald",
                Heightcm = 154.432,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            context.Add(horse);
            context.SaveChanges();           

            Owner owner = context.Owners.Where(o => o.LastName == "Smith").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Bella").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            owner = context.Owners.Where(o => o.LastName == "Green").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Star").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            owner = context.Owners.Where(o => o.LastName == "French").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Tilly").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            owner = context.Owners.Where(o => o.LastName == "Melchard").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Apollo").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            owner = context.Owners.Where(o => o.LastName == "Morton").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Oliver Twist").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            owner = context.Owners.Where(o => o.LastName == "Thomas").FirstOrDefault();
            horse = context.Horse.Where(h => h.Name == "Polly").FirstOrDefault();
            context.Add(new HorseOwner { HorseId = horse.Id, OwnerId = owner.Id });

            context.SaveChanges();

        }

        public static async Task InitializeIdentity(UserManager<User> userManager,
                                                          RoleManager<Role> roleManager,
                                                          IConfiguration config)
        {
            User managerUser = await userManager.FindByEmailAsync("manager@sunnystables.com");

            if (managerUser != null)
            {
                return;
            }
            
            roleManager.CreateAsync(new Role { Name = "Manager" }).Wait();
            roleManager.CreateAsync(new Role { Name = "Admin" }).Wait();
            roleManager.CreateAsync(new Role { Name = "Client" }).Wait(); 

            CreateUserForRole(new Staff() { Email = "manager@sunnystables.com", UserName = "Manager",
                                           FirstName = "Melanie", LastName = "Morris" },
                              new string[] { "Manager", "Admin" },
                              userManager,
                              config.GetSection("InitialSetupPassword").Value);
 
            CreateUserForRole(new Staff() { Email = "admin@sunnystables.com", UserName = "Admin",
                                           FirstName = "Alex", LastName = "Armstrong" },
                              new string[] { "Admin" },
                              userManager,
                              config.GetSection("DemoPassword").Value); 
            
            CreateUserForRole(new Owner() { Email = "molly@gmail.com", UserName = "molly@gmail.com",
                                           FirstName = "Molly", LastName = "Smith" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value);  
                       
            CreateUserForRole(new Owner() { Email = "jenny@gmail.com", UserName = "jenny@gmail.com",
                                           FirstName = "Jenny", LastName = "Green" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value);  
                      
            CreateUserForRole(new Owner() { Email = "simon@gmail.com", UserName = "simon@gmail.com",
                                           FirstName = "Simon", LastName = "French" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value);  
                     
            CreateUserForRole(new Owner() { Email = "dawn@gmail.com", UserName = "dawn@gmail.com",
                                           FirstName = "Dawn", LastName = "Melchard" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value); 

                     
            CreateUserForRole(new Owner() { Email = "sophie@gmail.com", UserName = "sophie@gmail.com",
                                           FirstName = "Sophie", LastName = "Morton" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value); 
                     
            CreateUserForRole(new Owner() { Email = "william@gmail.com", UserName = "william@gmail.com",
                                           FirstName = "William", LastName = "Thomas" },
                              new string[] { "Client" },
                              userManager,
                              config.GetSection("DemoPassword").Value); 
        }

        private static void CreateUserForRole(User user,
                           string[] roles,
                           UserManager<User> userManager,
                           string password)
        {

            var result = userManager.CreateAsync(user, password).Result;

            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not create user in seeder");
            }

            var addedUser = userManager.FindByEmailAsync(user.Email).Result;

            userManager.AddToRolesAsync(addedUser, roles).Wait();
        }
    }
}
