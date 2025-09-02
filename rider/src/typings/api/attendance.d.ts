declare global {
  namespace Api {
    namespace Attendance {
      // 考勤记录
      export interface AttendanceRecord {
        id: string;
        riderId: string;
        date: string;
        checkInTime?: string;
        checkOutTime?: string;
        status: 'present' | 'absent' | 'late' | 'leave';
        location?: string;
        notes?: string;
        workHours?: number;
      }

      // 获取考勤记录列表响应
      export interface AttendanceListResponse {
        code: number;
        data: {
          records: AttendanceRecord[];
          total: number;
          page: number;
          pageSize: number;
        };
        message: string;
        timestamp: number;
      }

      // 创建考勤记录响应
      export interface AttendanceResponse {
        code: number;
        data: AttendanceRecord;
        message: string;
        timestamp: number;
      }

      // 更新考勤记录响应
      export interface UpdateAttendanceResponse {
        code: number;
        data: AttendanceRecord;
        message: string;
        timestamp: number;
      }

      // 考勤统计
      export interface AttendanceStatistics {
        total: number;
        present: number;
        late: number;
        absent: number;
        leave: number;
        attendanceRate: number;
      }
    }
  }
}

export {};
