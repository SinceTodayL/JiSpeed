using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Entities.Rider;
using JISpeed.Api.DTOS;

namespace JISpeed.Api.Controllers
{
    // 考勤管理控制器
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // === 签到签退操作 ===
        // 骑手签到
        // <param name="riderId">骑手ID</param>
        // <returns>考勤记录</returns>
        [HttpPost("checkin/{riderId}")]
        public async Task<ActionResult<AttendanceDTO>> CheckIn(string riderId)
        {
            try
            {
                var attendance = await _attendanceService.CheckInAsync(riderId, DateTime.Now);
                var dto = MapToDto(attendance);
                return Ok(new { message = "签到成功", data = dto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 骑手签退
        // <param name="riderId">骑手ID</param>
        // <returns>考勤记录</returns>
        [HttpPost("checkout/{riderId}")]
        public async Task<ActionResult<AttendanceDTO>> CheckOut(string riderId)
        {
            try
            {
                var attendance = await _attendanceService.CheckOutAsync(riderId, DateTime.Now);
                var dto = MapToDto(attendance);
                return Ok(new { message = "签退成功", data = dto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 获取骑手今日考勤
        // <param name="riderId">骑手ID</param>
        // <returns>今日考勤记录</returns>
        [HttpGet("today/{riderId}")]
        public async Task<ActionResult<AttendanceDTO>> GetTodayAttendance(string riderId)
        {
            var attendance = await _attendanceService.GetTodayAttendanceAsync(riderId);
            if (attendance == null)
            {
                return NotFound(new { message = "今日无考勤记录" });
            }

            var dto = MapToDto(attendance);
            return Ok(new { data = dto });
        }

        // === 考勤查询 ===
        // 获取考勤记录详情
        // <param name="attendanceId">考勤ID</param>
        // <returns>考勤记录</returns>
        [HttpGet("{attendanceId}")]
        public async Task<ActionResult<AttendanceDTO>> GetAttendance(string attendanceId)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(attendanceId);
            if (attendance == null)
            {
                return NotFound(new { message = "考勤记录不存在" });
            }

            var dto = MapToDto(attendance);
            return Ok(new { data = dto });
        }

        // 获取所有考勤记录
        // <returns>考勤记录列表</returns>
        [HttpGet]
        public async Task<ActionResult<List<AttendanceDTO>>> GetAllAttendances()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 获取骑手考勤记录
        // <param name="riderId">骑手ID</param>
        // <returns>考勤记录列表</returns>
        [HttpGet("rider/{riderId}")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetRiderAttendances(string riderId)
        {
            var attendances = await _attendanceService.GetRiderAttendancesAsync(riderId);
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 按日期范围查询考勤记录
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤记录列表</returns>
        [HttpGet("daterange")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetAttendancesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var attendances = await _attendanceService.GetAttendancesByDateRangeAsync(startDate, endDate);
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 获取骑手指定日期范围的考勤记录
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤记录列表</returns>
        [HttpGet("rider/{riderId}/daterange")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetRiderAttendancesByDateRange(
            string riderId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var attendances = await _attendanceService.GetRiderAttendancesByDateRangeAsync(riderId, startDate, endDate);
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // === 考勤状态查询 ===
        // 获取迟到记录
        // <returns>迟到记录列表</returns>
        [HttpGet("late")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetLateAttendances()
        {
            var attendances = await _attendanceService.GetLateAttendancesAsync();
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 获取缺勤记录
        // <returns>缺勤记录列表</returns>
        [HttpGet("absent")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetAbsentAttendances()
        {
            var attendances = await _attendanceService.GetAbsentAttendancesAsync();
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 获取未签退记录
        // <returns>未签退记录列表</returns>
        [HttpGet("not-checked-out")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetNotCheckedOutAttendances()
        {
            var attendances = await _attendanceService.GetNotCheckedOutAttendancesAsync();
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // 获取今日所有考勤记录
        // <returns>今日考勤记录列表</returns>
        [HttpGet("today")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetTodayAttendances()
        {
            var attendances = await _attendanceService.GetTodayAttendancesAsync();
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // === 考勤统计 ===
        // 获取迟到统计
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>迟到统计</returns>
        [HttpGet("stats/late")]
        public async Task<ActionResult> GetLateStats(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var stats = await _attendanceService.GetLateCountByRiderAsync(startDate, endDate);
            return Ok(new { data = stats });
        }

        // 获取缺勤统计
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>缺勤统计</returns>
        [HttpGet("stats/absent")]
        public async Task<ActionResult> GetAbsentStats(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var stats = await _attendanceService.GetAbsentCountByRiderAsync(startDate, endDate);
            return Ok(new { data = stats });
        }

        // 获取骑手考勤统计
        // <param name="riderId">骑手ID</param>
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤统计</returns>
        [HttpGet("stats/rider/{riderId}")]
        public async Task<ActionResult> GetRiderAttendanceStats(
            string riderId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var stats = await _attendanceService.GetRiderAttendanceStatsAsync(riderId, startDate, endDate);
            return Ok(new { data = stats });
        }

        // === 考勤管理操作 ===
        // 创建考勤记录
        // <param name="request">创建请求</param>
        // <returns>考勤记录</returns>
        [HttpPost]
        public async Task<ActionResult<AttendanceDTO>> CreateAttendance([FromBody] CreateAttendanceRequest request)
        {
            try
            {
                var attendance = await _attendanceService.CreateAttendanceAsync(request.RiderId, request.CheckDate);
                var dto = MapToDto(attendance);
                return CreatedAtAction(nameof(GetAttendance), new { attendanceId = attendance.AttendanceId },
                    new { message = "考勤记录创建成功", data = dto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 标记缺勤
        // <param name="request">缺勤请求</param>
        // <returns>操作结果</returns>
        [HttpPost("mark-absent")]
        public async Task<ActionResult> MarkAbsent([FromBody] MarkAbsentRequest request)
        {
            try
            {
                await _attendanceService.MarkAbsentAsync(request.RiderId, request.CheckDate);
                return Ok(new { message = "标记缺勤成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 计算工作时长
        // <param name="riderId">骑手ID</param>
        // <param name="checkDate">考勤日期</param>
        // <returns>工作时长</returns>
        [HttpGet("working-hours/{riderId}")]
        public async Task<ActionResult> GetWorkingHours(string riderId, [FromQuery] DateTime checkDate)
        {
            var workingHours = await _attendanceService.CalculateWorkingHoursAsync(riderId, checkDate);
            if (workingHours == null)
            {
                return NotFound(new { message = "无法计算工作时长，签到或签退时间缺失" });
            }

            return Ok(new
            {
                data = new
                {
                    workingHours = workingHours.Value.TotalHours,
                    formattedHours = $"{workingHours.Value.Hours}小时{workingHours.Value.Minutes}分钟"
                }
            });
        }

        // 获取考勤报表
        // <param name="startDate">开始日期</param>
        // <param name="endDate">结束日期</param>
        // <returns>考勤报表</returns>
        [HttpGet("report")]
        public async Task<ActionResult<List<AttendanceDTO>>> GetAttendanceReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var attendances = await _attendanceService.GetAttendanceReportAsync(startDate, endDate);
            var dtos = attendances.Select(MapToDto).ToList();
            return Ok(new { data = dtos, total = dtos.Count });
        }

        // === 私有方法 ===
        private AttendanceDTO MapToDto(Attendance attendance)
        {
            return new AttendanceDTO
            {
                AttendanceId = attendance.AttendanceId,
                CheckDate = attendance.CheckDate,
                CheckInAt = attendance.CheckInAt,
                CheckoutAt = attendance.CheckoutAt,
                IsLate = attendance.IsLate,
                IsAbsent = attendance.IsAbsent
            };
        }
    }

    // === 请求DTO ===
    public class CreateAttendanceRequest
    {
        public required string RiderId { get; set; }
        public required DateTime CheckDate { get; set; }
    }

    public class MarkAbsentRequest
    {
        public required string RiderId { get; set; }
        public required DateTime CheckDate { get; set; }
    }
}
