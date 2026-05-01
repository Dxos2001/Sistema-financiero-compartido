using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Infrastructure
{
    public class DBConnection : DbContext
    {
        public DBConnection(DbContextOptions<DBConnection> options) : base(options)
        {
        }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets("48db1399-5775-4b06-9f1c-e0b215c9901c")
                .Build();
            var connectionString = config.GetConnectionString("MySqlConnection");
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
        }
    }
        public DbSet<Users> Users { get; set; }
        public DbSet<Persons> Persons { get; set; }
        public DbSet<FinancialGroups> FinancialGroups { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<ConsensusRequests> ConsensusRequests { get; set; }
        public DbSet<AuditLogs> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<Users>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure composite key for GroupMembers
            modelBuilder.Entity<GroupMembers>().HasKey(gm => new { gm.GroupId, gm.UserId });

            // Configure relationships
            modelBuilder.Entity<FinancialGroups>()
                .HasOne(fg => fg.Creator)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(fg => fg.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupMembers>()
                .HasOne(gm => gm.Group)
                .WithMany(fg => fg.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMembers>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Group)
                .WithMany(fg => fg.Transactions)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsensusRequests>()
                .HasOne(cr => cr.Transaction)
                .WithMany(t => t.ConsensusRequests)
                .HasForeignKey(cr => cr.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsensusRequests>()
                .HasOne(cr => cr.RequestedByUser)
                .WithMany(u => u.RequestedConsensus)
                .HasForeignKey(cr => cr.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsensusRequests>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(cr => cr.ApproverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AuditLogs>()
                .HasOne(al => al.Transaction)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(al => al.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}