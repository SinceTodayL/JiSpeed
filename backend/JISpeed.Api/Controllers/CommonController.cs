using AutoMapper;
using JISpeed.Api.Common;
using JISpeed.Api.DTOs;
using JISpeed.Core.Constants;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JISpeed.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;
        private readonly IPlatformService  _platformService;
        private readonly IMapper _mapper;

        public CommonController(
            ILogger<CommonController> logger,
            IPlatformService platformService,
            IMapper mapper
        )
        {
            _logger = logger;
            _platformService = platformService;
            _mapper = mapper;
        }
        
        [HttpGet("operations/recent")]
        public async Task<ActionResult<ApiResponse<RecentOperationsDto>>> GetOperationalDataAsync(
            [FromQuery]DateTime? start,
            [FromQuery]DateTime? end)
        {
            try
            {
                _logger.LogInformation("收到查看平台运营数据的请求");
                var entity = await _platformService.GetOperationalDataByTimeRangeAsync(start,end);
                var response = _mapper.Map<RecentOperationsDto>(entity);
                _logger.LogInformation("成功查看平台运营数据");
                return Ok(ApiResponse<RecentOperationsDto>.Success(response, "查看平台运营数据成功"));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "参数验证失败");
                return BadRequest(ApiResponse<object>.Fail(
                    ErrorCodes.ValidationFailed,
                    ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "资源不存在");
                return NotFound(ApiResponse<object>.Fail(
                    ErrorCodes.ResourceNotFound,
                    ex.Message));
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "业务处理异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.GeneralError,
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查看平台运营数据时发生未知异常");
                return StatusCode(500, ApiResponse<object>.Fail(
                    ErrorCodes.SystemError,
                    "系统繁忙，请稍后再试"));
            }
        }
    }

}
