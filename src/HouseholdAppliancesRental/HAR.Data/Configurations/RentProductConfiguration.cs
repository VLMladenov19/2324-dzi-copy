using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAR.Data.Configurations
{
    internal class RentProductConfiguration : IEntityTypeConfiguration<RentProduct>
    {
        public void Configure(EntityTypeBuilder<RentProduct> builder)
        {
            builder
                .HasKey(rp => new { rp.RentId, rp.ProductId });
        }
    }
}
