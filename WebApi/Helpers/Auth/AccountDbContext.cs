using Microsoft.EntityFrameworkCore;
using WebApi.Extensions.Auth;
using WebApi.Entities.Auth;

namespace WebApi.Helpers.Auth
{
    public partial class AccountDbContext : DbContext
    {

        public AccountDbContext()
        {
        }

        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        //
        public virtual DbSet<Nationality> Nationalities { get; set; }
        public virtual DbSet<Profession> Professions { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts", "AuthDBO");

                entity.HasIndex(e => e.TitleId)
                   .HasDatabaseName("accounts_title_id_foreign");

                entity.HasIndex(e => e.GenderId)
                    .HasDatabaseName("accounts_gender_id_foreign");

                entity.HasIndex(e => e.NationalityId)
                    .HasDatabaseName("accounts_nationality_id_foreign");

                entity.HasIndex(e => e.ProfessionId)
                    .HasDatabaseName("accounts_profession_id_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("UniqueIdentifier")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AcceptTerms)
                    .HasColumnName("accept_terms")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasMaxLength(255);

                entity.Property(e => e.TitleId)
                   .HasColumnName("title_id")
                   .HasMaxLength(5);

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("date");

                entity.Property(e => e.Settings)
                    .HasColumnName("settings")
                    .HasDefaultValueSql("(N'{}')")
                    .IsRequired();

                entity.Property(e => e.GenderId)
                    .HasColumnName("gender_id")
                    .HasMaxLength(1)
                    .HasDefaultValueSql("(N'f')")
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("password_hash")
                    .HasMaxLength(1000);

                entity.Property(e => e.PasswordResetedAt)
                    .HasColumnName("password_reseted_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.ResetToken)
                    .HasColumnName("reset_token")
                    .HasMaxLength(255);

                entity.Property(e => e.ResetTokenExpiresAt)
                    .HasColumnName("reset_token_expires_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.VerificationToken)
                    .HasColumnName("verification_token")
                    .HasMaxLength(1000);

                entity.Property(e => e.VerifiedAt)
                    .HasColumnName("verified_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.Street)
                   .HasColumnName("street")
                   .HasMaxLength(255);

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasMaxLength(255);

                entity.Property(e => e.PostCode)
                   .HasColumnName("postcode")
                   .HasMaxLength(255);

                entity.Property(e => e.NationalityId)
                    .HasColumnName("nationality_id")
                    .HasMaxLength(3);

                entity.Property(e => e.ProfessionId)
                    .HasColumnName("profession_id")
                    .HasColumnType("UniqueIdentifier");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.TitleId)
                    .HasConstraintName("accounts$accounts_title_id_foreign");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("accounts$accounts_gender_id_foreign");

                entity.HasOne(d => d.Nationality)
                   .WithMany(p => p.Accounts)
                   .HasForeignKey(d => d.NationalityId)
                   .HasConstraintName("accounts$accounts_nationality_id_foreign");

                entity.HasOne(d => d.Profession)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ProfessionId)
                    .HasConstraintName("accounts$accounts_profession_id_foreign");
            });

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.ToTable("nationalities", "AuthDBO");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(3);

                entity.Property(e => e.Name)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Profession>(entity =>
            {
                entity.ToTable("professions", "AuthDBO");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("UniqueIdentifier")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("cities", "AuthDBO");

                entity.HasIndex(e => e.CountryId)
                    .HasDatabaseName("cities_country_id_foreign");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("UniqueIdentifier")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CountryId)
                    .IsRequired()
                    .HasColumnName("country_id")
                    .HasMaxLength(3);

                entity.Property(e => e.Name)
                    .HasColumnName("name");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("cities$cities_country_id_foreign");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("countries", "AuthDBO");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(3);

                entity.Property(e => e.Name)
                    .HasColumnName("name");

                entity.Property(e => e.TellCode)
                    .HasColumnName("tell_code")
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("genders", "AuthDBO");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.ToTable("titles", "AuthDBO");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(5);

                entity.Property(e => e.Name)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens", "AuthDBO");

                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_RefreshToken_Accounts_AccountId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("UniqueIdentifier")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("UniqueIdentifier");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.CreatedByIp)
                    .HasColumnName("created_by_ip")
                    .HasMaxLength(255);

                entity.Property(e => e.ExpiresAt)
                    .HasColumnName("expires_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.ReplacedByToken)
                    .HasColumnName("Replaced_by_token")
                    .HasMaxLength(1000);

                entity.Property(e => e.RevokedAt)
                    .HasColumnName("revoked_at")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.RevokedByIp)
                    .HasColumnName("revoked_by_ip")
                    .HasMaxLength(255);

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("refresh_tokens$FK_RefreshToken_Accounts_AccountId");
            });

            modelBuilder.Seed();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}