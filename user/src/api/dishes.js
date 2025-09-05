import axios from 'axios'

export const dishesAPI = {
  // 获取菜品详情
  getDishDetail(merchantId, dishId) {
    return axios.get(`/api/merchants/${merchantId}/dishes/${dishId}`)
      .then(res => res.data)
  }
}
