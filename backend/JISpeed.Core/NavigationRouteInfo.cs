namespace JISpeed.Core.DTOs
{
    // 导航路径信息
    public class NavigationRouteInfo
    {
        public string RouteId { get; set; } = string.Empty;
        public double TotalDistance { get; set; } // 总距离（米）
        public int EstimatedDuration { get; set; } // 预计时间（秒）
        public string Polyline { get; set; } = string.Empty; // 路径坐标串
        public List<RouteStep> Steps { get; set; } = new List<RouteStep>(); // 路径步骤
        public string Mode { get; set; } = "driving"; // 导航模式
    }

    // 路径步骤
    public class RouteStep
    {
        public string Instruction { get; set; } = string.Empty; // 导航指令
        public double Distance { get; set; } // 步骤距离（米）
        public int Duration { get; set; } // 步骤时间（秒）
        public string RoadName { get; set; } = string.Empty; // 道路名称
        public string TurnType { get; set; } = string.Empty; // 转向类型
        public List<(decimal longitude, decimal latitude)> Polyline { get; set; } = new List<(decimal longitude, decimal latitude)>();
    }

    // 导航路线请求
    public class NavigationRouteRequest
    {
        public decimal StartLongitude { get; set; }
        public decimal StartLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public decimal EndLatitude { get; set; }
        public string Mode { get; set; } = "driving"; // 导航模式：driving, walking, bicycling, transit
    }

    // 导航更新信息
    public class NavigationUpdateInfo
    {
        public string RouteId { get; set; } = string.Empty;
        public decimal CurrentLongitude { get; set; }
        public decimal CurrentLatitude { get; set; }
        public double CurrentSpeed { get; set; } // 当前速度（米/秒）
        public int RemainingTime { get; set; } // 剩余时间（秒）
        public double RemainingDistance { get; set; } // 剩余距离（米）
        public string NextInstruction { get; set; } = string.Empty; // 下一个导航指令
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // 开始导航请求
    public class StartNavigationRequest
    {
        public string RouteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string VehicleType { get; set; } = "car"; // 车辆类型
        public Dictionary<string, object> Preferences { get; set; } = new Dictionary<string, object>(); // 导航偏好设置
    }

    // 导航会话信息
    public class NavigationSessionInfo
    {
        public string SessionId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "active"; // active, paused, completed, cancelled
        public NavigationRouteInfo RouteInfo { get; set; } = new NavigationRouteInfo();
    }

    // 更新导航状态请求
    public class UpdateNavigationStatusRequest
    {
        public decimal CurrentLongitude { get; set; }
        public decimal CurrentLatitude { get; set; }
        public double CurrentSpeed { get; set; }
        public string CurrentRoadName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // 状态更新
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}