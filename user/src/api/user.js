import request from '@/utils/request.js'

// 用户认证相关API
export const authAPI = {
  // 用户登录
  login: (account, password) => {
    return request.post('/api/users/login', {
      account,
      password
    })
  },

  // 用户注册
  register: (userData) => {
    return request.post('/api/users/register', {
      account: userData.account,
      nickName: userData.nickName,
      gender: userData.gender,
      birthday: userData.birthday,
      password: userData.password
    })
  },

  // 重置密码
  resetPassword: (userId, oldPassword, newPassword) => {
    return request.post(`/api/users/${userId}/password/reset`, {
      oldPassword,
      newPassword
    })
  },

  // 用户注销
  logout: (userId) => {
    return request.post('/api/users/logout', {
      userId
    })
  }
}

// 用户信息相关API
export const userAPI = {
  // 获取用户信息列表
  getUserList: (params = {}) => {
    const { page, size } = params
    return request.get('/api/users', {
      params: {
        page,
        size
      }
    })
  },

  // 根据id获取用户信息
  getUserById: (userId) => {
    return request.get(`/api/users/${userId}`)
  },

  // 根据id部分修改用户信息
  updateUser: (userId, userData) => {
    return request.patch(`/api/users/${userId}`, userData)
  },

  // 根据id部分删除用户信息
  deleteUser: (userId) => {
    return request.delete(`/api/users/${userId}`)
  }
}

// 用户收藏相关API
export const favoriteAPI = {
  // 根据id获取用户收藏的商品列表
  getUserFavorites: (userId, params = {}) => {
    const { page, size } = params
    return request.get(`/api/users/${userId}/favorites`, {
      params: {
        page,
        size
      }
    })
  },

  // 用户添加收藏夹
  addToFavorites: (userId, dishId) => {
    return request.post(`/api/users/${userId}/favorites`, {
      dishId
    })
  },

  // 根据id删除收藏内容
  removeFavorite: (userId, favoriteId) => {
    return request.delete(`/api/users/${userId}/favorites/${favoriteId}`)
  },

  // 检查菜品是否被收藏
  checkFavoriteStatus: (userId, dishId) => {
    return request.get(`/api/users/${userId}/favorites/${dishId}`)
  }
}


// 用户评论相关API
export const reviewAPI = {
  // 根据id获取用户发布的所有评论
  getUserReviews: (userId, params = {}) => {
    const { page, size } = params
    return request.get(`/api/users/${userId}/review`, {
      params: {
        page,
        size
      }
    })
  },

  // 用户提交评论
  submitReview: (userId, reviewData) => {
    return request.post(`/api/users/${userId}/review`, {
      orderId: reviewData.orderId,
      dishId: reviewData.dishId,
      rating: reviewData.rating,
      content: reviewData.content,
      isAnonymous: reviewData.isAnonymous
    })
  },

  // 根据id删除评论
  deleteReview: (userId, reviewId) => {
    return request.delete(`/api/users/${userId}/reviews/${reviewId}`)
  }
}

// 用户投诉相关API
export const complaintAPI = {
  // 根据id获取用户提交的所有投诉
  getUserComplaints: (userId, params = {}) => {
    const { page, size } = params
    return request.get(`/api/users/${userId}/complaints`, {
      params: {
        page,
        size
      }
    })
  },

  // 用户添加投诉
  submitComplaint: (userId, complaintData) => {
    return request.post(`/api/users/${userId}/complaints`, {
      orderId: complaintData.orderId,
      description: complaintData.description
    })
  },

  // 根据id删除投诉
  deleteComplaint: (userId, complaintId) => {
    return request.delete(`/api/users/${userId}/complaints/${complaintId}`)
  }
}


// 地址管理API
export const addressAPI = {
  // 获取用户地址列表
  getUserAddresses: (userId, params = {}) => {
    const { page, size } = params
    // 只有当 page 和 size 有有效值时才加入 params
    const queryParams = {}
    if (page !== undefined && page !== null) queryParams.page = page
    if (size !== undefined && size !== null) queryParams.size = size
    
    return request.get(`/api/users/${userId}/addresses`, {
      params: queryParams
    })
  },

  // 添加新地址
  addAddress: (userId, addressData) => {
    return request.post(`/api/users/${userId}/addresses`, {
      receiverName: addressData.receiverName,
      receiverPhone: addressData.receiverPhone,
      detailedAddress: addressData.detailedAddress,
      isDefault: addressData.isDefault || 0
    })
  },

  // 更新地址
  updateAddress: (userId, addressId, addressData) => {
    return request.patch(`/api/users/${userId}/addresses/${addressId}`, {
      receiverName: addressData.receiverName,
      receiverPhone: addressData.receiverPhone,
      detailedAddress: addressData.detailedAddress,
      isDefault: addressData.isDefault
    })
  },

  // 删除地址
  deleteAddress: (userId, addressId) => {
    return request.delete(`/api/users/${userId}/addresses/${addressId}`)
  }
}

// 导出默认的request实例，供其他模块使用
export default request