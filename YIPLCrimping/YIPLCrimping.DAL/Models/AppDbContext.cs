using Microsoft.EntityFrameworkCore;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Models;

public partial class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    //public AppDbContext(DbContextOptions<Microsoft.EntityFrameworkCore.DbContext> options)
    //    : base(options)
    //{
    //}

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    public virtual DbSet<CrimpingStandard> CrimpingStandards { get; set; }

    public virtual DbSet<CrimpingStandardCustomerTerminal> CrimpingStandardCustomerTerminals { get; set; }

    public virtual DbSet<CrimpingStandardDy> CrimpingStandardDies { get; set; }

    public virtual DbSet<CrimpingStandardKindDetail> CrimpingStandardKindDetails { get; set; }

    public virtual DbSet<MCrimpingShape> MCrimpingShapes { get; set; }

    public virtual DbSet<MCrimpingStandardWireInfo> MCrimpingStandardWireInfos { get; set; }

    public virtual DbSet<MCustomer> MCustomer { get; set; }
    public virtual DbSet<MDepartment> MDepartments { get; set; }

    public virtual DbSet<MPlant> MPlants { get; set; }

    public virtual DbSet<MRole> MRoles { get; set; }

    public virtual DbSet<MSupplier> MSuppliers { get; set; }

    public virtual DbSet<MTemplateFile> MTemplateFiles { get; set; }
    public virtual DbSet<MMachine> MMachines { get; set; }

    public virtual DbSet<UserAccount> UserAccount { get; set; }
    public virtual DbSet<MWireType> MWireTypes { get; set; }
    public virtual DbSet<MWireSize> MWireSizes { get; set; }
    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<T1Terminal> T1Terminals { get; set; }
    public virtual DbSet<T1C1Accessory> T1C1Accessories { get; set; }
    public virtual DbSet<T1C2ApplicatorDetail> T1C2ApplicatorDetails { get; set; }
    public virtual DbSet<T1C3StripingDetail> T1C3StripingDetails { get; set; }
    public virtual DbSet<T1C4CombinationDetail> T1C4CombinationDetails { get; set; }
    public virtual DbSet<T1C5CrimpingStandardDetail> T1C5CrimpingStandardDetails { get; set; }
    public virtual DbSet<T1C6CrimpingOtherParameter> T1C6CrimpingOtherParameters { get; set; }
    public virtual DbSet<T1C7CrimpingDiesDetail> T1C7CrimpingDiesDetails { get; set; }
    public virtual DbSet<T1C8ApprovalDetail> T1C8ApprovalDetails { get; set; }
    public virtual DbSet<T1C9TerminalSupplierCrimpingSpec> T1C9TerminalSupplierCrimpingSpecs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CrimpingStandardRequestVM>().HasNoKey().ToView(null);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=192.168.1.251;Database=YIPL_Crimping;uid=BBD_SQLSEVER;Password=Benz10(!$#){~$$#}[%]2025;TrustServerCertificate=True;");

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    //modelBuilder.Entity<CrimpingStandard>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__Crimping__3214EC0730FE27BA");

    //    //    entity.HasOne(d => d.InsulationCrimpShape).WithMany(p => p.CrimpingStandards).HasConstraintName("FK_CrimpingStandards_CrimpingShapes");

    //    //    entity.HasOne(d => d.PlantNavigation).WithMany(p => p.CrimpingStandards)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_CrimpingStandards_M_Plant");
    //    //});

    //    //modelBuilder.Entity<CrimpingStandardCustomerTerminal>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07784B153B");

    //    //    entity.HasOne(d => d.CrimpingStandard).WithMany(p => p.CrimpingStandardCustomerTerminals)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_CustomerTerminal_CrimpingStandards");

    //    //    entity.HasOne(d => d.Customer).WithMany(p => p.CrimpingStandardCustomerTerminals)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_CustomerTerminal_M_Customer");
    //    //});

    //    //modelBuilder.Entity<CrimpingStandardDy>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__Crimping__3214EC0755B4494D");

    //    //    entity.HasOne(d => d.CrimpingStandard).WithMany(p => p.CrimpingStandardDies)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_CrimpingStandardDies_Standard");
    //    //});

    //    //modelBuilder.Entity<CrimpingStandardKindDetail>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__Crimping__3214EC07537A5AEE");

    //    //    entity.HasOne(d => d.CrimpingStandard).WithMany(p => p.CrimpingStandardKindDetails)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_KindDetails_CrimpingStandards");
    //    //});

    //    //modelBuilder.Entity<MCrimpingShape>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Crimpi__3214EC07CE560244");
    //    //});

    //    //modelBuilder.Entity<MCrimpingStandardWireInfo>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Crimpi__3214EC07894A950D");

    //    //    entity.HasOne(d => d.CrimpingStandard).WithMany(p => p.MCrimpingStandardWireInfos)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_WireInfo_CrimpingStandards");
    //    //});

    //    //modelBuilder.Entity<MCustomer>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Custom__3214EC07AD475F5A");
    //    //});

    //    //modelBuilder.Entity<MDepartment>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Depart__3214EC07FE8AE91F");
    //    //});

    //    //modelBuilder.Entity<MPlant>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Plant__3214EC078C44B43F");
    //    //});

    //    //modelBuilder.Entity<MRole>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Role__3214EC079B35FCE1");
    //    //});

    //    //modelBuilder.Entity<MSupplier>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__M_Suppli__3214EC0702CE1849");
    //    //});

    //    //modelBuilder.Entity<UserAccount>(entity =>
    //    //{
    //    //    entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC072BD8A3F9");

    //    //    entity.HasOne(d => d.Department).WithMany(p => p.UserAccount)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_UserAccount_M_Department");

    //    //    entity.HasOne(d => d.MPlant).WithMany(p => p.UserAccount)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_UserAccount_M_Plant");

    //    //    entity.HasOne(d => d.MRoleCode).WithMany(p => p.UserAccount)
    //    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //    //        .HasConstraintName("FK_UserAccount_M_Role");
    //    //});

    //    OnModelCreatingPartial(modelBuilder);
    //}

    //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public partial class CommonDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    //public AppDbContext(DbContextOptions<Microsoft.EntityFrameworkCore.DbContext> options)
    //    : base(options)
    //{
    //}

    public CommonDbContext(DbContextOptions<CommonDbContext> options)
    : base(options)
    {
    }

    public virtual DbSet<UserDetails> UserDetails { get; set; }
}