using api.artpixxel.data.Models;
using api.artpixxel.data.Models.Base;
using api.artpixxel.data.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace api.artpixxel.Data
{
    public class ArtPixxelContext : IdentityDbContext<User, UserRole, string>
    {
        private readonly ICurrentUserService _currentUserService;
        public ArtPixxelContext(DbContextOptions<ArtPixxelContext> options,
               ICurrentUserService currentUserService)
            : base(options) => _currentUserService = currentUserService;




        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInformation();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }




        public DbSet<Flag> Flags { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerCategory> CustomerCategories { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<MixnMatch> MixnMatches { get; set; }
        public DbSet<MixnMatchCategory> MixnMatchCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set;  }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<OrderItemImage> OrderItemImages { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<WallArt> WallArts { get; set; }
        public DbSet<WallArtCategory> WallArtCategories { get; set; }
        public DbSet<WallArtImage> WallArtImages { get; set; }
        public DbSet<WallArtSize> WallArtSizes { get; set; }
        public DbSet<EmailList> EmailLists { get; set; }
        public DbSet<PhoneList> PhoneLists { get; set; }
        public DbSet<SMTP> SMTPs { get; set; }
        public DbSet<AddressBook> AddressBooks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<UserSentMessage> UserSentMessages { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<LeadTime> LeadTimes { get; set; }
        public DbSet<DefNotification> DefNotifications { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<MixedTemplateSize> MixedTemplateSizes { get; set; }
        public DbSet<KidsTemplateSize> KidsTemplateSizes { get; set; } 
        public DbSet<KidsGalleryImage> KidsGalleryImages { get; set; }   
        public DbSet<HomeGallery> HomeGalleries { get; set; }

        public DbSet<RegularTemplateSize> RegularTemplateSizes { get; set; }
        public DbSet<ChristmasTemplateSize> ChristmasTemplateSizes { get; set; }  
        public DbSet<FestiveDesign> FestiveDesigns { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Frame> Frames { get; set; }

        public DbSet<frame_categories> FrameCategories { get; set; }
        public DbSet<line_colors_Master> line_colors_Master { get; set; }

        public DbSet<Template_Configs> TemplateConfigs { get; set; }

        public DbSet<Template_Frames_Mapping> TemplateFramesMapping { get; set; }

        public DbSet<GalleryImages> GalleryImages { get; set; }

        public DbSet<OrderCartItems> OrderCartItems { get; set; }

        public DbSet<OrderCartItemImages> OrderCartItemImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //OrderCartItems
            builder.Entity<OrderCartItems>()
              .HasQueryFilter(w => !w.IsDeleted);

            //OrderCartItemImages

            builder.Entity<OrderCartItemImages>()
             .HasQueryFilter(w => !w.IsDeleted);

            //GalleryImages 

            builder.Entity<GalleryImages>()
                .HasQueryFilter(w => !w.IsDeleted);

            //Template_Frames_Mapping
            builder.Entity<Template_Frames_Mapping>()
                .HasOne(tfm => tfm.TemplateConfig)
                .WithMany()
                .HasForeignKey(tfm => tfm.TemplateConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Template_Frames_Mapping>()
             .HasQueryFilter(w => !w.IsDeleted);

            //Template_Configs
            builder.Entity<Template_Configs>()
               .Property(l => l.Price)
               .HasColumnType("decimal(18,2)");
            builder.Entity<Template_Configs>()
            .HasQueryFilter(w => !w.IsDeleted);

            // frame_categories

            builder.Entity<frame_categories>()
                    .Property(fc => fc.CategoryType)
                    .HasConversion<string>();

            builder.Entity<frame_categories>()
                .HasQueryFilter(w => !w.IsDeleted);

            //line_colors_Master

            builder.Entity<line_colors_Master>()
                .HasQueryFilter(w => !w.IsDeleted);

            // HomeGallery

            builder.Entity<HomeGallery>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");


            //Festive Design

            builder.Entity<FestiveDesign>()
                 .HasQueryFilter(w => !w.IsDeleted)
                 .Property(x => x.Id).HasDefaultValueSql("NEWID()");



            //LeadTime

            builder.Entity<LeadTime>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");
            //City

            builder.Entity<City>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            // Contact
            builder.Entity<Contact>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<AddressBook>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<City>()
                     .Property(l => l.DeliveryFee)
                     .HasColumnType("decimal(28,18)");


            //KidsGalleryImages

            builder.Entity<KidsGalleryImage>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            //Sizes

            builder.Entity<Size>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<Size>()
                     .Property(l => l.Amount)
                     .HasColumnType("decimal(28,18)");

            builder.Entity<Size>()
                 .HasMany(w => w.OrderItems)
                 .WithOne(sz => sz.Size)
                 .HasForeignKey(fk => fk.SizeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 ;

            //Email List 
            builder.Entity<EmailList>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            //MixTemplate Sizes
            builder.Entity<MixedTemplateSize>()
              .HasQueryFilter(w => !w.IsDeleted)
              .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<MixedTemplateSize>()
               .Property(l => l.Amount)
               .HasColumnType("decimal(28,18)");


            //Kids Template Sizes
            builder.Entity<KidsTemplateSize>()
              .HasQueryFilter(w => !w.IsDeleted)
              .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<KidsTemplateSize>()
               .Property(l => l.Amount)
               .HasColumnType("decimal(28,18)");



            //Regular Template Sizes
            builder.Entity<RegularTemplateSize>()
              .HasQueryFilter(w => !w.IsDeleted)
              .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<RegularTemplateSize>()
               .Property(l => l.Amount)
               .HasColumnType("decimal(28,18)");


            //Christmas Template Sizes
            builder.Entity<ChristmasTemplateSize>()
              .HasQueryFilter(w => !w.IsDeleted)
              .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<ChristmasTemplateSize>()
               .Property(l => l.Amount)
               .HasColumnType("decimal(28,18)");



            //Phone List 
            builder.Entity<PhoneList>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            //SMTP 
            builder.Entity<SMTP>()
                  .HasQueryFilter(w => !w.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            // Country

            builder.Entity<Country>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<Country>()
                    .Property(l => l.DeliveryFee)
                    .HasColumnType("decimal(28,18)");

            builder.Entity<Country>()
                   .HasMany(st => st.States)
                   .WithOne(c => c.Country)
                   .HasForeignKey(fk => fk.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);


            //Customer 

            builder.Entity<Customer>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<Customer>()
                 .HasOne(ty => ty.Category)
                 .WithMany(cs => cs.Customers)
                 .HasForeignKey(fk => fk.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                   .HasMany(ord => ord.Orders)
                   .WithOne(cs => cs.Customer)
                   .HasForeignKey(fk => fk.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   ;

            builder.Entity<Customer>()
                 .HasMany(ord => ord.AddressBooks)
                 .WithOne(cs => cs.Customer)
                 .HasForeignKey(fk => fk.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict)
                 ;


            builder.Entity<Customer>()
                    .Property(l => l.TotalOrder)
                    .HasColumnType("decimal(28,18)");

            builder.Entity<Customer>()
                   .Property(l => l.AverageOrder)
                   .HasColumnType("decimal(28,18)");

            builder.Entity<Customer>()
                    .Property(l => l.LastOrder)
                    .HasColumnType("decimal(28,18)");

            // Customer Type

            builder.Entity<CustomerCategory>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

          
            // Flag

            builder.Entity<Flag>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

           
            // Carousel

            builder.Entity<Carousel>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            //MixnMatch

            builder.Entity<MixnMatch>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<MixnMatch>()
                 .Property(l => l.Price)
                 .HasColumnType("decimal(28,18)");

            builder.Entity<MixnMatch>()
                  .HasOne(ct => ct.Category)
                  .WithMany(mx => mx.MixnMatches)
                  .HasForeignKey(fk => fk.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Mix n Match Category

            builder.Entity<MixnMatchCategory>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");


            //DefNotification

            builder.Entity<DefNotification>()
                  .HasQueryFilter(n => !n.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            
            //Notification

            builder.Entity<Notification>()
                   .HasQueryFilter(n => !n.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");


            //Message  

            builder.Entity<Message>()
                   .HasQueryFilter(n => !n.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<OrderStatusHistory>()
                .HasQueryFilter(c => !c.IsDeleted)
                .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                ;


            builder.Entity<Product>()
                    .HasQueryFilter(c => !c.IsDeleted)
                    .Property(x => x.Id).HasDefaultValueSql("NEWID()");


            builder.Entity<Template>()
                    .HasQueryFilter(c => !c.IsDeleted)
                    .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<Template>()
                    .HasOne(tp => tp.Product)
                    .WithMany(ord => ord.Templates)
                    .HasForeignKey(fk => fk.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    ;

            // Frame
            builder.Entity<Frame>()
                    .HasQueryFilter(c => !c.IsDeleted)
                    .Property(x => x.Id).HasDefaultValueSql("NEWID()");

                    builder.Entity<Frame>()
            .Property(x => x.DisplayOrder)
            .HasDefaultValue(0);


            //Order 

            builder.Entity<Order>()
                  .HasQueryFilter(c => !c.IsDeleted)
                  .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                  ;

            builder.Entity<Order>()
                     .Property(l => l.SubTotal)
                     .HasColumnType("decimal(28,18)");

            builder.Entity<Order>()
                    .Property(l => l.GrandTotal)
                    .HasColumnType("decimal(28,18)");

            builder.Entity<Order>()
                  .Property(l => l.DeliveryFee)
                  .HasColumnType("decimal(28,18)");

            builder.Entity<Order>()
                .Property(l => l.VAT)
                .HasColumnType("decimal(28,18)");


            builder.Entity<Order>()
                    .HasOne(st => st.Status)
                    .WithMany(ord => ord.Orders)
                    .HasForeignKey(fk => fk.StatusId)
                    .OnDelete(DeleteBehavior.Restrict)
                    ;


            builder.Entity<OrderStatusHistory>()
                    .HasOne(oh => oh.Order)
                    .WithMany(hh => hh.Histories)
                    .HasForeignKey(fk => fk.OrderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    ;


            //Order Item

            builder.Entity<OrderItem>()
                 .HasQueryFilter(c => !c.IsDeleted)
                 .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                 ;

            builder.Entity<OrderItem>()
                   .Property(l => l.Amount)
                   .HasColumnType("decimal(28,18)");

            builder.Entity<OrderItem>()
                 .Property(l => l.TotalAmount)
                 .HasColumnType("decimal(28,18)");

            builder.Entity<OrderItem>()
                    .HasOne(ord => ord.Order)
                    .WithMany(ords => ords.Items)
                    .HasForeignKey(fk => fk.OrderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    ;

           


            //Order Status

            builder.Entity<OrderStatus>()
                 .HasQueryFilter(c => !c.IsDeleted)
                 .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                 ;



            builder.Entity<OrderItemImage>()
                 .HasQueryFilter(c => !c.IsDeleted)
                 .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                 ;

            builder.Entity<OrderItemImage>()
                   .HasOne(ord => ord.OrderItem)
                   .WithMany(ords => ords.Images)
                   .HasForeignKey(fk => fk.OrderItemId)
                   .OnDelete(DeleteBehavior.Restrict)
                   ;



            // Meta

            builder.Entity<Meta>()
               .HasQueryFilter(c => !c.IsDeleted)
               .Property(x => x.Id).HasDefaultValueSql("NEWID()")
               ;

            


            // State

            builder.Entity<State>()
                   .HasQueryFilter(w => !w.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<State>()
                    .Property(l => l.DeliveryFee)
                    .HasColumnType("decimal(28,18)");

            builder.Entity<State>()
                   .HasMany(cts => cts.Cities)
                   .WithOne(st => st.State)
                   .HasForeignKey(fk => fk.StateId)
                   .OnDelete(DeleteBehavior.Restrict)
                   ;



            // User Notification

            builder.Entity<UserNotification>()
                   .HasKey(un => new { un.UserId, un.NotificationId });


            builder.Entity<UserNotification>()
                   .HasOne(pt => pt.User)
                   .WithMany("UserNotifications")
                   ;

            builder.Entity<UserNotification>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .HasOne(pt => pt.Notification)
                   .WithMany("UserNotifications")
                   .OnDelete(DeleteBehavior.Restrict)
                   ;



            //user Message
            builder.Entity<UserMessage>()
                   .HasKey(us => new { us.UserId, us.MessageId });


            builder.Entity<UserMessage>()
                   .HasOne(pt => pt.User)
                   .WithMany("UserMessages")
                   ;


            builder.Entity<UserMessage>()
                  .HasQueryFilter(c => !c.IsDeleted)
                  .HasOne(pt => pt.Message)
                  .WithMany("UserMessages")
                  .OnDelete(DeleteBehavior.ClientCascade)
                  ;


            builder.Entity<UserSentMessage>()
                  .HasKey(us => new { us.UserId, us.MessageId });


            builder.Entity<UserSentMessage>()
                   .HasOne(pt => pt.User)
                   .WithMany("UserSentMessages")
                   ;


            builder.Entity<UserSentMessage>()
                  .HasQueryFilter(c => !c.IsDeleted)
                  .HasOne(pt => pt.Message)
                  .WithMany("UserSentMessages")
                  .OnDelete(DeleteBehavior.ClientCascade)
                  ;
            ;


            // WallArt


            builder.Entity<WallArt>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                   ;

            builder.Entity<WallArt>()
                  .Property(l => l.Amount)
                  .HasColumnType("decimal(28,18)");

            builder.Entity<WallArt>()
                 .HasMany(im => im.Images)
                 .WithOne(img => img.WallArt)
                 .HasForeignKey(fk => fk.WallArtId)
                 .OnDelete(DeleteBehavior.Restrict)
                 ;



            // WallArt Category

            builder.Entity<WallArtCategory>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                   ;


            builder.Entity<WallArtCategory>()
                .HasMany(w => w.WallArts)
                .WithOne(wc => wc.Category)
                .HasForeignKey(fk => fk.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                ;


            // WallArt Image

            builder.Entity<WallArtImage>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                   ;


            // WallArt Size

            builder.Entity<WallArtSize>()
                   .HasQueryFilter(c => !c.IsDeleted)
                   .Property(x => x.Id).HasDefaultValueSql("NEWID()")
                    ;

            builder.Entity<WallArtSize>()
                    .Property(l => l.Amount)
                    .HasColumnType("decimal(28,18)");


            builder.Entity<WallArtSize>()
                   .HasMany(w => w.WallArts)
                   .WithOne(sz => sz.WallArtSize)
                   .HasForeignKey(fk => fk.WallArtSizeId)
                   .OnDelete(DeleteBehavior.Restrict)
                   ;

            builder.Entity<WallArtSize>()
                  .HasMany(w => w.OrderItems)
                  .WithOne(sz => sz.WallArtSize)
                  .HasForeignKey(fk => fk.WallArtSizeId)
                  .OnDelete(DeleteBehavior.Restrict)
                  ;



            base.OnModelCreating(builder);


        }






        private void ApplyAuditInformation()
       =>
           this
               .ChangeTracker.Entries()
               .ToList()
               .ForEach(entry =>
               {
                   var UserName = _currentUserService.GetUsername();
                   if (entry.Entity is IAuditedDeletableEntity deletableEntity)
                   {
                       if (entry.State == EntityState.Deleted)
                       {
                           deletableEntity.DeletedOn = DateTime.UtcNow;
                           deletableEntity.DeletedBy = UserName;
                           deletableEntity.IsDeleted = true;
                           entry.State = EntityState.Modified;
                           return;
                       }
                   }

                   if (entry.Entity is IEntity entity)
                   {
                       if (entry.State == EntityState.Added)
                       {
                           entity.CreatedOn = DateTime.UtcNow;
                           entity.CreatedBy = UserName;
                       }
                       else if (entry.State == EntityState.Modified)
                       {
                           entity.ModifiedOn = DateTime.UtcNow;
                           entity.ModifiedBy = UserName;
                       };
                   }
               });

    }
}
