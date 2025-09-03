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

- HTTP Authentication, scheme: bearer

# users/Get

## GET 获取用户信息列表

GET /users

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|page|query|string| 否 |none|
|size|query|string| 否 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "userId": "1d804350373d47b2b66feb88f8403e7d",
      "nickname": "testuser002",
      "avatarUrl": "default.jpg",
      "gender": 3,
      "birthday": null,
      "level": 0,
      "userName": null,
      "email": null,
      "phoneNumber": null,
      "defaultAddress": null,
      "stats": {
        "totalOrders": 0,
        "favoriteCount": 0,
        "cartItemCount": 0,
        "availableCouponCount": 0,
        "addressCount": 0
      }
    },
    {
      "userId": "184268d41d9f446d9ed7d88ccf10a64a",
      "nickname": "testuser001",
      "avatarUrl": "default.jpg",
      "gender": 3,
      "birthday": null,
      "level": 0,
      "userName": null,
      "email": null,
      "phoneNumber": null,
      "defaultAddress": null,
      "stats": {
        "totalOrders": 0,
        "favoriteCount": 0,
        "cartItemCount": 0,
        "availableCouponCount": 0,
        "addressCount": 0
      }
    },
    {
      "userId": "CU20240601001",
      "nickname": "测试用户",
      "avatarUrl": "http://example.com/avatar.png",
      "gender": 1,
      "birthday": null,
      "level": 1,
      "userName": null,
      "email": null,
      "phoneNumber": null,
      "defaultAddress": null,
      "stats": {
        "totalOrders": 0,
        "favoriteCount": 0,
        "cartItemCount": 0,
        "availableCouponCount": 0,
        "addressCount": 0
      }
    },
    {
      "userId": "819524ce084149aa9883193b0a068c0f",
      "nickname": "test03",
      "avatarUrl": "default.jpg",
      "gender": 3,
      "birthday": null,
      "level": 0,
      "userName": null,
      "email": null,
      "phoneNumber": null,
      "defaultAddress": null,
      "stats": {
        "totalOrders": 0,
        "favoriteCount": 0,
        "cartItemCount": 0,
        "availableCouponCount": 0,
        "addressCount": 0
      }
    },
    {
      "userId": "c5b370a1af08464ca7243cee690509ac",
      "nickname": "ZHENG",
      "avatarUrl": "default.jpg",
      "gender": 3,
      "birthday": null,
      "level": 0,
      "userName": null,
      "email": null,
      "phoneNumber": null,
      "defaultAddress": null,
      "stats": {
        "totalOrders": 0,
        "favoriteCount": 0,
        "cartItemCount": 0,
        "availableCouponCount": 0,
        "addressCount": 0
      }
    }
  ],
  "timestamp": 1755070526286
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|

### 返回数据结构

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
|size|query|string| 否 |none|
|page|query|string| 否 |none|

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
|size|query|string| 否 |none|
|page|query|string| 否 |none|

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
|size|query|string| 否 |none|
|page|query|string| 否 |none|

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
|size|query|string| 否 |none|
|page|query|string| 否 |none|

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
|size|query|string| 否 |none|
|page|query|string| 否 |none|

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

## GET 获取骑手最新位置

GET /GET /api/riderlocations/{riderId}/latest

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": {
    "locationId": "string",
    "riderId": "string",
    "longitude": 116.407526,
    "latitude": 39.9042,
    "locationTime": "2025-01-20T14:30:25Z",
    "accuracy": 10.5,
    "speed": 25.3,
    "direction": 180,
    "locationType": "GPS",
    "status": 1
  },
  "timestamp": 1678864000000
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
|»» locationId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» longitude|number|true|none||none|
|»» latitude|number|true|none||none|
|»» locationTime|string|true|none||none|
|»» accuracy|number|true|none||none|
|»» speed|number|true|none||none|
|»» direction|integer|true|none||none|
|»» locationType|string|true|none||none|
|»» status|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取骑手历史轨迹

GET /GET /api/riderlocations/{riderId}/history

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|startTime|query|string| 否 |none|
|endTime|query|string| 否 |none|
|pageIndex|query|integer| 否 |页码|
|pageSize|query|integer| 否 |每页数量|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": [
    {
      "locationId": "string",
      "riderId": "string",
      "longitude": 116.407526,
      "latitude": 39.9042,
      "locationTime": "2025-01-20T14:30:25Z",
      "accuracy": 10.5,
      "speed": 25.3,
      "direction": 180,
      "locationType": "GPS",
      "status": 1
    },
    {
      "locationId": "string2",
      "riderId": "string",
      "longitude": 116.408526,
      "latitude": 39.9052,
      "locationTime": "2025-01-20T14:31:25Z",
      "accuracy": 12,
      "speed": 28.5,
      "direction": 185,
      "locationType": "GPS",
      "status": 1
    }
  ],
  "timestamp": 1678864000000
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
|»» locationId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» longitude|number|true|none||none|
|»» latitude|number|true|none||none|
|»» locationTime|string|true|none||none|
|»» accuracy|number|true|none||none|
|»» speed|number|true|none||none|
|»» direction|integer|true|none||none|
|»» locationType|string|true|none||none|
|»» status|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取指定区域内的骑手

GET /GET /api/riderlocations/area

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|minLongitude|query|number| 是 |none|
|maxLongitude|query|number| 是 |none|
|minLatitude|query|number| 是 |none|
|maxLatitude|query|number| 是 |none|
|status|query|integer| 否 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": [
    {
      "locationId": "string",
      "riderId": "R001",
      "longitude": 116.407526,
      "latitude": 39.9042,
      "locationTime": "2025-01-20T14:30:25Z",
      "accuracy": 10.5,
      "speed": 25.3,
      "direction": 180,
      "locationType": "GPS",
      "status": 1
    },
    {
      "locationId": "string2",
      "riderId": "R002",
      "longitude": 116.408526,
      "latitude": 39.9052,
      "locationTime": "2025-01-20T14:29:15Z",
      "accuracy": 8,
      "speed": 0,
      "direction": 0,
      "locationType": "GPS",
      "status": 1
    }
  ],
  "timestamp": 1678864000000
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
|»» locationId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» longitude|number|true|none||none|
|»» latitude|number|true|none||none|
|»» locationTime|string|true|none||none|
|»» accuracy|number|true|none||none|
|»» speed|number|true|none||none|
|»» direction|integer|true|none||none|
|»» locationType|string|true|none||none|
|»» status|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取所有在线骑手位置

GET /GET /api/riderlocations/online

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|pageIndex|query|integer| 否 |none|
|pageSize|query|integer| 否 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": [
    {
      "locationId": "string",
      "riderId": "R001",
      "longitude": 116.407526,
      "latitude": 39.9042,
      "locationTime": "2025-01-20T14:30:25Z",
      "accuracy": 10.5,
      "speed": 25.3,
      "direction": 180,
      "locationType": "GPS",
      "status": 1
    },
    {
      "locationId": "string2",
      "riderId": "R003",
      "longitude": 116.425526,
      "latitude": 39.9152,
      "locationTime": "2025-01-20T14:28:45Z",
      "accuracy": 15,
      "speed": 32.1,
      "direction": 90,
      "locationType": "GPS",
      "status": 1
    }
  ],
  "timestamp": 1678864000000
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
|»» locationId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» longitude|number|true|none||none|
|»» latitude|number|true|none||none|
|»» locationTime|string|true|none||none|
|»» accuracy|number|true|none||none|
|»» speed|number|true|none||none|
|»» direction|integer|true|none||none|
|»» locationType|string|true|none||none|
|»» status|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 计算骑手到指定点的距离

GET /GET /api/riderlocations/{riderId}/distance

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|targetLongitude|query|number| 是 |目标经度|
|targetLatitude|query|number| 是 |目标纬度|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "计算成功",
  "data": {
    "riderId": "R001",
    "targetLongitude": 116.41,
    "targetLatitude": 39.9,
    "distance": 523.45
  },
  "timestamp": 1678864000000
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
|»» targetLongitude|number|true|none||none|
|»» targetLatitude|number|true|none||none|
|»» distance|number|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取骑手当前位置的地址信息

GET /GET /api/riderlocations/{riderId}/address

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": {
    "riderId": "R001",
    "address": "北京市朝阳区建国门外大街1号国贸大厦附近"
  },
  "timestamp": 1678864000000
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
|»» address|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取骑手绩效趋势

GET /GET /api/riders/{riderId}/performances/trend

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|riderId|path|string| 是 |none|
|Query|query|integer| 否 |月份数量，默认6|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": [
    {
      "statsMonth": "2025-01-01T00:00:00Z",
      "totalOrders": 89,
      "onTimeRate": 0.95,
      "goodReviewRate": 0.92,
      "badReviewRate": 0.03,
      "income": 3456.78
    },
    {
      "statsMonth": "2024-12-01T00:00:00Z",
      "totalOrders": 76,
      "onTimeRate": 0.93,
      "goodReviewRate": 0.9,
      "badReviewRate": 0.05,
      "income": 2987.45
    }
  ],
  "timestamp": 1678864000000
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
|»» statsMonth|string|true|none||none|
|»» totalOrders|integer|true|none||none|
|»» onTimeRate|number|true|none||none|
|»» goodReviewRate|number|true|none||none|
|»» badReviewRate|number|true|none||none|
|»» income|number|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取骑手绩效排名

GET /GET /api/riders/{riderId}/performances/ranking/{year}/{month}

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
  "message": "获取成功",
  "data": {
    "totalOrdersRank": 5,
    "onTimeRateRank": 3,
    "goodReviewRateRank": 2,
    "incomeRank": 4,
    "overallRank": 3
  },
  "timestamp": 1678864000000
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
|»» totalOrdersRank|integer|true|none||none|
|»» onTimeRateRank|integer|true|none||none|
|»» goodReviewRateRank|integer|true|none||none|
|»» incomeRank|integer|true|none||none|
|»» overallRank|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取绩效优秀骑手排行榜

GET /GET /api/riders/performances/top-performers/{year}/{month}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|year|path|integer| 是 |none|
|month|path|integer| 是 |none|
|topCount|query|integer| 否 |返回数量，默认10|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": [
    {
      "statsMonth": "2025-01-01T00:00:00Z",
      "totalOrders": 125,
      "onTimeRate": 0.98,
      "goodReviewRate": 0.96,
      "badReviewRate": 0.01,
      "income": 4567.89
    },
    {
      "statsMonth": "2025-01-01T00:00:00Z",
      "totalOrders": 118,
      "onTimeRate": 0.97,
      "goodReviewRate": 0.95,
      "badReviewRate": 0.02,
      "income": 4123.45
    }
  ],
  "timestamp": 1678864000000
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
|»» statsMonth|string|true|none||none|
|»» totalOrders|integer|true|none||none|
|»» onTimeRate|number|true|none||none|
|»» goodReviewRate|number|true|none||none|
|»» badReviewRate|number|true|none||none|
|»» income|number|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取月度绩效概览

GET /GET /api/riders/performances/overview/{year}/{month}

### 请求参数

|名称|位置|类型|必选|说明|
|---|---|---|---|---|
|year|path|integer| 是 |none|
|month|path|integer| 是 |none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取成功",
  "data": {
    "TotalRiders": 45,
    "TotalOrders": 2567,
    "AverageOnTimeRate": 0.94,
    "AverageGoodReviewRate": 0.91,
    "TotalIncome": 125678.9,
    "AverageIncome": 2792.86
  },
  "timestamp": 1678864000000
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
|»» TotalRiders|integer|true|none||none|
|»» TotalOrders|integer|true|none||none|
|»» AverageOnTimeRate|number|true|none||none|
|»» AverageGoodReviewRate|number|true|none||none|
|»» TotalIncome|number|true|none||none|
|»» AverageIncome|number|true|none||none|
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

## POST 更新骑手位置

POST /POST /api/riderlocations

> Body 请求参数

```json
{
  "riderId": "string",
  "longitude": 116.407526,
  "latitude": 39.9042,
  "accuracy": 10.5,
  "speed": 25.3,
  "direction": 180,
  "locationType": "GPS"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|body|body|object| 否 ||none|
|» riderId|body|string| 是 | 骑手ID|none|
|» longitude|body|number| 是 | 经度 (-180到180之间)|none|
|» latitude|body|number| 是 | 纬度|none|
|» accuracy|body|number| 否 | 定位精度|none|
|» speed|body|number| 否 | 速度(km/h，0到200之间)|none|
|» direction|body|integer| 否 | 方向(0-360度)|none|
|» locationType|body|string| 否 | 定位类型(如"GPS"、"网络"等，最长20字符)|none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "位置更新成功",
  "data": {
    "locationId": "string",
    "riderId": "string",
    "longitude": 116.407526,
    "latitude": 39.9042,
    "locationTime": "2025-01-20T14:30:25Z",
    "accuracy": 10.5,
    "speed": 25.3,
    "direction": 180,
    "locationType": "GPS",
    "status": 1
  },
  "timestamp": 1678864000000
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
|»» locationId|string|true|none||none|
|»» riderId|string|true|none||none|
|»» longitude|number|true|none||none|
|»» latitude|number|true|none||none|
|»» locationTime|string|true|none||none|
|»» accuracy|number|true|none||none|
|»» speed|number|true|none||none|
|»» direction|integer|true|none||none|
|»» locationType|string|true|none||none|
|»» status|integer|true|none||none|
|» timestamp|integer|true|none||none|

## POST 生成骑手月度绩效

POST /POST /api/riders/{riderId}/performances/generate/{year}/{month}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|riderId|path|string| 是 ||none|
|year|path|integer| 是 ||none|
|month|path|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "骑手月度绩效生成成功",
  "data": {
    "statsMonth": "2025-01-01T00:00:00Z",
    "totalOrders": 89,
    "onTimeRate": 0.95,
    "goodReviewRate": 0.92,
    "badReviewRate": 0.03,
    "income": 3456.78
  },
  "timestamp": 1678864000000
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
|»» statsMonth|string|true|none||none|
|»» totalOrders|integer|true|none||none|
|»» onTimeRate|number|true|none||none|
|»» goodReviewRate|number|true|none||none|
|»» badReviewRate|number|true|none||none|
|»» income|number|true|none||none|
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

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|riderId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» name|body|string| 是 ||none|
|» phoneNumber|body|string| 是 ||none|
|» vehicleNumber|body|string| 是 ||none|

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

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|riderId|path|string| 是 ||none|
|assignId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» acceptedStatus|body|integer| 是 ||none|

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

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|riderId|path|string| 是 ||none|
|attendanceId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» checkoutAt|body|string| 是 ||none|

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

## PATCH 更新骑手在线状态

PATCH /PATCH /api/riderlocations/{riderId}/status

> Body 请求参数

```json
{
  "status": 1
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|riderId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» status|body|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "骑手在线状态更新成功",
  "data": {},
  "timestamp": 1678864000000
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
|» timestamp|integer|true|none||none|

# 登录注册

## POST 登录

POST /api/auth/login

登录请求（username/email 2选1）

> Body 请求参数

```json
{
  "Password": "4URBDFyKts37uRo",
  "Email": "tbfv9r_g9z@qq.com"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userType|query|integer| 是 ||none|
|body|body|object| 否 ||none|
|» Password|body|string| 是 ||密码长度不能少于6位|
|» Email|body|string| 否 ||后端会进行邮箱的格式校验，前端看情况也可以再审核一遍|
|» UserName|body|string| 否 ||只允许数字和字母|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": "string",
  "timestamp": 0
}
```

> 403 Response

```json
{
  "code": 2002,
  "message": "用户被封禁",
  "data": null,
  "timestamp": 1754276287920
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|none|Inline|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **401**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **403**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## POST 预注册（发送验证邮件）

POST /api/auth/register

> Body 请求参数

```json
{
  "UserName": "闽磊",
  "PassWord": "dizI6fIabdJtWmr",
  "Email": "lm1sds_man@vip.qq.com",
  "PhoneNumber": "08731135405"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userType|query|integer| 是 ||用户类型（1-普通用户, 2-商家, 3-骑手, 4-管理员）|
|body|body|object| 否 ||none|
|» UserName|body|string| 是 ||只允许数字或字母|
|» PassWord|body|string| 是 ||前端可以加密码的格式要求|
|» Email|body|string| 是 ||后端会进行邮箱的格式校验，前端看情况也可以再审核一遍|
|» PhoneNumber|body|string| 否 ||做不到发验证码，手机号为选填|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": null,
  "timestamp": 0
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
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 正式注册（验证邮箱）

GET /api/auth/verify-email

链接1小时内有效

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|query|string| 否 ||none|
|token|query|string| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": null,
  "timestamp": 0
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
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/merchant

## GET 根据 ID 获取商家信息

GET /api/merchants/{merchantId}

public enum MerchantStatus
{
InReview = 0, // 审核中
Open = 1, // 正常营业
Close = 2, // 休息中
Baned =3 // 封禁中
}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|

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

> 404 Response

```json
{
  "code": 0,
  "message": "string",
  "data": {
    "merchantId": "string",
    "merchantName": "example.com",
    "status": 0,
    "contactInfo": "user@example.com",
    "location": "string"
  }
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
|» data|object|true|none||none|
|»» merchantId|string|true|none||none|
|»» merchantName|string|true|none||none|
|»» status|integer|true|none||none|
|»» contactInfo|string|true|none||none|
|»» location|string|true|none||none|
|»» description|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer(int32)|true|none||none|
|» message|string|true|none||none|
|» data|object¦null|true|none||none|
|»» merchantId|string|true|none||none|
|»» merchantName|string(hostname)|true|none||none|
|»» status|integer|true|none||none|
|»» contactInfo|string(email)|true|none||none|
|»» location|string|true|none||none|

## PATCH 根据ID修改商家信息

PATCH /api/merchants/{merchantId}

   public enum MerchantStatus
    { 
        InReview = 0,            // 审核中
        Open = 1,                // 正常营业
        Close = 2,               // 休息中
        Baned =3                 // 封禁中
    }

> Body 请求参数

```json
{
  "merchantName": "string",
  "status": 1,
  "contactInfo": "string",
  "location": "string",
  "description": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» merchantName|body|string| 否 ||none|
|» status|body|integer| 否 ||none|
|» contactInfo|body|string| 否 ||none|
|» location|body|string| 否 ||none|
|» description|body|string| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|» status|1|
|» status|2|
|» status|3|
|» status|0|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家信息修改成功",
  "data": true,
  "timestamp": 1754447095827
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "商家不存在，ID: e71a686e0334db5b3b3bf77f0437ffa",
  "data": null,
  "timestamp": 1754447125447
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 自定义筛选条件获取商家列表

GET /api/merchants

目前支持4种筛选方式：
1. 模糊名字搜索（merchanName）
2. 模糊地址搜索（location）
3. 状态搜索（status）
4. 不限制

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantName|query|string| 否 ||none|
|location|query|string| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|status|query|integer| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|status|0|
|status|1|
|status|2|
|status|3|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家信息获取成功",
  "data": [
    {
      "merchantId": "66f63f6d40bf483cb371648b239a610d",
      "merchantName": "Today",
      "status": 1,
      "contactInfo": "LiuZhen_1226@163.com",
      "location": ""
    },
    {
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "merchantName": "test",
      "status": 1,
      "contactInfo": "2356215@tongji.edu.cn",
      "location": ""
    },
    {
      "merchantId": "0b5feb36d23443eab85fa5eb8c1a538b",
      "merchantName": "merchant_lz",
      "status": 1,
      "contactInfo": "2352471@tongji.edu.cn",
      "location": ""
    }
  ],
  "timestamp": 1754444437577
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
|»» merchantId|string|true|none||none|
|»» merchantName|string|true|none||none|
|»» status|integer|true|none||none|
|»» contactInfo|string|true|none||none|
|»» location|string|true|none||none|
|»» description|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 模糊搜索，不太智能的智能匹配

GET /api/merchants/autocomplete

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|prefix|query|string| 是 ||none|
|limit|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "智能补全成功",
  "data": [
    "预制菜中王",
    "预制菜大王",
    "预制菜小王"
  ],
  "timestamp": 1754461025205
}
```

> 400 Response

```json
{
  "code": 1001,
  "message": "请加长前缀长度来启用智能匹配",
  "data": null,
  "timestamp": 1754461076844
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **400**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/dish

## GET 根据商家 ID 获取所有菜品信息

GET /api/merchants/{merchantId}/dishes

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|categoryId|query|integer| 否 ||none|
|orderByRating|query|boolean| 否 ||none|
|orderByHighPrice|query|boolean| 否 ||none|
|orderByLowPrice|query|boolean| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家菜品信息获取成功",
  "data": [
    {
      "dishId": "5fa45665bc39407b9e9883889fb8c5cb",
      "categoryId": "2                               ",
      "dishName": "猪肉佐荔枝酱",
      "price": 185.64,
      "originPrice": 250.19,
      "coverUrl": "https://picsum.photos/seed/QX5hjq/3890/3023",
      "monthlySales": 0,
      "rating": 0,
      "onSale": 0,
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "reviewQuantity": 0,
      "categoryName": "CAT002",
      "description": "这是一道好吃的菜",
      "stockQuantity": 0
    },
    {
      "dishId": "f97ca0a13ad3481bb548cfbce1b570c7",
      "categoryId": "2                               ",
      "dishName": "臭豆腐风味沙拉",
      "price": 570.89,
      "originPrice": 340.3,
      "coverUrl": "https://picsum.photos/seed/E3dEyZ0r9/2073/1857",
      "monthlySales": 0,
      "rating": 0,
      "onSale": 0,
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "reviewQuantity": 0,
      "categoryName": "CAT002",
      "description": "经过32天熟成的兔肉牛排，可选3款配菜。",
      "stockQuantity": 6
    }
  ],
  "timestamp": 1755161247226
}
```

> 404 Response

```json
{
  "code": 0,
  "message": "string",
  "data": null,
  "timestamp": 0
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
|» data|[object]|true|none||none|
|»» dishId|string|true|none||none|
|»» categoryId|string|true|none||none|
|»» dishName|string|true|none||none|
|»» price|number|true|none||none|
|»» originPrice|number|true|none||none|
|»» coverUrl|string|true|none||none|
|»» monthlySales|integer|true|none||none|
|»» rating|integer|true|none||none|
|»» onSale|integer|true|none||none|
|»» merchantId|string|true|none||none|
|»» reviewQuantity|integer|true|none||none|
|»» categoryName|string|true|none||none|
|»» description|string|true|none||none|
|»» stockQuantity|integer|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据主键获取菜品

GET /api/merchants/{merchantId}/dishes/{dishId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|dishId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家菜品信息获取成功",
  "data": {
    "dishId": "f97ca0a13ad3481bb548cfbce1b570c7",
    "categoryId": "2                               ",
    "dishName": "臭豆腐风味沙拉",
    "price": 570.89,
    "originPrice": 340.3,
    "coverUrl": "https://picsum.photos/seed/E3dEyZ0r9/2073/1857",
    "monthlySales": 0,
    "rating": 0,
    "onSale": 0,
    "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
    "reviewQuantity": 0,
    "categoryName": "CAT002",
    "description": "经过32天熟成的兔肉牛排，可选3款配菜。",
    "stockQuantity": 6
  },
  "timestamp": 1755161445088
}
```

> 404 Response

```json
{
  "code": 40001,
  "message": "无相关数据，ID: e71a686e03f4db5b3b3bf77f0437ffa",
  "data": null,
  "timestamp": 1754378301082
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
|» data|object|true|none||none|
|»» dishId|string|true|none||none|
|»» categoryId|string|true|none||none|
|»» dishName|string|true|none||none|
|»» price|number|true|none||none|
|»» originPrice|number|true|none||none|
|»» coverUrl|string|true|none||none|
|»» monthlySales|integer|true|none||none|
|»» rating|integer|true|none||none|
|»» onSale|integer|true|none||none|
|»» merchantId|string|true|none||none|
|»» reviewQuantity|integer|true|none||none|
|»» categoryName|string|true|none||none|
|»» description|string|true|none||none|
|»» stockQuantity|integer|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## POST 根据商家ID添加新菜品

POST /api/merchants/{merchantId}/addNewDish

> Body 请求参数

```json
{
  "categoryId": "string",
  "dishName": "string",
  "price": 0,
  "originPrice": 0,
  "coverUrl": "string",
  "description": "string",
  "stockQuantity": 0
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» categoryId|body|string| 是 | 分类ID|分组|
|» dishName|body|string| 是 | 菜品名|none|
|» price|body|number| 否 | 现价|none|
|» originPrice|body|number| 是 | 原价|none|
|» coverUrl|body|string| 否 | 封面链接|none|
|» description|body|string| 否 | 描述|none|
|» stockQuantity|body|integer| 否 ||none|

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

## DELETE 根据商家ID和菜品ID删除对应的菜品

DELETE /api/merchants/{merchantId}/{dishId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|dishId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1754448047159
}
```

> 404 Response

```json
{
  "code": 40001,
  "message": "无相关数据，DishID: 6a08300d6e3e42b3b99e9a336f6214c7",
  "data": null,
  "timestamp": 1754448083021
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 根据商家ID和菜品ID修改菜品信息

PATCH /api/merchants/{merchantId}/{dishId}

这个功能需要先通过前端将菜品信息预填入表单，所以请求体内的参数都为必选

> Body 请求参数

```json
{
  "categoryId": "string",
  "dishName": "string",
  "price": 0,
  "originPrice": 0,
  "coverUrl": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|dishId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» categoryId|body|string| 否 ||none|
|» dishName|body|string| 否 ||none|
|» price|body|number| 否 ||none|
|» originPrice|body|number| 否 ||none|
|» coverUrl|body|string| 否 ||none|
|» onSale|body|integer| 否 ||暂定0-2，由前端决定|
|» description|body|string| 是 ||none|
|» stockQuantity|body|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1754447955233
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据商家ID获取所有菜品分类

GET /api/merchants/{merchantId}/dish-categories

菜品分类是由平台预先定义，商家拥有其子集

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家分类信息获取成功",
  "data": [
    {
      "categoryId": "1                               ",
      "categoryName": "CAT001",
      "parentId": "1                               ",
      "sortOrder": 1
    },
    {
      "categoryId": "3                               ",
      "categoryName": "CAT003",
      "parentId": "2                               ",
      "sortOrder": 1
    },
    {
      "categoryId": "4                               ",
      "categoryName": "CAT004",
      "parentId": "2                               ",
      "sortOrder": 1
    },
    {
      "categoryId": "2                               ",
      "categoryName": "CAT002",
      "parentId": "1                               ",
      "sortOrder": 2
    }
  ],
  "timestamp": 1754385001328
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
|»» categoryId|string|true|none||none|
|»» categoryName|string|true|none||none|
|»» parentId|string|true|none||none|
|»» sortOrder|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 按分类获取商家的所有菜品(嵌套)

GET /api/merchants/{merchantId}/dishesByCategory

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家菜品信息获取成功",
  "data": [
    {
      "categoryId": "1                               ",
      "categoryName": "CAT001",
      "dishes": [
        {
          "dishId": "8459aac761424fa0a71cc5e521e01f8f",
          "categoryId": "1                               ",
          "dishName": "孜然羊肉",
          "price": 230.75,
          "originPrice": 979.55,
          "coverUrl": "https://picsum.photos/seed/rtaNJJ2yuq/3617/3889",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "698d2ef64b9f42f0b74ba6c414a82425",
          "categoryId": "1                               ",
          "dishName": "爽口的的田鸡配芦笋",
          "price": 406.49,
          "originPrice": 381.19,
          "coverUrl": "https://loremflickr.com/1592/1434?lock=3572024428382264",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "95252778c4af4e18a70dd1fcdc9041fc",
          "categoryId": "1                               ",
          "dishName": "茄子炒肉",
          "price": 769.49,
          "originPrice": 47.8,
          "coverUrl": "https://picsum.photos/seed/dUo59nj/3278/2989",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "fa5873ecd2cf4beda2bda45a2a52a04b",
          "categoryId": "1                               ",
          "dishName": "葡萄派",
          "price": 536.99,
          "originPrice": 25.89,
          "coverUrl": "https://picsum.photos/seed/rDChR1Vi94/3830/2327",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        }
      ]
    },
    {
      "categoryId": "2                               ",
      "categoryName": "CAT002",
      "dishes": [
        {
          "dishId": "1b55484c33a047d68b4e8f807ad9961a",
          "categoryId": "2                               ",
          "dishName": "木须肉",
          "price": 907.89,
          "originPrice": 331.25,
          "coverUrl": "https://picsum.photos/seed/ixo2W/835/498",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "2560ca5cff9f452fb5ad88e308eda2f0",
          "categoryId": "2                               ",
          "dishName": "水煮牛肉",
          "price": 879.99,
          "originPrice": 539.49,
          "coverUrl": "https://loremflickr.com/1472/3395?lock=6589695793590999",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "5fa45665bc39407b9e9883889fb8c5cb",
          "categoryId": "2                               ",
          "dishName": "猪肉佐荔枝酱",
          "price": 185.64,
          "originPrice": 250.19,
          "coverUrl": "https://picsum.photos/seed/QX5hjq/3890/3023",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "f97ca0a13ad3481bb548cfbce1b570c7",
          "categoryId": "2                               ",
          "dishName": "臭豆腐风味沙拉",
          "price": 570.89,
          "originPrice": 340.3,
          "coverUrl": "https://picsum.photos/seed/E3dEyZ0r9/2073/1857",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        }
      ]
    },
    {
      "categoryId": "3                               ",
      "categoryName": "CAT003",
      "dishes": [
        {
          "dishId": "6961b1217d26438d8c7e897584df0625",
          "categoryId": "3                               ",
          "dishName": "刀削面",
          "price": 858.82,
          "originPrice": 597.13,
          "coverUrl": "https://picsum.photos/seed/qbPKBNOzwm/2863/1296",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "ecbff19ad0ee4f3cbe1c8cdaea4a99e4",
          "categoryId": "3                               ",
          "dishName": "蔓越莓香煎驴肉",
          "price": 29,
          "originPrice": 96,
          "coverUrl": "https://loremflickr.com/1613/1066?lock=7246958666000966",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 2,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "3e3cb76445e44a6b8339a6320505ca11",
          "categoryId": "3                               ",
          "dishName": "酸菜鱼",
          "price": 108.49,
          "originPrice": 966.57,
          "coverUrl": "https://loremflickr.com/2811/2008?lock=4370184519791220",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        }
      ]
    },
    {
      "categoryId": "4                               ",
      "categoryName": "CAT004",
      "dishes": [
        {
          "dishId": "2ec2d7311b6246b8a868d1c43649cb67",
          "categoryId": "4                               ",
          "dishName": "红烧鱼块",
          "price": 132.15,
          "originPrice": 559.69,
          "coverUrl": "https://picsum.photos/seed/9T4AN/591/3269",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        },
        {
          "dishId": "36cc5526bf244c8f9fa605ccfac49a51",
          "categoryId": "4                               ",
          "dishName": "蛇肉佐柑橘酱",
          "price": 850.13,
          "originPrice": 754.79,
          "coverUrl": "https://picsum.photos/seed/zIJxA/682/177",
          "monthlySales": 0,
          "rating": 0,
          "onSale": 0,
          "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
          "reviewQuantity": 0,
          "categoryName": null,
          "description": null,
          "stockQuantity": null
        }
      ]
    }
  ],
  "timestamp": 1755161803105
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
|»» categoryId|string|true|none||none|
|»» categoryName|string|true|none||none|
|»» dishes|[object]|true|none||none|
|»»» dishId|string|true|none||none|
|»»» categoryId|string|true|none||none|
|»»» dishName|string|true|none||none|
|»»» price|number|true|none||none|
|»»» originPrice|number|true|none||none|
|»»» coverUrl|string|true|none||none|
|»»» monthlySales|integer|true|none||none|
|»»» rating|integer|true|none||none|
|»»» onSale|integer|true|none||none|
|»»» merchantId|string|true|none||none|
|»»» reviewQuantity|integer|true|none||none|
|»»» categoryName|null|true|none||none|
|»»» description|null|true|none||none|
|»»» stockQuantity|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/Application

## POST 商家发起申请

POST /api/merchants/{merchantId}/applications

返回参数由bool改为生成的applyID

> Body 请求参数

```json
{
  "CompanyName": "string",
  "ApplicationMaterials": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» CompanyName|body|string| 是 ||none|
|» ApplicationMaterials|body|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": "f6969f49ab374483acc4011e273e3696",
  "timestamp": 1755480055984
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "商家不存在，ID: e71a686e033fdb5b3b3bf77f0437ffa",
  "data": null,
  "timestamp": 1753774030773
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
|» data|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 商家根据商家ID和自定义筛选条件获取商家申请队列

GET /api/merchants/{merchantId}/applications

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|auditStatus|query|integer| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "applyId": "6587daf51b9d40c7a940200f77b44772",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:33:20",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "d5cf775d7fcf4a2094026b586b808b9e",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:24",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "9aabfa797ebb411abd4e4eb20041a4bd",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:23",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "33cb10a62b7d4a9e9cc3bc1bc6264086",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:23",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "da6888d9b8014106a93a41c9f667901a",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:22",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "b158b257a5d44a3eac50a5266dca4da1",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:21",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "16fe804841c34eb2b30ed5c23504c865",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:20",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "d467d98455f345358f83d3744e669d40",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:19",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "f56c3af62cc7437a8a98d8f56538d839",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 13:52:26",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "2",
      "auditAt": "2025/7/29 06:33:48",
      "rejectReason": "ullamco sit",
      "adminId": "6f7af74d972c481c91f19596e07aae3a"
    }
  ],
  "timestamp": 1753770868975
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "申请不存在，ID: e71a686e033fdb5b3b3bf77f0437ffa",
  "data": null,
  "timestamp": 1753774229561
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
|» data|[object]|true|none||none|
|»» applyId|string|true|none||none|
|»» companyName|string|true|none||none|
|»» submittedAt|string|true|none||none|
|»» merchantId|string|true|none||none|
|»» auditStatus|string|true|none||none|
|»» auditAt|string¦null|true|none||none|
|»» rejectReason|string¦null|true|none||none|
|»» adminId|string¦null|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 管理员同意/拒绝申请

PATCH /api/admin/{adminId}/audit/{applyId}

AuditStatus = 1：同意申请
AuditStatus = 2 ：拒绝申请

> Body 请求参数

```json
{
  "AuditStatus": 0,
  "RejectReason": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|applyId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» AuditStatus|body|integer| 是 ||none|
|» RejectReason|body|string| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1753774074217
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "管理员不存在，ID: 6f7af74d72c481c91f19596e07aae3a",
  "data": null,
  "timestamp": 1753774100847
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据申请ID获取申请状态

GET /api/applications/{applicationId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|applicationId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "applyId": "f6969f49ab374483acc4011e273e3696",
    "companyName": "贵州省天磊旅游发展无限责任公司",
    "submittedAt": "2025/8/18 09:20:55",
    "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
    "auditStatus": "1",
    "auditAt": "2025/8/18 01:25:01",
    "rejectReason": null,
    "adminId": "6f7af74d972c481c91f19596e07aae3a",
    "applicationMaterials": "重新调整的基于情境框架"
  },
  "timestamp": 1755480309571
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "申请不存在，ID: 12312",
  "data": null,
  "timestamp": 1753771229190
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
|» data|object|true|none||none|
|»» applyId|string|true|none||none|
|»» companyName|string|true|none||none|
|»» submittedAt|string|true|none||none|
|»» merchantId|string|true|none||none|
|»» auditStatus|string¦null|true|none||none|
|»» auditAt|string¦null|true|none||none|
|»» rejectReason|string¦null|true|none||none|
|»» adminId|string¦null|true|none||none|
|»» applicationMaterials|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 管理员根据管理员ID和自定义筛选条件获取事务

GET /api/admin/{adminId}/applications

> Body 请求参数

```json
{}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|auditStatus|query|integer| 否 ||none|
|startDate|query|string| 否 ||none|
|endDate|query|string| 否 ||none|
|merchantId|query|string| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|checkProfile|query|boolean| 是 ||none|
|body|body|object| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "applyId": "6587daf51b9d40c7a940200f77b44772",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:33:20",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "d5cf775d7fcf4a2094026b586b808b9e",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:24",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "9aabfa797ebb411abd4e4eb20041a4bd",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:23",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "33cb10a62b7d4a9e9cc3bc1bc6264086",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:23",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "da6888d9b8014106a93a41c9f667901a",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:22",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "b158b257a5d44a3eac50a5266dca4da1",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:21",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "16fe804841c34eb2b30ed5c23504c865",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:20",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    },
    {
      "applyId": "d467d98455f345358f83d3744e669d40",
      "companyName": "库悦",
      "submittedAt": "2025/7/29 14:17:19",
      "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
      "auditStatus": "0",
      "auditAt": null,
      "rejectReason": null,
      "adminId": null
    }
  ],
  "timestamp": 1753773941779
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "无该状态，AuditStatus: 22",
  "data": null,
  "timestamp": 1753773894889
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
|» data|[object]|true|none||none|
|»» applyId|string|true|none||none|
|»» companyName|string|true|none||none|
|»» submittedAt|string|true|none||none|
|»» merchantId|string|true|none||none|
|»» auditStatus|string|true|none||none|
|»» auditAt|null|true|none||none|
|»» rejectReason|null|true|none||none|
|»» adminId|null|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/SaleState

## GET 根据主键获取统计信息

GET /api/merchants/{merchantId}/SalesStat/{statDate}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|statDate|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家信息获取成功",
  "data": {
    "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
    "statDate": "2025-08-04T16:14:42",
    "salesQty": 2,
    "salesAmount": 3
  },
  "timestamp": 1754296067397
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
|»» merchantId|string|true|none||none|
|»» statDate|string|true|none||none|
|»» salesQty|integer|true|none||none|
|»» salesAmount|integer|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据商家ID和自定义筛选条件获取统计信息

GET /api/merchants/{merchantId}/SalesStat

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|startTime|query|string| 否 ||none|
|endTime|query|string| 否 ||none|
|minAmount|query|number| 否 ||none|
|maxAmount|query|number| 否 ||none|
|statType|query|integer| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "string",
  "data": [
    {
      "merchantId": "string",
      "statDate": "string",
      "salesQty": 0,
      "salesAmount": 0
    }
  ],
  "timestamp": 0
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
|»» merchantId|string|false|none||none|
|»» statDate|string|false|none||none|
|»» salesQty|integer|false|none||none|
|»» salesAmount|integer|false|none||none|
|» timestamp|integer|true|none||none|

# merchant/order

## GET 根据商家 ID 和自定义筛选获取商家所有订单

GET /api/merchants/{merchantId}/orders

目前3种筛选方法（可增加筛选方法，可更改排序优先级）：

1.获取目标时间内的订单（startDate，endDate）:可以都填，也可以只填一个

2.获取目标状态的订单（orderStatus）

3.不做筛选，获取该商家所有的订单（）

  public enum OrderStatus
    {
        Unpaid = 0,    // 未支付（初始状态）
        Paid = 1,      // 已支付
        Confirmed = 2, // 确认收货
        Reviewed =3,   // 已经评价
        Aftersales = 4,// 售后中
        Cancelled = 5, // 订单关闭(超时/主动取消)
    }

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|startDate|query|string| 否 ||none|
|endDate|query|string| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|orderStatus|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家订单信息获取成功",
  "data": [
    "04d86dd0b06340f7b0f8787b332c692c",
    "c0e1c12ea45d4d86aa2021b5266141f5"
  ],
  "timestamp": 1755242758676
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据OrderId获取订单详情

GET /api/orders/{orderId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|orderId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家菜品信息获取成功",
  "data": {
    "orderId": "e9adee7215e34d7eadd25992c6529553",
    "userId": "1d804350373d47b2b66feb88f8403e7d",
    "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
    "addressId": "ADDR20240601001                 ",
    "orderAmount": 885.85,
    "createAt": "2025-08-15T14:25:40.636722",
    "orderStatus": 0,
    "reconId": null,
    "couponId": null,
    "assignId": null,
    "orderDishes": [
      {
        "quantity": 38,
        "dishId": "698d2ef64b9f42f0b74ba6c414a82425"
      },
      {
        "quantity": 66,
        "dishId": "8459aac761424fa0a71cc5e521e01f8f"
      },
      {
        "quantity": 51,
        "dishId": "95252778c4af4e18a70dd1fcdc9041fc"
      }
    ],
    "orderLogIds": [
      "30770f113ca24fb087ef28bfb9a14543"
    ],
    "paymentIds": [],
    "refundIds": [],
    "complaintIds": [],
    "reviewIds": []
  },
  "timestamp": 1755246277722
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
|»» orderId|string|true|none||none|
|»» userId|string|true|none||none|
|»» merchantId|string|true|none||none|
|»» addressId|string|true|none||none|
|»» orderAmount|number|true|none||none|
|»» createAt|string|true|none||none|
|»» orderStatus|integer|true|none||none|
|»» reconId|string¦null|true|none||none|
|»» couponId|string¦null|true|none||none|
|»» assignId|string¦null|true|none||none|
|»» orderDishes|[object]|true|none||none|
|»»» quantity|integer|true|none||none|
|»»» dishId|string|true|none||none|
|»» orderLogIds|[string]|true|none||none|
|»» paymentIds|[string]|true|none||none|
|»» refundIds|[string]|true|none||none|
|»» complaintIds|[string]|true|none||none|
|»» reviewIds|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据用户ID获取用户所有订单

GET /api/users/{userId}/orders

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|orderStatus|query|integer| 是 ||none|
|size|query|integer| 是 ||none|
|page|query|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户订单信息获取成功",
  "data": [
    "e9adee7215e34d7eadd25992c6529553",
    "04d86dd0b06340f7b0f8787b332c692c",
    "c0e1c12ea45d4d86aa2021b5266141f5"
  ],
  "timestamp": 1755244757352
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## POST 发起支付请求，创建待支付实体

POST /api/orders/{orderId}/createPayment

    public enum PayStatus
    {
        Unpaid = 0,    // 未支付（初始状态）
        Paid = 1,      // 已支付
        Cancelled = 2  // 已取消
    }

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|orderId|path|string| 是 ||none|
|channel|query|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户订单信息获取成功",
  "data": {
    "payId": "3636ea755a9744cb8c67317f871730c2",
    "orderId": "04d86dd0b06340f7b0f8787b332c692c",
    "channel": "weixin",
    "payAmount": 50
  },
  "timestamp": 1754639088593
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
|»» payId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» channel|string|true|none||none|
|»» payAmount|integer|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 取消支付

PATCH /api/payments/{payid}/cancel

修改payment对应的payStatus为Cancelled
同步创建orderLog

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|payid|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户取消订单成功",
  "data": true,
  "timestamp": 1754639112263
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 确认支付

PATCH /api/payments/{payid}/pay

修改payment对应的payStatus为Paid，同步修改Order对应的orderStatus为Paid
只有orderStatus为UnPaid时才可以确认支付，否则就是重复支付，返回异常，
同步创建orderLog

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|payid|path|string| 是 ||none|
|payAmount|query|number| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户支付订单成功",
  "data": true,
  "timestamp": 1754639319003
}
```

> 400 Response

```json
{
  "code": 60006,
  "message": "重复支付！",
  "data": null,
  "timestamp": 1754642923869
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **400**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 确认收货

PATCH /api/orders/{orderId}/confirm

无法重复设置为相同的order状态，
order状态有
        Unpaid = 0,    // 未支付（初始状态）
        Paid = 1,      // 已支付
        Confirmed = 2, // 确认收货
        Reviewed =3,   // 已经评价
        Aftersales = 4,// 售后中
        Cancelled = 5, // 订单关闭(超时/主动取消)

同步创建orderLog

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|orderId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户确认收货成功",
  "data": true,
  "timestamp": 1754640179536
}
```

> 500 Response

```json
{
  "code": 1000,
  "message": "无法重复操作！",
  "data": null,
  "timestamp": 1754643336882
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **500**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 取消订单

PATCH /api/orders/{orderId}/cancel

同步创建orderLog

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|orderId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户取消订单成功",
  "data": true,
  "timestamp": 1754643740805
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## POST 用户创建订单

POST /api/users/{userId}/createOrder

用户创建订单，同步创建对应的ordrLog和orderDish，创建成功返回对应的orderLogId

> Body 请求参数

```json
{
  "orderAmount": 885.85,
  "addressId": "ADDR20240601001",
  "merchantId": "e71a686e033f4db5b3b3bf77f0437ffa",
  "dishQuantities": [
    {
      "dishId": "698d2ef64b9f42f0b74ba6c414a82425",
      "quantity": 38
    },
    {
      "dishId": "8459aac761424fa0a71cc5e521e01f8f",
      "quantity": 66
    },
    {
      "dishId": "95252778c4af4e18a70dd1fcdc9041fc",
      "quantity": 51
    }
  ]
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» orderAmount|body|number| 是 ||none|
|» couponId|body|string| 否 ||none|
|» addressId|body|string| 是 ||none|
|» merchantId|body|string| 是 ||none|
|» dishQuantities|body|[object]| 是 ||none|
|»» dishId|body|string| 是 ||none|
|»» quantity|body|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户创建订单的请求成功",
  "data": "3a39e65def6d424b992dee4163e46475",
  "timestamp": 1755223883134
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
|» data|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据Logid获取OrderLog的详情

GET /api/orderLogs/{logId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|logId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "订单日志详细信息获取成功",
  "data": {
    "logId": "30770f113ca24fb087ef28bfb9a14543",
    "statusCode": 0,
    "loggedAt": "2025-08-15T14:25:40.934645",
    "actor": "user",
    "remark": "用户创建订单",
    "orderId": "e9adee7215e34d7eadd25992c6529553"
  },
  "timestamp": 1755246991512
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
|»» logId|string|true|none||none|
|»» statusCode|integer|true|none||none|
|»» loggedAt|string|true|none||none|
|»» actor|string|true|none||none|
|»» remark|string|true|none||none|
|»» orderId|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据PayId获取Payment的详情

GET /api/payments/{payId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|payId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "订单日志详细信息获取成功",
  "data": {
    "payId": "9a8d3b17cff141dfb08d9dd2e4c15f4c",
    "orderId": "c0e1c12ea45d4d86aa2021b5266141f5",
    "channel": "weixin",
    "payAmount": 60,
    "payStatus": 1,
    "payTime": "2025-08-08T16:46:59.047624"
  },
  "timestamp": 1755247564464
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
|»» payId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» channel|string|true|none||none|
|»» payAmount|integer|true|none||none|
|»» payStatus|integer|true|none||none|
|»» payTime|string|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/order/refund

## POST 用户向商家发起退款请求，创建退款实体

POST /api/users/{userId}/orders/{orderId}/refunds

同步更新order状态为：售后中

> Body 请求参数

```json
{
  "reason": "string",
  "refundAmount": 0
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|orderId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» reason|body|string| 是 ||none|
|» refundAmount|body|number| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户创建订单退款的请求成功",
  "data": "977c4d62ff264a37b7a9cdd1657d935a",
  "timestamp": 1755486233870
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
|» data|string|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 商家同意退款申请

PATCH /api/merchants/{merchantId}/refunds/{refundId}/approve

> Body 请求参数

```json
{}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|refundId|path|string| 是 ||none|
|body|body|object| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家退款成功",
  "data": true,
  "timestamp": 1755487765650
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 商家拒绝退款申请

PATCH /api/merchants/{merchantId}/refunds/{refundId}/reject

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|refundId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家拒绝退款成功",
  "data": true,
  "timestamp": 1755488114626
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 管理员处理退款请求

PATCH /api/admins/{adminId}/refunds/{refundId}/audit

> Body 请求参数

```json
{
  "RefundStatus": 4
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|refundId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» RefundStatus|body|integer| 是 ||none|

#### 枚举值

|属性|值|
|---|---|
|» RefundStatus|4|
|» RefundStatus|5|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "管理员更新退款实体成功",
  "data": true,
  "timestamp": 1755503548083
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据RefundId获取Refund的详情

GET /api/refunds/{refundId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|refundId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "退款详细信息获取成功",
  "data": {
    "refundId": "977c4d62ff264a37b7a9cdd1657d935a",
    "orderId": "04d86dd0b06340f7b0f8787b332c692c",
    "applicationId": "1d804350373d47b2b66feb88f8403e7d",
    "reason": "生程马林。",
    "refundAmount": 100,
    "applyAt": "2025-08-18T11:03:53.663328",
    "auditStatus": 2,
    "finishAt": "2025-08-18T11:29:25.515251"
  },
  "timestamp": 1755497642797
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
|»» refundId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» applicationId|string|true|none||none|
|»» reason|string¦null|true|none||none|
|»» refundAmount|integer|true|none||none|
|»» applyAt|string|true|none||none|
|»» auditStatus|integer|true|none||none|
|»» finishAt|string¦null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取用户的退款申请历史

GET /api/users/{userId}/refunds

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|auditStatus|query|integer| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|auditStatus|1|
|auditStatus|2|
|auditStatus|3|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "用户退款记录获取成功",
  "data": [
    "e91eb9822ba249228e8c6f3ec4d2df95",
    "977c4d62ff264a37b7a9cdd1657d935a"
  ],
  "timestamp": 1755500634309
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## GET 获取商家拥有的退款申请

GET /api/merchants/{merchantId}/refunds

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|auditStatus|query|integer| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|auditStatus|1|
|auditStatus|2|
|auditStatus|3|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "商家关联退款记录获取成功",
  "data": [
    "e91eb9822ba249228e8c6f3ec4d2df95",
    "977c4d62ff264a37b7a9cdd1657d935a"
  ],
  "timestamp": 1755501124558
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## GET 管理员获取对应状态的退款申请

GET /api/admins/{adminId}/refunds

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|auditStatus|query|integer| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|auditStatus|1|
|auditStatus|2|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "管理员查看退款记录获取成功",
  "data": [
    "e91eb9822ba249228e8c6f3ec4d2df95"
  ],
  "timestamp": 1755503132483
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/settlement

## GET 用于管理员自定义筛选结算单

GET /api/settlements

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|startDate|query|string| 否 ||none|
|endDate|query|string| 否 ||none|
|isSettled|query|boolean| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取结算单详细信息成功",
  "data": [
    {
      "settleId": "6e1dfb9b09b3466390f3d33660e5c153",
      "periodStart": "2025-08-10T13:52:35",
      "periodEnd": "2025-08-11T13:52:36",
      "settledAt": "2025-08-11T13:52:49"
    },
    {
      "settleId": "6e1dfb9b09b3466390f3d33660e5c159",
      "periodStart": "2025-08-10T13:52:35",
      "periodEnd": "2025-08-11T13:52:36",
      "settledAt": "2025-08-11T13:52:49"
    }
  ],
  "timestamp": 1754892063049
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
|»» settleId|string|true|none||none|
|»» periodStart|string|true|none||none|
|»» periodEnd|string|true|none||none|
|»» settledAt|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 用于商家自定义筛选结算单

GET /api/settlements/merchants/{merchantId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|merchantId|path|string| 是 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|startDate|query|string| 否 ||none|
|endDate|query|string| 否 ||none|
|isSettled|query|boolean| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取结算单详细信息成功",
  "data": [
    {
      "settleId": "6e1dfb9b09b3466390f3d33660e5c153",
      "periodStart": "2025-08-10T13:52:35",
      "periodEnd": "2025-08-11T13:52:36",
      "settledAt": "2025-08-11T13:52:49"
    },
    {
      "settleId": "6e1dfb9b09b3466390f3d33660e5c159",
      "periodStart": "2025-08-10T13:52:35",
      "periodEnd": "2025-08-11T13:52:36",
      "settledAt": "2025-08-11T13:52:49"
    }
  ],
  "timestamp": 1754891990512
}
```

> 404 Response

```json
{
  "code": 40001,
  "message": "无相关数据，ID: e71a686e0334db5b3b3bf77f0437ffa",
  "data": null,
  "timestamp": 1754891963513
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
|» data|[object]|true|none||none|
|»» settleId|string|true|none||none|
|»» periodStart|string|true|none||none|
|»» periodEnd|string|true|none||none|
|»» settledAt|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 通过settlementId获取结算单详情

GET /api/settlements/{settlemmentId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|settlemmentId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取结算单详细信息成功",
  "data": {
    "settleId": "6e1dfb9b09b3466390f3d33660e5c159",
    "periodStart": "2025-08-10T13:52:35",
    "periodEnd": "2025-08-11T13:52:36",
    "grossAmount": 100,
    "commissionFee": 100,
    "netAmount": 100,
    "settledAt": "2025-08-11T13:52:49"
  },
  "timestamp": 1754892178595
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "无相关数据，ID: 6e1dfb9b09b3466390f3d33660e5c15",
  "data": null,
  "timestamp": 1754892207969
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
|» data|object|true|none||none|
|»» settleId|string|true|none||none|
|»» periodStart|string|true|none||none|
|»» periodEnd|string|true|none||none|
|»» grossAmount|integer|true|none||none|
|»» commissionFee|integer|true|none||none|
|»» netAmount|integer|true|none||none|
|»» settledAt|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/announcement

## POST 管理员发布公告

POST /api/admin/{adminId}/announcements

> Body 请求参数

```json
{
  "title": "string",
  "content": "string",
  "targetRole": "string",
  "startAt": "string",
  "endAt": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» title|body|string| 是 ||none|
|» content|body|string| 否 ||none|
|» targetRole|body|string| 否 ||参数为Merchants,Customers,Riders,3选1，或为null|
|» startAt|body|string| 是 ||none|
|» endAt|body|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1754902724669
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "无相关数据，ID: 6f7af74972c481c91f19596e07aae3a",
  "data": null,
  "timestamp": 1754902766364
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据当前日期和适用范围返回可用的公告list

GET /api/announcements/activate/{TargetRole}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|TargetRole|path|string| 是 ||参数为：Merchants,Customers,Riders,All，4选1|
|size|query|integer| 是 ||none|
|page|query|integer| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "announceId": "9ea673c35bd342abaff1a6ab936ed299",
      "title": "重要消息",
      "content": "今天是感恩节",
      "targetRole": null,
      "startAt": "2025-08-11T00:00:00",
      "endAt": "2025-08-14T00:00:00"
    }
  ],
  "timestamp": 1754965404671
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
|»» announceId|string|false|none||none|
|»» title|string|false|none||none|
|»» content|string|false|none||none|
|»» targetRole|null|false|none||none|
|»» startAt|string|false|none||none|
|»» endAt|string|false|none||none|
|» timestamp|integer|true|none||none|

## GET 根据适用范围获取公告list

GET /api/announcements/all/{TargetRole}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|TargetRole|path|string| 是 ||参数为：Merchants,Customers,Riders,All，4选1|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "announceId": "9ea673c35bd342abaff1a6ab936ed299",
      "title": "重要消息",
      "content": "今天是感恩节",
      "targetRole": null,
      "startAt": "2025-08-11T00:00:00",
      "endAt": "2025-08-14T00:00:00"
    },
    {
      "announceId": "d870b300c34147729b7b5db1f60da1e2",
      "title": "1",
      "content": "sit anim",
      "targetRole": null,
      "startAt": "2023-08-11T00:00:00",
      "endAt": "2024-12-11T00:00:00"
    }
  ],
  "timestamp": 1754966607859
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
|»» announceId|string|true|none||none|
|»» title|string|true|none||none|
|»» content|string|true|none||none|
|»» targetRole|null|true|none||none|
|»» startAt|string|true|none||none|
|»» endAt|string|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 根据公告id修改公告

PATCH /api/announcements/{announcementId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|announcementId|path|string| 是 ||none|
|title|query|string| 否 ||none|
|content|query|string| 否 ||none|
|targetRole|query|string| 否 ||none|
|startAt|query|string| 否 ||none|
|endAt|query|string| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1754967324268
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "无相关数据，ID: 1",
  "data": null,
  "timestamp": 1754967274585
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据公告id获取公告

GET /api/announcements/{announcementId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|announcementId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "announceId": "9ea673c35bd342abaff1a6ab936ed299",
    "title": "重要消息",
    "content": "今天是感恩节",
    "targetRole": null,
    "startAt": "2025-08-11T00:00:00",
    "endAt": "2025-08-14T00:00:00"
  },
  "timestamp": 1754967891543
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "string",
  "data": null,
  "timestamp": 0
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
|» data|object|true|none||none|
|»» announceId|string|true|none||none|
|»» title|string|true|none||none|
|»» content|string|true|none||none|
|»» targetRole|null|true|none||none|
|»» startAt|string|true|none||none|
|»» endAt|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/Reconciliation

## POST 新增Reconciliation实体

POST /api/reconciliations

> Body 请求参数

```json
{
  "PeriodStart": "2025-08-11T00:00:00",
  "PeriodEnd": "2025-08-11T00:00:00",
  "ReconType": 3,
  "OrderIds": [
    "04d86dd0b06340f7b0f8787b332c692c"
  ]
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|body|body|object| 否 ||none|
|» PeriodStart|body|string| 是 ||none|
|» PeriodEnd|body|string| 是 ||none|
|» ReconType|body|integer| 是 ||none|
|» OrderIds|body|[string]| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1754984889891
}
```

> 500 Response

```json
{
  "code": 1000,
  "message": "至少要有一个order",
  "data": null,
  "timestamp": 1754985906387
}
```

### 返回结果

|状态码|状态码含义|说明|数据模型|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|none|Inline|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|none|Inline|

### 返回数据结构

状态码 **200**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **500**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 自定义筛选Reconciliations

GET /api/reconciliations

目前4种筛选方法（可增加筛选方法，可更改排序方法）：

1.获取已经被解决的异常（isResolved）

2.获取未被解决的异常（isResolved）

3.获取对应类型的异常（reconType）

4.不做筛选，获取所有的异常（）

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|
|isResolved|query|boolean| 否 ||none|
|reconType|query|integer| 否 ||（1 = 金额不匹配、2 = 订单数量不一致、3 = 重复结算等）|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": [
    {
      "reconId": "6ff0d222c9e944989f1859b3dc8c6477",
      "periodStart": "2025-08-11T00:00:00",
      "periodEnd": "2025-08-11T00:00:00",
      "reconType": 3,
      "diffAmount": -49,
      "affectedOrders": 2,
      "isResolved": false,
      "orderIdList": [
        "04d86dd0b06340f7b0f8787b332c692c",
        "c0e1c12ea45d4d86aa2021b5266141f5"
      ]
    },
    {
      "reconId": "ac755ccc173f4b87ab991b15cc4de879",
      "periodStart": "2025-08-11T00:00:00",
      "periodEnd": "2025-08-11T00:00:00",
      "reconType": 3,
      "diffAmount": 0,
      "affectedOrders": 1,
      "isResolved": false,
      "orderIdList": []
    },
    {
      "reconId": "adce4d923b2e4c20aa47a2ba729d372d",
      "periodStart": "2025-08-11T00:00:00",
      "periodEnd": "2025-08-11T00:00:00",
      "reconType": 3,
      "diffAmount": -49,
      "affectedOrders": 1,
      "isResolved": false,
      "orderIdList": []
    }
  ],
  "timestamp": 1755052134806
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
|»» reconId|string|true|none||none|
|»» periodStart|string|true|none||none|
|»» periodEnd|string|true|none||none|
|»» reconType|integer|true|none||none|
|»» diffAmount|integer|true|none||none|
|»» affectedOrders|integer|true|none||none|
|»» isResolved|boolean|true|none||none|
|»» orderIdList|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据reconId获取其实体和关联的orderID

GET /api/reconciliations/{reconId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|reconId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": {
    "reconId": "6ff0d222c9e944989f1859b3dc8c6477",
    "periodStart": "2025-08-11T00:00:00",
    "periodEnd": "2025-08-11T00:00:00",
    "reconType": 3,
    "diffAmount": -49,
    "affectedOrders": 2,
    "isResolved": false,
    "orderIdList": [
      "04d86dd0b06340f7b0f8787b332c692c",
      "c0e1c12ea45d4d86aa2021b5266141f5"
    ]
  },
  "timestamp": 1754988847207
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "获取对账异常主体失败,对象不存在",
  "data": null,
  "timestamp": 1755053060257
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
|» data|object|true|none||none|
|»» reconId|string|true|none||none|
|»» periodStart|string|true|none||none|
|»» periodEnd|string|true|none||none|
|»» reconType|integer|true|none||none|
|»» diffAmount|integer|true|none||none|
|»» affectedOrders|integer|true|none||none|
|»» isResolved|boolean|true|none||none|
|»» orderIdList|[string]|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 管理员解决目标异常

PATCH /api/reconciliations/{reconId}/resolve

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|reconId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "操作成功",
  "data": true,
  "timestamp": 1755052722386
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "获取对账异常主体列表失败",
  "data": null,
  "timestamp": 1755052889609
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/cupon

## GET 返回user的couponId列表

GET /api/users/{userId}/coupons

目前4种筛选方法（可增加筛选方法，可更改排序优先级）：

1.获取目前可用的优惠券（isActive）

2.获取目前过期的优惠券（isActive）

3.获取满足满减要求的可用优惠券（amount）

4.不做筛选，获取该用户所有的优惠券（）

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|isActive|query|boolean| 否 ||是否在有效期内|
|amount|query|number| 否 ||优惠前金额|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "顾客优惠券信息获取成功",
  "data": [
    "69876272f4b3486a86cd23597825199d",
    "69876272f4b3486a86cd23597825188d"
  ],
  "timestamp": 1755139914950
}
```

> 404 Response

```json
{
  "code": 50001,
  "message": "无该用户",
  "data": null,
  "timestamp": 1755140052963
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## GET 根据主键获取coupon详情

GET /api/coupons/{couponId}

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|couponId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "优惠券信息获取成功",
  "data": {
    "couponId": "69876272f4b3486a86cd23597825199d",
    "userId": "1d804350373d47b2b66feb88f8403e7d",
    "faceValue": 10,
    "threshold": 10,
    "startTime": "2025-08-12T10:25:10",
    "endTime": "2025-08-28T10:25:24"
  },
  "timestamp": 1755140806636
}
```

> 404 Response

```json
{
  "code": 1006,
  "message": "无该优惠券",
  "data": null,
  "timestamp": 1755140842933
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
|» data|object|true|none||none|
|»» couponId|string|true|none||none|
|»» userId|string|true|none||none|
|»» faceValue|integer|true|none||none|
|»» threshold|integer|true|none||none|
|»» startTime|string|true|none||none|
|»» endTime|string|true|none||none|
|» timestamp|integer|true|none||none|

状态码 **404**

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|» code|integer|true|none||none|
|» message|string|true|none||none|
|» data|null|true|none||none|
|» timestamp|integer|true|none||none|

## POST 管理员发放优惠券

POST /api/admin/{adminId}/coupons

有3种发放方法
（默认发放给全体用户，可以指定userId来指定用户补偿优惠券。同理，可以增加为特定用户发放，如：根据等级/订单量/地址）：
1.只指定起止时间，面额和门槛完全随机（随机范围目前是面额（5～15），门槛（面额+5～15），下同）
2.指定起止时间，面额固定，门槛随机
3.指定起止时间，面额和门槛

> Body 请求参数

```json
{
  "startTime": "string",
  "endTime": "string",
  "faceValue": 0,
  "threshold": 0,
  "userId": "string"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|body|body|object| 否 ||none|
|» startTime|body|string| 是 ||none|
|» endTime|body|string| 是 ||none|
|» faceValue|body|number| 否 | 面额|none|
|» threshold|body|number| 否 | 门槛|none|
|» userId|body|string| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "优惠券发放成功",
  "data": true,
  "timestamp": 1755151283368
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/Complaint

## GET 根据ComplaintId获取Complaint详情

GET /api/complaints/{complaintId}

//投诉主体当前状态
 public enum ComplaintStatus
        Default  = 0,         // 处理中
        Resolved = 1,         // 已解决
        Cancelled = 2,        // 关闭  

//投诉对象
public enum Role
        User = 0,
        Merchant = 1,
        Rider = 2,

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|complaintId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "查看投诉详情成功",
  "data": {
    "complaintId": "2044bf5a6540490c89f8a4ee6c9515af",
    "orderId": "04d86dd0b06340f7b0f8787b332c692c",
    "complainantId": "1d804350373d47b2b66feb88f8403e7d",
    "cmplRole": 1,
    "cmplDescription": "二也口点器。市类决办料反。",
    "cmplStatus": 0,
    "createdAt": "2025-08-19T05:05:03.397683"
  },
  "timestamp": 1755579977398
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
|»» complaintId|string|true|none||none|
|»» orderId|string|true|none||none|
|»» complainantId|string|true|none||none|
|»» cmplRole|integer|true|none||none|
|»» cmplDescription|string|true|none||none|
|»» cmplStatus|integer|true|none||none|
|»» createdAt|string|true|none||none|
|» timestamp|integer|true|none||none|

## POST 用户创建投诉实体

POST /api/complaints

返回投诉主体ID

> Body 请求参数

```json
{
  "orderId": "62",
  "complainantId": "87",
  "cmplRole": 2,
  "cmplDescription": "出物厂局值部。王置写真当京。并格研经半很公结。何收众石市增品包只开。然存关地他育书多论。求许但学率程件受。"
}
```

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|body|body|object| 否 ||none|
|» orderId|body|string| 是 ||none|
|» complainantId|body|string| 是 | 用户ID|none|
|» cmplRole|body|integer| 是 | 投诉对象|none|
|» cmplDescription|body|string| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|» cmplRole|1|
|» cmplRole|2|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "查看投诉详情成功",
  "data": "2044bf5a6540490c89f8a4ee6c9515af",
  "timestamp": 1755579903616
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
|» data|string|true|none||none|
|» timestamp|integer|true|none||none|

## GET 自定义筛选complaints

GET /api/complaints

目前支持4种筛选方式(status可以为null，表示获取所有状态)
1. 获取目标商家目标状态的投诉单（merchantId，status）
2. 获取目标用户目标状态的投诉单（userId，status）
3. 管理员获取目标状态投诉单（adminId，status）
4. 不限制

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|query|string| 否 ||none|
|merchantId|query|string| 否 ||none|
|status|query|integer| 否 ||none|
|adminId|query|string| 否 ||none|
|size|query|integer| 否 ||none|
|page|query|integer| 否 ||none|

#### 枚举值

|属性|值|
|---|---|
|status|0|
|status|1|
|status|2|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "获取列表成功",
  "data": [
    "2044bf5a6540490c89f8a4ee6c9515af"
  ],
  "timestamp": 1755580641450
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
|» data|[string]|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 管理员审批投诉申请

PATCH /api/admin/{adminId}complaints/{complaintId}/audit

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|adminId|path|string| 是 ||none|
|complaintId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "更新投诉详情成功",
  "data": true,
  "timestamp": 1755580227778
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

## PATCH 用户取消投诉申请

PATCH /api/users/{userId}complaints/{complaintId}/cancel

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|userId|path|string| 是 ||none|
|complaintId|path|string| 是 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "关闭投诉详情成功",
  "data": true,
  "timestamp": 1755580312317
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
|» data|boolean|true|none||none|
|» timestamp|integer|true|none||none|

# merchant/复合路由

## GET 根据时间范围获取营业数据

GET /api/operations/recent

支持3种筛选方式
1. 设置起止时间（start，end）
2. 不设置起止时间（）
3. 设置起或止时间（start/end）

### 请求参数

|名称|位置|类型|必选|中文名|说明|
|---|---|---|---|---|---|
|start|query|string| 否 ||none|
|end|query|string| 否 ||none|

> 返回示例

> 200 Response

```json
{
  "code": 0,
  "message": "查看平台运营数据成功",
  "data": {
    "orderQuantity": 3,
    "cancelledOrderQuantity": 0,
    "aftersalesCompletedOrderQuantity": 1,
    "userQuantity": 4,
    "merchantQuantity": 6,
    "riderQuantity": 1
  },
  "timestamp": 1755659466545
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
|»» orderQuantity|integer|true|none|订单总数|none|
|»» cancelledOrderQuantity|integer|true|none|取消的订单总数|none|
|»» aftersalesCompletedOrderQuantity|integer|true|none|售后的订单|none|
|»» userQuantity|integer|true|none|用户数量|none|
|»» merchantQuantity|integer|true|none|商家数量|none|
|»» riderQuantity|integer|true|none|骑手数量|none|
|» timestamp|integer|true|none||none|

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

<h2 id="tocS_APIResponse">APIResponse</h2>

<a id="schemaapiresponse"></a>
<a id="schema_APIResponse"></a>
<a id="tocSapiresponse"></a>
<a id="tocsapiresponse"></a>

```json
{
  "code": 0,
  "message": "string",
  "data": {
    "merchantId": "string",
    "merchantName": "example.com",
    "status": 0,
    "contactInfo": "user@example.com",
    "location": "string"
  }
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|code|integer(int32)|true|none||none|
|message|string|true|none||none|
|data|object|true|none||none|
|» merchantId|string|true|none||none|
|» merchantName|string(hostname)|true|none||none|
|» status|integer|true|none||none|
|» contactInfo|string(email)|true|none||none|
|» location|string|true|none||none|

<h2 id="tocS_ApiResponse">ApiResponse</h2>

<a id="schemaapiresponse"></a>
<a id="schema_ApiResponse"></a>
<a id="tocSapiresponse"></a>
<a id="tocsapiresponse"></a>

```json
{
  "code": 0,
  "message": "string",
  "data": null,
  "timestamp": 0
}

```

### 属性

|名称|类型|必选|约束|中文名|说明|
|---|---|---|---|---|---|
|code|integer|true|none||none|
|message|string|true|none||none|
|data|null|true|none||none|
|timestamp|integer|true|none||none|

