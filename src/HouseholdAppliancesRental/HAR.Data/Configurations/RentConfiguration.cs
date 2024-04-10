using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAR.Data.Configurations
{
    internal class RentConfiguration : IEntityTypeConfiguration<Rent>
    {
        public void Configure(EntityTypeBuilder<Rent> builder)
        {
            builder
                .HasOne(r => r.Address)
                .WithMany(a => a.Rents)
                .HasForeignKey(r => r.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Rents)
                .HasForeignKey(r => r.UserId);
        }
    }
}
