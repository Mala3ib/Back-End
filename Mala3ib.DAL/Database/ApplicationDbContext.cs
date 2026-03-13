
namespace Mala3ib.DAL.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<FieldOwner> FieldOwners { get; set; }
        public DbSet<EmailVerficationOtp> EmailVerficationOtps { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldImage> FieldImages { get; set; }
        public DbSet<FieldSlot> FieldSlots { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<FieldReview> FieldReviews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            base.OnModelCreating(modelBuilder);
            
            // Application User with Admin & FieldOwner & Player
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithOne(u => u.Player)
                .HasForeignKey<Player>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FieldOwner>()
                .HasOne(f => f.User)
                .WithOne(u => u.FieldOwner)
                .HasForeignKey<FieldOwner>(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Fields & FieldOwner
            modelBuilder.Entity<Field>()
                .HasOne(f => f.FieldOwner)
                .WithMany(fo => fo.Fields)
                .HasForeignKey(f => f.FieldOwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // FieldImages
            modelBuilder.Entity<FieldImage>()
                .HasOne(fi => fi.Field)
                .WithMany(f => f.Images)
                .HasForeignKey(fi => fi.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            // FieldSlots
            modelBuilder.Entity<FieldSlot>()
                .HasOne(fs => fs.Field)
                .WithMany(f => f.Slots)
                .HasForeignKey(fs => fs.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            // Matches
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Player)
                .WithMany(p => p.CreatedMatches)
                .HasForeignKey(m => m.CreatorPlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Field)
                .WithMany(f => f.Matches)
                .HasForeignKey(m => m.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.FieldSlot)
                .WithMany()
                .HasForeignKey(m => m.FieldSlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Match)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Player)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payments
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invitations
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Match)
                .WithMany(m => m.Invitations)
                .HasForeignKey(i => i.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Sender)
                .WithMany(p => p.SentInvitations)
                .HasForeignKey(i => i.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Receiver)
                .WithMany(p => p.ReceivedInvitations)
                .HasForeignKey(i => i.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            // FieldReviews
            modelBuilder.Entity<FieldReview>()
                .HasOne(fr => fr.Field)
                .WithMany(f => f.Reviews)
                .HasForeignKey(fr => fr.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FieldReview>()
                .HasOne(fr => fr.Player)
                .WithMany(p => p.FieldReviews)
                .HasForeignKey(fr => fr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}