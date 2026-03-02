namespace Mala3ib.DAL.Database.EntitiesConfigurations
{
    public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsMany(u => u.RefreshTokens)
            .ToTable("RefreshTokens")   
            .WithOwner()
            .HasForeignKey("UserId");
        }
    }
}
