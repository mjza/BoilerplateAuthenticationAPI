using System;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities.Auth;
using BC = BCrypt.Net.BCrypt;


namespace WebApi.Extensions.Auth
{
    public static class ModelBuilderExtension
    {

        public static void Seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Title>().HasData(
                new Title { Id = "mr", Name = "Mr." },
                new Title { Id = "mrs", Name = "Mrs." },
                new Title { Id = "ms", Name = "Ms." },
                new Title { Id = "dr", Name = "Dr." },
                new Title { Id = "pr", Name = "Prof." }
            );

            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = "m", Name = "Male" },
                new Gender { Id = "f", Name = "Female" },
                new Gender { Id = "d", Name = "Diversed" }
           );

            modelBuilder.Entity<Country>().HasData(
                new Country { Id = "de", Name = "Germany"},
                new Country { Id = "us", Name = "USA" }
           );

            modelBuilder.Entity<Profession>().HasData(
                new Profession { Id = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000001"), Name = "Others" },
                new Profession { Id = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000002"), Name = "CEO" },
                new Profession { Id = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000003"), Name = "Employee" }
           );

            modelBuilder.Entity<Nationality>().HasData(
                new Nationality { Id = "de", Name = "German" },
                new Nationality { Id = "us", Name = "American" }
           );

            modelBuilder.Entity<City>().HasData(
                new City { Id = Guid.Parse("52a130d2-502f-4cf1-a376-63edeb000001"), Name = "Cologne", CountryId = "de" },
                new City { Id = Guid.Parse("52a130d2-502f-4cf1-a376-63edeb000002"), Name = "Bonn", CountryId = "de" },
                new City { Id = Guid.Parse("52a130d2-502f-4cf1-a376-63edeb000003"), Name = "Berlin", CountryId = "de" },
                new City { Id = Guid.Parse("52a130d2-502f-4cf1-a376-63edeb000004"), Name = "New Yourk", CountryId = "us" }
           );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    FirstName = "Super",
                    LastName = "1",
                    Email = "super1@gmail.com",
                    PasswordHash = BC.HashPassword("Qwert56789"),
                    AcceptTerms = true,
                    TitleId = "mr",
                    GenderId = "m",
                    Birthday = Convert.ToDateTime("1985-05-15"),
                    Settings = "{}",
                    Street = "Bonnstr.",
                    Number = "137a",
                    PostCode = "50825",
                    NationalityId = "de",
                    ProfessionId = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000003"),
                    Role = Role.Super,
                    VerificationToken = null,
                    VerifiedAt = Convert.ToDateTime("2020-01-01T09:10:30"),
                    ResetToken = null,
                    ResetTokenExpiresAt = null,
                    PasswordResetedAt = null,
                    CreatedAt = Convert.ToDateTime("2019-12-30T09:10:30")
                },
                new Account
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    FirstName = "Admin",
                    LastName = "1",
                    Email = "admin1@gmail.com",
                    PasswordHash = BC.HashPassword("Qwert56789"),
                    AcceptTerms = true,
                    TitleId = "mr",
                    GenderId = "m",
                    Birthday = Convert.ToDateTime("1985-05-15"),
                    Settings = "{}",
                    Street = "Bonnstr.",
                    Number = "138a",
                    PostCode = "53698",
                    NationalityId = "de",
                    ProfessionId = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000001"),
                    Role = Role.Admin,
                    VerificationToken = null,
                    VerifiedAt = Convert.ToDateTime("2020-04-01T09:10:30"),
                    ResetToken = null,
                    ResetTokenExpiresAt = null,
                    PasswordResetedAt = null,
                    CreatedAt = Convert.ToDateTime("2020-01-05T09:10:30")
                },               
                new Account
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                    FirstName = "User",
                    LastName = "1",
                    Email = "user1@gmail.com",
                    PasswordHash = BC.HashPassword("Qwert56789"),
                    AcceptTerms = false,
                    TitleId = "mr",
                    GenderId = "m",
                    Birthday = Convert.ToDateTime("2000-01-15"),
                    Settings = "{}",
                    Street = "Alexstr.",
                    Number = "137c",
                    PostCode = null,
                    NationalityId = "de",
                    ProfessionId = Guid.Parse("54a130d2-502f-4cf1-a376-63edeb000002"),
                    Role = Role.User,
                    VerificationToken = null,
                    VerifiedAt = null,
                    ResetToken = null,
                    ResetTokenExpiresAt = null,
                    PasswordResetedAt = null,
                    CreatedAt = Convert.ToDateTime("2020-01-07T09:10:30")
                }
           );
        }
    }
}
