using Microsoft.EntityFrameworkCore;
using PdfMergerAPI.Models;

namespace PdfMergerAPI.DBContexts
{
    public class PdfMergerDBContext:DbContext
    {
        public PdfMergerDBContext(DbContextOptions<PdfMergerDBContext> opt):base(opt)
        {
           
        }
        public DbSet<FileDomain> FilesMergerTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileDomain>().HasKey(f=>f.FileId);
        }
     }
}
