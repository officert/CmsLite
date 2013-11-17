using System.Data;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;

namespace CmsLite.Data
{
    public class CmsDbContext : DbContext, IDbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public DbSet<SectionNode> SectionNodes { get; set; }
        public DbSet<PageNode> PageNodes { get; set; }

        public DbSet<PageProperty> Properties { get; set; }

        public DbSet<SectionTemplate> SectionTemplates { get; set; }
        public DbSet<PageTemplate> PageTemplates { get; set; }
        public DbSet<PagePropertyTemplate> PropertyTemplates { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public IDbSet<T> GetDbSet<T>() where T : class
        {
            return Set<T>();
        }

        public override int SaveChanges()
        {
            // Need to manually delete all PageNodes that have no ParentSectionNode, otherwise they'll be orphaned.
            var orphanedPageNodes = ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified) &&
                                                                                    e.Entity is PageNode &&
                                                                                    e.Reference("ParentSectionNode").CurrentValue == null);

            foreach (var dbEntityEntry in orphanedPageNodes)
            {
                PageNodes.Remove(dbEntityEntry.Entity as PageNode);
            }

            var orphanedProperties = ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified) &&
                                                                                    e.Entity is PageProperty &&
                                                                                    e.Reference("ParentPageNode").CurrentValue == null);

            foreach (var dbEntityEntry in orphanedProperties)
            {
                Properties.Remove(dbEntityEntry.Entity as PageProperty);
            }
            
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //when a SectionTemplate is deleted, delete any PageTemplates that are parented by that SectionTemplate
            //modelBuilder.Entity<SectionTemplate>()
            //    .HasMany(x => x.PageTemplates)
            //    .WithRequired(x => x.ParentSectionTemplate)
            //    .WillCascadeOnDelete(true);

            //when a SectionTemplate is deleted, delete any SectionNodes that use that SectionTemplate
            modelBuilder.Entity<SectionTemplate>()
                .HasMany(x => x.SectionNodes)
                .WithRequired(x => x.SectionTemplate)
                .WillCascadeOnDelete(true);

            //when a SectionNode is deleted, do not delete the page nodes - TODO: must manually clean these up
            modelBuilder.Entity<SectionNode>()
                .HasMany(x => x.PageNodes)
                .WithOptional(x => x.ParentSectionNode)
                .WillCascadeOnDelete(false);

            //when a PageNode is deleted, do not delete the child page nodes - TODO: must manually clean these up
            modelBuilder.Entity<PageNode>()
                .HasMany(x => x.PageNodes)
                .WithOptional(x => x.ParentPageNode)
                .WillCascadeOnDelete(false);

            //when a PageTemplate is deleted, do not delete the PageNode that uses it - TODO need to manual clean this up - can't use cascade delete because of SQL constraint multiple cascade paths
            modelBuilder.Entity<PageTemplate>()
                .HasMany(x => x.PageNodes)
                .WithRequired(x => x.PageTemplate)
                .WillCascadeOnDelete(false);

            //when a PageNode is deleted, delete any Properties that use that PageNode
            modelBuilder.Entity<PageNode>()
                .HasMany(x => x.Properties)
                .WithRequired(x => x.ParentPageNode)
                .WillCascadeOnDelete(true);

            //when a PageTemplate is deleted, delete any PropertyTemplates that use that PageTemplate
            modelBuilder.Entity<PageTemplate>()
                .HasMany(x => x.PropertyTemplates)
                .WithRequired(x => x.ParentPageTemplate)
                .WillCascadeOnDelete(true);

            //when a PageTemplate is deleted, delete any PropertyTemplates that use that PageTemplate
            modelBuilder.Entity<PageTemplate>()
                .HasMany(x => x.PropertyTemplates)
                .WithRequired(x => x.ParentPageTemplate)
                .WillCascadeOnDelete(true);

            //when a PropertyTemplate is deleted, delete any Properties that use that PropertyTemplate
            modelBuilder.Entity<PagePropertyTemplate>()
                .HasMany(x => x.Properties)
                .WithRequired(x => x.PropertyTemplate)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PageNode>()
                .HasMany(x => x.PageNodes)
                .WithOptional(x => x.ParentPageNode);
        }
    }
}
