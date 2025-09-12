<template>
  <div class="addresses-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">收货地址</h1>
        <button @click="addAddress" class="add-btn">新增</button>
      </div>
    </div>

    <!-- 地址列表 -->
    <div class="addresses-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>正在加载地址...</p>
      </div>

      <!-- 空状态 -->
      <div v-else-if="addresses.length === 0" class="empty-state">
        <div class="empty-icon">📍</div>
        <h3 class="empty-title">暂无收货地址</h3>
        <p class="empty-desc">添加收货地址，享受便捷配送服务</p>
        <button @click="addAddress" class="add-address-btn">添加地址</button>
      </div>

      <!-- 地址列表 -->
      <div v-else class="addresses-list">
        <div 
          v-for="address in addresses" 
          :key="address.id"
          class="address-card"
        >
          <!-- 地址信息 -->
          <div class="address-content" @click="selectAddress(address)">
            <div class="address-header">
              <div class="contact-info">
                <span class="contact-name">{{ address.receiverName }}</span>
                <span class="contact-phone">{{ address.receiverPhone }}</span>
              </div>
              <div v-if="address.isDefault" class="default-badge">默认</div>
            </div>
            
            <div class="address-detail">
              <p class="address-text">{{ address.detailedAddress }}</p>
            </div>
          </div>

          <!-- 操作按钮 -->
          <div class="address-actions">
            <button 
              v-if="!address.isDefault"
              @click="setDefault(address.id)"
              class="action-btn default-btn"
            >
              设为默认
            </button>
            <button 
              @click="editAddress(address)"
              class="action-btn edit-btn"
            >
              编辑
            </button>
              <button
                @click="deleteAddress(address.addressId)"
                class="action-btn delete-btn"
              >
                删除
              </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 添加/编辑地址弹窗 -->
    <div v-if="showAddressModal" class="address-modal" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3 class="modal-title">{{ editingAddress ? '编辑地址' : '新增地址' }}</h3>
          <button @click="closeModal" class="close-btn">✕</button>
        </div>
        
        <div class="modal-body">
          <div class="form-group">
            <label class="form-label">收货人</label>
            <input 
              v-model="addressForm.receiverName"
              type="text"
              placeholder="请输入收货人姓名"
              class="form-input"
            />
          </div>
          
          <div class="form-group">
            <label class="form-label">手机号</label>
            <input 
              v-model="addressForm.receiverPhone"
              type="tel"
              placeholder="请输入手机号"
              class="form-input"
            />
          </div>
          
          <div class="form-group">
            <label class="form-label">详细地址</label>
            <textarea 
              v-model="addressForm.detailedAddress"
              placeholder="请输入详细地址"
              class="form-textarea"
              rows="3"
            ></textarea>
          </div>
          
          <div class="form-group">
            <label class="form-checkbox">
              <input 
                v-model="addressForm.isDefault"
                type="checkbox"
              />
              <span class="checkbox-text">设为默认地址</span>
            </label>
          </div>
        </div>
        
        <div class="modal-footer">
          <button @click="closeModal" class="cancel-btn">取消</button>
          <button @click="saveAddress" class="save-btn">保存</button>
        </div>
      </div>
    </div>

    <!-- 删除确认弹窗 -->
    <div v-if="showDeleteModal" class="delete-modal" @click="cancelDelete">
      <div class="modal-content" @click.stop>
        <h3 class="modal-title">确认删除</h3>
        <p class="modal-message">确定要删除这个收货地址吗？</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="cancel-btn">取消</button>
          <button @click="confirmDelete" class="confirm-btn">确认删除</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { addressAPI } from '@/api/user.js'

export default {
  name: 'AddressesPage',
  setup() {
    const router = useRouter()
    
    const loading = ref(false)
    const addresses = ref([])
    const showAddressModal = ref(false)
    const showDeleteModal = ref(false)
    const editingAddress = ref(null)
    const pendingDeleteId = ref(null)
    
    // 地址表单
    const addressForm = reactive({
      receiverName: '',
      receiverPhone: '',
      detailedAddress: '',
      isDefault: false
    })
    
    // 获取地址列表
    const fetchAddresses = async () => {
      loading.value = true
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        const response = await addressAPI.getUserAddresses(userId, {})
        console.log('地址接口返回:', response)
        if (response.code === 0) {
          addresses.value = response.data
        } else {
          console.error('获取地址列表失败:', response.message)
        }
      } catch (error) {
        console.error('获取地址列表失败:', error)
      } finally {
        loading.value = false
      }
    }
    
    // 添加地址
    const addAddress = () => {
      editingAddress.value = null
      Object.assign(addressForm, {
        receiverName: '',
        receiverPhone: '',
        detailedAddress: '',
        isDefault: false
      })
      showAddressModal.value = true
    }
    
    // 编辑地址
    const editAddress = (address) => {
      editingAddress.value = address
      Object.assign(addressForm, {
        receiverName: address.receiverName,
        receiverPhone: address.receiverPhone,
        detailedAddress: address.detailedAddress,
        isDefault: address.isDefault
      })
      showAddressModal.value = true
    }
    
    // 保存地址
    const saveAddress = async () => {
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        // 构造后端要求的地址数据，isDefault 转为整数
        // 保存前完整输出 addressForm
        console.log('【调试】addressForm 原始数据:', addressForm)
        // 构造后端要求的地址数据，字段名首字母大写
        const addressData = {
          receiverName: addressForm.receiverName,
          receiverPhone: addressForm.receiverPhone,
          detailedAddress: addressForm.detailedAddress,
          isDefault: addressForm.isDefault ? 1 : 0
        }
        console.log('【调试】用户ID:', userId)
        console.log('【调试】新增地址参数:', addressData)
        if (editingAddress.value) {
          // 编辑地址
          await addressAPI.updateAddress(userId, editingAddress.value.id, addressData)
          console.log('地址更新成功')
        } else {
          // 新增地址
          await addressAPI.addAddress(userId, addressData)
          console.log('地址添加成功')
        }
        closeModal()
        fetchAddresses()
      } catch (error) {
        console.error('保存地址失败:', error)
        if (error && error.response && error.response.data) {
          console.error('后端完整返回:', error.response.data)
        }
      }
    }
    
    // 设为默认地址
    const setDefault = async (addressId) => {
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        
        // 先找到当前地址对象，获取完整信息
        const currentAddress = addresses.value.find(addr => addr.id === addressId)
        if (!currentAddress) {
          console.error('未找到对应的地址信息')
          return
        }
        
        // 更新地址，传递所有必需参数
        await addressAPI.updateAddress(userId, addressId, {
          receiverName: currentAddress.receiverName,
          receiverPhone: currentAddress.receiverPhone,
          detailedAddress: currentAddress.detailedAddress,
          isDefault: true
        })
        
        console.log('设置默认地址成功')
        fetchAddresses()
      } catch (error) {
        console.error('设置默认地址失败:', error)
      }
    }
    
    // 删除地址：设置待删除ID，弹出确认框
    const deleteAddress = (addressId) => {
  console.log('【调试】点击删除，待删除地址ID:', addressId)
      pendingDeleteId.value = addressId
  console.log('【调试】pendingDeleteId.value 设置为:', pendingDeleteId.value)
      showDeleteModal.value = true
    }

    // 确认删除：用 pendingDeleteId.value 进行删除
    const confirmDelete = async () => {
  console.log('【调试】确认删除，pendingDeleteId.value:', pendingDeleteId.value)
      try {
        const userId = (typeof localStorage !== 'undefined' && localStorage.getItem && localStorage.getItem('userId'))
          ? localStorage.getItem('userId')
          : ''
        if (!pendingDeleteId.value) {
          console.error('删除地址失败：addressId 未设置', pendingDeleteId.value)
          return
        }
        await addressAPI.deleteAddress(userId, pendingDeleteId.value)
        console.log('地址删除成功')
        cancelDelete()
        fetchAddresses()
      } catch (error) {
        console.error('删除地址失败:', error)
      }
    }
    
    // 取消删除
    const cancelDelete = () => {
      showDeleteModal.value = false
      pendingDeleteId.value = null
    }
    
    // 选择地址（用于结算页面）
    const selectAddress = (address) => {
      // 如果是从结算页面跳转过来的，选择地址后返回
      if (router.currentRoute.value.query.from === 'checkout') {
        // 存储选中的地址到 localStorage
        localStorage.setItem('selectedAddress', JSON.stringify(address))
        router.back()
      }
    }
    
    // 关闭弹窗
    const closeModal = () => {
      showAddressModal.value = false
      editingAddress.value = null
    }
    
    // 返回上一页
    const goBack = () => {
      router.back()
    }
    
    onMounted(() => {
      fetchAddresses()
    })
    
    return {
      loading,
      addresses,
      showAddressModal,
      showDeleteModal,
      editingAddress,
      addressForm,
      addAddress,
      editAddress,
      saveAddress,
      setDefault,
      deleteAddress,
      confirmDelete,
      cancelDelete,
      selectAddress,
      closeModal,
      goBack
    }
  }
}
</script>

<style scoped>
.addresses-page {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 80px;
}

/* 页面头部 */
.page-header {
  background: white;
  border-bottom: 1px solid #e9ecef;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.back-btn {
  width: 40px;
  height: 40px;
  background: none;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 18px;
  color: #333;
  transition: all 0.3s ease;
}

.back-btn:hover {
  background: #f8f9fa;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.add-btn {
  padding: 8px 16px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

/* 地址内容 */
.addresses-content {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.loading-container {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e1e5e9;
  border-top: 4px solid #4facfe;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

.empty-state {
  text-align: center;
  padding: 80px 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 24px;
}

.empty-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0 0 12px 0;
}

.empty-desc {
  font-size: 14px;
  color: #666;
  margin: 0 0 32px 0;
}

.add-address-btn {
  padding: 12px 32px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-address-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

/* 地址列表 */
.addresses-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.address-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.address-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.address-content {
  padding: 20px;
  cursor: pointer;
}

.address-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.contact-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.contact-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.contact-phone {
  font-size: 14px;
  color: #666;
}

.default-badge {
  padding: 4px 8px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  font-size: 12px;
  border-radius: 12px;
}

.address-detail {
  margin-bottom: 0;
}

.address-text {
  font-size: 14px;
  color: #666;
  line-height: 1.5;
  margin: 0;
}

.address-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 16px 20px;
  background: #f8f9fa;
  border-top: 1px solid #e9ecef;
}

.action-btn {
  padding: 8px 16px;
  border: 1px solid #ddd;
  background: white;
  color: #333;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn:hover {
  border-color: #4facfe;
  color: #4facfe;
}

.default-btn:hover {
  background: #4facfe;
  color: white;
}

.edit-btn:hover {
  background: #28a745;
  color: white;
  border-color: #28a745;
}

.delete-btn {
  color: #dc3545;
  border-color: #dc3545;
}

.delete-btn:hover {
  background: #dc3545;
  color: white;
}

/* 弹窗样式 */
.address-modal,
.delete-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 500px;
  width: 100%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 20px 0;
}

.modal-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.close-btn {
  width: 32px;
  height: 32px;
  background: none;
  border: none;
  font-size: 18px;
  color: #999;
  cursor: pointer;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.close-btn:hover {
  background: #f8f9fa;
  color: #333;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
}

.form-input,
.form-textarea {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.3s ease;
  box-sizing: border-box;
}

.form-input:focus,
.form-textarea:focus {
  outline: none;
  border-color: #4facfe;
  box-shadow: 0 0 0 2px rgba(79, 172, 254, 0.2);
}

.form-checkbox {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.form-checkbox input {
  margin-right: 8px;
}

.checkbox-text {
  font-size: 14px;
  color: #333;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 0 20px 20px;
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 12px;
  padding: 20px;
}

.cancel-btn,
.save-btn,
.confirm-btn {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #f8f9fa;
  color: #333;
  border: 1px solid #ddd;
}

.cancel-btn:hover {
  background: #e9ecef;
}

.save-btn {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
}

.save-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(79, 172, 254, 0.3);
}

.confirm-btn {
  background: #dc3545;
  color: white;
}

.confirm-btn:hover {
  background: #c82333;
}

.modal-message {
  font-size: 14px;
  color: #666;
  margin: 16px 0;
  text-align: center;
  line-height: 1.5;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .addresses-content {
    padding: 12px;
  }
  
  .address-card {
    margin-bottom: 12px;
  }
  
  .address-content {
    padding: 16px;
  }
  
  .address-actions {
    flex-wrap: wrap;
    gap: 8px;
  }
  
  .action-btn {
    flex: 1;
    min-width: 80px;
  }
  
  .modal-content {
    margin: 10px;
    max-width: none;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
