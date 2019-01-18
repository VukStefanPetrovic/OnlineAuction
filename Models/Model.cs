namespace OnlineAuctions.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Entities")
        {
        }

        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<SystemParameter> SystemParameters { get; set; }
        public virtual DbSet<TokenOrder> TokenOrders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .Property(e => e.currentPrice)
                .HasPrecision(10, 3);

            modelBuilder.Entity<Auction>()
                .Property(e => e.startingPrice)
                .HasPrecision(10, 3);

            modelBuilder.Entity<Auction>()
                .HasMany(e => e.Bids)
                .WithRequired(e => e.Auction)
                .HasForeignKey(e => e.idAuction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SystemParameter>()
                .Property(e => e.Currency)
                .HasPrecision(10, 3);

            modelBuilder.Entity<SystemParameter>()
                .Property(e => e.TokensValue)
                .HasPrecision(10, 3);

            modelBuilder.Entity<TokenOrder>()
                .Property(e => e.Price)
                .HasPrecision(10, 3);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TokenOrders)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Bids)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
