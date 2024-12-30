using System.Data.Entity;
using FinalProjectTrainingFTS.Modules;
using Microsoft.EntityFrameworkCore;
using DbContext = System.Data.Entity.DbContext;

namespace FinalProjectTrainingFTS.DataBase;


public class ConnectionDataBase : DbContext
{
    public Microsoft.EntityFrameworkCore.DbSet<User> User { get; set; }
    
    public ConnectionDataBase()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer("Server=WAJEED\\MSSQLSERVER01;Database=FinalProjectTrainingFTS;ConnectRetryCount=0");

        //modelBuilder = new ModelBuilder();
       // base.OnModelCreating(modelBuilder);

        // Optional: Configure table name if it doesn't match the DbSet property name
       // modelBuilder.Entity<User>().ToTable("User");
    }
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlServer(
    //         @"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
    // }
    
}