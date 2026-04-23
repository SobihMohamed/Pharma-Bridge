using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;
namespace PharmaBridge.Persistence.Seeds
{
    public static class SeederAsync
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(UserRole)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "admin@softbridge.com";

            // ensure existence of admin
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    FullName = "System Admin",
                    Role = UserRole.Admin,
                    EmailConfirmed = true
                };

                // create admin
                var result = await userManager.CreateAsync(newAdmin, "Admin@123456");

                // if created 
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, UserRole.Admin.ToString());
                }
            }
        }

        public static async Task SeedDummyUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var teamMembers = new List<string> { "nour", "hossam", "adel", "maryam", "essam", "sobih" };

            int uniqueCounter = 1;

            foreach (var member in teamMembers)
            {
                // 1️⃣ Seed Patient for each member
                string patientEmail = $"{member}.patient@softbridge.com";
                if (await userManager.FindByEmailAsync(patientEmail) == null)
                {
                    var patientUser = new ApplicationUser
                    {
                        UserName = patientEmail.ToUpper(),
                        Email = patientEmail,
                        FullName = char.ToUpper(member[0]) + member.Substring(1) + " Patient",
                        Role = UserRole.Patient,
                        EmailConfirmed = true,
                        PatientProfile = new PatientProfile
                        {
                            Id = Guid.NewGuid().ToString(),
                            PatientAddresses = new List<PatientAddress>
                    {
                        new PatientAddress
                        {
                            AddressLine = $"{member} Home Address", 
                            City = "Cairo",
                            Latitude = 30.0383m,
                            Longitude = 31.2114m,
                            IsDefault = true
                        }
                    }
                        }
                    };

                    var patientResult = await userManager.CreateAsync(patientUser, "Password@123");
                    if (patientResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(patientUser, UserRole.Patient.ToString());
                    }
                }

                // 2️⃣ Seed Pharmacy Owner (WITH A PHARMACY) for each member
                string pharmaEmail = $"{member}.pharma@softbridge.com";
                if (await userManager.FindByEmailAsync(pharmaEmail) == null)
                {
                    string CapitalizedName = char.ToUpper(member[0]) + member.Substring(1);

                    var pharmaUser = new ApplicationUser
                    {
                        UserName = pharmaEmail.ToUpper(),
                        Email = pharmaEmail,
                        FullName = $"{CapitalizedName} Pharma Owner",
                        Role = UserRole.PharmacyOwner,
                        EmailConfirmed = true,

                        PharmaOwnerProfile = new PharmaOwner
                        {
                            Id = Guid.NewGuid().ToString(),

                            NationalId = $"2900101123450{uniqueCounter}",
                            Status = PharmaOwnerStatus.Approved,

                            Pharmacies = new List<Pharmacy>
                    {
                        new Pharmacy
                        {
                            PharmacyName = $"{CapitalizedName} Pharmacy",
                            Area = "Cairo",
                            TextAddress = "Main Street, Cairo",
                            Latitude = 30.0500m,
                            Longitude = 31.2333m,
                            OpenTime = new TimeOnly(8, 0),
                            CloseTime = new TimeOnly(23, 59),
                            Is24Hours = false,
                            Status = PharmacyStatus.Active, 

                            LicenseNumber = $"LIC-1000{uniqueCounter}",
                            ContactPhone = $"0100000000{uniqueCounter}", 
                            AverageRating = 0.0m
                        }
                    }
                        }
                    };

                    var pharmaResult = await userManager.CreateAsync(pharmaUser, "Password@123");
                    if (pharmaResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(pharmaUser, UserRole.PharmacyOwner.ToString());
                    }
                }

                uniqueCounter++;
            }
        }
    }
}
