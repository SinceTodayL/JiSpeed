# 外卖浏览界面所需的缺失API接口文档

## 概述
本文档记录了在开发外卖浏览界面时发现的，现有API文档中未提供但功能实现所必需的接口。

---

##  商家相关接口

### 1. 获取所有商家列表（分页）
**接口名称**: 获取商家列表  
**请求方法**: GET  
**请求路径**: `/api/merchant/all`

**功能描述**: 
- 获取平台所有商家的基本信息列表
- 支持分页查询
- 用于外卖首页展示商家列表

**请求参数**:
```javascript
{
  page: 1,        // 页码，默认1
  limit: 20,      // 每页数量，默认20
  keyword: "",    // 搜索关键词（可选）
  category: "",   // 商家分类（可选）
  sortBy: "rating", // 排序方式：rating(评分)、distance(距离)、sales(销量)、hot(热门)
}
```

**返回数据结构**:

参考返回的数据结构示例：
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "merchants": [
      {
        "merchantId": "MER1234567890001",
        "merchantName": "老王家川菜馆",
        "coverImage": "https://example.com/merchant-cover.jpg",
        "logo": "https://example.com/merchant-logo.jpg",
        "rating": 4.8, // 商家评分
        "monthlySales": 1200, // 月销量
        "deliveryTime": 35, // 配送时间（分钟）
        "deliveryFee": 3.5, // 配送费
        "minOrderAmount": 20, // 最低起送金额
        "distance": 1.2, // 距离（公里）
        "status": 1, // 商家状态：1-营业中，0-已打烊
        "location": "北京市朝阳区建国路88号",
        "tags": ["川菜", "家常菜", "辣"],
        "featuredDishes": [
          {
            "dishId": "DISH001",
            "dishName": "宫保鸡丁",
            "coverUrl": "https://example.com/dish1.jpg",
            "price": 28.5
          },
          {
            "dishId": "DISH002", 
            "dishName": "麻婆豆腐",
            "coverUrl": "https://example.com/dish2.jpg",
            "price": 18.0
          }
        ]
      }
    ],
    "total": 150,
    "page": 1,
    "limit": 20,
    "totalPages": 8
  }
}
```

### 2. 根据关键词搜索商家
**接口名称**: 搜索商家  
**请求方法**: GET  
**请求路径**: `/api/merchant/search`

**功能描述**: 
- 根据关键词搜索商家
- 支持商家名称、菜品名称搜索

**请求参数**:
```javascript
{
  keyword: "川菜",
  page: 1,
  limit: 20
}
```
**返回数据结构**
参照 获取所有商家列表（分页）


---

##  菜品相关接口

### 3. 获取菜品分类列表
**接口名称**: 获取菜品分类  
**请求方法**: GET  
**请求路径**: `/api/merchant/{merchantID}/getAllDishesByCategory`

**功能描述**: 
- 获取某一商家中所有菜品分类及对应菜品
- 用于菜品分类浏览和筛选

**返回数据结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [
    {
      "categoryId": "CAT10001",
      "categoryName": "热菜",
      "dishes": [
        {
          "dishId": "DISH001",
          "dishName": "宫保鸡丁",
          "coverUrl": "https://example.com/dish1.jpg",
          "price": 28.5,
          "description": "经典川菜，口感鲜香微辣"
        },
        {
          "dishId": "DISH002",
          "dishName": "麻婆豆腐",
          "coverUrl": "https://example.com/dish2.jpg",
          "price": 18.0,
          "description": "麻辣鲜香，豆腐嫩滑"
        }
      ],
      
    },
    {
      "categoryId": "CAT10002",
      "categoryName": "凉菜",
      "dishes": [
        {
          "dishId": "DISH003",
          "dishName": "凉拌黄瓜",
          "coverUrl": "https://example.com/dish3.jpg",
          "price": 12.0,
          "description": "清爽可口，开胃小菜"
        },
        {
          "dishId": "DISH004",
          "dishName": "皮蛋豆腐",
          "coverUrl": "https://example.com/dish4.jpg",
          "price": 15.0,
          "description": "滑嫩豆腐配上皮蛋，鲜香四溢"
        }
      ]
    },
    {
      "categoryId": "CAT10003",
      "categoryName": "汤类",
      "dishes": [
        {
          "dishId": "DISH005",
          "dishName": "酸辣汤",
          "coverUrl": "https://example.com/dish5.jpg",
          "price": 20.0,
          "description": "酸辣开胃，汤汁浓郁"
        },
        {
          "dishId": "DISH006",
          "dishName": "番茄蛋汤",
          "coverUrl": "https://example.com/dish6.jpg",
          "price": 15.0,
          "description": "清淡可口，营养丰富"
        }
      ]
    }
  ]
}
```

---

## 订单相关接口

### 4. 创建订单（下单）
**接口名称**: 创建订单  
**请求方法**: POST  
**请求路径**: `/api/orders`

**功能描述**: 
- 用户提交购物车商品，创建新订单
- 支持地址选择、备注信息、优惠券使用

**请求参数**:
```json
{
  "userId": "USER123",
  "merchantId": "MER1234567890001",
  "addressId": "ADDR001", 
  "orderItems": [
    {
      "dishId": "DISH001",
      "quantity": 2,
      "price": 28.5
    },
    {
      "dishId": "DISH002", 
      "quantity": 1,
      "price": 18.0
    }
  ],
  "totalAmount": 75.0,
  "deliveryFee": 3.5,
  "couponId": "COUPON001", // 可选
  "discountAmount": 5.0, // 可选
  "finalAmount": 73.5,
  "remark": "不要辣椒，多放香菜",
  "paymentMethod": "ALIPAY" // ALIPAY, WECHAT, CASH
}
```

**返回数据结构**:
```json
{
  "code": 200,
  "message": "下单成功",
  "data": {
    "orderId": "ORDER202507240001",
    "orderStatus": 0, // 0-待付款, 1-待接单, 2-已接单, 3-配送中, 4-已完成, 5-已取消
    "totalAmount": 75.0,
    "finalAmount": 73.5,
    "createAt": "2025-07-24T10:30:00",
    "estimatedDeliveryTime": "2025-07-24T11:15:00",
    "paymentUrl": "https://payment.gateway.com/pay/ORDER202507240001" // 支付链接
  }
}
```

### 5. 获取用户订单列表
**接口名称**: 获取订单列表  
**请求方法**: GET  
**请求路径**: `/api/users/{userId}/orders`

**功能描述**: 
- 获取用户的所有订单
- 支持按状态筛选、分页查询

**请求参数**:
```javascript
{
  page: 1,
  limit: 10,
  status: 0, // 可选，订单状态筛选
  startDate: "2025-07-01", // 可选，开始时间
  endDate: "2025-07-31" // 可选，结束时间
}
```

**返回数据结构**:
```json
{
  "code": 200,
  "message": "获取成功",
  "data": {
    "orders": [
      {
        "orderId": "ORDER202507240001",
        "userId": "USER123",
        "merchantId": "MER1234567890001",
        "merchantName": "老王家川菜馆",
        "merchantLogo": "https://example.com/logo.jpg",
        "orderStatus": 2,
        "orderStatusText": "已接单",
        "totalAmount": 75.0,
        "finalAmount": 73.5,
        "deliveryFee": 3.5,
        "discountAmount": 5.0,
        "createAt": "2025-07-24T10:30:00",
        "estimatedDeliveryTime": "2025-07-24T11:15:00",
        "deliveryAddress": {
          "addressId": "ADDR001",
          "receiverName": "张三",
          "receiverPhone": "13800138000", 
          "detailedAddress": "北京市朝阳区xxx小区1号楼101室"
        },
        "orderItems": [
          {
            "dishId": "DISH001",
            "dishName": "宫保鸡丁",
            "coverUrl": "https://example.com/dish1.jpg",
            "price": 28.5,
            "quantity": 2,
            "subtotal": 57.0
          },
          {
            "dishId": "DISH002",
            "dishName": "麻婆豆腐", 
            "coverUrl": "https://example.com/dish2.jpg",
            "price": 18.0,
            "quantity": 1,
            "subtotal": 18.0
          }
        ],
        "remark": "不要辣椒，多放香菜"
      }
    ],
    "total": 25,
    "page": 1,
    "limit": 10,
    "totalPages": 3
  }
}
```

### 6. 获取订单详情
**接口名称**: 获取订单详情  
**请求方法**: GET  
**请求路径**: `/api/orders/{orderId}`

**功能描述**: 
- 获取单个订单的详细信息
- 包含订单状态、配送信息、支付信息等

**返回数据结构**:
```json
{
  "code": 200,
  "message": "获取成功",
  "data": {
    "orderId": "ORDER202507240001",
    "userId": "USER123",
    "merchantId": "MER1234567890001",
    "merchantName": "老王家川菜馆",
    "merchantPhone": "13800001111",
    "merchantAddress": "北京市朝阳区建国路88号",
    "orderStatus": 3,
    "orderStatusText": "配送中",
    "totalAmount": 75.0,
    "finalAmount": 73.5,
    "deliveryFee": 3.5,
    "discountAmount": 5.0,
    "paymentMethod": "ALIPAY",
    "paymentStatus": 1, // 0-未支付, 1-已支付, 2-退款中, 3-已退款
    "createAt": "2025-07-24T10:30:00",
    "payAt": "2025-07-24T10:32:00",
    "acceptAt": "2025-07-24T10:35:00",
    "estimatedDeliveryTime": "2025-07-24T11:15:00",
    "deliveryAddress": {
      "addressId": "ADDR001",
      "receiverName": "张三",
      "receiverPhone": "13800138000",
      "detailedAddress": "北京市朝阳区xxx小区1号楼101室"
    },
    "deliveryInfo": {
      "riderId": "RIDER001",
      "riderName": "李师傅",
      "riderPhone": "13900139000",
      "currentLocation": "距离您500米"
    },
    "orderItems": [
      {
        "dishId": "DISH001", 
        "dishName": "宫保鸡丁",
        "coverUrl": "https://example.com/dish1.jpg",
        "price": 28.5,
        "quantity": 2,
        "subtotal": 57.0
      },
      {
        "dishId": "DISH002",
        "dishName": "麻婆豆腐",
        "coverUrl": "https://example.com/dish2.jpg", 
        "price": 18.0,
        "quantity": 1,
        "subtotal": 18.0
      }
    ],
    "remark": "不要辣椒，多放香菜",
    "statusHistory": [
      {
        "status": 0,
        "statusText": "订单已创建",
        "timestamp": "2025-07-24T10:30:00"
      },
      {
        "status": 1,
        "statusText": "支付成功，等待商家接单",
        "timestamp": "2025-07-24T10:32:00"
      },
      {
        "status": 2,
        "statusText": "商家已接单，准备中",
        "timestamp": "2025-07-24T10:35:00"
      },
      {
        "status": 3,
        "statusText": "订单已出餐，配送中",
        "timestamp": "2025-07-24T10:50:00"
      }
    ]
  }
}
```

### 7. 订单支付
**接口名称**: 订单支付  
**请求方法**: POST  
**请求路径**: `/api/orders/{orderId}/pay`

**功能描述**: 
- 用户对订单进行支付
- 支持多种支付方式

**请求参数**:
```json
{
  "paymentMethod": "ALIPAY", // ALIPAY, WECHAT, CASH
  "returnUrl": "https://app.example.com/order/success", // 支付成功后跳转地址
  "notifyUrl": "https://api.example.com/payment/notify" // 支付结果异步通知地址
}
```

**返回数据结构**:
```json
{
  "code": 200,
  "message": "支付请求成功",
  "data": {
    "paymentId": "PAY202507240001",
    "paymentUrl": "https://payment.gateway.com/pay/ORDER202507240001",
    "qrCode": "data:image/png;base64,iVBORw0KGgoAAAANS...", // 支付二维码
    "expireTime": "2025-07-24T10:45:00" // 支付过期时间
  }
}
```

### 8. 取消订单
**接口名称**: 取消订单  
**请求方法**: PUT  
**请求路径**: `/api/orders/{orderId}/cancel`

**功能描述**: 
- 用户取消订单
- 只能取消未支付或已支付但未接单的订单

**请求参数**:
```json
{
  "reason": "商品选择错误" // 取消原因
}
```

**返回数据结构**:
```json
{
  "code": 200,
  "message": "订单取消成功",
  "data": {
    "orderId": "ORDER202507240001",
    "orderStatus": 5,
    "cancelAt": "2025-07-24T10:35:00",
    "refundAmount": 73.5, // 退款金额（如果已支付）
    "refundStatus": 2 // 退款状态：0-无需退款, 1-退款处理中, 2-退款成功
  }
}
```

### 9. 确认收货
**接口名称**: 确认收货  
**请求方法**: PUT  
**请求路径**: `/api/orders/{orderId}/confirm`

**功能描述**: 
- 用户确认收到订单
- 订单状态变更为已完成

**返回数据结构**:
```json
{
  "code": 200,
  "message": "确认收货成功",
  "data": {
    "orderId": "ORDER202507240001",
    "orderStatus": 4,
    "completeAt": "2025-07-24T11:20:00"
  }
}
```

---








