using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System;
using CARDS.Business.Models;
using System.Reflection.Emit;
using CARDS.Business.Authorization.Seeds;
using CARDS.Business.Views;

namespace CARDS.Business
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
			
			builder.Entity<SystemCodeDetailsView>().HasNoKey().ToTable("SystemCodeDetailsView", t => t.ExcludeFromMigrations());
			builder.Entity<UsersView>().HasNoKey().ToTable("UsersView", t => t.ExcludeFromMigrations());
			builder.Entity<UserClaimsView>().HasNoKey().ToTable("UserClaimsView", t => t.ExcludeFromMigrations());
			builder.Entity<CardsView>().HasNoKey().ToTable("CardsView", t => t.ExcludeFromMigrations());
			builder.Entity<CardStatusHistoriesView>().HasNoKey().ToTable("CardStatusHistoriesView", t => t.ExcludeFromMigrations());

			base.OnModelCreating(builder);
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<ApplicationRole> ApplicationRoles { get; set; }
		public DbSet<SystemCode> SystemCodes { get; set; }
		public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<CardStatusHistory> CardStatusHistories { get; set; }

		#region VIES
		public DbSet<SystemCodeDetailsView> SystemCodeDetailsView { get; set; }
		public DbSet<UsersView> UsersView { get; set; }
		public DbSet<UserClaimsView> UserClaimsView { get; set; }
		public DbSet<CardsView> CardsView { get; set; }
		public DbSet<CardStatusHistoriesView> CardStatusHistoriesView { get; set; }
		#endregion
	}
}
