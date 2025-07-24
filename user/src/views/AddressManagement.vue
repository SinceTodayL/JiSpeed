<template>
  <div class="address-management">
    <!-- 顶部导航 -->
    <div class="header">
      <button @click="goBack" class="back-btn">
        <i class="back-icon">←</i>
      </button>
      <h1 class="header-title">地址管理</h1>
      <button @click="showAddForm" class="add-btn">
        <i class="add-icon">+</i>
      </button>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>正在加载地址列表...</p>
    </div>

    <!-- 地址列表 -->
    <div v-else class="address-content">
      <div v-if="addresses.length === 0" class="empty-addresses">
        <div class="empty-icon">📍</div>
        <p class="empty-text">还没有添加地址</p>
        <button @click="showAddForm" class="add-address-btn">
          添加新地址
        </button>
      </div>

      <div v-else class="addresses-list">
        <div 
          v-for="address in addresses"
          :key="address.addressId"
          class="address-card"
        >
          <div class="address-main">
            <div class="address-info">
              <div class="receiver-info">
                <span class="receiver-name">{{ address.receiverName }}</span>
                <span class="receiver-phone">{{ address.receiverPhone }}</span>
                <span v-if="address.isDefault === 1" class="default-tag">默认</span>
                <span v-if="address.label" class="label-tag">{{ address.label }}</span>
              </div>
              <div class="address-detail">{{ address.detailedAddress }}</div>
            </div>
            
            <div class="address-actions">
              <button @click="editAddress(address)" class="edit-btn">
                编辑
              </button>
              <button 
                v-if="address.isDefault !== 1"
                @click="setAsDefault(address.addressId)"
                class="default-btn"
              >
                设为默认
              </button>
              <button 
                @click="confirmDelete(address)"
                :disabled="address.isDefault === 1"
                class="delete-btn"
              >
                删除
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 添加/编辑地址表单弹窗 -->
    <div v-if="showFormModal" class="modal-overlay" @click="closeFormModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>{{ isEditing ? '编辑地址' : '添加新地址' }}</h3>
          <button @click="closeFormModal" class="modal-close">✕</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="submitForm" class="address-form">
            <div class="form-group">
              <label for="receiverName">收货人姓名 *</label>
              <input
                id="receiverName"
                v-model="formData.receiverName"
                type="text"
                placeholder="请输入收货人姓名"
                maxlength="50"
                required
              />
            </div>

            <div class="form-group">
              <label for="receiverPhone">手机号码 *</label>
              <input
                id="receiverPhone"
                v-model="formData.receiverPhone"
                type="tel"
                placeholder="请输入手机号码"
                maxlength="11"
                required
              />
            </div>

            <div class="form-group">
              <label for="detailedAddress">详细地址 *</label>
              <textarea
                id="detailedAddress"
                v-model="formData.detailedAddress"
                placeholder="请输入详细地址，如街道、门牌号、楼层、房间号等"
                maxlength="200"
                rows="3"
                required
              ></textarea>
            </div>

            <div class="form-group">
              <label for="label">地址标签</label>
              <div class="label-options">
                <button
                  v-for="labelOption in labelOptions"
                  :key="labelOption"
                  type="button"
                  :class="['label-option', { active: formData.label === labelOption }]"
                  @click="formData.label = labelOption"
                >
                  {{ labelOption }}
                </button>
              </div>
              <input
                v-if="formData.label === '自定义'"
                v-model="customLabel"
                type="text"
                placeholder="请输入自定义标签"
                maxlength="10"
                class="custom-label-input"
              />
            </div>

            <div class="form-group checkbox-group">
              <label class="checkbox-label">
                <input
                  v-model="formData.isDefault"
                  type="checkbox"
                  :true-value="1"
                  :false-value="0"
                />
                <span class="checkbox-text">设为默认地址</span>
              </label>
            </div>

            <div class="form-actions">
              <button type="button" @click="closeFormModal" class="cancel-btn">
                取消
              </button>
              <button 
                type="submit" 
                :disabled="submitting || !isFormValid"
                class="submit-btn"
              >
                {{ submitting ? '提交中...' : (isEditing ? '更新地址' : '添加地址') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- 删除确认弹窗 -->
    <div v-if="showDeleteModal" class="modal-overlay" @click="closeDeleteModal">
      <div class="modal-content confirm-modal" @click.stop>
        <div class="modal-header">
          <h3>确认删除</h3>
        </div>
        <div class="modal-body">
          <p>确定要删除这个地址吗？</p>
          <div class="delete-address-info">
            <div class="receiver-info">
              {{ addressToDelete?.receiverName }} {{ addressToDelete?.receiverPhone }}
            </div>
            <div class="address-detail">{{ addressToDelete?.detailedAddress }}</div>
          </div>
        </div>
        <div class="modal-actions">
          <button @click="closeDeleteModal" class="cancel-btn">取消</button>
          <button 
            @click="deleteAddress" 
            :disabled="deleting"
            class="confirm-delete-btn"
          >
            {{ deleting ? '删除中...' : '确认删除' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { addressAPI } from '@/api/address.js'

export default {
  name: 'AddressManagement',
  setup() {
    const router = useRouter()

    // 响应式数据
    const loading = ref(false)
    const addresses = ref([])
    const showFormModal = ref(false)
    const showDeleteModal = ref(false)
    const isEditing = ref(false)
    const submitting = ref(false)
    const deleting = ref(false)
    const editingAddressId = ref(null)
    const addressToDelete = ref(null)
    const customLabel = ref('')

    // 表单数据
    const formData = ref({
      receiverName: '',
      receiverPhone: '',
      detailedAddress: '',
      label: '家',
      isDefault: 0
    })

    // 标签选项
    const labelOptions = ref(['家', '公司', '学校', '自定义'])

    // 计算属性
    const isFormValid = computed(() => {
      return formData.value.receiverName.trim() && 
             formData.value.receiverPhone.trim() && 
             formData.value.detailedAddress.trim() &&
             /^1[3-9]\d{9}$/.test(formData.value.receiverPhone)
    })

    // 方法
    const goBack = () => {
      router.back()
    }

    const loadAddresses = async () => {
      loading.value = true
      try {
        const response = await addressAPI.getUserAddresses('USER123')
        if (response && response.code === 200) {
          addresses.value = response.data.addresses
        }
      } catch (error) {
        console.error('加载地址列表失败:', error)
        alert('加载地址列表失败，请重试')
      } finally {
        loading.value = false
      }
    }

    const showAddForm = () => {
      isEditing.value = false
      editingAddressId.value = null
      resetForm()
      showFormModal.value = true
    }

    const editAddress = (address) => {
      isEditing.value = true
      editingAddressId.value = address.addressId
      formData.value = {
        receiverName: address.receiverName,
        receiverPhone: address.receiverPhone,
        detailedAddress: address.detailedAddress,
        label: address.label || '家',
        isDefault: address.isDefault
      }
      showFormModal.value = true
    }

    const resetForm = () => {
      formData.value = {
        receiverName: '',
        receiverPhone: '',
        detailedAddress: '',
        label: '家',
        isDefault: 0
      }
      customLabel.value = ''
    }

    const closeFormModal = () => {
      showFormModal.value = false
      resetForm()
    }

    const submitForm = async () => {
      if (!isFormValid.value) return

      submitting.value = true
      try {
        const submitData = { ...formData.value }
        
        // 处理自定义标签
        if (submitData.label === '自定义' && customLabel.value.trim()) {
          submitData.label = customLabel.value.trim()
        }
        
        submitData.userId = 'USER123'

        let response
        if (isEditing.value) {
          response = await addressAPI.updateAddress(editingAddressId.value, submitData)
        } else {
          response = await addressAPI.addAddress(submitData)
        }

        if (response && response.code === 200) {
          alert(isEditing.value ? '地址更新成功' : '地址添加成功')
          closeFormModal()
          loadAddresses() // 重新加载地址列表
        }
      } catch (error) {
        console.error('提交地址失败:', error)
        alert(error.message || '操作失败，请重试')
      } finally {
        submitting.value = false
      }
    }

    const confirmDelete = (address) => {
      if (address.isDefault === 1) {
        alert('不能删除默认地址，请先设置其他地址为默认地址')
        return
      }
      addressToDelete.value = address
      showDeleteModal.value = true
    }

    const closeDeleteModal = () => {
      showDeleteModal.value = false
      addressToDelete.value = null
    }

    const deleteAddress = async () => {
      if (!addressToDelete.value) return

      deleting.value = true
      try {
        const response = await addressAPI.deleteAddress(addressToDelete.value.addressId)
        if (response && response.code === 200) {
          alert('地址删除成功')
          closeDeleteModal()
          loadAddresses() // 重新加载地址列表
        }
      } catch (error) {
        console.error('删除地址失败:', error)
        alert(error.message || '删除失败，请重试')
      } finally {
        deleting.value = false
      }
    }

    const setAsDefault = async (addressId) => {
      try {
        const response = await addressAPI.setDefaultAddress(addressId, 'USER123')
        if (response && response.code === 200) {
          alert('默认地址设置成功')
          loadAddresses() // 重新加载地址列表
        }
      } catch (error) {
        console.error('设置默认地址失败:', error)
        alert('设置失败，请重试')
      }
    }

    // 监听自定义标签变化
    watch(() => formData.value.label, (newLabel) => {
      if (newLabel !== '自定义') {
        customLabel.value = ''
      }
    })

    // 生命周期
    onMounted(() => {
      loadAddresses()
    })

    return {
      loading,
      addresses,
      showFormModal,
      showDeleteModal,
      isEditing,
      submitting,
      deleting,
      formData,
      labelOptions,
      customLabel,
      addressToDelete,
      isFormValid,
      goBack,
      showAddForm,
      editAddress,
      closeFormModal,
      submitForm,
      confirmDelete,
      closeDeleteModal,
      deleteAddress,
      setAsDefault
    }
  }
}
</script>

<style scoped>
.address-management {
  min-height: 100vh;
  background: #f8f9fa;
  padding-bottom: 20px;
}

/* 顶部导航 */
.header {
  position: relative;
  height: 60px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.back-btn {
  position: absolute;
  left: 16px;
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: white;
  font-size: 18px;
}

.header-title {
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

.add-btn {
  position: absolute;
  right: 16px;
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: white;
  font-size: 20px;
  font-weight: bold;
}

/* 加载状态 */
.loading-container {
  text-align: center;
  padding: 60px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e1e5e9;
  border-top: 4px solid #007BFF;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

/* 空状态 */
.empty-addresses {
  text-align: center;
  padding: 80px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 16px;
  color: #666;
  margin-bottom: 24px;
}

.add-address-btn {
  padding: 12px 24px;
  background: linear-gradient(135deg, #007BFF, #00D4FF);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
}

/* 地址列表 */
.address-content {
  padding: 16px;
}

.addresses-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.address-card {
  background: white;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.address-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

.address-main {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
}

.address-info {
  flex: 1;
}

.receiver-info {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
  flex-wrap: wrap;
}

.receiver-name {
  font-weight: 600;
  color: #333;
  font-size: 16px;
}

.receiver-phone {
  color: #666;
  font-size: 14px;
}

.default-tag {
  background: #ff6b6b;
  color: white;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 10px;
  font-weight: 500;
}

.label-tag {
  background: #007BFF;
  color: white;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 10px;
  font-weight: 500;
}

.address-detail {
  color: #666;
  font-size: 14px;
  line-height: 1.4;
}

.address-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-width: 80px;
}

.edit-btn, .default-btn, .delete-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 16px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.edit-btn {
  background: #007BFF;
  color: white;
}

.edit-btn:hover {
  background: #0056b3;
}

.default-btn {
  background: #28a745;
  color: white;
}

.default-btn:hover {
  background: #1e7e34;
}

.delete-btn {
  background: #dc3545;
  color: white;
}

.delete-btn:hover:not(:disabled) {
  background: #c82333;
}

.delete-btn:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

/* 弹窗样式 */
.modal-overlay {
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
  border-radius: 16px;
  max-width: 500px;
  width: 100%;
  max-height: 80vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.confirm-modal {
  max-width: 400px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.modal-close {
  width: 32px;
  height: 32px;
  background: #f8f9fa;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #666;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
}

/* 表单样式 */
.address-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 500;
  color: #333;
  font-size: 14px;
}

.form-group input,
.form-group textarea {
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007BFF;
}

.label-options {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.label-option {
  padding: 8px 16px;
  border: 1px solid #ddd;
  border-radius: 20px;
  background: white;
  color: #666;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.label-option.active {
  background: #007BFF;
  border-color: #007BFF;
  color: white;
}

.custom-label-input {
  margin-top: 8px;
}

.checkbox-group {
  flex-direction: row;
  align-items: center;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.checkbox-text {
  font-size: 14px;
  color: #333;
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}

.cancel-btn, .submit-btn {
  flex: 1;
  padding: 12px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #6c757d;
  color: white;
}

.cancel-btn:hover {
  background: #5a6268;
}

.submit-btn {
  background: #007BFF;
  color: white;
}

.submit-btn:hover:not(:disabled) {
  background: #0056b3;
}

.submit-btn:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

/* 删除确认弹窗 */
.delete-address-info {
  background: #f8f9fa;
  padding: 12px;
  border-radius: 8px;
  margin: 16px 0;
}

.delete-address-info .receiver-info {
  font-weight: 500;
  color: #333;
  margin-bottom: 4px;
}

.delete-address-info .address-detail {
  color: #666;
  font-size: 14px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  padding: 16px 20px;
  border-top: 1px solid #e9ecef;
}

.confirm-delete-btn {
  flex: 1;
  padding: 10px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.confirm-delete-btn:hover:not(:disabled) {
  background: #c82333;
}

.confirm-delete-btn:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .address-main {
    flex-direction: column;
    gap: 12px;
  }

  .address-actions {
    flex-direction: row;
    min-width: auto;
  }

  .receiver-info {
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }

  .form-actions {
    flex-direction: column;
  }

  .modal-actions {
    flex-direction: column;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
