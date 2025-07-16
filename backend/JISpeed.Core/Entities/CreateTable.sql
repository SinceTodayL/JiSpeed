-- Oracle 18c 建表语句 - JiSpeed 平台

-- 用户表
CREATE TABLE "User" (
    "UserId" CHAR(32) PRIMARY KEY,
    "Account" VARCHAR2(50) NOT NULL,
    "PasswordHash" CHAR(64) NOT NULL,
    "Status" NUMBER(1) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "DeletedAt" TIMESTAMP,
    "LastLoginAt" TIMESTAMP,
    "LastLoginIp" VARCHAR2(20),
    CONSTRAINT "UK_User_Account" UNIQUE ("Account")
);

-- 用户档案表
CREATE TABLE "UserProfile" (
    "UserId" CHAR(32) PRIMARY KEY,
    "Nickname" VARCHAR2(50) NOT NULL,
    "AvatarUrl" VARCHAR2(200) NOT NULL,
    "Gender" NUMBER(1) NOT NULL,
    "Birthday" DATE,
    "Level" NUMBER(2) NOT NULL,
    "DefaultAddrId" CHAR(32),
    CONSTRAINT "FK_UserProfile_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 地址表
CREATE TABLE "Address" (
    "AddressId" CHAR(32) PRIMARY KEY,
    "UserId" CHAR(32) NOT NULL,
    "ReceiverName" VARCHAR2(50) NOT NULL,
    "ReceiverPhone" VARCHAR2(20) NOT NULL,
    "DetailedAddress" VARCHAR2(200) NOT NULL,
    "IsDefault" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Address_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 添加 UserProfile 表的外键约束（在 Address 表创建后添加）
ALTER TABLE "UserProfile" ADD CONSTRAINT "FK_UserProfile_Address" 
    FOREIGN KEY ("DefaultAddrId") REFERENCES "Address"("AddressId");

-- 收藏表
CREATE TABLE "Favorite" (
    "FavoriteId" CHAR(32) PRIMARY KEY,
    "UserId" CHAR(32) NOT NULL,
    "DishId" CHAR(32) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_Favorite_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 购物车表
CREATE TABLE "CartItem" (
    "CartItemId" CHAR(32) PRIMARY KEY,
    "UserId" CHAR(32) NOT NULL,
    "DishId" CHAR(32) NOT NULL,
    "Quantity" NUMBER(5) NOT NULL,
    "MerchantId" CHAR(32) NOT NULL,
    "AddedAt" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_CartItem_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 管理员表
CREATE TABLE "Admin" (
    "AdminId" CHAR(32) PRIMARY KEY,
    "RoleId" CHAR(32) NOT NULL,
    "PermId" CHAR(32) NOT NULL,
    "GrantedAt" TIMESTAMP NOT NULL
);

-- 商家表
CREATE TABLE "Merchant" (
    "MerchantId" CHAR(32) PRIMARY KEY,
    "MerchantName" VARCHAR2(20) NOT NULL,
    "Status" NUMBER(1) NOT NULL,
    "ContactInfo" VARCHAR2(20),
    "Location" VARCHAR2(20)
);

-- 商家申请表
CREATE TABLE "Application" (
    "ApplicationId" CHAR(32) PRIMARY KEY,
    "MerchantId" CHAR(32) NOT NULL,
    "AdminId" CHAR(32),
    "ApplyTime" TIMESTAMP NOT NULL,
    "Status" NUMBER(1) NOT NULL,
    "AuditTime" TIMESTAMP,
    "Remark" VARCHAR2(200),
    CONSTRAINT "FK_Application_Merchant" FOREIGN KEY ("MerchantId") REFERENCES "Merchant"("MerchantId"),
    CONSTRAINT "FK_Application_Admin" FOREIGN KEY ("AdminId") REFERENCES "Admin"("AdminId")
);

-- 商家销售统计表
CREATE TABLE "SalesStat" (
    "StatId" CHAR(32) PRIMARY KEY,
    "MerchantId" CHAR(32) NOT NULL,
    "StatDate" DATE NOT NULL,
    "TotalAmount" DECIMAL(10, 2) NOT NULL,
    "OrderCount" NUMBER(10) NOT NULL,
    CONSTRAINT "FK_SalesStat_Merchant" FOREIGN KEY ("MerchantId") REFERENCES "Merchant"("MerchantId")
);

-- 商家结算表
CREATE TABLE "Settlement" (
    "SettlementId" CHAR(32) PRIMARY KEY,
    "MerchantId" CHAR(32) NOT NULL,
    "SettleAmount" DECIMAL(10, 2) NOT NULL,
    "SettleTime" TIMESTAMP NOT NULL,
    "SettlePeriodStart" DATE NOT NULL,
    "SettlePeriodEnd" DATE NOT NULL,
    "Status" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Settlement_Merchant" FOREIGN KEY ("MerchantId") REFERENCES "Merchant"("MerchantId")
);

-- 菜品分类表
CREATE TABLE "Category" (
    "CategoryId" CHAR(32) PRIMARY KEY,
    "CategoryName" VARCHAR2(20) NOT NULL,
    "ParentId" CHAR(32),
    CONSTRAINT "FK_Category_Parent" FOREIGN KEY ("ParentId") REFERENCES "Category"("CategoryId")
);

-- 菜品表
CREATE TABLE "Dish" (
    "DishId" CHAR(32) PRIMARY KEY,
    "CategoryId" CHAR(32) NOT NULL,
    "DishName" VARCHAR2(20) NOT NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "OriginPrice" DECIMAL(10, 2) NOT NULL,
    "CoverUrl" VARCHAR2(200),
    "MonthlySales" NUMBER(10) NOT NULL,
    "Rating" DECIMAL(5, 2) NOT NULL,
    "OnSale" NUMBER(1) NOT NULL,
    "MerchantId" CHAR(32) NOT NULL,
    "ReviewQuantity" NUMBER(10) NOT NULL,
    CONSTRAINT "FK_Dish_Category" FOREIGN KEY ("CategoryId") REFERENCES "Category"("CategoryId"),
    CONSTRAINT "FK_Dish_Merchant" FOREIGN KEY ("MerchantId") REFERENCES "Merchant"("MerchantId")
);

-- 添加 Favorite 表的外键约束（在 Dish 表创建后添加）
ALTER TABLE "Favorite" ADD CONSTRAINT "FK_Favorite_Dish" 
    FOREIGN KEY ("DishId") REFERENCES "Dish"("DishId");

-- 添加 CartItem 表的外键约束（在 Dish 和 Merchant 表创建后添加）
ALTER TABLE "CartItem" ADD CONSTRAINT "FK_CartItem_Dish" 
    FOREIGN KEY ("DishId") REFERENCES "Dish"("DishId");
ALTER TABLE "CartItem" ADD CONSTRAINT "FK_CartItem_Merchant" 
    FOREIGN KEY ("MerchantId") REFERENCES "Merchant"("MerchantId");

-- 优惠券表
CREATE TABLE "Coupon" (
    "CouponId" CHAR(32) PRIMARY KEY,
    "UserId" CHAR(32) NOT NULL,
    "FaceValue" DECIMAL(10, 2) NOT NULL,
    "Threshold" DECIMAL(10, 2) NOT NULL,
    "TotalQty" NUMBER(10) NOT NULL,
    "IssuedQty" NUMBER(10) NOT NULL,
    "StartTime" TIMESTAMP NOT NULL,
    "EndTime" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_Coupon_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 公告表
CREATE TABLE "Announcement" (
    "AnnouncementId" CHAR(32) PRIMARY KEY,
    "AdminId" CHAR(32) NOT NULL,
    "Title" VARCHAR2(100) NOT NULL,
    "Content" CLOB NOT NULL,
    "PublishTime" TIMESTAMP NOT NULL,
    "IsActive" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Announcement_Admin" FOREIGN KEY ("AdminId") REFERENCES "Admin"("AdminId")
);

-- 骑手表
CREATE TABLE "rider" (
    "RiderId" CHAR(32) PRIMARY KEY,
    "Name" VARCHAR2(20) NOT NULL,
    "PhoneNumber" CHAR(11) NOT NULL,
    "VehicleNumber" VARCHAR2(20)
);

-- 排班表
CREATE TABLE "Schedule" (
    "ScheduleId" CHAR(32) PRIMARY KEY,
    "Date" DATE NOT NULL,
    "StartTime" TIMESTAMP NOT NULL,
    "EndTime" TIMESTAMP NOT NULL,
    "ScheduleType" NUMBER(1) NOT NULL
);

-- 骑手排班表
CREATE TABLE "RiderSchedule" (
    "RiderScheduleId" CHAR(32) PRIMARY KEY,
    "RiderId" CHAR(32) NOT NULL,
    "ScheduleId" CHAR(32) NOT NULL,
    "AssignedAt" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_RiderSchedule_Rider" FOREIGN KEY ("RiderId") REFERENCES "rider"("RiderId"),
    CONSTRAINT "FK_RiderSchedule_Schedule" FOREIGN KEY ("ScheduleId") REFERENCES "Schedule"("ScheduleId")
);

-- 考勤表
CREATE TABLE "Attendance" (
    "AttendanceId" CHAR(32) PRIMARY KEY,
    "RiderId" CHAR(32) NOT NULL,
    "CheckInTime" TIMESTAMP,
    "CheckOutTime" TIMESTAMP,
    "AttendanceDate" DATE NOT NULL,
    "AttendanceStatus" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Attendance_Rider" FOREIGN KEY ("RiderId") REFERENCES "rider"("RiderId")
);

-- 排班考勤关联表
CREATE TABLE "ScheduleAttendance" (
    "ScheduleAttendanceId" CHAR(32) PRIMARY KEY,
    "ScheduleId" CHAR(32) NOT NULL,
    "AttendanceId" CHAR(32) NOT NULL,
    CONSTRAINT "FK_SchedAttend_Schedule" FOREIGN KEY ("ScheduleId") REFERENCES "Schedule"("ScheduleId"),
    CONSTRAINT "FK_SchedAttend_Attendance" FOREIGN KEY ("AttendanceId") REFERENCES "Attendance"("AttendanceId")
);

-- 骑手绩效表
CREATE TABLE "Performance" (
    "PerformanceId" CHAR(32) PRIMARY KEY,
    "RiderId" CHAR(32) NOT NULL,
    "EvaluationDate" DATE NOT NULL,
    "DeliveryCount" NUMBER(5) NOT NULL,
    "ComplaintCount" NUMBER(5) NOT NULL,
    "OnTimeRate" DECIMAL(5, 2) NOT NULL,
    "Rating" DECIMAL(5, 2) NOT NULL,
    CONSTRAINT "FK_Performance_Rider" FOREIGN KEY ("RiderId") REFERENCES "rider"("RiderId")
);

-- 对账表
CREATE TABLE "Reconciliation" (
    "ReconId" CHAR(32) PRIMARY KEY,
    "ReconDate" DATE NOT NULL,
    "ReconType" NUMBER(1) NOT NULL,
    "Amount" DECIMAL(10, 2) NOT NULL,
    "Status" NUMBER(1) NOT NULL,
    "ResolvedAt" TIMESTAMP
);

-- 订单表
CREATE TABLE "Order" (
    "OrderId" CHAR(32) PRIMARY KEY,
    "UserId" CHAR(32) NOT NULL,
    "AddressId" CHAR(32) NOT NULL,
    "OrderAmount" DECIMAL(10, 2) NOT NULL,
    "CreateAt" TIMESTAMP NOT NULL,
    "OrderStatus" NUMBER(1) NOT NULL,
    "ReconId" CHAR(32),
    "CouponId" CHAR(32),
    "AssignId" CHAR(32),
    CONSTRAINT "FK_Order_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId"),
    CONSTRAINT "FK_Order_Address" FOREIGN KEY ("AddressId") REFERENCES "Address"("AddressId"),
    CONSTRAINT "FK_Order_Reconciliation" FOREIGN KEY ("ReconId") REFERENCES "Reconciliation"("ReconId"),
    CONSTRAINT "FK_Order_Coupon" FOREIGN KEY ("CouponId") REFERENCES "Coupon"("CouponId")
);

-- 派单表
CREATE TABLE "Assignment" (
    "AssignId" CHAR(32) PRIMARY KEY,
    "RiderId" CHAR(32) NOT NULL,
    "AssignTime" TIMESTAMP NOT NULL,
    "CompleteTime" TIMESTAMP,
    "Status" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Assignment_Rider" FOREIGN KEY ("RiderId") REFERENCES "rider"("RiderId")
);

-- 添加 Order 表的派单外键约束（在 Assignment 表创建后添加）
ALTER TABLE "Order" ADD CONSTRAINT "FK_Order_Assignment" 
    FOREIGN KEY ("AssignId") REFERENCES "Assignment"("AssignId");

-- 订单日志表
CREATE TABLE "OrderLog" (
    "LogId" CHAR(32) PRIMARY KEY,
    "OrderId" CHAR(32) NOT NULL,
    "LogTime" TIMESTAMP NOT NULL,
    "LogType" NUMBER(1) NOT NULL,
    "LogContent" VARCHAR2(200) NOT NULL,
    CONSTRAINT "FK_OrderLog_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId")
);

-- 支付表
CREATE TABLE "Payment" (
    "PaymentId" CHAR(32) PRIMARY KEY,
    "OrderId" CHAR(32) NOT NULL,
    "PaymentAmount" DECIMAL(10, 2) NOT NULL,
    "PaymentTime" TIMESTAMP NOT NULL,
    "PaymentMethod" NUMBER(1) NOT NULL,
    "PaymentStatus" NUMBER(1) NOT NULL,
    "TransactionId" VARCHAR2(64),
    CONSTRAINT "FK_Payment_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId")
);

-- 退款表
CREATE TABLE "Refund" (
    "RefundId" CHAR(32) PRIMARY KEY,
    "OrderId" CHAR(32) NOT NULL,
    "UserId" CHAR(32) NOT NULL,
    "RefundAmount" DECIMAL(10, 2) NOT NULL,
    "RefundReason" VARCHAR2(200) NOT NULL,
    "ApplyTime" TIMESTAMP NOT NULL,
    "ProcessTime" TIMESTAMP,
    "Status" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Refund_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId"),
    CONSTRAINT "FK_Refund_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 投诉表
CREATE TABLE "Complaint" (
    "ComplaintId" CHAR(32) PRIMARY KEY,
    "OrderId" CHAR(32) NOT NULL,
    "UserId" CHAR(32) NOT NULL,
    "ComplaintType" NUMBER(1) NOT NULL,
    "ComplaintContent" VARCHAR2(500) NOT NULL,
    "SubmitTime" TIMESTAMP NOT NULL,
    "ProcessTime" TIMESTAMP,
    "Status" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Complaint_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId"),
    CONSTRAINT "FK_Complaint_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 评价表
CREATE TABLE "Review" (
    "ReviewId" CHAR(32) PRIMARY KEY,
    "OrderId" CHAR(32) NOT NULL,
    "UserId" CHAR(32) NOT NULL,
    "Rating" NUMBER(1) NOT NULL,
    "Content" VARCHAR2(500),
    "ReviewTime" TIMESTAMP NOT NULL,
    "IsAnonymous" NUMBER(1) NOT NULL,
    CONSTRAINT "FK_Review_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId"),
    CONSTRAINT "FK_Review_User" FOREIGN KEY ("UserId") REFERENCES "User"("UserId")
);

-- 订单菜品关联表
CREATE TABLE "order_dish" (
    "OrderId" CHAR(32),
    "DishId" CHAR(32),
    CONSTRAINT "PK_OrderDish" PRIMARY KEY ("OrderId", "DishId"),
    CONSTRAINT "FK_OrderDish_Order" FOREIGN KEY ("OrderId") REFERENCES "Order"("OrderId"),
    CONSTRAINT "FK_OrderDish_Dish" FOREIGN KEY ("DishId") REFERENCES "Dish"("DishId")
);

-- 菜品评价关联表
CREATE TABLE "dish_review" (
    "DishId" CHAR(32),
    "ReviewId" CHAR(32),
    CONSTRAINT "PK_DishReview" PRIMARY KEY ("DishId", "ReviewId"),
    CONSTRAINT "FK_DishReview_Dish" FOREIGN KEY ("DishId") REFERENCES "Dish"("DishId"),
    CONSTRAINT "FK_DishReview_Review" FOREIGN KEY ("ReviewId") REFERENCES "Review"("ReviewId")
);

-- 注意: 不需要创建序列用于ID自增
-- 因为所有ID字段都使用GUID在应用程序实体层生成和赋值

-- 创建索引以提高查询性能
CREATE INDEX IDX_USER_ACCOUNT ON "User"("Account");
CREATE INDEX IDX_ADDRESS_USER ON "Address"("UserId");
CREATE INDEX IDX_CART_USER ON "CartItem"("UserId");
CREATE INDEX IDX_CART_MERCHANT ON "CartItem"("MerchantId");
CREATE INDEX IDX_DISH_MERCHANT ON "Dish"("MerchantId");
CREATE INDEX IDX_DISH_CATEGORY ON "Dish"("CategoryId");
CREATE INDEX IDX_ORDER_USER ON "Order"("UserId");
CREATE INDEX IDX_ORDER_STATUS ON "Order"("OrderStatus");
CREATE INDEX IDX_PAYMENT_ORDER ON "Payment"("OrderId");
CREATE INDEX IDX_REFUND_ORDER ON "Refund"("OrderId");
CREATE INDEX IDX_REVIEW_ORDER ON "Review"("OrderId");
CREATE INDEX IDX_COMPLAINT_ORDER ON "Complaint"("OrderId");
CREATE INDEX IDX_RIDER_PHONE ON "rider"("PhoneNumber");
CREATE INDEX IDX_ATTENDANCE_RIDER ON "Attendance"("RiderId");
CREATE INDEX IDX_PERFORMANCE_RIDER ON "Performance"("RiderId");
CREATE INDEX IDX_ASSIGNMENT_RIDER ON "Assignment"("RiderId");
CREATE INDEX IDX_ASSIGNMENT_STATUS ON "Assignment"("Status");