# 购物车API接口文档

## 概述

购物车模块提供了完整的购物车管理功能，包括商品的增删改查、批量操作、状态管理等。本文档基于项目的API规范和错误处理框架。

## 基础信息

- **基础路径**: `/api/users/{userId}/`
- **认证方式**: Bearer Token
- **响应格式**: JSON (统一响应体格式)

## 通用响应格式

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    // 业务数据
  },
  "timestamp": 1678886400000
}
```

## API接口列表

### 1. 获取用户购物车内容

**接口**: `GET /api/users/{userId}/cart`

**描述**: 获取指定用户的完整购物车信息，包括商品详情、数量、价格等。

**路径参数**:
- `userId` (string): 用户ID

**查询参数**:
- `includeUnavailable` (boolean, 可选): 是否包含不可用商品，默认true

**响应示例**:
```json
{
  "code": 0,
  "message": "获取购物车成功",
  "data": {
    "userId": "USER001",
    "items": [
      {
        "cartItemId": "CART001",
        "dishId": "DISH001",
        "dishName": "宫保鸡丁",
        "dishImage": "https://example.com/image.jpg",
        "price": 26.8,
        "quantity": 2,
        "merchantId": "MERCHANT001",
        "merchantName": "川味小厨",
        "subtotal": 53.6,
        "addedAt": "2024-01-15T10:30:00Z",
        "isAvailable": true,
        "selected": true
      }
    ],
    "summary": {
      "totalItems": 4,
      "totalAmount": 108.0,
      "selectedAmount": 80.0,
      "merchantCount": 2
    }
  },
  "timestamp": 1678886400000
}
```

### 2. 添加商品到购物车

**接口**: `POST /api/users/{userId}/cartitems`

**描述**: 将指定商品添加到用户购物车中。如果商品已存在，则增加数量。

**路径参数**:
- `userId` (string): 用户ID

**请求体**:
```json
{
  "dishId": "DISH001",
  "merchantId": "MERCHANT001",
  "quantity": 1
}
```

**响应示例**:
```json
{
  "code": 0,
  "message": "添加到购物车成功",
  "data": {
    "cartItemId": "CART001",
    "dishId": "DISH001",
    "quantity": 3,
    "subtotal": 80.4,
    "addedAt": "2024-01-15T10:30:00Z"
  },
  "timestamp": 1678886400000
}
```

### 3. 更新购物车商品数量

**接口**: `PATCH /api/users/{userId}/cartitems/{cartItemId}`

**描述**: 更新购物车中指定商品的数量。

**路径参数**:
- `userId` (string): 用户ID
- `cartItemId` (string): 购物车项ID

**请求体**:
```json
{
  "quantity": 3
}
```

**响应示例**:
```json
{
  "code": 0,
  "message": "购物车更新成功",
  "data": {
    "cartItemId": "CART001",
    "quantity": 3,
    "subtotal": 80.4,
    "updatedAt": "2024-01-15T10:35:00Z"
  },
  "timestamp": 1678886400000
}
```

### 4. 删除购物车商品

**接口**: `DELETE /api/users/{userId}/cartitems/{cartItemId}`

**描述**: 从购物车中删除指定商品。

**路径参数**:
- `userId` (string): 用户ID
- `cartItemId` (string): 购物车项ID

**响应示例**:
```json
{
  "code": 0,
  "message": "商品已从购物车删除",
  "data": {
    "cartItemId": "CART001",
    "removedAt": "2024-01-15T10:40:00Z"
  },
  "timestamp": 1678886400000
}
```

### 5. 批量删除购物车商品

**接口**: `DELETE /api/users/{userId}/cartitems/batch`

**描述**: 批量删除购物车中的多个商品。

**路径参数**:
- `userId` (string): 用户ID

**请求体**:
```json
{
  "cartItemIds": ["CART001", "CART002", "CART003"]
}
```

**响应示例**:
```json
{
  "code": 0,
  "message": "批量删除成功",
  "data": {
    "deletedCount": 3,
    "deletedItems": ["CART001", "CART002", "CART003"],
    "removedAt": "2024-01-15T10:45:00Z"
  },
  "timestamp": 1678886400000
}
```

### 6. 清空购物车

**接口**: `DELETE /api/users/{userId}/cart`

**描述**: 清空用户的整个购物车。

**路径参数**:
- `userId` (string): 用户ID

**响应示例**:
```json
{
  "code": 0,
  "message": "购物车已清空",
  "data": {
    "clearedCount": 5,
    "clearedAt": "2024-01-15T10:50:00Z"
  },
  "timestamp": 1678886400000
}
```

### 7. 批量选择购物车商品

**接口**: `PATCH /api/users/{userId}/cartitems/select`

**描述**: 批量设置购物车商品的选中状态。

**路径参数**:
- `userId` (string): 用户ID

**请求体**:
```json
{
  "cartItemIds": ["CART001", "CART002"],
  "selected": true
}
```

**响应示例**:
```json
{
  "code": 0,
  "message": "选择状态更新成功",
  "data": {
    "updatedCount": 2,
    "selectedItems": ["CART001", "CART002"],
    "updatedAt": "2024-01-15T10:55:00Z"
  },
  "timestamp": 1678886400000
}
```

### 8. 获取购物车统计信息

**接口**: `GET /api/users/{userId}/cart/summary`

**描述**: 获取购物车的统计信息，用于页面显示或快速访问。

**路径参数**:
- `userId` (string): 用户ID

**响应示例**:
```json
{
  "code": 0,
  "message": "获取统计信息成功",
  "data": {
    "totalItems": 8,
    "availableItems": 7,
    "selectedItems": 5,
    "totalAmount": 156.5,
    "selectedAmount": 120.0,
    "merchantCount": 3,
    "estimatedDeliveryFee": 15.0,
    "lastUpdated": "2024-01-15T10:30:00Z"
  },
  "timestamp": 1678886400000
}
```

### 9. 移至收藏夹

**接口**: `POST /api/users/{userId}/cartitems/{cartItemId}/move-to-favorites`

**描述**: 将购物车中的商品移至收藏夹。

**路径参数**:
- `userId` (string): 用户ID
- `cartItemId` (string): 购物车项ID

**响应示例**:
```json
{
  "code": 0,
  "message": "已移至收藏夹",
  "data": {
    "cartItemId": "CART001",
    "favoriteId": "FAV001",
    "movedAt": "2024-01-15T11:00:00Z"
  },
  "timestamp": 1678886400000
}
```

## 错误代码

购物车模块使用以下错误代码（基于项目错误码体系）：

### 业务逻辑错误 (3xxx)

- **3001**: 商品已存在
- **3002**: 购物车项不存在
- **3003**: 商品库存不足
- **3004**: 商品状态不正确（已下架）
- **3005**: 购物车为空

### 参数错误 (1xxx)

- **1001**: 参数验证失败
- **1002**: 缺少必填参数
- **1003**: 数量必须大于0

### 系统错误 (5xxx)

- **5001**: 数据库连接失败
- **5002**: 服务内部错误

**错误响应示例**:
```json
{
  "code": 3003,
  "message": "商品'宫保鸡丁'库存不足，当前库存：2，请求数量：5",
  "data": {
    "dishId": "DISH001",
    "availableStock": 2,
    "requestedQuantity": 5
  },
  "timestamp": 1678886400000
}
```

## 使用示例

### JavaScript/Vue.js 示例

```javascript
import { cartAPI } from '@/api/cart.js'

// 获取购物车
const getCart = async (userId) => {
  try {
    const response = await cartAPI.getUserCart(userId)
    if (response.code === 0) {
      console.log('购物车数据:', response.data)
      return response.data
    } else {
      console.error('获取购物车失败:', response.message)
    }
  } catch (error) {
    console.error('网络错误:', error)
  }
}

// 添加到购物车
const addToCart = async (userId, dishData) => {
  try {
    const response = await cartAPI.addToCart(userId, {
      dishId: dishData.dishId,
      merchantId: dishData.merchantId,
      quantity: 1
    })
    
    if (response.code === 0) {
      console.log('添加成功:', response.data)
      return { success: true, data: response.data }
    } else {
      return { success: false, message: response.message }
    }
  } catch (error) {
    return { success: false, message: '添加失败，请重试' }
  }
}
```

## 注意事项

1. **认证**: 所有接口都需要用户认证，请在请求头中包含有效的 Bearer Token
2. **幂等性**: GET 请求是幂等的，多次调用结果相同
3. **并发**: 购物车支持并发操作，使用乐观锁机制处理冲突
4. **缓存**: 购物车数据会被缓存，建议客户端定期刷新
5. **限制**: 单个用户购物车最多支持100个不同商品
6. **商家限制**: 一次结算只能选择同一个商家的商品

## 相关模块

- **商品模块**: 获取商品详情和库存信息
- **用户模块**: 用户认证和权限管理
- **订单模块**: 购物车结算生成订单
- **收藏模块**: 购物车与收藏夹的互操作
