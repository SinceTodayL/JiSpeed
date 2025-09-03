import axios from 'axios'

// 地址管理API
export const addressAPI = {
  // 获取用户地址列表
  getUserAddresses: (userId) => {
    return new Promise((resolve) => {
      // 模拟API调用
      setTimeout(() => {
        const mockAddresses = [
          {
            addressId: 'addr_001',
            userId: userId,
            receiverName: '张三',
            receiverPhone: '13800138001',
            detailedAddress: '北京市朝阳区建国路1号国贸大厦A座1001室',
            isDefault: 1,
            label: '公司'
          },
          {
            addressId: 'addr_002',
            userId: userId,
            receiverName: '李四',
            receiverPhone: '13800138002',
            detailedAddress: '北京市海淀区中关村大街27号中关村广场B座2002室',
            isDefault: 0,
            label: '家'
          },
          {
            addressId: 'addr_003',
            userId: userId,
            receiverName: '王五',
            receiverPhone: '13800138003',
            detailedAddress: '上海市浦东新区陆家嘴环路1000号恒生银行大厦50层',
            isDefault: 0,
            label: '朋友家'
          }
        ]

        resolve({
          code: 200,
          message: '获取地址列表成功',
          data: {
            addresses: mockAddresses,
            total: mockAddresses.length
          }
        })
      }, 300)
    })
  },

  // 添加新地址
  addAddress: (addressData) => {
    return new Promise((resolve, reject) => {
      // 模拟API调用
      setTimeout(() => {
        // 基本验证
        if (!addressData.receiverName || !addressData.receiverPhone || !addressData.detailedAddress) {
          reject({
            code: 1001,
            message: '地址信息不完整',
            data: null
          })
          return
        }

        // 手机号验证
        const phoneRegex = /^1[3-9]\d{9}$/
        if (!phoneRegex.test(addressData.receiverPhone)) {
          reject({
            code: 1001,
            message: '手机号格式不正确',
            data: null
          })
          return
        }

        const newAddress = {
          addressId: 'addr_' + Date.now(),
          userId: addressData.userId,
          receiverName: addressData.receiverName,
          receiverPhone: addressData.receiverPhone,
          detailedAddress: addressData.detailedAddress,
          isDefault: addressData.isDefault || 0,
          label: addressData.label || '其他',
          createTime: new Date().toISOString()
        }

        resolve({
          code: 200,
          message: '地址添加成功',
          data: {
            address: newAddress
          }
        })
      }, 500)
    })
  },

  // 更新地址
  updateAddress: (addressId, addressData) => {
    return new Promise((resolve, reject) => {
      // 模拟API调用
      setTimeout(() => {
        // 基本验证
        if (!addressData.receiverName || !addressData.receiverPhone || !addressData.detailedAddress) {
          reject({
            code: 1001,
            message: '地址信息不完整',
            data: null
          })
          return
        }

        // 手机号验证
        const phoneRegex = /^1[3-9]\d{9}$/
        if (!phoneRegex.test(addressData.receiverPhone)) {
          reject({
            code: 1001,
            message: '手机号格式不正确',
            data: null
          })
          return
        }

        const updatedAddress = {
          addressId: addressId,
          userId: addressData.userId,
          receiverName: addressData.receiverName,
          receiverPhone: addressData.receiverPhone,
          detailedAddress: addressData.detailedAddress,
          isDefault: addressData.isDefault || 0,
          label: addressData.label || '其他',
          updateTime: new Date().toISOString()
        }

        resolve({
          code: 200,
          message: '地址更新成功',
          data: {
            address: updatedAddress
          }
        })
      }, 500)
    })
  },

  // 删除地址
  deleteAddress: (addressId) => {
    return new Promise((resolve, reject) => {
      // 模拟API调用
      setTimeout(() => {
        // 模拟删除默认地址的检查
        if (addressId === 'addr_001') {
          reject({
            code: 3001,
            message: '不能删除默认地址，请先设置其他地址为默认地址',
            data: null
          })
          return
        }

        resolve({
          code: 200,
          message: '地址删除成功',
          data: {
            deletedAddressId: addressId
          }
        })
      }, 500)
    })
  },

  // 设置默认地址
  setDefaultAddress: (addressId, userId) => {
    return new Promise((resolve) => {
      // 模拟API调用
      setTimeout(() => {
        resolve({
          code: 200,
          message: '默认地址设置成功',
          data: {
            addressId: addressId,
            userId: userId
          }
        })
      }, 300)
    })
  },

  // 获取地址详情
  getAddressDetail: (addressId) => {
    return new Promise((resolve, reject) => {
      // 模拟API调用
      setTimeout(() => {
        // 模拟地址不存在的情况
        if (addressId === 'invalid_addr') {
          reject({
            code: 50003,
            message: '地址不存在',
            data: null
          })
          return
        }

        const mockAddress = {
          addressId: addressId,
          userId: 'USER123',
          receiverName: '张三',
          receiverPhone: '13800138001',
          detailedAddress: '北京市朝阳区建国路1号国贸大厦A座1001室',
          isDefault: 1,
          label: '公司',
          createTime: '2024-01-01T10:00:00Z',
          updateTime: '2024-01-15T15:30:00Z'
        }

        resolve({
          code: 200,
          message: '获取地址详情成功',
          data: {
            address: mockAddress
          }
        })
      }, 300)
    })
  },

  // 地址验证（检查配送范围）
  validateAddress: (address) => {
    return new Promise((resolve, reject) => {
      // 模拟API调用
      setTimeout(() => {
        // 模拟一些地址不在配送范围内
        const unsupportedAreas = ['远郊区', '偏远地区', '海外']
        const isUnsupported = unsupportedAreas.some(area => 
          address.detailedAddress.includes(area)
        )

        if (isUnsupported) {
          reject({
            code: 30003,
            message: '该地址暂不支持配送',
            data: {
              address: address,
              reason: '超出配送范围'
            }
          })
          return
        }

        resolve({
          code: 200,
          message: '地址验证通过',
          data: {
            isValid: true,
            deliveryFee: 5.0,
            estimatedTime: '30-45分钟'
          }
        })
      }, 800)
    })
  }
}

// 导出默认对象
export default addressAPI
