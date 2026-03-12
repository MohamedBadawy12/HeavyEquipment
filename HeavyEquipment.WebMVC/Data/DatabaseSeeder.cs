using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.ValueObjects;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.WebMVC.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                await db.Database.MigrateAsync();

                await SeedRolesAsync(roleManager, logger);
                await SeedUsersAsync(userManager, logger);
                await SeedEquipmentsAsync(db, userManager, logger);
                await SeedLogisticsProvidersAsync(db, logger);
                await SeedRentalOrdersAsync(db, userManager, logger);

                logger.LogInformation("✅ Database seeded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error seeding database.");
                throw;
            }
        }

        // ================================================================
        // 1. ROLES
        // ================================================================
        private static async Task SeedRolesAsync(
            RoleManager<AppRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                ("Admin",    "مدير النظام"),
                ("Owner",    "مالك معدات"),
                ("Customer", "مستأجر معدات"),
            };

            foreach (var (roleName, description) in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;

                await roleManager.CreateAsync(new AppRole(roleName, description));

                logger.LogInformation("✅ Role: {Role}", roleName);
            }
        }

        // ================================================================
        // 2. USERS
        // ================================================================
        private static async Task SeedUsersAsync(
            UserManager<AppUser> userManager, ILogger logger)
        {
            var users = new[]
            {
                ( FullName: "مدير النظام",         Email: "admin@heavyhub.eg",     Phone: "01000000000", NationalId: "00000000000000", Password: "Admin@123456",    Role: UserType.Admin,    RoleName: "Admin",    TrustScore: 100m, IsVerified: true  ),
                ( FullName: "محمد أحمد السيد",     Email: "owner1@heavyhub.eg",    Phone: "01011111111", NationalId: "29001011234567", Password: "Owner@123456",    Role: UserType.Owner,    RoleName: "Owner",    TrustScore: 92m,  IsVerified: true  ),
                ( FullName: "خالد محمود إبراهيم",  Email: "owner2@heavyhub.eg",    Phone: "01022222222", NationalId: "28505021234567", Password: "Owner@123456",    Role: UserType.Owner,    RoleName: "Owner",    TrustScore: 85m,  IsVerified: true  ),
                ( FullName: "أحمد علي حسن",        Email: "owner3@heavyhub.eg",    Phone: "01033333333", NationalId: "27803031234567", Password: "Owner@123456",    Role: UserType.Owner,    RoleName: "Owner",    TrustScore: 78m,  IsVerified: true  ),
                ( FullName: "سامي عبدالله النجار", Email: "customer1@heavyhub.eg", Phone: "01044444444", NationalId: "29204041234567", Password: "Customer@123456", Role: UserType.Customer, RoleName: "Customer", TrustScore: 88m,  IsVerified: true  ),
                ( FullName: "هاني فوزي مصطفى",    Email: "customer2@heavyhub.eg", Phone: "01055555555", NationalId: "29605051234567", Password: "Customer@123456", Role: UserType.Customer, RoleName: "Customer", TrustScore: 74m,  IsVerified: false ),
            };

            foreach (var u in users)
            {
                if (await userManager.FindByEmailAsync(u.Email) is not null) continue;

                var user = new AppUser(u.FullName, u.Email, u.Phone, u.NationalId, u.Role)
                {
                    UserName = u.Email,
                };
                var delta = u.TrustScore - 100m;
                if (delta != 0) user.AdjustTrustScore(delta);

                if (u.IsVerified) user.Verify();

                var result = await userManager.CreateAsync(user, u.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, u.RoleName);
                    logger.LogInformation("✅ User: {Email}", u.Email);
                }
                else
                {
                    foreach (var e in result.Errors)
                        logger.LogWarning("⚠️ {Email} — {Err}", u.Email, e.Description);
                }
            }
        }

        // ================================================================
        // 3. EQUIPMENTS
        // ================================================================
        private static async Task SeedEquipmentsAsync(
            AppDbContext db, UserManager<AppUser> userManager, ILogger logger)
        {
            if (await db.Equipments.AnyAsync()) return;

            var o1 = await userManager.FindByEmailAsync("owner1@heavyhub.eg");
            var o2 = await userManager.FindByEmailAsync("owner2@heavyhub.eg");
            var o3 = await userManager.FindByEmailAsync("owner3@heavyhub.eg");
            if (o1 is null || o2 is null || o3 is null) return;

            var equipments = new List<Equipment>
            {
                // Owner 1 — أوناش
                new("رافعة برجية LIEBHERR 380",
                    "رافعة برجية بحمولة قصوى 16 طن، ارتفاع 60 متر، مثالية للأبراج السكنية والتجارية.",
                    "LIEBHERR 380 EC-H", 2021, EquipmentCategory.Crane,
                    850m, 15000m, 500, o1.Id,
                    new Location("القاهرة", "المنطقة الصناعية، مدينة نصر", 30.0626, 31.3297)),

                new("رافعة متحركة GROVE GMK 5130",
                    "رافعة متحركة على عجلات بحمولة 130 طن، مناسبة لمشاريع البنية التحتية والجسور.",
                    "GROVE GMK 5130-2", 2020, EquipmentCategory.Crane,
                    1200m, 25000m, 400, o1.Id,
                    new Location("القاهرة", "العاشر من رمضان، المنطقة الصناعية", 30.2972, 31.7400)),

                // Owner 2 — حفارات ومولدات
                new("حفارة كاتربيلر CAT 320",
                    "حفارة هيدروليكية بقوة 162 كيلوواط، عمق حفر 6.7 متر. مثالية لأعمال الحفر والأساسات.",
                    "CAT 320 GC", 2022, EquipmentCategory.Excavator,
                    620m, 10000m, 600, o2.Id,
                    new Location("الإسكندرية", "سيدي بشر، المنطقة الصناعية", 31.2565, 29.9695)),

                new("مولد كهربائي Perkins 500KVA",
                    "مولد كهربائي صامت بقدرة 500 كيلوفولت أمبير، مناسب لمواقع البناء الكبيرة.",
                    "Perkins 2806A-E18TAG2", 2021, EquipmentCategory.Generator,
                    450m, 8000m, 250, o2.Id,
                    new Location("الجيزة", "مدينة 6 أكتوبر، الحي الصناعي", 29.9337, 30.9208)),

                new("مولد كهربائي Cummins 250KVA",
                    "مولد متنقل بقدرة 250 كيلوفولت أمبير، سهل النقل، للمشاريع المتوسطة.",
                    "Cummins C275D5", 2020, EquipmentCategory.Generator,
                    280m, 5000m, 300, o2.Id,
                    new Location("القاهرة", "شبرا الخيمة، المنطقة الصناعية", 30.1264, 31.2426)),

                // Owner 3 — بلدوزر ولودر وحفارة
                new("بلدوزر كاتربيلر D6T",
                    "بلدوزر بقوة 175 حصان لتسوية الأراضي وأعمال الردم في مشاريع الطرق.",
                    "CAT D6T LGP", 2019, EquipmentCategory.Bulldozer,
                    520m, 9000m, 400, o3.Id,
                    new Location("الشرقية", "العاشر من رمضان", 30.2972, 31.7400)),

                new("لودر كوماتسو WA380",
                    "لودر بحمولة 3.5 طن، لنقل المواد وتحميل الشاحنات في مواقع البناء.",
                    "Komatsu WA380-8", 2021, EquipmentCategory.Loader,
                    380m, 7000m, 500, o3.Id,
                    new Location("الإسماعيلية", "المنطقة الصناعية الجديدة", 30.5965, 32.2715)),

                new("حفارة كوماتسو PC210",
                    "حفارة متوسطة الحجم بوزن تشغيل 20.8 طن، لأعمال الحفر والصرف الصحي.",
                    "Komatsu PC210LC-11", 2020, EquipmentCategory.Excavator,
                    550m, 9500m, 550, o3.Id,
                    new Location("السويس", "المنطقة الصناعية، العين السخنة", 29.9668, 32.5498)),
            };

            // تنويع الحالات
            equipments[2].UpdateStatus(EquipmentStatus.Rented);
            equipments[5].UpdateStatus(EquipmentStatus.UnderMaintenance);

            await db.Equipments.AddRangeAsync(equipments);
            foreach (var eq in equipments)
                eq.ClearDomainEvents();

            await db.SaveChangesAsync();
            logger.LogInformation("✅ {Count} equipments seeded.", equipments.Count);
        }

        // ================================================================
        // 4. LOGISTICS PROVIDERS
        // ================================================================
        private static async Task SeedLogisticsProvidersAsync(
            AppDbContext db, ILogger logger)
        {
            if (await db.LogisticsProviders.AnyAsync()) return;

            await db.LogisticsProviders.AddRangeAsync(
                new LogisticsProvider("شركة النيل للنقل الثقيل", "01099991111", 15.0m),
                new LogisticsProvider("مؤسسة الدلتا للشحن", "01099992222", 12.5m),
                new LogisticsProvider("شركة الصقر للمعدات الثقيلة", "01099993333", 18.0m)
            );
            await db.SaveChangesAsync();
            logger.LogInformation("✅ Logistics providers seeded.");
        }

        // ================================================================
        // 5. RENTAL ORDERS
        // ================================================================
        private static async Task SeedRentalOrdersAsync(
            AppDbContext db, UserManager<AppUser> userManager, ILogger logger)
        {
            if (await db.RentalOrders.AnyAsync()) return;

            var c1 = await userManager.FindByEmailAsync("customer1@heavyhub.eg");
            var c2 = await userManager.FindByEmailAsync("customer2@heavyhub.eg");
            if (c1 is null || c2 is null) return;

            var eq = await db.Equipments.ToListAsync();
            if (eq.Count < 8) return;

            var now = DateTime.UtcNow;

            var orders = new List<RentalOrder>
            {
                MakeOrder(eq[0], c1.Id, now.AddDays(-30), now.AddDays(-20), OrderStatus.Completed),
                MakeOrder(eq[7], c2.Id, now.AddDays(-60), now.AddDays(-50), OrderStatus.Completed),

                MakeOrder(eq[2], c1.Id, now.AddDays(-3),  now.AddDays(7),   OrderStatus.Active),

                MakeOrder(eq[1], c2.Id, now.AddDays(2),   now.AddDays(10),  OrderStatus.Confirmed),

                MakeOrder(eq[3], c2.Id, now.AddDays(5),   now.AddDays(12),  OrderStatus.Pending),
                MakeOrder(eq[4], c1.Id, now.AddDays(8),   now.AddDays(15),  OrderStatus.Pending),

                MakeOrder(eq[6], c1.Id, now.AddDays(-15), now.AddDays(-8),  OrderStatus.Cancelled,
                    cancellationReason: "تغيير في خطة المشروع"),
            };

            await db.RentalOrders.AddRangeAsync(orders);
            foreach (var order in orders)
                order.ClearDomainEvents();
            await db.SaveChangesAsync();

            await SeedInspectionsAsync(db, orders, logger);

            logger.LogInformation("✅ {Count} rental orders seeded.", orders.Count);
        }

        private static RentalOrder MakeOrder(
        Equipment eq,
        Guid customerId,
        DateTime start,
        DateTime end,
        OrderStatus targetStatus,
        string? cancellationReason = null)
        {
            var order = (RentalOrder)Activator.CreateInstance(
                typeof(RentalOrder), nonPublic: true)!;

            var entry = new
            {
                CustomerId = customerId,
                EquipmentId = eq.Id,
                RentalStart = start,
                RentalEnd = end,
                HourlyRate = eq.HourlyRate,
                TotalPrice = eq.HourlyRate * 8 * Math.Max(1, (int)(end - start).TotalDays),
                Status = OrderStatus.Pending
            };

            typeof(RentalOrder).GetProperty("CustomerId")!.SetValue(order, entry.CustomerId);
            typeof(RentalOrder).GetProperty("EquipmentId")!.SetValue(order, entry.EquipmentId);
            typeof(RentalOrder).GetProperty("RentalStart")!.SetValue(order, entry.RentalStart);
            typeof(RentalOrder).GetProperty("RentalEnd")!.SetValue(order, entry.RentalEnd);
            typeof(RentalOrder).GetProperty("HourlyRate")!.SetValue(order, entry.HourlyRate);
            typeof(RentalOrder).GetProperty("TotalPrice")!.SetValue(order, entry.TotalPrice);
            typeof(RentalOrder).GetProperty("Status")!.SetValue(order, entry.Status);

            // نحرك الـ Status من غير Domain Validation
            if (targetStatus >= OrderStatus.Confirmed) order.Confirm();
            if (targetStatus >= OrderStatus.Active) order.MarkAsActive();
            if (targetStatus == OrderStatus.Completed) order.Complete();
            if (targetStatus == OrderStatus.Cancelled) order.Cancel(cancellationReason ?? "إلغاء");

            return order;
        }

        // ================================================================
        // 6. INSPECTION REPORTS
        // ================================================================
        private static async Task SeedInspectionsAsync(
            AppDbContext db, List<RentalOrder> orders, ILogger logger)
        {
            if (await db.InspectionReports.AnyAsync()) return;

            var inspections = new List<InspectionReport>
            {
                new(orders[2].Id, orders[2].CustomerId,
                    InspectionType.PreRental, InspectionStatus.Passed, 500,
                    "تم فحص المعدة قبل التسليم. المعدة جاهزة للعمل."),

                new(orders[3].Id, orders[3].CustomerId,
                    InspectionType.PreRental, InspectionStatus.Passed, 0,
                    "فحص أولي — في انتظار تأكيد موعد التسليم."),

                new(orders[1].Id, orders[1].CustomerId,
                    InspectionType.PostRental, InspectionStatus.PassedWithIssues, 880,
                    "تم استلام المعدة بحالة جيدة مع خدش بسيط في الهيكل تم توثيقه."),
            };

            await db.InspectionReports.AddRangeAsync(inspections);
            await db.SaveChangesAsync();
            logger.LogInformation("✅ {Count} inspection reports seeded.", inspections.Count);
        }
    }
}
