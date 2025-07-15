# 全局异常处理中间件使用说明

## 概述

全局异常处理中间件用于统一处理应用程序中的所有未捕获异常，避免在每个控制器中重复编写异常处理代码，提供统一的API响应格式。

## 架构设计

### 1. 异常类层次结构

```
BaseException (抽象基类)
├── BusinessException (业务逻辑异常)
├── ValidationException (参数验证异常)
├── NotFoundException (资源未找到异常)
├── UnauthorizedException (未授权异常)
└── ForbiddenException (禁止访问异常)
```

### 2. 主要文件说明

- **Core/Exceptions/**: 自定义异常类定义
- **Api/Middleware/GlobalExceptionMiddleware.cs**: 核心异常处理中间件
- **Api/Extensions/MiddlewareExtensions.cs**: 中间件扩展方法
- **Api/Controllers/TestController.cs**: 测试控制器（演示用）

## 功能特性

### 1. 统一的响应格式
所有异常都会被转换为统一的 `ApiResponse` 格式：
```json
{
  "code": 错误码,
  "message": "错误描述",
  "data": null,
  "timestamp": 时间戳
}
```

### 2. 智能异常分类处理
- **自定义异常**: 根据异常类型返回对应的错误码和消息
- **系统异常**: 自动映射到合适的HTTP状态码和错误码
- **开发/生产环境**: 生产环境隐藏敏感错误信息

### 3. 完善的日志记录
- **业务异常**: 记录信息级别日志
- **系统异常**: 记录错误级别日志，包含完整堆栈信息
- **请求上下文**: 记录IP地址、User-Agent、请求路径等

### 4. HTTP状态码映射
```
UnauthorizedException -> 401 Unauthorized
ForbiddenException -> 403 Forbidden
NotFoundException -> 404 Not Found
ValidationException -> 400 Bad Request
BusinessException -> 400 Bad Request
系统异常 -> 500 Internal Server Error
```

## 使用方法

### 1. 抛出业务异常
```csharp
// 通用业务异常
throw new BusinessException("库存不足");

// 带错误码的业务异常
throw new BusinessException(ErrorCodes.InsufficientStock, "商品库存不足");

// 资源未找到
throw new NotFoundException("用户", userId);

// 参数验证失败
throw new ValidationException("用户名不能为空");

// 权限相关
throw new UnauthorizedException("请先登录");
throw new ForbiddenException("权限不足");
```

### 2. 在控制器中使用
```csharp
[HttpGet("{id}")]
public async Task<ApiResponse<UserDto>> GetUser(int id)
{
    var user = await _userService.GetByIdAsync(id);
    if (user == null)
    {
        throw new NotFoundException("用户", id);
    }
    
    return ApiResponse<UserDto>.Success(user);
}
```

### 3. 服务层中使用
```csharp
public async Task<User> CreateUserAsync(CreateUserRequest request)
{
    if (await _userRepository.ExistsAsync(request.Username))
    {
        throw new BusinessException(ErrorCodes.ResourceAlreadyExists, "用户名已存在");
    }
    
    // 创建用户逻辑...
}
```

## 测试API端点

项目包含了测试控制器，可以通过以下端点测试异常处理：

- `GET /api/test/success` - 成功响应
- `GET /api/test/business-exception` - 业务异常
- `GET /api/test/validation-exception` - 验证异常
- `GET /api/test/not-found-exception` - 未找到异常
- `GET /api/test/unauthorized-exception` - 未授权异常
- `GET /api/test/forbidden-exception` - 禁止访问异常
- `GET /api/test/system-exception` - 系统异常
- `GET /api/test/argument-exception` - 参数异常
- `GET /api/test/argument-null-exception` - 空参数异常
- `GET /api/test/generic-exception` - 通用异常

## 最佳实践

### 1. 异常使用原则
- **预期的业务场景**: 使用 `BusinessException`
- **参数验证**: 使用 `ValidationException`
- **资源查找**: 使用 `NotFoundException`
- **权限控制**: 使用 `UnauthorizedException` 或 `ForbiddenException`

### 2. 不要过度使用异常
- 异常应该用于异常情况，不是正常的控制流
- 频繁的业务场景考虑返回错误码而不是抛异常

### 3. 自定义异常扩展
如果需要新的异常类型，继承 `BaseException` 并设置合适的错误码：
```csharp
public class PaymentException : BaseException
{
    public PaymentException(string message) 
        : base(ErrorCodes.PaymentFailed, message)
    {
    }
}
```

## 配置说明

中间件已在 `Program.cs` 中配置，确保在管道的最前面注册：
```csharp
// 全局异常处理中间件（应该在管道的最前面）
app.UseGlobalExceptionHandling();
```

## 扩展功能

### 1. 添加更多错误码
在 `ApiResponse.cs` 的 `ErrorCodes` 类中添加新的错误码。

### 2. 自定义日志格式
修改 `GlobalExceptionMiddleware.cs` 中的 `LogException` 方法。

### 3. 集成监控系统
在异常处理方法中添加监控系统的调用，如APM工具。

### 4. 多语言支持
可以结合本地化服务，根据请求头返回不同语言的错误消息。
