namespace Mala3ib.DAL.Database.EntitiesConfigurations
{
    public class FieldOwnerConfigurations : IEntityTypeConfiguration<FieldOwner>
    {
        public void Configure(EntityTypeBuilder<FieldOwner> builder)
        {
            builder.Property(x => x.IsApproved)
                .HasConversion<string>();
        }
    }
}
