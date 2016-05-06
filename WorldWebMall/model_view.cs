namespace WorldWebMall
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class model_view : DbContext
    {
        public model_view()
            : base("name=model_view")
        {
        }

        public virtual DbSet<prime_view> prime_view { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
