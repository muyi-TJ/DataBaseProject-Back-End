using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Back_End.Models;
using System.Linq;

#nullable disable

namespace Back_End.Contexts
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<AdministratorStay> AdministratorStays { get; set; }
        public virtual DbSet<Bed> Beds { get; set; }
        public virtual DbSet<Collect> Collects { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<CouponType> CouponTypes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerComment> CustomerComments { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<CustomerGroupCoupon> CustomerGroupCoupons { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Favoritestay> Favoritestays { get; set; }
        public virtual DbSet<Generate> Generates { get; set; }
        public virtual DbSet<Host> Hosts { get; set; }
        public virtual DbSet<HostComment> HostComments { get; set; }
        public virtual DbSet<HostGroup> HostGroups { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Near> Nears { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Peripheral> Peripherals { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomBed> RoomBeds { get; set; }
        public virtual DbSet<RoomPhoto> RoomPhotos { get; set; }
        public virtual DbSet<Stay> Stays { get; set; }
        public virtual DbSet<StayLabel> StayLabels { get; set; }
        public virtual DbSet<StayType> StayTypes { get; set; }

        public void DetachAll() {
            //循环遍历DbContext中所有被跟踪的实体
            while (true) {
                //每次循环获取DbContext中一个被跟踪的实体
                var currentEntry = ChangeTracker.Entries().FirstOrDefault();

                //currentEntry不为null，就将其State设置为EntityState.Detached，即取消跟踪该实体
                if (currentEntry != null) {
                    //设置实体State为EntityState.Detached，取消跟踪该实体，之后dbContext.ChangeTracker.Entries().Count()的值会减1
                    currentEntry.State = EntityState.Detached;
                }
                //currentEntry为null，表示DbContext中已经没有被跟踪的实体了，则跳出循环
                else {
                    break;
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("User Id=xytql;Password=xybxl;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=81.68.122.212)(PORT=1521))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = xe))); ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("XYTQL");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .HasName("SYS_C0010709");

                entity.ToTable("ADMINISTRATOR");

                entity.Property(e => e.AdminId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("ADMIN_ID");

                entity.Property(e => e.AdminAvatar)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("ADMIN_AVATAR")
                    .HasDefaultValueSql("'img/admin_img' ");

                entity.Property(e => e.AdminCreateTime)
                    .HasColumnType("DATE")
                    .HasColumnName("ADMIN_CREATE_TIME");

                entity.Property(e => e.AdminPassword)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ADMIN_PASSWORD");

                entity.Property(e => e.AdminTel)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("ADMIN_TEL");

                entity.Property(e => e.AdminUsername)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ADMIN_USERNAME")
                    .HasDefaultValueSql("'administrator' ");
            });

            modelBuilder.Entity<AdministratorStay>(entity =>
            {
                entity.HasKey(e => new { e.AdminId, e.StayId })
                    .HasName("SYS_C0010719");

                entity.ToTable("ADMINISTRATOR_STAY");

                entity.Property(e => e.AdminId)
                    .HasPrecision(10)
                    .HasColumnName("ADMIN_ID");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.ValReplyTime)
                    .HasColumnType("DATE")
                    .HasColumnName("VAL_REPLY_TIME");

                entity.Property(e => e.ValidateReply)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("VALIDATE_REPLY")
                    .HasDefaultValueSql("'No reply' ");

                entity.Property(e => e.ValidateResult)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("VALIDATE_RESULT")
                    .HasDefaultValueSql("0 ");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdministratorStays)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("SYS_C0010720");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.AdministratorStays)
                    .HasForeignKey(d => d.StayId)
                    .HasConstraintName("SYS_C0010721");
            });

            modelBuilder.Entity<Bed>(entity =>
            {
                entity.HasKey(e => e.BedType)
                    .HasName("SYS_C0010698");

                entity.ToTable("BED");

                entity.Property(e => e.BedType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BED_TYPE");

                entity.Property(e => e.BedLength)
                    .HasColumnType("NUMBER(2,1)")
                    .HasColumnName("BED_LENGTH");

                entity.Property(e => e.PersonNum)
                    .HasPrecision(2)
                    .HasColumnName("PERSON_NUM");
            });

            modelBuilder.Entity<Collect>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.StayId })
                    .HasName("SYS_C0010744");

                entity.ToTable("COLLECT");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.CollectDate)
                    .HasColumnType("DATE")
                    .HasColumnName("COLLECT_DATE");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Collects)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("SYS_C0010745");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.Collects)
                    .HasForeignKey(d => d.StayId)
                    .HasConstraintName("SYS_C0010746");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("COUPON");

                entity.Property(e => e.CouponId)
                    .HasPrecision(10)
                    .ValueGeneratedNever()
                    .HasColumnName("COUPON_ID");

                entity.Property(e => e.CouponEnd)
                    .HasColumnType("DATE")
                    .HasColumnName("COUPON_END");

                entity.Property(e => e.CouponStart)
                    .HasColumnType("DATE")
                    .HasColumnName("COUPON_START");

                entity.Property(e => e.CouponTypeId)
                    .HasPrecision(10)
                    .HasColumnName("COUPON_TYPE_ID");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_ID");

                entity.HasOne(d => d.CouponType)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.CouponTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0010642");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0010643");
            });

            modelBuilder.Entity<CouponType>(entity =>
            {
                entity.ToTable("COUPON_TYPE");

                entity.Property(e => e.CouponTypeId)
                    .HasPrecision(10)
                    .ValueGeneratedNever()
                    .HasColumnName("COUPON_TYPE_ID");

                entity.Property(e => e.CouponAmount)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("COUPON_AMOUNT");

                entity.Property(e => e.CouponLimit)
                    .HasColumnType("NUMBER(8,2)")
                    .HasColumnName("COUPON_LIMIT");

                entity.Property(e => e.CouponName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("COUPON_NAME");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMER");

                entity.HasIndex(e => e.CustomerPhone, "SYS_C0010632")
                    .IsUnique();

                entity.HasIndex(e => e.CustomerEmail, "SYS_C0010633")
                    .IsUnique();

                entity.Property(e => e.CustomerId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.CustomerBirthday)
                    .HasColumnType("DATE")
                    .HasColumnName("CUSTOMER_BIRTHDAY");

                entity.Property(e => e.CustomerCreatetime)
                    .HasColumnType("DATE")
                    .HasColumnName("CUSTOMER_CREATETIME")
                    .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.CustomerDegree)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_DEGREE")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CustomerEmail)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_EMAIL");

                entity.Property(e => e.CustomerGender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_GENDER");

                entity.Property(e => e.CustomerLevel)
                    .HasPrecision(5)
                    .HasColumnName("CUSTOMER_LEVEL");

                entity.Property(e => e.CustomerMood)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CUSTOMER_MOOD")
                    .HasDefaultValueSql("0 ");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_NAME")
                    .HasDefaultValueSql("'CUSTOMER'");

                entity.Property(e => e.CustomerPassword)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_PASSWORD");

                entity.Property(e => e.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_PHONE");

                entity.Property(e => e.CustomerPhoto)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_PHOTO")
                    .HasDefaultValueSql("'https://guisu.oss-cn-shanghai.aliyuncs.com/img/customer_img.png'");

                entity.Property(e => e.CustomerPrephone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_PREPHONE")
                    .HasDefaultValueSql("'+86'");

                entity.Property(e => e.CustomerState)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CUSTOMER_STATE")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.CustomerLevelNavigation)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CustomerLevel)
                    .HasConstraintName("SYS_C0010634");
            });

            modelBuilder.Entity<CustomerComment>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("SYS_C0010732");

                entity.ToTable("CUSTOMER_COMMENT");

                entity.Property(e => e.OrderId)
                    .HasPrecision(10)
                    .ValueGeneratedNever()
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.CommentTime)
                    .HasColumnType("DATE")
                    .HasColumnName("COMMENT_TIME");

                entity.Property(e => e.CustomerComment1)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_COMMENT");

                entity.Property(e => e.HouseStars)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("HOUSE_STARS");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.CustomerComment)
                    .HasForeignKey<CustomerComment>(d => d.OrderId)
                    .HasConstraintName("SYS_C0010733");
            });

            modelBuilder.Entity<CustomerGroup>(entity =>
            {
                entity.HasKey(e => e.CustomerLevel)
                    .HasName("SYS_C0010624");

                entity.ToTable("CUSTOMER_GROUP");

                entity.Property(e => e.CustomerLevel)
                    .HasPrecision(5)
                    .ValueGeneratedNever()
                    .HasColumnName("CUSTOMER_LEVEL");

                entity.Property(e => e.CustomerLevelDegree)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_LEVEL_DEGREE");

                entity.Property(e => e.CustomerLevelName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_LEVEL_NAME");
            });

            modelBuilder.Entity<CustomerGroupCoupon>(entity =>
            {
                entity.HasKey(e => e.CustomerLevel)
                    .HasName("SYS_C0010646");

                entity.ToTable("CUSTOMER_GROUP_COUPON");

                entity.Property(e => e.CustomerLevel)
                    .HasPrecision(5)
                    .ValueGeneratedNever()
                    .HasColumnName("CUSTOMER_LEVEL");

                entity.Property(e => e.CouponNum)
                    .HasPrecision(10)
                    .HasColumnName("COUPON_NUM");

                entity.Property(e => e.CouponTypeId)
                    .HasPrecision(10)
                    .HasColumnName("COUPON_TYPE_ID");

                entity.HasOne(d => d.CouponType)
                    .WithMany(p => p.CustomerGroupCoupons)
                    .HasForeignKey(d => d.CouponTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SYS_C0010647");

                entity.HasOne(d => d.CustomerLevelNavigation)
                    .WithOne(p => p.CustomerGroupCoupon)
                    .HasForeignKey<CustomerGroupCoupon>(d => d.CustomerLevel)
                    .HasConstraintName("SYS_C0010648");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("FAVORITE");

                entity.Property(e => e.FavoriteId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("FAVORITE_ID");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0010765");
            });

            modelBuilder.Entity<Favoritestay>(entity =>
            {
                entity.HasKey(e => new { e.FavoriteId, e.StayId })
                    .HasName("SYS_C0010766");

                entity.ToTable("FAVORITESTAY");

                entity.Property(e => e.FavoriteId)
                    .HasPrecision(10)
                    .HasColumnName("FAVORITE_ID");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.HasOne(d => d.Favorite)
                    .WithMany(p => p.Favoritestays)
                    .HasForeignKey(d => d.FavoriteId)
                    .HasConstraintName("SYS_C0010767");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.Favoritestays)
                    .HasForeignKey(d => d.StayId)
                    .HasConstraintName("SYS_C0010768");
            });

            modelBuilder.Entity<Generate>(entity =>
            {
                entity.HasKey(e => new { e.OrdersId, e.RoomId, e.StartTime })
                    .HasName("SYS_C0010756");

                entity.ToTable("GENERATE");

                entity.Property(e => e.OrdersId)
                    .HasPrecision(10)
                    .HasColumnName("ORDERS_ID");

                entity.Property(e => e.RoomId)
                    .HasPrecision(10)
                    .HasColumnName("ROOM_ID");

                entity.Property(e => e.StartTime)
                    .HasColumnType("DATE")
                    .HasColumnName("START_TIME");

                entity.Property(e => e.EndTime)
                    .HasColumnType("DATE")
                    .HasColumnName("END_TIME");

                entity.Property(e => e.Money)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("MONEY");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.HasOne(d => d.Orders)
                    .WithMany(p => p.Generates)
                    .HasForeignKey(d => d.OrdersId)
                    .HasConstraintName("SYS_C0010757");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Generates)
                    .HasForeignKey(d => new { d.StayId, d.RoomId })
                    .HasConstraintName("SYS_C0010758");
            });

            modelBuilder.Entity<Host>(entity =>
            {
                entity.ToTable("HOSTS");

                entity.HasIndex(e => e.HostPhone, "SYS_C0010665")
                    .IsUnique();

                entity.HasIndex(e => e.HostEmail, "SYS_C0010666")
                    .IsUnique();

                entity.HasIndex(e => e.HostIdnumber, "SYS_C0010667")
                    .IsUnique();

                entity.Property(e => e.HostId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("HOST_ID");

                entity.Property(e => e.HostAvatar)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("HOST_AVATAR")
                    .HasDefaultValueSql("'https://guisu.oss-cn-shanghai.aliyuncs.com/img/host_img.png'");

                entity.Property(e => e.HostCreateTime)
                    .HasColumnType("DATE")
                    .HasColumnName("HOST_CREATE_TIME")
                    .HasDefaultValueSql("SYSDATE");

                entity.Property(e => e.HostEmail)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("HOST_EMAIL");

                entity.Property(e => e.HostGender)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("HOST_GENDER");

                entity.Property(e => e.HostIdnumber)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("HOST_IDNUMBER");

                entity.Property(e => e.HostLevel)
                    .HasPrecision(5)
                    .HasColumnName("HOST_LEVEL");

                entity.Property(e => e.HostPassword)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("HOST_PASSWORD");

                entity.Property(e => e.HostPhone)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("HOST_PHONE");

                entity.Property(e => e.HostPrephone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HOST_PREPHONE")
                    .HasDefaultValueSql("'+86'");

                entity.Property(e => e.HostRealname)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HOST_REALNAME");

                entity.Property(e => e.HostScore)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("HOST_SCORE")
                    .HasDefaultValueSql("0 ");

                entity.Property(e => e.HostState)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("HOST_STATE")
                    .HasDefaultValueSql("0 ");

                entity.Property(e => e.HostUsername)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("HOST_USERNAME")
                    .HasDefaultValueSql("'HOST'");

                entity.HasOne(d => d.HostLevelNavigation)
                    .WithMany(p => p.Hosts)
                    .HasForeignKey(d => d.HostLevel)
                    .HasConstraintName("SYS_C0010668");
            });

            modelBuilder.Entity<HostComment>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("SYS_C0010738");

                entity.ToTable("HOST_COMMENT");

                entity.Property(e => e.OrderId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.CommentTime)
                    .HasColumnType("DATE")
                    .HasColumnName("COMMENT_TIME");

                entity.Property(e => e.CustomerStars)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("CUSTOMER_STARS");

                entity.Property(e => e.HostComment1)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("HOST_COMMENT");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.HostComment)
                    .HasForeignKey<HostComment>(d => d.OrderId)
                    .HasConstraintName("SYS_C0010739");
            });

            modelBuilder.Entity<HostGroup>(entity =>
            {
                entity.HasKey(e => e.HostLevel)
                    .HasName("SYS_C0010651");

                entity.ToTable("HOST_GROUP");

                entity.Property(e => e.HostLevel)
                    .HasPrecision(5)
                    .ValueGeneratedNever()
                    .HasColumnName("HOST_LEVEL");

                entity.Property(e => e.HostLevelDegree)
                    .HasPrecision(10)
                    .HasColumnName("HOST_LEVEL_DEGREE");

                entity.Property(e => e.HostLevelName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("HOST_LEVEL_NAME");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasKey(e => e.LabelName)
                    .HasName("SYS_C0010772");

                entity.ToTable("LABELS");

                entity.Property(e => e.LabelName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LABEL_NAME");

                entity.Property(e => e.LabelType)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("LABEL_TYPE");
            });

            modelBuilder.Entity<Near>(entity =>
            {
                entity.HasKey(e => new { e.PeripheralId, e.StayId })
                    .HasName("SYS_C0010748");

                entity.ToTable("NEAR");

                entity.Property(e => e.PeripheralId)
                    .HasPrecision(10)
                    .HasColumnName("PERIPHERAL_ID");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.Distance)
                    .HasColumnType("NUMBER(3,2)")
                    .HasColumnName("DISTANCE");

                entity.HasOne(d => d.Peripheral)
                    .WithMany(p => p.Nears)
                    .HasForeignKey(d => d.PeripheralId)
                    .HasConstraintName("SYS_C0010749");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.Nears)
                    .HasForeignKey(d => d.StayId)
                    .HasConstraintName("SYS_C0010750");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.OrderId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(10)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.MemberNum)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("MEMBER_NUM")
                    .HasDefaultValueSql("1 ");

                entity.Property(e => e.OrderTime)
                    .HasColumnType("DATE")
                    .HasColumnName("ORDER_TIME")
                    .HasDefaultValueSql("sysdate");

                entity.Property(e => e.TotalCost)
                    .HasColumnType("NUMBER(15,2)")
                    .HasColumnName("TOTAL_COST")
                    .HasDefaultValueSql("0 ");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("SYS_C0010714");
            });

            modelBuilder.Entity<Peripheral>(entity =>
            {
                entity.ToTable("PERIPHERAL");

                entity.Property(e => e.PeripheralId)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .UseHiLo("SEQ")
                    .HasColumnName("PERIPHERAL_ID");

                entity.Property(e => e.DetailedAddress)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("DETAILED_ADDRESS");

                entity.Property(e => e.PeripheralClass)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PERIPHERAL_CLASS");

                entity.Property(e => e.PeripheralName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PERIPHERAL_NAME");

                entity.Property(e => e.PeripheralPopularity)
                    .HasPrecision(10)
                    .HasColumnName("PERIPHERAL_POPULARITY")
                    .HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("SYS_C0010724");

                entity.ToTable("REPORTS");

                entity.Property(e => e.OrderId)
                    .HasPrecision(10)
                    .ValueGeneratedNever()
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.AdminId)
                    .HasPrecision(10)
                    .HasColumnName("ADMIN_ID");

                entity.Property(e => e.DealTime)
                    .HasColumnType("DATE")
                    .HasColumnName("DEAL_TIME");

                entity.Property(e => e.IsDealed)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("IS_DEALED")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("REASON");

                entity.Property(e => e.Reply)
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("REPLY");

                entity.Property(e => e.ReportTime)
                    .HasColumnType("DATE")
                    .HasColumnName("REPORT_TIME");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("SYS_C0010726");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Report)
                    .HasForeignKey<Report>(d => d.OrderId)
                    .HasConstraintName("SYS_C0010725");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => new { e.StayId, e.RoomId })
                    .HasName("SYS_C0010693");

                entity.ToTable("ROOM");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.RoomId)
                    .HasPrecision(10)
                    .HasColumnName("ROOM_ID");

                entity.Property(e => e.BathroomNum)
                    .HasPrecision(2)
                    .HasColumnName("BATHROOM_NUM");

                entity.Property(e => e.Price)
                    .HasPrecision(8)
                    .HasColumnName("PRICE");

                entity.Property(e => e.RoomArea)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("ROOM_AREA");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.StayId)
                    .HasConstraintName("SYS_C0010694");
            });

            modelBuilder.Entity<RoomBed>(entity =>
            {
                entity.HasKey(e => new { e.StayId, e.RoomId, e.BedType })
                    .HasName("SYS_C0010700");

                entity.ToTable("ROOM_BED");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.RoomId)
                    .HasPrecision(10)
                    .HasColumnName("ROOM_ID");

                entity.Property(e => e.BedType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BED_TYPE");

                entity.Property(e => e.BedNum)
                    .HasPrecision(3)
                    .HasColumnName("BED_NUM");

                entity.HasOne(d => d.BedTypeNavigation)
                    .WithMany(p => p.RoomBeds)
                    .HasForeignKey(d => d.BedType)
                    .HasConstraintName("SYS_C0010701");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomBeds)
                    .HasForeignKey(d => new { d.StayId, d.RoomId })
                    .HasConstraintName("SYS_C0010702");
            });

            modelBuilder.Entity<RoomPhoto>(entity =>
            {
                entity.HasKey(e => new { e.StayId, e.RoomId, e.RPhoto })
                    .HasName("SYS_C0010761");

                entity.ToTable("ROOM_PHOTOS");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.RoomId)
                    .HasPrecision(10)
                    .HasColumnName("ROOM_ID");

                entity.Property(e => e.RPhoto)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("R_PHOTO")
                    .HasDefaultValueSql("'img/home_img'");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomPhotos)
                    .HasForeignKey(d => new { d.StayId, d.RoomId })
                    .HasConstraintName("SYS_C0010762");
            });

            modelBuilder.Entity<Stay>(entity =>
            {
                entity.ToTable("STAY");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .ValueGeneratedNever()
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.BedNum)
                    .HasPrecision(3)
                    .HasColumnName("BED_NUM");

                entity.Property(e => e.Characteristic)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("CHARACTERISTIC");

                entity.Property(e => e.CommentNum)
                    .HasPrecision(10)
                    .HasColumnName("COMMENT_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CommentScore)
                    .HasPrecision(10)
                    .HasColumnName("COMMENT_SCORE")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DaysMax)
                    .HasPrecision(4)
                    .HasColumnName("DAYS_MAX");

                entity.Property(e => e.DaysMin)
                    .HasPrecision(4)
                    .HasColumnName("DAYS_MIN");

                entity.Property(e => e.DetailedAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DETAILED_ADDRESS");

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("END_TIME");

                entity.Property(e => e.HostId)
                    .HasPrecision(10)
                    .HasColumnName("HOST_ID");

                entity.Property(e => e.Latitude)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Longitude)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.NonBarrierFacility)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("NON_BARRIER_FACILITY");

                entity.Property(e => e.PublicBathroom)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PUBLIC_BATHROOM");

                entity.Property(e => e.PublicToilet)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PUBLIC_TOILET");

                entity.Property(e => e.RoomNum)
                    .HasPrecision(3)
                    .HasColumnName("ROOM_NUM");

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("START_TIME");

                entity.Property(e => e.StayCapacity)
                    .HasPrecision(3)
                    .HasColumnName("STAY_CAPACITY");

                entity.Property(e => e.StayName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("STAY_NAME");

                entity.Property(e => e.StayStatus)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("STAY_STATUS");

                entity.Property(e => e.StayType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("STAY_TYPE");

                entity.HasOne(d => d.Host)
                    .WithMany(p => p.Stays)
                    .HasForeignKey(d => d.HostId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C0010689");

                entity.HasOne(d => d.StayTypeNavigation)
                    .WithMany(p => p.Stays)
                    .HasForeignKey(d => d.StayType)
                    .HasConstraintName("SYS_C0010690");
            });

            modelBuilder.Entity<StayLabel>(entity =>
            {
                entity.HasKey(e => new { e.StayId, e.LabelName })
                    .HasName("SYS_C0010775");

                entity.ToTable("STAY_LABELS");

                entity.Property(e => e.StayId)
                    .HasPrecision(10)
                    .HasColumnName("STAY_ID");

                entity.Property(e => e.LabelName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LABEL_NAME");

                entity.HasOne(d => d.LabelNameNavigation)
                    .WithMany(p => p.StayLabels)
                    .HasForeignKey(d => d.LabelName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SYS_C0010777");

                entity.HasOne(d => d.Stay)
                    .WithMany(p => p.StayLabels)
                    .HasForeignKey(d => d.StayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SYS_C0010776");
            });

            modelBuilder.Entity<StayType>(entity =>
            {
                entity.HasKey(e => e.StayType1)
                    .HasName("SYS_C0010670");

                entity.ToTable("STAY_TYPE");

                entity.Property(e => e.StayType1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("STAY_TYPE");

                entity.Property(e => e.Characteristic)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CHARACTERISTIC");
            });

            modelBuilder.HasSequence("SEQ").IncrementsBy(1);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
