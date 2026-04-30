using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Persistence.Pharma_BridgeDbContext;
using PharmaBridge.Shared.EnumHelper.PaymentEnums;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
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
        public static async Task SeedOrderTestDataAsync(PharmaDbContext context)
        {
            if (await context.Set<Domain.Models.UserAccess.Order>().AnyAsync()) return;

            var patientUser = await context.Users
                .Include(u => u.PatientProfile)
                    .ThenInclude(p => p!.PatientAddresses)
                .FirstOrDefaultAsync(u => u.Email == "nour.patient@softbridge.com");

            var pharmacy = await context.Set<Pharmacy>().FirstOrDefaultAsync(p => p.PharmacyName == "Nour Pharmacy");
            var pharmacy2 = await context.Set<Pharmacy>().FirstOrDefaultAsync(p => p.PharmacyName == "Hossam Pharmacy");

            if (patientUser?.PatientProfile == null || pharmacy == null)
            {
                Console.WriteLine("❌ Seed failed: Run SeedDummyUsersAsync first.");
                return;
            }

            var patientProfile = patientUser.PatientProfile;
            var deliveryAddress = patientProfile.PatientAddresses.FirstOrDefault();

            if (deliveryAddress == null)
            {
                Console.WriteLine("❌ Seed failed: No address found for patient.");
                return;
            }

            var seedData = new List<(
                string MedicineName,
                string PatientNotes,
                decimal Subtotal,
                decimal Discount,
                decimal Delivery,
                decimal Total,
                int DeliveryTime,
                OrderStatus OrderStatus,
                List<(string Name, decimal Unit, short Qty, bool IsAlt)> Items,
                Pharmacy Pharmacy
            )>
{
    (
        "Panadol 500mg x2, Amoxicillin 250mg x1",
        "Please deliver before 5 PM",
        150.00m, 10.00m, 20.00m, 160.00m, 30,
        OrderStatus.Pending,
        new() {
            ("Panadol 500mg",     25.00m, 2, false),
            ("Amoxicillin 250mg", 100.00m, 1, false)
        },
        pharmacy
    ),
    (
        "Voltaren Gel x1, Vitamin C 1000mg x3",
        "Ring the bell twice",
        200.00m, 0.00m, 15.00m, 215.00m, 45,
        OrderStatus.Accepted,
        new() {
            ("Voltaren Gel",     120.00m, 1, false),
            ("Vitamin C 1000mg",  30.00m, 3, false)
        },
        pharmacy
    ),
    (
        "Insulin Pen x1, Glucometer Strips x2",
        "Fragile items — handle with care",
        400.00m, 20.00m, 25.00m, 405.00m, 60,
        OrderStatus.Preparing,
        new() {
            ("Insulin Pen",       300.00m, 1, false),
            ("Glucometer Strips",  50.00m, 2, false)
        },
        pharmacy2 ?? pharmacy
    ),
    (
        "Omeprazole 20mg x1, Gaviscon Syrup x1",
        "Leave at the door if no answer",
        85.00m, 5.00m, 10.00m, 90.00m, 20,
        OrderStatus.Delivered,
        new() {
            ("Omeprazole 20mg", 35.00m, 1, false),
            ("Gaviscon Syrup",  55.00m, 1, false)
        },
        pharmacy2 ?? pharmacy
    ),
    (
        "Augmentin 625mg x1, Brufen 400mg x2",
        "Call before arriving",
        175.00m, 15.00m, 20.00m, 180.00m, 35,
        OrderStatus.Cancelled,
        new() {
            ("Augmentin 625mg", 125.00m, 1, false),
            ("Brufen 400mg",     25.00m, 2, false)
        },
        pharmacy
    ),
};

            int bidIndex = 1;
            foreach (var data in seedData)
            {
                // 1 — PrescriptionRequest
                var prescription = new PrescriptionRequestEntity
                {
                    MedicineName = data.MedicineName,
                    PatientNotes = data.PatientNotes,
                    Status = PrescriptionStatus.Pending,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    PatientProfileId = patientProfile.Id,
                    DeliveryAddressId = deliveryAddress.Id
                };
                await context.Set<PrescriptionRequestEntity>().AddAsync(prescription);
                await context.SaveChangesAsync();
                // 2 — Bid
                var bid = new Bid
                {
                    Subtotal = data.Subtotal,
                    DiscountAmount = data.Discount,
                    DeliveryFee = data.Delivery,
                    TotalPrice = data.Total,
                    Status = BidStatus.Accepted,
                    Notes = $"Bid {bidIndex} — all items available",
                    DeliveryTimeInMinutes = data.DeliveryTime,
                    PharmacyId = data.Pharmacy.Id,
                    PrescriptionRequestId = prescription.Id,
                    BidItems = data.Items.Select(i => new BidItem
                    {
                        ItemName = i.Name,
                        UnitPrice = i.Unit,
                        Quantity = i.Qty,
                        LineTotal = i.Unit * i.Qty,
                        IsAlternative = i.IsAlt
                    }).ToList()
                };
                await context.Set<Bid>().AddAsync(bid);
                await context.SaveChangesAsync();

                // 3 — Order
                var order = new Domain.Models.UserAccess.Order
                {
                    Amount = data.Total,
                    PaymentMethod = PaymentMethodType.CashOnDelivery,
                    PaymentStatus = data.OrderStatus == OrderStatus.Delivered
                                                ? PaymentStatus.Succeeded
                                                : PaymentStatus.Pending,
                    OrderStatus = data.OrderStatus,
                    BidId = bid.Id,
                    PharmacyId = data.Pharmacy.Id,
                    PrescriptionRequestId = prescription.Id,
                    PatientProfileId = patientProfile.Id,
                    PatientAddressId = deliveryAddress.Id,
                    CancelReason = data.OrderStatus == OrderStatus.Cancelled
                                                ? "Patient changed their mind"
                                                : null,
                    CancelledAt = data.OrderStatus == OrderStatus.Cancelled
                                                ? DateTime.UtcNow.AddDays(-1)
                                                : null,
                    DeliveredAt = data.OrderStatus == OrderStatus.Delivered
                                                ? DateTime.UtcNow.AddHours(-2)
                                                : null,
                };
                await context.Set<Domain.Models.UserAccess.Order>().AddAsync(order);
                await context.SaveChangesAsync();

                Console.WriteLine($"   ✅ Order {bidIndex} seeded — Status: {data.OrderStatus} | Bid ID: {bid.Id} | Pharmacy ID: {data.Pharmacy.Id}");
                bidIndex++;
            }
            Console.WriteLine("==============================================");
            Console.WriteLine("✅ OrderTestSeeder completed — 5 orders created");
            Console.WriteLine($"   Pharmacy ID (Nour)   : {pharmacy.Id}");
            Console.WriteLine($"   Pharmacy ID (Hossam) : {pharmacy2?.Id ?? pharmacy.Id}");
            Console.WriteLine("==============================================");
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
