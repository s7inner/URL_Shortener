using URL_Shortener.Areas.Identity.Data;
using URL_Shortener.Entities;
using URL_Shortener.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace URL_Shortener.Data;
public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
    public DbSet<ShortenedUrl> ShortUrls { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ShortenedUrl>(builder => 
        {
            builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
            builder.HasIndex(s => s.Code).IsUnique();
        });
    }
}
