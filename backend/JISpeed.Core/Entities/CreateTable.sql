-- 用户表
CREATE TABLE CustomUser (
    UserId          CHAR(32)        PRIMARY KEY
);

-- 统一登录表
CREATE TABLE Login (
    Account         VARCHAR2(32)    NOT NULL,
    PasswordHash    VARCHAR2(64)    NOT NULL,
    LoginRole       NUMBER(10)      NOT NULL,
    Status          NUMBER(10)      NOT NULL,
    CreatedAt       DATE            NOT NULL,
    DeletedAt       DATE            NULL,
    LastLoginAt     DATE            NULL,
    LastLoginIp     VARCHAR2(20)    NULL,
    RoleId          CHAR(32)        NOT NULL,
    CONSTRAINT PK_Login_Account PRIMARY KEY (Account)
);

-- 地址表
CREATE TABLE Address (
    AddressId       CHAR(32)        PRIMARY KEY,
    UserId          CHAR(32)        NOT NULL,
    ReceiverName    VARCHAR2(50)    NOT NULL,
    ReceiverPhone   VARCHAR2(20)    NOT NULL,
    DetailedAddress VARCHAR2(200)   NOT NULL,
    IsDefault       NUMBER(1)       NOT NULL,
    CONSTRAINT FK_Address_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId)
);

-- 用户档案表
CREATE TABLE UserProfile (
    UserId          CHAR(32)        PRIMARY KEY,
    Nickname        VARCHAR2(50)    NOT NULL,
    AvatarUrl       VARCHAR2(200)   NOT NULL,
    Gender          NUMBER(1)       NOT NULL,
    Birthday        DATE            NULL,
    Levels           NUMBER(2)       NOT NULL,
    DefaultAddrId   CHAR(32)        NULL,
    CONSTRAINT FK_UserProfile_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId),
    CONSTRAINT FK_UserProfile_Address FOREIGN KEY (DefaultAddrId) REFERENCES Address(AddressId)
);

-- 菜品分类表
CREATE TABLE Category (
    CategoryId      CHAR(32)        PRIMARY KEY,
    CategoryName    VARCHAR2(20)    NOT NULL,
    ParentId        CHAR(32)        NULL,
    SortOrder       NUMBER(10)      NOT NULL,
    CONSTRAINT FK_Category_Parent FOREIGN KEY (ParentId) REFERENCES Category(CategoryId)
);

-- 商家表
CREATE TABLE Merchant (
    MerchantId      CHAR(32)        PRIMARY KEY,
    MerchantName    VARCHAR2(20)    NOT NULL,
    Status          NUMBER(1)       NOT NULL,
    ContactInfo     VARCHAR2(20)    NULL,
    Location        VARCHAR2(20)    NULL
);

-- 菜品表
CREATE TABLE Dish (
    DishId          CHAR(32)        PRIMARY KEY,
    CategoryId      CHAR(32)        NOT NULL,
    DishName        VARCHAR2(20)    NOT NULL,
    Price           NUMBER(10, 2)   NOT NULL,
    OriginPrice     NUMBER(10, 2)   NOT NULL,
    CoverUrl        VARCHAR2(200)   NULL,
    MonthlySales    NUMBER(10)      NOT NULL,
    Rating          NUMBER(5, 2)    NOT NULL,
    OnSale          NUMBER(1)       NOT NULL,
    MerchantId      CHAR(32)        NOT NULL,
    ReviewQuantity  NUMBER(10)      NOT NULL,
    CONSTRAINT FK_Dish_Category FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    CONSTRAINT FK_Dish_Merchant FOREIGN KEY (MerchantId) REFERENCES Merchant(MerchantId)
);

-- 收藏表
CREATE TABLE Favorite (
    UserId          CHAR(32)        NOT NULL,
    DishId          CHAR(32)        NOT NULL,
    FavorAt         DATE            NOT NULL,
    CONSTRAINT PK_Favorite PRIMARY KEY (UserId, DishId),
    CONSTRAINT FK_Favorite_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId),
    CONSTRAINT FK_Favorite_Dish FOREIGN KEY (DishId) REFERENCES Dish(DishId)
);

-- 购物车表
CREATE TABLE CartItem (
    CartItemId      CHAR(32)        NOT NULL,
    UserId          CHAR(32)        NOT NULL,
    DishId          CHAR(32)        NOT NULL,
    MerchantId      CHAR(32)        NOT NULL,
    AddedAt         DATE            NOT NULL,
    CONSTRAINT PK_CartItem PRIMARY KEY (CartItemId, UserId),
    CONSTRAINT FK_CartItem_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId),
    CONSTRAINT FK_CartItem_Dish FOREIGN KEY (DishId) REFERENCES Dish(DishId),
    CONSTRAINT FK_CartItem_Merchant FOREIGN KEY (MerchantId) REFERENCES Merchant(MerchantId)
);

-- 管理员表
CREATE TABLE Admin (
    AdminId         CHAR(32)        PRIMARY KEY,
    RoleId          CHAR(32)        NOT NULL,
    PermId          CHAR(32)        NOT NULL,
    GrantedAt       DATE            NOT NULL
);

-- 商家申请表
CREATE TABLE Application (
    ApplyId         CHAR(32)        PRIMARY KEY,
    CompanyName     VARCHAR2(255)   NOT NULL,
    SubmittedAt     DATE            NOT NULL,
    AuditStatus     NUMBER(1)       NOT NULL,
    AuditAt         DATE            NULL,
    RejectReason    CLOB            NULL,
    AdminId         CHAR(32)        NULL,
    MerchantId      CHAR(32)        NOT NULL,
    CONSTRAINT FK_Application_Merchant FOREIGN KEY (MerchantId) REFERENCES Merchant(MerchantId),
    CONSTRAINT FK_Application_Admin FOREIGN KEY (AdminId) REFERENCES Admin(AdminId)
);

-- 商家销售统计表
CREATE TABLE SalesStat (
    StatDate        DATE            PRIMARY KEY,
    MerchantId      CHAR(32)        NOT NULL,
    SalesQty        NUMBER(10)      NOT NULL,
    SalesAmount     NUMBER(10, 2)   NOT NULL,
    CONSTRAINT FK_SalesStat_Merchant FOREIGN KEY (MerchantId) REFERENCES Merchant(MerchantId)
);

-- 商家结算表
CREATE TABLE Settlement (
    SettleId        CHAR(32)        PRIMARY KEY,
    MerchantId      CHAR(32)        NOT NULL,
    GrossAmount     NUMBER(10, 2)   NOT NULL,
    CommissionFee   NUMBER(10, 2)   NOT NULL,
    NetAmount       NUMBER(10, 2)   NOT NULL,
    SettledAt       DATE            NULL,
    PeriodStart     DATE            NOT NULL,
    PeriodEnd       DATE            NOT NULL,
    CONSTRAINT FK_Settlement_Merchant FOREIGN KEY (MerchantId) REFERENCES Merchant(MerchantId)
);

-- 优惠券表
CREATE TABLE Coupon (
    CouponId        CHAR(32)        PRIMARY KEY,
    UserId          CHAR(32)        NOT NULL,
    FaceValue       NUMBER(10, 2)   NOT NULL,
    Threshold       NUMBER(10, 2)   NOT NULL,
    TotalQty        NUMBER(10)      NOT NULL,
    IssuedQty       NUMBER(10)      NOT NULL,
    StartTime       DATE            NOT NULL,
    EndTime         DATE            NOT NULL,
    CONSTRAINT FK_Coupon_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId)
);

-- 公告表
CREATE TABLE Announcement (
    AnnounceId      CHAR(32)        PRIMARY KEY,
    AdminId         CHAR(32)        NOT NULL,
    Title           VARCHAR2(100)   NOT NULL,
    Content         CLOB            NULL,
    TargetRole      NUMBER(1)       NULL,
    StartAt         DATE            NOT NULL,
    EndAt           DATE            NOT NULL,
    CONSTRAINT FK_Announcement_Admin FOREIGN KEY (AdminId) REFERENCES Admin(AdminId)
);

-- 骑手表
CREATE TABLE Rider (
    RiderId         CHAR(32)        PRIMARY KEY,
    Name            VARCHAR2(20)    NOT NULL,
    PhoneNumber     CHAR(11)        NOT NULL,
    VehicleNumber   VARCHAR2(20)    NULL
);

-- 排班表
CREATE TABLE Schedule (
    ScheduleId      CHAR(32)        PRIMARY KEY,
    WorkDate        DATE            NOT NULL,
    ShiftStart      DATE            NOT NULL,
    ShiftEnd        DATE            NOT NULL
);

-- 骑手排班表
CREATE TABLE RiderSchedule (
    RiderId         CHAR(32)        NOT NULL,
    ScheduleId      CHAR(32)        NOT NULL,
    CONSTRAINT PK_RiderSchedule PRIMARY KEY (RiderId, ScheduleId),
    CONSTRAINT FK_RiderSchedule_Rider FOREIGN KEY (RiderId) REFERENCES Rider(RiderId),
    CONSTRAINT FK_RiderSchedule_Schedule FOREIGN KEY (ScheduleId) REFERENCES Schedule(ScheduleId)
);

-- 考勤表
CREATE TABLE Attendance (
    AttendanceId    CHAR(32)        PRIMARY KEY,
    RiderId         CHAR(32)        NOT NULL,
    CheckDate       DATE            NOT NULL,
    CheckInAt       DATE            NULL,
    CheckOutAt      DATE            NULL,
    IsLate          NUMBER(1)       NOT NULL,
    IsAbsent        NUMBER(1)       NOT NULL,
    CONSTRAINT FK_Attendance_Rider FOREIGN KEY (RiderId) REFERENCES Rider(RiderId)
);

-- 排班考勤关联表
CREATE TABLE ScheduleAttendance (
    ScheduleId      CHAR(32)        NOT NULL,
    AttendanceId    CHAR(32)        NOT NULL,
    CONSTRAINT PK_SchedAttend PRIMARY KEY (ScheduleId, AttendanceId),
    CONSTRAINT FK_SchedAttend_Schedule FOREIGN KEY (ScheduleId) REFERENCES Schedule(ScheduleId),
    CONSTRAINT FK_SchedAttend_Attendance FOREIGN KEY (AttendanceId) REFERENCES Attendance(AttendanceId)
);

-- 骑手绩效表
CREATE TABLE Performance (
    RiderId         CHAR(32)        NOT NULL,
    StatsMonth      DATE            NOT NULL,
    TotalOrders     NUMBER(10)      NOT NULL,
    OnTimeRate      NUMBER(5, 2)    NOT NULL,
    GoodReviewRate  NUMBER(5, 2)    NOT NULL,
    BadReviewRate   NUMBER(5, 2)    NOT NULL,
    CONSTRAINT PK_Performance PRIMARY KEY (RiderId, StatsMonth),
    CONSTRAINT FK_Performance_Rider FOREIGN KEY (RiderId) REFERENCES Rider(RiderId)
);

-- 对账表
CREATE TABLE Reconciliation (
    ReconId         CHAR(32)        PRIMARY KEY,
    PeriodStart     DATE            NOT NULL,
    PeriodEnd       DATE            NOT NULL,
    FoundAt         DATE            NOT NULL,
    ReconType       NUMBER(1)       NOT NULL,
    DiffAmount      NUMBER(10, 2)   NOT NULL,
    AffectedOrders  NUMBER(10)      NOT NULL,
    IsResolved      NUMBER(1)       NOT NULL
);

-- 派单表
CREATE TABLE Assignment (
    AssignId        CHAR(32)        PRIMARY KEY,
    RiderId         CHAR(32)        NOT NULL,
    AssignAt        DATE            NOT NULL,
    AcceptedStatus  NUMBER(1)       NOT NULL,
    AcceptedAt      DATE            NULL,
    TimeOutLength   NUMBER(10)      NOT NULL,
    CONSTRAINT FK_Assignment_Rider FOREIGN KEY (RiderId) REFERENCES Rider(RiderId)
);

-- 订单表
CREATE TABLE Orders (
    OrdersId        CHAR(32)        PRIMARY KEY,
    UserId          CHAR(32)        NOT NULL,
    AddressId       CHAR(32)        NOT NULL,
    OrdersAmount    NUMBER(10, 2)   NOT NULL,
    CreateAt        DATE            NOT NULL,
    OrdersStatus    NUMBER(1)       NOT NULL,
    ReconId         CHAR(32)        NULL,
    CouponId        CHAR(32)        NULL,
    AssignId        CHAR(32)        NULL,
    CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId),
    CONSTRAINT FK_Orders_Address FOREIGN KEY (AddressId) REFERENCES Address(AddressId),
    CONSTRAINT FK_Orders_Reconciliation FOREIGN KEY (ReconId) REFERENCES Reconciliation(ReconId),
    CONSTRAINT FK_Orders_Coupon FOREIGN KEY (CouponId) REFERENCES Coupon(CouponId),
    CONSTRAINT FK_Orders_Assignment FOREIGN KEY (AssignId) REFERENCES Assignment(AssignId)
);

-- 订单日志表
CREATE TABLE OrdersLog (
    LogId           CHAR(32)        NOT NULL,
    OrdersId        CHAR(32)        NOT NULL,
    LoggedAt        DATE            NOT NULL,
    StatusCode      NUMBER(1)       NOT NULL,
    Actor           VARCHAR2(20)    NOT NULL,
    Remark          CLOB            NULL,
    CONSTRAINT PK_OrdersLog PRIMARY KEY (LogId, OrdersId),
    CONSTRAINT FK_OrdersLog_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId)
);

-- 支付表
CREATE TABLE Payment (
    PayId           CHAR(32)        PRIMARY KEY,
    OrdersId        CHAR(32)        NOT NULL,
    Channel         VARCHAR2(20)    NOT NULL,
    PayAmount       NUMBER(10, 2)   NOT NULL,
    PayStatus       NUMBER(1)       NOT NULL,
    PayTime         DATE            NULL,
    CONSTRAINT FK_Payment_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId)
);

-- 退款表
CREATE TABLE Refund (
    RefundId        CHAR(32)        PRIMARY KEY,
    OrdersId        CHAR(32)        NOT NULL,
    ApplicationId   CHAR(32)        NOT NULL,
    RefundAmount    NUMBER(10, 2)   NOT NULL,
    Reason          CLOB            NULL,
    ApplyAt         DATE            NOT NULL,
    AuditStatus     NUMBER(1)       NOT NULL,
    FinishAt        DATE            NULL,
    CONSTRAINT FK_Refund_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId),
    CONSTRAINT FK_Refund_User FOREIGN KEY (ApplicationId) REFERENCES CustomUser(UserId)
);

-- 投诉表
CREATE TABLE Complaint (
    ComplaintId     CHAR(32)        PRIMARY KEY,
    OrdersId        CHAR(32)        NOT NULL,
    ComplainantId   CHAR(32)        NOT NULL,
    CmplRole        NUMBER(1)       NOT NULL,
    CmplDescription CLOB            NULL,
    CmplStatus      NUMBER(1)       NOT NULL,
    CreatedAt       DATE            NOT NULL,
    CONSTRAINT FK_Complaint_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId),
    CONSTRAINT FK_Complaint_User FOREIGN KEY (ComplainantId) REFERENCES CustomUser(UserId)
);

-- 评价表
CREATE TABLE Review (
    ReviewId        CHAR(32)        PRIMARY KEY,
    OrdersId        CHAR(32)        NOT NULL,
    UserId          CHAR(32)        NOT NULL,
    Rating          NUMBER(1)       NOT NULL,
    Content         VARCHAR2(500)   NULL,
    ReviewTime      DATE            NOT NULL,
    IsAnonymous     NUMBER(1)       NOT NULL,
    CONSTRAINT FK_Review_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId),
    CONSTRAINT FK_Review_User FOREIGN KEY (UserId) REFERENCES CustomUser(UserId)
);


-- 订单菜品关联表
CREATE TABLE order_dish (
    OrdersId        CHAR(32)        NOT NULL,
    DishId          CHAR(32)        NOT NULL,
    CONSTRAINT PK_OrdersDish PRIMARY KEY (OrdersId, DishId),
    CONSTRAINT FK_OrdersDish_Orders FOREIGN KEY (OrdersId) REFERENCES Orders(OrdersId),
    CONSTRAINT FK_OrdersDish_Dish FOREIGN KEY (DishId) REFERENCES Dish(DishId)
);

-- 菜品评价关联表
CREATE TABLE dish_review (
    DishId          CHAR(32)        NOT NULL,
    ReviewId        CHAR(32)        NOT NULL,
    CONSTRAINT PK_DishReview PRIMARY KEY (DishId, ReviewId),
    CONSTRAINT FK_DishReview_Dish FOREIGN KEY (DishId) REFERENCES Dish(DishId),
    CONSTRAINT FK_DishReview_Review FOREIGN KEY (ReviewId) REFERENCES Review(ReviewId)
);