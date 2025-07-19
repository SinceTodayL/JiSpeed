注：错误码表请详见Core/Constants/ErrorCodes.cs文件
# JiSpeed 全局异常处理框架

## 目录

1. [架构概述](#1-架构概述)
2. [文件结构](#2-文件结构)
3. [异常处理流程](#3-异常处理流程)
4. [各模块职责](#4-各模块职责)
5. [使用指南](#5-使用指南)
   - [后端开发人员](#5.1-后端开发人员)
   - [前端开发人员](#5.2-前端开发人员)
6. [扩展指南](#6-扩展指南)
7. [最佳实践](#7-最佳实践)
8. [常见问题](#8-常见问题)

## 1. 架构概述

JiSpeed的异常处理框架采用三层架构设计，实现了前后端分离、关注点分离和标准化错误处理：

- **Core层**：定义错误码常量和异常类型，提供模块化的异常工厂
- **API层**：通过全局异常中间件自动捕获并处理异常，转换为标准响应格式
- **前端层**：接收标准化的错误响应，根据错误码执行相应的UI逻辑

这种设计使得业务逻辑层可以专注于业务规则，API层专注于请求处理，前端专注于用户体验，同时保持错误处理的一致性和可维护性。

## 2. 文件结构

```
JISpeed.Core/
├── Constants/
│   └── ErrorCodes.cs              # 所有模块的错误码常量定义
├── Exceptions/
│   ├── BaseException.cs           # 异常基类
│   ├── BusinessException.cs       # 业务逻辑异常类
│   ├── ValidationException.cs     # 数据验证异常类
│   ├── NotFoundException.cs       # 资源未找到异常类
│   └── Modules/                   # 模块化异常工厂
│       ├── RiderExceptions.cs     # 骑手模块异常工厂
│       ├── OrderExceptions.cs     # 订单模块异常工厂
│       ├── MerchantExceptions.cs  # 商家模块异常工厂
│       ├── UserExceptions.cs      # 用户模块异常工厂
│       ├── PaymentExceptions.cs   # 支付模块异常工厂
│       ├── DeliveryExceptions.cs  # 配送模块异常工厂
│       ├── ReviewExceptions.cs    # 评价模块异常工厂
│       ├── AuthExceptions.cs      # 认证模块异常工厂
│       ├── SystemExceptions.cs    # 系统模块异常工厂
│       └── CommonExceptions.cs    # 通用异常工厂

JISpeed.Api/
├── Common/
│   └── ApiResponse.cs                      # 标准API响应类
├── Middleware/
│   └── GlobalExceptionMiddleware.cs        # 全局异常处理中间件
└── Controllers/
     ├── ModuleExceptionTestController.cs    # 模块异常测试控制器
     └── ...                                 # 各模块控制器

ModuleExceptionTest.http 			   # 模块异常测试的HTTP请求测试文件

Documents/
└── ExceptionHandling.md            # 异常处理部分说明文档（此文档）
```

## 3. 异常处理流程

1. **异常产生**：业务逻辑层检测到错误条件（如资源未找到、验证失败等）
2. **异常创建**：通过模块化异常工厂创建包含错误码和消息的异常对象
3. **异常抛出**：业务层将异常抛出，不进行捕获和处理
4. **中间件捕获**：GlobalExceptionMiddleware自动捕获所有未处理的异常
5. **异常转换**：中间件将异常转换为标准API响应格式
6. **状态码映射**：根据异常类型设置适当的HTTP状态码
7. **日志记录**：记录异常详情，包括请求信息和堆栈跟踪
8. **响应返回**：将标准化的错误响应返回给客户端
9. **前端处理**：前端根据错误码和消息执行相应的UI逻辑

## 4. 各模块职责

### Core层

- **ErrorCodes.cs**：定义所有模块的错误码常量，采用分段设计（1000-1999通用错误，2000-2999认证错误等）
- **BaseException及子类**：提供异常基类和特定类型的异常类
- **模块化异常工厂**：为每个业务模块提供创建标准异常的静态方法

### API层

- **ApiResponse**：定义标准API响应格式，包含code、message、data和timestamp字段
- **GlobalExceptionMiddleware**：自动捕获异常，转换为标准响应，设置HTTP状态码
- **控制器**：调用业务逻辑，不需要手动处理异常

### 前端层

- **API客户端**：发送请求，接收响应，解析错误信息
- **错误处理逻辑**：根据错误码执行特定的UI逻辑（如显示错误消息、重定向等）

## 5. 使用指南

### 5.1 后端开发人员

#### 5.1.1 抛出异常

在业务逻辑中，当检测到错误条件时，使用相应模块的异常工厂创建并抛出异常：

// 不要这样做
// ❌ 直接创建异常
```csharp
throw new NotFoundException(ErrorCodes.RiderNotFound, $"骑手 (ID: {riderId}) 未找到");
```

// 正确做法
// ✅ 使用模块化异常工厂（详见Core/Exceptions/Modules里的文件）
```csharp
if (rider == null)
{
    throw RiderExceptions.RiderNotFound(riderId);
}

if (rider.Status != requiredStatus)
{
    throw RiderExceptions.RiderStatusError(riderId, rider.Status, requiredStatus);
}
```

// 示例：在RiderService中检查骑手是否存在
```csharp
public async Task<RiderDto> GetRiderByIdAsync(string riderId)
{
    // 从数据库查询骑手
    var rider = await _riderRepository.GetByIdAsync(riderId);
    
    // 如果骑手不存在，抛出RiderNotFound异常
    // RiderExceptions.RiderNotFound：创建一个包含错误码10001和格式化错误消息的NotFoundException
    if (rider == null)
    {
        throw RiderExceptions.RiderNotFound(riderId);
    }
    
    // 如果骑手状态异常，抛出RiderStatusError异常
    // RiderExceptions.RiderStatusError：创建一个包含错误码10002和状态信息的BusinessException
    if (rider.Status == RiderStatus.Suspended)
    {
        throw RiderExceptions.RiderStatusError(
            riderId, 
            rider.Status.ToString(), 
            RiderStatus.Active.ToString()
        );
    }
    
    // 正常业务逻辑
    return _mapper.Map<RiderDto>(rider);
}
```

#### 5.1.2 控制器实现

控制器不需要手动处理异常，直接调用业务逻辑：

// 示例：
```csharp
[HttpGet("{id}")]
public async Task<ApiResponse<RiderDto>> GetRider(string id)
{
    // 直接调用业务服务，不需要try-catch
    // 如果发生异常，会被GlobalExceptionMiddleware自动捕获
    var rider = await _riderService.GetRiderByIdAsync(id);
    
    // ApiResponse.Success：创建一个成功的API响应，code=0，包含数据
    return ApiResponse.Success(rider);
}

[HttpPost]
public async Task<ApiResponse<RiderDto>> CreateRider(CreateRiderRequest request)
{
    // 业务逻辑可能会抛出各种异常，如验证错误、重复数据等
    // 这些异常会被中间件捕获并转换为适当的HTTP响应
    var rider = await _riderService.CreateRiderAsync(request);
    
    // 返回成功响应，包含自定义成功消息
    return ApiResponse.Success(rider, "骑手创建成功");
}
```

#### 5.1.3 添加新的错误码

1. 在`ErrorCodes.cs`中添加新的错误码常量，遵循模块分段原则
2. 在相应模块的异常工厂类中添加新的方法

```csharp
// 在ErrorCodes.cs中
public static class ErrorCodes
{
    // 骑手模块 (10000-10999)
    public const int RiderNotFound = 10001;
    public const int RiderStatusError = 10002;
    // 添加新错误码
    public const int RiderVehicleInvalid = 10011;
}

// 在RiderExceptions.cs中
public static class RiderExceptions
{
    // 添加新方法（继承自BusinessException异常类）
    public static BusinessException RiderVehicleInvalid(string riderId, string vehicleNumber, string reason)
    {
        return new BusinessException(ErrorCodes.RiderVehicleInvalid,
            $"骑手 (ID: {riderId}) 的车辆 (车牌: {vehicleNumber}) 信息无效: {reason}");
    }
}
```

### 5.2 前端开发人员

#### 5.2.1 处理错误响应

前端会收到统一格式的错误响应：

```json
{
  "code": 10001,
  "message": "骑手 (编号: R12345) 未找到",
  "data": null,
  "timestamp": 1752895433205
}
```

处理这些响应的示例代码（注：此部分本人不是特别了解，可以去问问ai讨论一下，此处仅给一个例子）：

```javascript
/**
 * 发送API请求并处理可能的错误
 * @param {string} riderId - 骑手ID
 * @returns {Object} 成功时返回骑手数据，失败时返回错误信息
 */
async function fetchRider(riderId) {
  try {
    // 发送API请求
    const response = await api.get(`/api/riders/${riderId}`);
    
    // 成功响应：code为0，data包含实际数据
    return {
      success: true,
      data: response.data.data
    };
  } catch (error) {
    // 检查是否有响应数据
    if (error.response && error.response.data) {
      const { code, message } = error.response.data;
      
      // 根据错误码进行特定处理
      switch (code) {
        case 10001: // RiderNotFound - 骑手未找到
          // 显示错误通知并导航到骑手列表页
          showErrorNotification(message);
          navigateToRiderList();
          break;
          
        case 10002: // RiderStatusError - 骑手状态错误
          // 显示警告并刷新骑手状态
          showWarningNotification(message);
          refreshRiderStatus(riderId);
          break;
          
        case 2001: // Unauthorized - 未授权
          // 显示登录提示并跳转到登录页
          showLoginPrompt();
          navigateToLogin();
          break;
          
        default:
          // 通用错误处理
          showErrorNotification(message || '操作失败，请稍后重试');
      }
      
      // 返回错误信息
      return {
        success: false,
        error: {
          code,
          message
        }
      };
    } else {
      // 网络错误或其他客户端错误
      showErrorNotification('网络异常，请检查网络连接');
      return {
        success: false,
        error: {
          code: -1,
          message: '网络异常'
        }
      };
    }
  }
}

/**
 * 在组件中使用
 */
async function loadRiderData() {
  setLoading(true);
  
  try {
    const result = await fetchRider(riderId);
    
    if (result.success) {
      // 处理成功响应
      setRiderData(result.data);
    } else {
      // 错误已在fetchRider中处理
      console.log('加载骑手数据失败:', result.error);
    }
  } finally {
    setLoading(false);
  }
}
```

#### 5.2.2 错误码文档

前端开发人员应参考`ErrorCodes.cs`文件，了解所有可能的错误码及其含义。对于特定模块的开发，重点关注该模块的错误码段。

## 6. 扩展指南（）
注：如果涉及到要增加一个异常类等大变动可以直接联系徐浩然改动。

### 6.1 添加新的异常类型

如果现有的异常类型不满足需求，可以添加新的异常类：

```csharp
// 在Core层添加新的异常类
public class ConflictException : BaseException
{
    public ConflictException(int errorCode, string message) 
        : base(errorCode, message)
    {
    }
}

// 在GlobalExceptionMiddleware中添加处理逻辑
private static int GetHttpStatusCode(Exception exception)
{
    return exception switch
    {
        // 添加新的映射
        ConflictException => (int)HttpStatusCode.Conflict, // 409
        // 其他映射...
        _ => (int)HttpStatusCode.InternalServerError // 500
    };
}
```

### 6.2 添加新的模块异常工厂

为新模块添加异常工厂类：

```csharp
// 在Core/Exceptions/Modules目录下添加新文件
public class PromotionExceptions
{
    // 促销活动未找到
    public static NotFoundException PromotionNotFound(string promotionId)
    {
        return new NotFoundException(ErrorCodes.PromotionNotFound, 
            $"促销活动 (ID: {promotionId}) 未找到");
    }
    
    // 促销活动已过期
    public static BusinessException PromotionExpired(string promotionId, DateTime endTime)
    {
        return new BusinessException(ErrorCodes.PromotionExpired,
            $"促销活动 (ID: {promotionId}) 已于 {endTime:yyyy-MM-dd HH:mm:ss} 过期");
    }
}
```

### 6.3 自定义中间件行为

可以根据需要修改GlobalExceptionMiddleware的行为：

```csharp
// 例如，添加特定环境下的详细错误信息
private async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    // ...
    
    if (_environment.IsDevelopment() || _environment.IsStaging())
    {
        // 在开发和测试环境中添加详细错误信息
        response.DevelopmentDetails = new
        {
            ExceptionType = exception.GetType().Name,
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.Message
        };
    }
    
    // ...
}
```

## 7. 最佳实践

### 7.1 异常命名和消息

- 错误码应具有描述性，并遵循模块分段原则
- 错误消息应清晰明了，包含必要的上下文信息（如ID、状态等）
- 消息应面向最终用户，避免技术术语

### 7.2 异常粒度

- 为每种具体的错误情况创建专门的异常方法
- 避免使用通用错误码，应使用模块特定的错误码

```csharp
// 不要这样做
// ❌ 使用通用错误码
throw new NotFoundException(ErrorCodes.ResourceNotFound, $"订单 {orderId} 未找到");

// 正确做法
// ✅ 使用模块特定错误码
throw OrderExceptions.OrderNotFound(orderId);
```

### 7.3 日志记录

- 业务异常通常记录为Information级别
- 系统异常记录为Error级别，包含完整堆栈跟踪
- 日志应包含足够的上下文信息（请求路径、方法、IP等）

### 7.4 前端错误处理

- 为常见错误码创建统一的处理逻辑
- 对特定错误提供有意义的用户反馈和操作建议
- 在适当情况下重试操作或提供替代方案

## 8. 常见问题

### Q: 如何处理第三方API的异常？

**A**: 捕获第三方API异常并转换为我们的标准异常：

```csharp
try
{
    await _paymentGateway.ProcessPaymentAsync(request);
}
catch (PaymentGatewayException ex)
{
    // 转换为我们的标准异常
    throw PaymentExceptions.PaymentFailed(request.PaymentId, ex.Message);
}
```

### Q: 如何处理批量操作中的多个错误？

**A**: 对于批量操作，可以收集所有错误并一次性返回：

```csharp
public async Task<ApiResponse<BatchResult>> ProcessBatchAsync(List<OrderRequest> requests)
{
    var result = new BatchResult();
    
    foreach (var request in requests)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request);
            result.SuccessItems.Add(new BatchItemResult { Id = order.Id, Status = "success" });
        }
        catch (BaseException ex)
        {
            result.FailedItems.Add(new BatchItemResult 
            { 
                Id = request.OrderId, 
                Status = "failed",
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message
            });
        }
    }
    
    return ApiResponse.Success(result);
}
```

### Q: 如何处理验证错误？

**A**: 使用ValidationException和ModelState：

```csharp
// 在控制器中
if (!ModelState.IsValid)
{
    var errors = ModelState
        .Where(e => e.Value.Errors.Count > 0)
        .Select(e => new { Field = e.Key, Message = e.Value.Errors.First().ErrorMessage })
        .ToList();
        
    throw CommonExceptions.ValidationFailed("请求数据验证失败", errors);
}
```

---
本文档旨在帮助开发人员理解和使用JiSpeed的异常处理框架。如有疑问或建议，直接联系徐浩然即可。
