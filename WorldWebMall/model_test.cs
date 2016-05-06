namespace WorldWebMall
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class model_test : DbContext
    {
        public model_test()
            : base("name=model_test")
        {
        }

        public virtual DbSet<prime_view> prime_view { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
