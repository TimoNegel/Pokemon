using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUserModel>(options)
    {
        public DbSet<PokemonModel> PokemonsCache { get; set; }
        public DbSet<MoveModel> Moves { get; set; }
        public DbSet<UserPokemonModel> UserPokemons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<PokemonModel>()
                .HasMany(p => p.Moves)
                .WithMany(m => m.PokemonWithThisMove)
            .UsingEntity<Dictionary<string, object>>(
                "PokemonMove", // Name der Join-Tabelle
                j => j.HasOne<MoveModel>().WithMany().HasForeignKey("MoveId"),
                j => j.HasOne<PokemonModel>().WithMany().HasForeignKey("PokemonId")
            );


            modelBuilder.Entity<UserPokemonModel>()
      .HasKey(up => new { up.UserId, up.PokemonId });

            modelBuilder.Entity<UserPokemonModel>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPokemons)
            .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserPokemonModel>()
            .HasOne(up => up.Pokemon)
            .WithMany(p => p.UserPokemons)
            .HasForeignKey(up => up.PokemonId);

        }
    }
}
