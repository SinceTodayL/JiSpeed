using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APP_ROLES",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    RoleType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_ROLES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_USERS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CATEGORY",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    CategoryName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ParentId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: true),
                    SortOrder = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORY", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_CATEGORY_CATEGORY_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CATEGORY",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RECONCILIATION",
                columns: table => new
                {
                    ReconId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ReconType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DiffAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    AffectedOrders = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsResolved = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECONCILIATION", x => x.ReconId);
                });

            migrationBuilder.CreateTable(
                name: "SCHEDULE",
                columns: table => new
                {
                    ScheduleId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    WorkDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCHEDULE", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "APP_ROLE_CLAIMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_ROLE_CLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APP_ROLE_CLAIMS_APP_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalTable: "APP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADMIN",
                columns: table => new
                {
                    AdminId = table.Column<string>(type: "VARCHAR2(450)", maxLength: 450, nullable: false),
                    PermissionLevel = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADMIN", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_ADMIN_APP_USERS_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_CLAIMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_CLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APP_USER_CLAIMS_APP_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_LOGINS",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_LOGINS", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_APP_USER_LOGINS_APP_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_ROLES",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_ROLES", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_APP_USER_ROLES_APP_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalTable: "APP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_APP_USER_ROLES_APP_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER_TOKENS",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER_TOKENS", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_APP_USER_TOKENS_APP_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MERCHANT",
                columns: table => new
                {
                    MerchantId = table.Column<string>(type: "VARCHAR2(450)", maxLength: 450, nullable: false),
                    MerchantName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ContactInfo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MERCHANT", x => x.MerchantId);
                    table.ForeignKey(
                        name: "FK_MERCHANT_APP_USERS_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RIDER",
                columns: table => new
                {
                    RiderId = table.Column<string>(type: "VARCHAR2(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    VehicleNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RIDER", x => x.RiderId);
                    table.ForeignKey(
                        name: "FK_RIDER_APP_USERS_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ANNOUNCEMENT",
                columns: table => new
                {
                    AnnounceId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    AdminId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true),
                    TargetRole = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    StartAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNOUNCEMENT", x => x.AnnounceId);
                    table.ForeignKey(
                        name: "FK_ANNOUNCEMENT_ADMIN_AdminId",
                        column: x => x.AdminId,
                        principalTable: "ADMIN",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APPLICATION",
                columns: table => new
                {
                    ApplyId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    CompanyName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AuditStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AuditAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RejectReason = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true),
                    AdminId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: true),
                    MerchantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPLICATION", x => x.ApplyId);
                    table.ForeignKey(
                        name: "FK_APPLICATION_ADMIN_AdminId",
                        column: x => x.AdminId,
                        principalTable: "ADMIN",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_APPLICATION_MERCHANT_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "MERCHANT",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DISH",
                columns: table => new
                {
                    DishId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    CategoryId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    DishName = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    OriginPrice = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    CoverUrl = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    MonthlySales = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Rating = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    OnSale = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MerchantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    ReviewQuantity = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISH", x => x.DishId);
                    table.ForeignKey(
                        name: "FK_DISH_CATEGORY_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CATEGORY",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DISH_MERCHANT_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "MERCHANT",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALESSTAT",
                columns: table => new
                {
                    StatDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MerchantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    SalesQty = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SalesAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALESSTAT", x => new { x.StatDate, x.MerchantId });
                    table.ForeignKey(
                        name: "FK_SALESSTAT_MERCHANT_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "MERCHANT",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SETTLEMENT",
                columns: table => new
                {
                    SettleId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    CommissionFee = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    MerchantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETTLEMENT", x => x.SettleId);
                    table.ForeignKey(
                        name: "FK_SETTLEMENT_MERCHANT_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "MERCHANT",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASSIGNMENT",
                columns: table => new
                {
                    AssignId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    RiderId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AcceptedStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    TimeOut = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSIGNMENT", x => x.AssignId);
                    table.ForeignKey(
                        name: "FK_ASSIGNMENT_RIDER_RiderId",
                        column: x => x.RiderId,
                        principalTable: "RIDER",
                        principalColumn: "RiderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATTENDANCE",
                columns: table => new
                {
                    AttendanceId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    CheckDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CheckInAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CheckoutAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IsLate = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RiderId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    IsAbsent = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATTENDANCE", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_ATTENDANCE_RIDER_RiderId",
                        column: x => x.RiderId,
                        principalTable: "RIDER",
                        principalColumn: "RiderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PERFORMANCE",
                columns: table => new
                {
                    RiderId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    StatsMonth = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TotalOrders = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OnTimeRate = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    GoodReviewRate = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    BadReviewRate = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    Income = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERFORMANCE", x => new { x.RiderId, x.StatsMonth });
                    table.ForeignKey(
                        name: "FK_PERFORMANCE_RIDER_RiderId",
                        column: x => x.RiderId,
                        principalTable: "RIDER",
                        principalColumn: "RiderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RIDER_SCHEDULE",
                columns: table => new
                {
                    RiderId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    ScheduleId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RIDER_SCHEDULE", x => new { x.RiderId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_RIDER_SCHEDULE_RIDER_RiderId",
                        column: x => x.RiderId,
                        principalTable: "RIDER",
                        principalColumn: "RiderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RIDER_SCHEDULE_SCHEDULE_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "SCHEDULE",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCHEDULE_ATTENDANCE",
                columns: table => new
                {
                    ScheduleId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    AttendanceId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCHEDULE_ATTENDANCE", x => new { x.ScheduleId, x.AttendanceId });
                    table.ForeignKey(
                        name: "FK_SCHEDULE_ATTENDANCE_ATTENDANCE_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "ATTENDANCE",
                        principalColumn: "AttendanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCHEDULE_ATTENDANCE_SCHEDULE_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "SCHEDULE",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADDRESS",
                columns: table => new
                {
                    AddressId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    ReceiverName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ReceiverPhone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DetailedAddress = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IsDefault = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADDRESS", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMUSER",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "VARCHAR2(450)", maxLength: 450, nullable: false),
                    Nickname = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AvatarUrl = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Gender = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Level = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DefaultAddrId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMUSER", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CUSTOMUSER_ADDRESS_DefaultAddrId",
                        column: x => x.DefaultAddrId,
                        principalTable: "ADDRESS",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CUSTOMUSER_APP_USERS_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "APP_USERS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CARTITEM",
                columns: table => new
                {
                    CartItemId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    MerchantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    DishId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARTITEM", x => new { x.CartItemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CARTITEM_CUSTOMUSER_UserId",
                        column: x => x.UserId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CARTITEM_DISH_DishId",
                        column: x => x.DishId,
                        principalTable: "DISH",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CARTITEM_MERCHANT_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "MERCHANT",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COUPON",
                columns: table => new
                {
                    CouponId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    FaceValue = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Threshold = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    TotalQty = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IssuedQty = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUPON", x => x.CouponId);
                    table.ForeignKey(
                        name: "FK_COUPON_CUSTOMUSER_UserId",
                        column: x => x.UserId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAVORITE",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    DishId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    FavorAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAVORITE", x => new { x.UserId, x.DishId });
                    table.ForeignKey(
                        name: "FK_FAVORITE_CUSTOMUSER_UserId",
                        column: x => x.UserId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FAVORITE_DISH_DishId",
                        column: x => x.DishId,
                        principalTable: "DISH",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    AddressId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OrderStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReconId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: true),
                    CouponId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: true),
                    AssignId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_ORDERS_ADDRESS_AddressId",
                        column: x => x.AddressId,
                        principalTable: "ADDRESS",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDERS_ASSIGNMENT_AssignId",
                        column: x => x.AssignId,
                        principalTable: "ASSIGNMENT",
                        principalColumn: "AssignId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ORDERS_COUPON_CouponId",
                        column: x => x.CouponId,
                        principalTable: "COUPON",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ORDERS_CUSTOMUSER_UserId",
                        column: x => x.UserId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDERS_RECONCILIATION_ReconId",
                        column: x => x.ReconId,
                        principalTable: "RECONCILIATION",
                        principalColumn: "ReconId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "COMPLAINT",
                columns: table => new
                {
                    ComplaintId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    ComplainantId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    CmplRole = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CmplDescription = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true),
                    CmplStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPLAINT", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_COMPLAINT_CUSTOMUSER_ComplainantId",
                        column: x => x.ComplainantId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_COMPLAINT_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_DISH",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    DishId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_DISH", x => new { x.OrderId, x.DishId });
                    table.ForeignKey(
                        name: "FK_ORDER_DISH_DISH_DishId",
                        column: x => x.DishId,
                        principalTable: "DISH",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_DISH_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERLOG",
                columns: table => new
                {
                    LogId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    StatusCode = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Actor = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERLOG", x => new { x.LogId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_ORDERLOG_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PAYMENT",
                columns: table => new
                {
                    PayId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    Channel = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    PayAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    PayStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PayTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENT", x => x.PayId);
                    table.ForeignKey(
                        name: "FK_PAYMENT_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REFUND",
                columns: table => new
                {
                    RefundId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    ApplicationId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true),
                    RefundAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    ApplyAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AuditStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFUND", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_REFUND_CUSTOMUSER_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REFUND_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REVIEW",
                columns: table => new
                {
                    ReviewId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    Rating = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Content = table.Column<string>(type: "NCLOB", maxLength: 65535, nullable: true),
                    IsAnonymous = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReviewAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REVIEW", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_REVIEW_CUSTOMUSER_UserId",
                        column: x => x.UserId,
                        principalTable: "CUSTOMUSER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REVIEW_ORDERS_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDERS",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DISH_REVIEW",
                columns: table => new
                {
                    DishId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false),
                    ReviewId = table.Column<string>(type: "CHAR(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISH_REVIEW", x => new { x.DishId, x.ReviewId });
                    table.ForeignKey(
                        name: "FK_DISH_REVIEW_DISH_DishId",
                        column: x => x.DishId,
                        principalTable: "DISH",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DISH_REVIEW_REVIEW_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "REVIEW",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADDRESS_UserId",
                table: "ADDRESS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ADMIN_ApplicationUserId",
                table: "ADMIN",
                column: "ApplicationUserId",
                unique: true,
                filter: "\"ApplicationUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ANNOUNCEMENT_AdminId",
                table: "ANNOUNCEMENT",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_APP_ROLE_CLAIMS_RoleId",
                table: "APP_ROLE_CLAIMS",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "APP_ROLES",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_APP_USER_CLAIMS_UserId",
                table: "APP_USER_CLAIMS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APP_USER_LOGINS_UserId",
                table: "APP_USER_LOGINS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APP_USER_ROLES_RoleId",
                table: "APP_USER_ROLES",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "APP_USERS",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "APP_USERS",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_APPLICATION_AdminId",
                table: "APPLICATION",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_APPLICATION_MerchantId",
                table: "APPLICATION",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_RiderId",
                table: "ASSIGNMENT",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_ATTENDANCE_RiderId",
                table: "ATTENDANCE",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_CARTITEM_DishId",
                table: "CARTITEM",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_CARTITEM_MerchantId",
                table: "CARTITEM",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_CARTITEM_UserId",
                table: "CARTITEM",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_ParentId",
                table: "CATEGORY",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_COMPLAINT_ComplainantId",
                table: "COMPLAINT",
                column: "ComplainantId");

            migrationBuilder.CreateIndex(
                name: "IX_COMPLAINT_OrderId",
                table: "COMPLAINT",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_COUPON_UserId",
                table: "COUPON",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMUSER_ApplicationUserId",
                table: "CUSTOMUSER",
                column: "ApplicationUserId",
                unique: true,
                filter: "\"ApplicationUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMUSER_DefaultAddrId",
                table: "CUSTOMUSER",
                column: "DefaultAddrId");

            migrationBuilder.CreateIndex(
                name: "IX_DISH_CategoryId",
                table: "DISH",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DISH_MerchantId",
                table: "DISH",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_DISH_REVIEW_ReviewId",
                table: "DISH_REVIEW",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_FAVORITE_DishId",
                table: "FAVORITE",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_MERCHANT_ApplicationUserId",
                table: "MERCHANT",
                column: "ApplicationUserId",
                unique: true,
                filter: "\"ApplicationUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DISH_DishId",
                table: "ORDER_DISH",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERLOG_OrderId",
                table: "ORDERLOG",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_AddressId",
                table: "ORDERS",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_AssignId",
                table: "ORDERS",
                column: "AssignId",
                unique: true,
                filter: "\"AssignId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_CouponId",
                table: "ORDERS",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_ReconId",
                table: "ORDERS",
                column: "ReconId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_UserId",
                table: "ORDERS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_OrderId",
                table: "PAYMENT",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_REFUND_ApplicationId",
                table: "REFUND",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_REFUND_OrderId",
                table: "REFUND",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_REVIEW_OrderId",
                table: "REVIEW",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_REVIEW_UserId",
                table: "REVIEW",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RIDER_ApplicationUserId",
                table: "RIDER",
                column: "ApplicationUserId",
                unique: true,
                filter: "\"ApplicationUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RIDER_SCHEDULE_ScheduleId",
                table: "RIDER_SCHEDULE",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SALESSTAT_MerchantId",
                table: "SALESSTAT",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_SCHEDULE_ATTENDANCE_AttendanceId",
                table: "SCHEDULE_ATTENDANCE",
                column: "AttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SETTLEMENT_MerchantId",
                table: "SETTLEMENT",
                column: "MerchantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ADDRESS_CUSTOMUSER_UserId",
                table: "ADDRESS",
                column: "UserId",
                principalTable: "CUSTOMUSER",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADDRESS_CUSTOMUSER_UserId",
                table: "ADDRESS");

            migrationBuilder.DropTable(
                name: "ANNOUNCEMENT");

            migrationBuilder.DropTable(
                name: "APP_ROLE_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_USER_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_USER_LOGINS");

            migrationBuilder.DropTable(
                name: "APP_USER_ROLES");

            migrationBuilder.DropTable(
                name: "APP_USER_TOKENS");

            migrationBuilder.DropTable(
                name: "APPLICATION");

            migrationBuilder.DropTable(
                name: "CARTITEM");

            migrationBuilder.DropTable(
                name: "COMPLAINT");

            migrationBuilder.DropTable(
                name: "DISH_REVIEW");

            migrationBuilder.DropTable(
                name: "FAVORITE");

            migrationBuilder.DropTable(
                name: "ORDER_DISH");

            migrationBuilder.DropTable(
                name: "ORDERLOG");

            migrationBuilder.DropTable(
                name: "PAYMENT");

            migrationBuilder.DropTable(
                name: "PERFORMANCE");

            migrationBuilder.DropTable(
                name: "REFUND");

            migrationBuilder.DropTable(
                name: "RIDER_SCHEDULE");

            migrationBuilder.DropTable(
                name: "SALESSTAT");

            migrationBuilder.DropTable(
                name: "SCHEDULE_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "SETTLEMENT");

            migrationBuilder.DropTable(
                name: "APP_ROLES");

            migrationBuilder.DropTable(
                name: "ADMIN");

            migrationBuilder.DropTable(
                name: "REVIEW");

            migrationBuilder.DropTable(
                name: "DISH");

            migrationBuilder.DropTable(
                name: "ATTENDANCE");

            migrationBuilder.DropTable(
                name: "SCHEDULE");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "CATEGORY");

            migrationBuilder.DropTable(
                name: "MERCHANT");

            migrationBuilder.DropTable(
                name: "ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "COUPON");

            migrationBuilder.DropTable(
                name: "RECONCILIATION");

            migrationBuilder.DropTable(
                name: "RIDER");

            migrationBuilder.DropTable(
                name: "CUSTOMUSER");

            migrationBuilder.DropTable(
                name: "ADDRESS");

            migrationBuilder.DropTable(
                name: "APP_USERS");
        }
    }
}
