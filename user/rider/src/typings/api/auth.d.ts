declare namespace Api {
  /**
   * namespace Auth
   *
   * backend api module: "auth"
   */
  namespace Auth {
    interface LoginToken {
      token: string;
      refreshToken: string;
    }

    interface UserInfo {
      userId: string;
      userName: string;
      roles: string[];
      buttons: string[];
    }

    // 骑手登录响应
    interface LoginResponse {
      code: number;
      message?: string;
      data?: {
        Id?: string;
        Token?: string;
        RefreshToken?: string;
      };
      // 兼容其他可能的字段名
      Id?: string;
      Token?: string;
      RefreshToken?: string;
      token?: string;
      refreshToken?: string;
      accessToken?: string;
    }
  }
}
