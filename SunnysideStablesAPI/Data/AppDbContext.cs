using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
                                                  UserRole, IdentityUserLogin<int>,
                                                  IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Horse> Horse { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<HorseOwner>(ho =>
            {
                ho.ToTable("HorseOwner");

                ho.HasKey(ho => new { ho.HorseId, ho.OwnerId });

                ho.HasOne(ho => ho.Owner)
                    .WithMany(ho => ho.HorseOwner)
                    .HasForeignKey(ho => ho.OwnerId)
                    .IsRequired();

                ho.HasOne(ho => ho.Horse)
                    .WithMany(ho => ho.HorseOwner)
                    .HasForeignKey(ho => ho.HorseId)
                    .IsRequired();
            });

            builder.Entity<Horse>().ToTable("Horse"); 

        }
    }
}
