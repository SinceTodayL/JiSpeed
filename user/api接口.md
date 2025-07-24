---
title: 用户模块
language_tabs:
  - shell: Shell
  - http: HTTP
  - javascript: JavaScript
  - ruby: Ruby
  - python: Python
  - php: PHP
  - java: Java
  - go: Go
toc_footers: []
includes: []
search: true
code_clipboard: true
highlight_theme: darkula
headingLevel: 2
generator: "@tarslib/widdershins v4.0.30"

---

# 用户模块

Base URLs:

# Authentication

# Address Management/地址管理

## GET 获取用户地址列表

GET /api/addresses

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|query|string| 是 |用户ID|

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "获取地址列表成功",
  "data": {
    "addresses": [
      {
        "addressId": "addr_001",
        "userId": "USER123",
        "receiverName": "张三",
        "receiverPhone": "13800138001",
        "detailedAddress": "北京市朝阳区建国路1号国贸大厦A座1001室",
        "isDefault": 1,
        "label": "公司"
      }
    ],
    "total": 1
  },
  "timestamp": 1678886400000
}
```

## POST 添加新地址

POST /api/addresses

### 请求体

```json
{
  "userId": "USER123",
  "receiverName": "张三",
  "receiverPhone": "13800138001",
  "detailedAddress": "北京市朝阳区建国路1号国贸大厦A座1001室",
  "isDefault": 1,
  "label": "公司"
}
```

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "地址添加成功",
  "data": {
    "address": {
      "addressId": "addr_1234567890",
      "userId": "USER123",
      "receiverName": "张三",
      "receiverPhone": "13800138001",
      "detailedAddress": "北京市朝阳区建国路1号国贸大厦A座1001室",
      "isDefault": 1,
      "label": "公司",
      "createTime": "2024-01-01T10:00:00Z"
    }
  },
  "timestamp": 1678886400000
}
```

## PUT 更新地址

PUT /api/addresses/{addressId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|addressId|path|string| 是 |地址ID|

### 请求体

```json
{
  "userId": "USER123",
  "receiverName": "李四",
  "receiverPhone": "13800138002",
  "detailedAddress": "北京市海淀区中关村大街27号中关村广场B座2002室",
  "isDefault": 0,
  "label": "家"
}
```

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "地址更新成功",
  "data": {
    "address": {
      "addressId": "addr_001",
      "userId": "USER123",
      "receiverName": "李四",
      "receiverPhone": "13800138002",
      "detailedAddress": "北京市海淀区中关村大街27号中关村广场B座2002室",
      "isDefault": 0,
      "label": "家",
      "updateTime": "2024-01-15T15:30:00Z"
    }
  },
  "timestamp": 1678886400000
}
```

## DELETE 删除地址

DELETE /api/addresses/{addressId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|addressId|path|string| 是 |地址ID|

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "地址删除成功",
  "data": {
    "deletedAddressId": "addr_001"
  },
  "timestamp": 1678886400000
}
```

> 错误响应 - 不能删除默认地址

```json
{
  "code": 3001,
  "message": "不能删除默认地址，请先设置其他地址为默认地址",
  "data": null,
  "timestamp": 1678886400000
}
```

## PUT 设置默认地址

PUT /api/addresses/{addressId}/default

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|addressId|path|string| 是 |地址ID|

### 请求体

```json
{
  "userId": "USER123"
}
```

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "默认地址设置成功",
  "data": {
    "addressId": "addr_001",
    "userId": "USER123"
  },
  "timestamp": 1678886400000
}
```

## GET 获取地址详情

GET /api/addresses/{addressId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|addressId|path|string| 是 |地址ID|

> 返回示例

> 成功响应

```json
{
  "code": 200,
  "message": "获取地址详情成功",
  "data": {
    "address": {
      "addressId": "addr_001",
      "userId": "USER123",
      "receiverName": "张三",
      "receiverPhone": "13800138001",
      "detailedAddress": "北京市朝阳区建国路1号国贸大厦A座1001室",
      "isDefault": 1,
      "label": "公司",
      "createTime": "2024-01-01T10:00:00Z",
      "updateTime": "2024-01-15T15:30:00Z"
    }
  },
  "timestamp": 1678886400000
}
```

> 错误响应 - 地址不存在

```json
{
  "code": 50003,
  "message": "地址不存在",
  "data": null,
  "timestamp": 1678886400000
}
```

## POST 地址验证

POST /api/addresses/validate

验证地址是否在配送范围内

### 请求体

```json
{
  "detailedAddress": "北京市朝阳区建国路1号国贸大厦A座1001室"
}
```

> 返回示例

> 成功响应 - 地址验证通过

```json
{
  "code": 200,
  "message": "地址验证通过",
  "data": {
    "isValid": true,
    "deliveryFee": 5.0,
    "estimatedTime": "30-45分钟"
  },
  "timestamp": 1678886400000
}
```

> 错误响应 - 地址不在配送范围

```json
{
  "code": 30003,
  "message": "该地址暂不支持配送",
  "data": {
    "address": "远郊区某地",
    "reason": "超出配送范围"
  },
  "timestamp": 1678886400000
}
```

# users/Get

## GET 根据id获取用户信息

GET /users/{userId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 200,
  "message": null,
  "data": {
    "userId": "6",
    "account": "123456",
    "status": 1,
    "createdAt": "2025-01-25 07:41:33",
    "deletedAt": "2025-01-28 10:18:40",
    "lastLoginAt": "2025-01-25 07:41:33",
    "lastLoginIp": "186.130.10.208",
    "nickName": "茆熙成",
    "avatarUrl": "https://avatars.githubusercontent.com/u/23182169",
    "gender": 1,
    "birthday": "2021-06-19",
    "level": 0,
    "defaultAddrId": "72"
  }
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string¦null|true|none||none|
|» data|[userProfiles](#schemauserprofiles)|true|none||none|
|»» userId|string|true|none||none|
|»» account|string|true|none||none|
|»» status|integer|true|none||none|
|»» createdAt|string|true|none||none|
|»» deletedAt|string|true|none||none|
|»» lastLoginAt|string|true|none||none|
|»» lastLoginIp|string|true|none||none|
|»» nickName|string|true|none||none|
|»» avatarUrl|string|true|none||none|
|»» gender|integer|true|none||none|
|»» birthday|string|true|none||none|
|»» level|integer|true|none||none|
|»» defaultAddrId|string|true|none||none|

## GET 根据id获取用户收藏的商品列表

GET /users/{userId}/favorites

获取指定 userId 用户收藏的商品列表

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "dishId": "string",
      "favorAt": "string"
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[[userFavorite](#schemauserfavorite)]|true|none||none|
|»» dishId|string|true|none||none|
|»» favorAt|string|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

## GET 根据id获取用户的所有地址列表

GET /users/{userId}/addresses

表示获取指定 userId 用户的所有地址列表。

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "addressId": "string",
      "receiverName": "string",
      "receiverPhone": "string",
      "detailedAddress": "string",
      "isDefault": 0
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[[userAddress](#schemauseraddress)]|true|none||none|
|»» addressId|string|true|none||none|
|»» receiverName|string|true|none||none|
|»» receiverPhone|string|true|none||none|
|»» detailedAddress|string|true|none||none|
|»» isDefault|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

## GET 根据id获取用户的购物车内容

GET /users/{userId}/cart

获取指定 userId 用户的购物车内容。

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "dishId": "string",
      "cartId": "string",
      "addedAt": "string",
      "userId": "string"
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[[userCartitem](#schemausercartitem)]|true|none||none|
|»» dishId|string|true|none||none|
|»» cartId|string|true|none||none|
|»» addedAt|string|true|none||none|
|»» userId|string|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

## GET 根据id获取用户发布的所有评论

GET /users/{userId}/review

表示获取指定 userId 用户发布的所有评论。

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "reviewId": "string",
      "orderId": "string",
      "dishId": "string",
      "userId": "string",
      "rating": 0,
      "content": "string",
      "isAnonymous": 0,
      "reviewAt": "string"
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[[userReview](#schemauserreview)]|true|none||none|
|»» reviewId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» dishId|string|true|none||none|
|»» userId|string|true|none||none|
|»» rating|integer|true|none||none|
|»» content|string|true|none||none|
|»» isAnonymous|integer|true|none||none|
|»» reviewAt|string|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

## GET 根据id获取用户提交的所有投诉

GET /users/{userId}/complaints

表示获取指定 userId 用户提交的所有投诉。

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "complaintId": "string",
      "orderId": "string",
      "complainantId": "string",
      "role": 0,
      "description": "string",
      "status": "string",
      "createdAt": "string"
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[[userComplaint](#schemausercomplaint)]|true|none||none|
|»» complaintId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» complainantId|string|true|none||none|
|»» role|integer|true|none||none|
|»» description|string|true|none||none|
|»» status|string|true|none||none|
|»» createdAt|string|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

# users/Post

## POST 用户添加收藏夹

POST /users/{userId}/favorites

> Body 请求参数

```json
{
  "dishId": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» dishId|body|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## POST 用户添加收货地址

POST /users/{userId}/addresses

> Body 请求参数

```json
{
  "receiverName": "string",
  "receiverPhone": "string",
  "detailedAddress": "string",
  "isDefault": 0
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» receiverName|body|string| 是 |none|
|» receiverPhone|body|string| 是 |none|
|» detailedAddress|body|string| 是 |none|
|» isDefault|body|integer| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none|收货地址添加成功|none|
|» data|object|true|none||none|

## POST 用户添加投诉

POST /users/{userId}/complaints

> Body 请求参数

```json
{
  "orderId": "string",
  "description": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» orderId|body|string| 是 |none|
|» description|body|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## POST 用户注册

POST /users/register

> Body 请求参数

```json
{
  "account": "string",
  "nickName": "string",
  "gender": 0,
  "birthday": "string",
  "password": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|body|body|object| 否 |none|
|» account|body|string| 是 |none|
|» nickName|body|string| 是 |none|
|» gender|body|integer| 是 |none|
|» birthday|body|string| 是 |none|
|» password|body|string| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {
    "userId": "string",
    "account": "string",
    "status": 0,
    "createdAt": "string",
    "deletedAt": "string",
    "lastLoginAt": "string",
    "lastLoginIp": "string",
    "nickName": "string",
    "avatarUrl": "string",
    "gender": 0,
    "birthday": "string",
    "level": 0,
    "defaultAddrId": "string"
  }
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none|200|none|
|» message|string|true|none|注册成功|none|
|» data|[userProfiles](#schemauserprofiles)|true|none||none|
|»» userId|string|true|none||none|
|»» account|string|true|none||none|
|»» status|integer|true|none||none|
|»» createdAt|string|true|none||none|
|»» deletedAt|string|true|none||none|
|»» lastLoginAt|string|true|none||none|
|»» lastLoginIp|string|true|none||none|
|»» nickName|string|true|none||none|
|»» avatarUrl|string|true|none||none|
|»» gender|integer|true|none||none|
|»» birthday|string|true|none||none|
|»» level|integer|true|none||none|
|»» defaultAddrId|string|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|false|none||none|

## POST 用户登录

POST /users/login

> Body 请求参数

```json
{
  "account": "string",
  "password": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|body|body|object| 否 |none|
|» account|body|string| 是 |none|
|» password|body|string| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {
    "userId": "string",
    "account": "string",
    "status": 0,
    "createdAt": "string",
    "deletedAt": "string",
    "lastLoginAt": "string",
    "lastLoginIp": "string",
    "nickName": "string",
    "avatarUrl": "string",
    "gender": 0,
    "birthday": "string",
    "level": 0,
    "defaultAddrId": "string"
  }
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none|0|none|
|» message|string|true|none|登录成功|none|
|» data|object|true|none||none|
|»» userId|string|true|none||none|
|»» account|string|true|none||none|
|»» status|integer|true|none||none|
|»» createdAt|string|true|none||none|
|»» deletedAt|string|true|none||none|
|»» lastLoginAt|string|true|none||none|
|»» lastLoginIp|string|true|none||none|
|»» nickName|string|true|none||none|
|»» avatarUrl|string|true|none||none|
|»» gender|integer|true|none||none|
|»» birthday|string|true|none||none|
|»» level|integer|true|none||none|
|»» defaultAddrId|string|true|none||none|

## POST 用户添加商品到购物车

POST /users/{userId}/cartitems

> Body 请求参数

```json
{
  "dishId": "string",
  "userId": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» dishId|body|string| 是 |none|
|» userId|body|string| 是 |none|

> 返回示例

> 0 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|

### 返回数据结构

## POST 用户提交评论

POST /users/{userId}/comments

> Body 请求参数

```json
{
  "orderId": "string",
  "dishId": "string",
  "rating": 0,
  "content": "string",
  "isAnonymous": 0
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» orderId|body|string| 是 |none|
|» dishId|body|string| 是 |none|
|» rating|body|integer| 是 |none|
|» content|body|string| 是 |none|
|» isAnonymous|body|integer| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string"
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none|0|none|
|» message|string|true|none|提交评论成功|none|

## POST 用户重置密码

POST /users/{userId}/password/reset

> Body 请求参数

```json
{
  "oldPassword": "string",
  "newPassword": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|object| 否 |none|
|» oldPassword|body|string| 是 |none|
|» newPassword|body|string| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string"
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none|0|none|
|» message|string|true|none|重置密码成功|none|

# users/Patch

## PATCH 根据id部分修改用户信息

PATCH /user/{userId}

> Body 请求参数

```
nickname: 乐乐小超人
avatarUrl: https://example.com/avatars/lele_new.jpg
gender: "0"
birthday: 2005-03-22Z
defaultAddrId: "222"

```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|body|body|string| 否 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|

## PATCH 根据id更新地址

PATCH /users/{userId}/addresses/{addressId}

> Body 请求参数

```json
{
  "receiverName": "独角兽",
  "receiverPhone": "18121958888",
  "detailedAddress": "同济大学",
  "isDefault": true
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|addressId|path|string| 是 |none|
|body|body|string| 否 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## PATCH 根据id修改评论

PATCH /users/{userId}/reviews/{reviewId}

> Body 请求参数

```
rating: 5
content: 好好好
isAnonymous: 1

```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|reviewId|path|string| 是 |none|
|body|body|string| 否 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## PATCH 根据id修改投诉

PATCH /users/{userId}/complaints/{complaintId}

> Body 请求参数

```
"{\r\n    description: \"退钱\"\r\n}"

```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|complaintId|path|string| 是 |none|
|body|body|string| 否 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

# users/Delete

## DELETE 根据id部分删除用户信息

DELETE /users/{userId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|

> 返回示例

> 0 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|0|Unknown|none|Inline|

### 返回数据结构

状态码 **0**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|

## DELETE 根据id删除地址

DELETE /users/{userId}/addresses/{addressId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|addressId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|

## DELETE 根据id删除评论

DELETE /users/{userId}/reviews/{reviewId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|reviewId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## DELETE 根据id删除投诉

DELETE /users/{userId}/complaints/{complaintId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|complaintId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## DELETE 根据id删除购物车内容

DELETE /users/{userId}/carts/{cartId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|cartId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

## DELETE 根据id删除收藏内容

DELETE /users/{userId}/favorites/{favortiteId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|userId|path|string| 是 |none|
|favortiteId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

# riders/Get

## GET 根据id获取骑手信息

GET /api/riders/{riderId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |骑手ID|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "riderId": "string",
    "name": "string",
    "phoneNumber": "string",
    "vehicleNumber": "string",
    "applicationUserId": "string"
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» riderId|string|true|none||none|
|»» name|string|true|none||none|
|»» phoneNumber|string|true|none||none|
|»» vehicleNumber|string|true|none||none|
|»» applicationUserId|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据id获取骑手订单列表

GET /api/riders/{riderId}/assignments

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |骑手ID|
|status|query|integer| 否 |接单状态（0:未处理, 1:已接单, 2:已拒单, 3:已完成）|

#### 枚举值

|属性|值|
|---|---|
|status|0|
|status|1|
|status|2|
|status|3|

> 返回示例

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "assignId": "assign123",
      "assignedAt": "2025-07-10T14:30:00",
      "acceptedStatus": 1,
      "acceptedAt": "2025-07-10T14:35:00",
      "timeOut": 30,
      "order": {
        "orderId": "order123",
        "orderAmount": 35.5,
        "createAt": "2025-07-10T14:25:00",
        "orderStatus": 2,
        "deliveryAddress": "上海市杨浦区同济大学",
        "merchantName": "测试商家"
      }
    },
    {
      "assignId": "assign456",
      "assignedAt": "2025-07-10T13:30:00",
      "acceptedStatus": 2,
      "acceptedAt": "2025-07-10T13:35:00",
      "timeOut": null,
      "order": {
        "orderId": "order456",
        "orderAmount": 42,
        "createAt": "2025-07-10T13:25:00",
        "orderStatus": 4,
        "deliveryAddress": "上海市杨浦区五角场",
        "merchantName": "另一家测试商家"
      }
    },
    {
      "assignId": "assign789",
      "assignedAt": "2025-07-10T15:30:00",
      "acceptedStatus": 0,
      "acceptedAt": null,
      "timeOut": null,
      "order": {
        "orderId": "order789",
        "orderAmount": 28.5,
        "createAt": "2025-07-10T15:25:00",
        "orderStatus": 1,
        "deliveryAddress": "上海市杨浦区国定路",
        "merchantName": "第三家测试商家"
      }
    }
  ],
  "timestamp": 1678886400000
}
```

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "assignId": "assign123",
      "assignedAt": "2025-07-10T14:30:00",
      "acceptedStatus": 1,
      "acceptedAt": "2025-07-10T14:35:00",
      "timeOut": 30,
      "order": {
        "orderId": "order123",
        "orderAmount": 35.5,
        "createAt": "2025-07-10T14:25:00",
        "orderStatus": 2,
        "deliveryAddress": "上海市杨浦区同济大学",
        "merchantName": "测试商家"
      }
    }
  ],
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» assignId|string|false|none||none|
|»» riderId|string|false|none||none|
|»» assignedAt|string|false|none||none|
|»» acceptedStatus|integer|false|none||none|
|»» acceptedAt|string|false|none||none|
|»» timeOut|integer|false|none||none|
|»» order|object|false|none||none|
|»»» orderId|string|true|none||none|
|»»» orderAmount|number|true|none||none|
|»»» createAt|string|true|none||none|
|»»» orderStatus|integer|true|none||none|
|»»» address|object|true|none||none|
|»»»» addressId|string|true|none||none|
|»»»» detailedAddress|string|true|none||none|
|»»»» receiverName|string|true|none||none|
|»»»» receiverPhone|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据id和时间获取骑手特定月份的绩效详情

GET /api/riders/{riderId}/performances/{year}/{month}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|year|path|integer| 是 |none|
|month|path|integer| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "riderId": "string",
    "statsMonth": "2023-07-01T00:00:00",
    "totalOrders": 120,
    "onTimeRate": 95.5,
    "goodReviewRate": 92.3,
    "badReviewRate": 2.1,
    "income": 3500
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» riderId|string|true|none||none|
|»» statsMonth|string|true|none||none|
|»» totalOrders|integer|true|none||none|
|»» onTimeRate|number|true|none||none|
|»» goodReviewRate|number|true|none||none|
|»» badReviewRate|number|true|none||none|
|»» income|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据id获取骑手考勤记录

GET /api/riders/{riderId}/attendances

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|startDate|query|string| 否 |开始日期（可选）|
|endDate|query|string| 否 |结束日期（可选）|
|isLate|query|integer| 否 |是否迟到筛选（可选）|
|isAbsent|query|integer| 否 |是否缺勤筛选（可选）|

#### 枚举值

|属性|值|
|---|---|
|isLate|0|
|isLate|1|
|isAbsent|0|
|isAbsent|1|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "attendanceId": "string",
      "riderId": "string",
      "checkDate": "2023-07-10",
      "checkInAt": "2023-07-10T08:30:00",
      "checkoutAt": "2023-07-10T17:30:00",
      "isLate": 0,
      "isAbsent": 0
    }
  ],
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» attendanceId|string|false|none||none|
|»» riderId|string|false|none||none|
|»» checkDate|string|false|none||none|
|»» checkInAt|string|false|none||none|
|»» checkoutAt|string|false|none||none|
|»» isLate|integer|false|none||none|
|»» isAbsent|integer|false|none||none|
|» timestamp|integer|true|none||none|

## GET 根据id获取骑手排班列表

GET /api/riders/{riderId}/schedules

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|startDate|query|string| 否 |开始日期（可选）|
|endDate|query|string| 否 |结束日期（可选）|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "scheduleId": "string",
      "workDate": "2025-07-10",
      "shiftStart": "2025-07-10T08:00:00",
      "shiftEnd": "2025-07-10T18:00:00"
    }
  ],
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» scheduleId|string|false|none||none|
|»» workDate|string|false|none||none|
|»» shiftStart|string|false|none||none|
|»» shiftEnd|string|false|none||none|
|» timestamp|integer|true|none||none|

## GET 根据骑手和订单id获取特定订单分配的详情

GET /api/riders/{riderId}/assignments/{assignId}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|assignId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "assignId": "string",
    "riderId": "string",
    "assignedAt": "2025-07-10T14:30:00",
    "acceptedStatus": 1,
    "acceptedAt": "2025-07-10T14:35:00",
    "timeOut": 30,
    "order": {
      "orderId": "string",
      "userId": "string",
      "addressId": "string",
      "orderAmount": 35.5,
      "createAt": "2025-07-10T14:25:00",
      "orderStatus": 2,
      "address": {
        "addressId": "string",
        "detailedAddress": "string",
        "receiverName": "string",
        "receiverPhone": "string"
      }
    }
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|string|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» assignId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» assignedAt|string|true|none||none|
|»» acceptedStatus|integer|true|none||none|
|»» acceptedAt|string|true|none||none|
|»» timeOut|integer|true|none||none|
|»» order|object|true|none||none|
|»»» orderId|string|true|none||none|
|»»» userId|string|true|none||none|
|»»» addressId|string|true|none||none|
|»»» orderAmount|number|true|none||none|
|»»» createAt|string|true|none||none|
|»»» orderStatus|integer|true|none||none|
|»»» address|object|true|none||none|
|»»»» addressId|string|true|none||none|
|»»»» detailedAddress|string|true|none||none|
|»»»» receiverName|string|true|none||none|
|»»»» receiverPhone|string|true|none||none|
|» timestamp|integer|true|none||none|

# riders/Post

## POST 创建考勤记录（签到）

POST /api/riders/{riderId}/attendances

> Body 请求参数

```json
{
  "checkDate": "2025-07-10",
  "checkInAt": "2025-07-10T08:30:00",
  "isLate": 0,
  "isAbsent": 0
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|body|body|object| 否 |none|
|» checkDate|body|string| 是 |none|
|» checkInAt|body|string| 是 |none|
|» isLate|body|integer| 是 |none|
|» isAbsent|body|integer| 是 |none|

#### 枚举值

|属性|值|
|---|---|
|» isLate|0|
|» isLate|1|
|» isAbsent|0|
|» isAbsent|1|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "签到成功",
  "data": {
    "attendanceId": "string",
    "riderId": "string",
    "checkDate": "2025-07-10",
    "checkInAt": "2025-07-10T08:30:00",
    "checkoutAt": null,
    "isLate": 0,
    "isAbsent": 0
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» attendanceId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» checkDate|string|true|none||none|
|»» checkInAt|string|true|none||none|
|»» checkoutAt|null|true|none||none|
|»» isLate|integer|true|none||none|
|»» isAbsent|integer|true|none||none|
|» timestamp|integer|true|none||none|

## POST 骑手注册

POST /api/riders

> Body 请求参数

```json
{
  "name": "string",
  "phoneNumber": "string",
  "vehicleNumber": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|body|body|object| 否 |none|
|» name|body|string| 是 |none|
|» phoneNumber|body|string| 是 |none|
|» vehicleNumber|body|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "骑手创建成功",
  "data": {
    "riderId": "string",
    "name": "string",
    "phoneNumber": "string",
    "vehicleNumber": "string",
    "applicationUserId": "string"
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» riderId|string|true|none||none|
|»» name|string|true|none||none|
|»» phoneNumber|string|true|none||none|
|»» vehicleNumber|string|true|none||none|
|» timestamp|integer|true|none||none|

# riders/Patch

## PATCH 更新骑手信息

PATCH /api/riders/{riderId}

> Body 请求参数

```json
{
  "name": "string",
  "phoneNumber": "string",
  "vehicleNumber": "string"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|body|body|object| 否 |none|
|» name|body|string| 是 |none|
|» phoneNumber|body|string| 是 |none|
|» vehicleNumber|body|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "骑手信息更新成功",
  "data": {
    "riderId": "string",
    "name": "string",
    "phoneNumber": "string",
    "vehicleNumber": "string",
    "applicationUserId": "string"
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» riderId|string|true|none||none|
|»» name|string|true|none||none|
|»» phoneNumber|string|true|none||none|
|»» vehicleNumber|string|true|none||none|
|»» applicationUserId|string|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 更新订单分配状态

PATCH /api/riders/{riderId}/assignments/{assignId}

> Body 请求参数

```json
{
  "acceptedStatus": 1
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|assignId|path|string| 是 |none|
|body|body|object| 否 |none|
|» acceptedStatus|body|integer| 是 |none|

#### 枚举值

|属性|值|
|---|---|
|» acceptedStatus|0|
|» acceptedStatus|1|
|» acceptedStatus|2|
|» acceptedStatus|3|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "接单成功",
  "data": {
    "assignId": "string",
    "riderId": "string",
    "assignedAt": "2025-07-10T14:30:00",
    "acceptedStatus": 1,
    "acceptedAt": "2025-07-10T15:00:00",
    "timeOut": null
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» assignId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» assignedAt|string|true|none||none|
|»» acceptedStatus|integer|true|none||none|
|»» acceptedAt|string|true|none||none|
|»» timeOut|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 更新考勤记录（签退）

PATCH /api/riders/{riderId}/attendances/{attendanceId}

> Body 请求参数

```json
{
  "checkoutAt": "2025-07-10T17:30:00"
}
```

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|attendanceId|path|string| 是 |none|
|body|body|object| 否 |none|
|» checkoutAt|body|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "签退成功",
  "data": {
    "attendanceId": "string",
    "riderId": "string",
    "checkDate": "2025-07-10",
    "checkInAt": "2025-07-10T08:30:00",
    "checkoutAt": "2025-07-10T17:30:00",
    "isLate": 0,
    "isAbsent": 0
  },
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» attendanceId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» checkDate|string|true|none||none|
|»» checkInAt|string|true|none||none|
|»» checkoutAt|string|true|none||none|
|»» isLate|integer|true|none||none|
|»» isAbsent|integer|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/merchant

## GET 根据商家 ID 获取所有菜品信息

GET /merchant/{merchantID}/getAllDishes

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|merchantID|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "dishId": "DISH1234567890001",
      "categoryId": "CAT10001",
      "dishName": "宫保鸡丁",
      "price": 28.5,
      "originPrice": 35,
      "coverUrl": "https://example.com/images/dish1.jpg",
      "monthlySales": 120,
      "rating": 95.5,
      "onSale": 1,
      "merchantId": "MER1234567890001",
      "quantity": 58
    },
    {
      "dishId": "DISH1234567890002",
      "categoryId": "CAT10002",
      "dishName": "麻婆豆腐",
      "price": 18,
      "originPrice": 22,
      "coverUrl": "https://example.com/images/dish2.jpg",
      "monthlySales": 85,
      "rating": 93,
      "onSale": 1,
      "merchantId": "MER1234567890001",
      "quantity": 33
    },
    {
      "dishId": "DISH1234567890003",
      "categoryId": "CAT10003",
      "dishName": "酸辣土豆丝",
      "price": 12,
      "originPrice": 15,
      "coverUrl": "https://example.com/images/dish3.jpg",
      "monthlySales": 140,
      "rating": 97,
      "onSale": 1,
      "merchantId": "MER1234567890001",
      "quantity": 76
    }
  ],
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer(int32)|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» dishId|string|true|none||none|
|»» categoryId|string|true|none||none|
|»» dishName|string|true|none||none|
|»» price|number|true|none||none|
|»» originPrice|integer|true|none||none|
|»» coverUrl|string|true|none||none|
|»» monthlySales|integer|true|none||none|
|»» rating|number|true|none||none|
|»» onSale|integer(int32)|true|none||none|
|»» merchantId|string|true|none||none|
|»» quantity|integer|true|none||none|

## GET 根据 ID 获取商家信息

GET /merchant/{merchantID}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|merchantID|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "merchantId": "MER1234567890001",
    "merchantName": "老王家川菜馆",
    "status": 1,
    "contactInfo": "13800001111",
    "location": "北京市朝阳区建国路88号"
  }
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer(int32)|true|none||none|
|» message|string|true|none||none|
|» data|object|true|none||none|
|»» merchantId|string|true|none||none|
|»» merchantName|string(hostname)|true|none||none|
|»» status|integer|true|none||none|
|»» contactInfo|string(email)|true|none||none|
|»» location|string|true|none||none|

## GET 根据 ID 获取商家店铺数据统计信息

GET /merchant/{merchantID}/SalesStat

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|merchantID|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "statDate": "2025-07-20T00:00:00",
      "salesQty": 150,
      "salesAmount": 4320.5,
      "merchantId": "MER1234567890001"
    },
    {
      "statDate": "2025-07-21T00:00:00",
      "salesQty": 132,
      "salesAmount": 3895.2,
      "merchantId": "MER1234567890001"
    },
    {
      "statDate": "2025-07-22T00:00:00",
      "salesQty": 178,
      "salesAmount": 5128,
      "merchantId": "MER1234567890001"
    }
  ],
  "timestamp": 1678886400000
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer(int32)|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» statDate|string(date)|true|none||none|
|»» salesQty|integer|true|none||none|
|»» salesAmount|number|true|none||none|
|»» merchantId|string|true|none||none|

## GET 根据商家 ID，优惠券 ID 获取优惠券信息

GET /merchant/{merchantID}/coupon/{couponID}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|merchantID|path|string| 是 |none|
|couponID|path|string| 是 |none|

> 返回示例

> 200 Response

```json
"  \"code\": 0,\r\n  \"message\": \"操作成功\",\r\n  \"data\": {\r\n    \"couponId\": \"COUPON202507220001\",\r\n    \"faceValue\": 20.00,\r\n    \"threshold\": 100.00,\r\n    \"totalQty\": 500,\r\n    \"issuedQty\": 120,\r\n    \"startTime\": \"2025-07-22T00:00:00\",\r\n    \"endTime\": \"2025-08-31T23:59:59\"\r\n  },\r\n}"
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» couponId|string|true|none||none|
|» faceValue|integer(int32)|true|none||none|
|» threshold|integer(int32)|true|none||none|
|» totalQty|integer|true|none||none|
|» issuedQty|integer|true|none||none|
|» startTime|string(date-time)|true|none||none|
|» endTime|string(date-time)|true|none||none|

## GET 根据商家 ID 获取所有优惠券

GET /merchant/{merchantID}/coupon

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|merchantID|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "couponId": "string",
      "faceValue": 20,
      "threshold": 100,
      "totalQty": 0,
      "issuedQty": 0,
      "startTime": "2019-08-24T14:15:22Z",
      "endTime": "2019-08-24T14:15:22Z"
    },
    {
      "couponId": "string",
      "faceValue": 20,
      "threshold": 100,
      "totalQty": 0,
      "issuedQty": 0,
      "startTime": "2019-08-24T14:15:22Z",
      "endTime": "2019-08-24T14:15:22Z"
    },
    {
      "couponId": "string",
      "faceValue": 20,
      "threshold": 100,
      "totalQty": 0,
      "issuedQty": 0,
      "startTime": "2019-08-24T14:15:22Z",
      "endTime": "2019-08-24T14:15:22Z"
    },
    {
      "couponId": "string",
      "faceValue": 20,
      "threshold": 100,
      "totalQty": 0,
      "issuedQty": 0,
      "startTime": "2019-08-24T14:15:22Z",
      "endTime": "2019-08-24T14:15:22Z"
    },
    {
      "couponId": "string",
      "faceValue": 20,
      "threshold": 100,
      "totalQty": 0,
      "issuedQty": 0,
      "startTime": "2019-08-24T14:15:22Z",
      "endTime": "2019-08-24T14:15:22Z"
    }
  ]
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[object]|true|none||none|
|»» couponId|string|true|none||none|
|»» faceValue|integer(int32)|true|none||none|
|»» threshold|integer(int32)|true|none||none|
|»» totalQty|integer|true|none||none|
|»» issuedQty|integer|true|none||none|
|»» startTime|string(date-time)|true|none||none|
|»» endTime|string(date-time)|true|none||none|

# 数据模型

<h2 id="tocS_userProfiles">userProfiles</h2>

<a id="schemauserprofiles"></a>
<a id="schema_userProfiles"></a>
<a id="tocSuserprofiles"></a>
<a id="tocsuserprofiles"></a>

```json
{
  "userId": "string",
  "account": "string",
  "status": 0,
  "createdAt": "string",
  "deletedAt": "string",
  "lastLoginAt": "string",
  "lastLoginIp": "string",
  "nickName": "string",
  "avatarUrl": "string",
  "gender": 0,
  "birthday": "string",
  "level": 0,
  "defaultAddrId": "string"
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|userId|string|true|none||none|
|account|string|true|none||none|
|status|integer|true|none||none|
|createdAt|string|true|none||none|
|deletedAt|string|true|none||none|
|lastLoginAt|string|true|none||none|
|lastLoginIp|string|true|none||none|
|nickName|string|true|none||none|
|avatarUrl|string|true|none||none|
|gender|integer|true|none||none|
|birthday|string|true|none||none|
|level|integer|true|none||none|
|defaultAddrId|string|true|none||none|

<h2 id="tocS_userAddress">userAddress</h2>

<a id="schemauseraddress"></a>
<a id="schema_userAddress"></a>
<a id="tocSuseraddress"></a>
<a id="tocsuseraddress"></a>

```json
{
  "addressId": "string",
  "receiverName": "string",
  "receiverPhone": "string",
  "detailedAddress": "string",
  "isDefault": 0
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|addressId|string|true|none||none|
|receiverName|string|true|none||none|
|receiverPhone|string|true|none||none|
|detailedAddress|string|true|none||none|
|isDefault|integer|true|none||none|

<h2 id="tocS_userFavorite">userFavorite</h2>

<a id="schemauserfavorite"></a>
<a id="schema_userFavorite"></a>
<a id="tocSuserfavorite"></a>
<a id="tocsuserfavorite"></a>

```json
{
  "dishId": "string",
  "favorAt": "string"
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|dishId|string|true|none||none|
|favorAt|string|true|none||none|

<h2 id="tocS_userCartitem">userCartitem</h2>

<a id="schemausercartitem"></a>
<a id="schema_userCartitem"></a>
<a id="tocSusercartitem"></a>
<a id="tocsusercartitem"></a>

```json
{
  "dishId": "string",
  "cartId": "string",
  "addedAt": "string",
  "userId": "string"
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|dishId|string|true|none||none|
|cartId|string|true|none||none|
|addedAt|string|true|none||none|
|userId|string|true|none||none|

<h2 id="tocS_userComplaint">userComplaint</h2>

<a id="schemausercomplaint"></a>
<a id="schema_userComplaint"></a>
<a id="tocSusercomplaint"></a>
<a id="tocsusercomplaint"></a>

```json
{
  "complaintId": "string",
  "orderId": "string",
  "complainantId": "string",
  "role": 0,
  "description": "string",
  "status": "string",
  "createdAt": "string"
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|complaintId|string|true|none||none|
|orderId|string|true|none||none|
|complainantId|string|true|none||none|
|role|integer|true|none||none|
|description|string|true|none||none|
|status|string|true|none||none|
|createdAt|string|true|none||none|

<h2 id="tocS_userReview">userReview</h2>

<a id="schemauserreview"></a>
<a id="schema_userReview"></a>
<a id="tocSuserreview"></a>
<a id="tocsuserreview"></a>

```json
{
  "reviewId": "string",
  "orderId": "string",
  "dishId": "string",
  "userId": "string",
  "rating": 0,
  "content": "string",
  "isAnonymous": 0,
  "reviewAt": "string"
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|reviewId|string|true|none||none|
|orderId|string|true|none||none|
|dishId|string|true|none||none|
|userId|string|true|none||none|
|rating|integer|true|none||none|
|content|string|true|none||none|
|isAnonymous|integer|true|none||none|
|reviewAt|string|true|none||none|

<h2 id="tocS_CommonResponse">CommonResponse</h2>

<a id="schemacommonresponse"></a>
<a id="schema_CommonResponse"></a>
<a id="tocScommonresponse"></a>
<a id="tocscommonresponse"></a>

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|code|integer|true|none||none|
|message|string|true|none||none|
|data|object|true|none||none|

