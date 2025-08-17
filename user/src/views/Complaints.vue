<template>
  <div class="complaints-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <button @click="goBack" class="back-btn">
          <i class="back-icon">←</i>
        </button>
        <h1 class="page-title">投诉与建议</h1>
      </div>
    </div>

    <!-- 页面内容 -->
    <div class="page-content">
      <!-- 标签页切换 -->
      <div class="tabs">
        <button 
          class="tab-btn" 
          :class="{ active: activeTab === 'submit' }"
          @click="activeTab = 'submit'"
        >
          提交投诉/建议
        </button>
        <button 
          class="tab-btn" 
          :class="{ active: activeTab === 'history' }"
          @click="activeTab = 'history'"
        >
          历史记录
        </button>
      </div>

      <!-- 提交投诉/建议 -->
      <div v-if="activeTab === 'submit'" class="submit-section">
        <form @submit.prevent="submitComplaint" class="complaint-form">
          <!-- 投诉类型 -->
          <div class="form-group">
            <label class="form-label">投诉类型 *</label>
            <select v-model="formData.type" required class="form-select">
              <option value="">请选择投诉类型</option>
              <option value="food_quality">食品质量问题</option>
              <option value="delivery_issue">配送问题</option>
              <option value="service_attitude">服务态度问题</option>
              <option value="order_error">订单错误</option>
              <option value="refund_issue">退款问题</option>
              <option value="app_bug">APP故障</option>
              <option value="other">其他问题</option>
              <option value="suggestion">意见建议</option>
            </select>
          </div>

          <!-- 相关订单（可选） -->
          <div class="form-group">
            <label class="form-label">相关订单号（可选）</label>
            <input 
              v-model="formData.orderId" 
              type="text" 
              class="form-input"
              placeholder="请输入订单号"
            />
          </div>

          <!-- 详细描述 -->
          <div class="form-group">
            <label class="form-label">详细描述 *</label>
            <textarea 
              v-model="formData.description" 
              required
              class="form-textarea"
              placeholder="请详细描述您遇到的问题或建议内容，我们会认真对待每一条反馈"
              rows="6"
            ></textarea>
            <div class="char-count">{{ formData.description.length }}/500</div>
          </div>

          <!-- 联系方式 -->
          <div class="form-group">
            <label class="form-label">联系方式 *</label>
            <input 
              v-model="formData.contact" 
              type="text" 
              required
              class="form-input"
              placeholder="请输入手机号或邮箱，方便我们联系您"
            />
          </div>

          <!-- 上传图片 -->
          <div class="form-group">
            <label class="form-label">上传相关图片（可选）</label>
            <div class="upload-area">
              <div v-if="uploadedImages.length === 0" class="upload-placeholder">
                <div class="upload-icon">📷</div>
                <div class="upload-text">点击上传图片</div>
                <div class="upload-hint">最多可上传3张图片，每张不超过5MB</div>
              </div>
              
              <div v-if="uploadedImages.length > 0" class="uploaded-images">
                <div v-for="(image, index) in uploadedImages" :key="index" class="image-item">
                  <img :src="image.url" :alt="image.name" class="uploaded-image" />
                  <button @click="removeImage(index)" class="remove-image-btn">×</button>
                </div>
                <div v-if="uploadedImages.length < 3" @click="selectImage" class="add-image-btn">
                  <div class="add-icon">+</div>
                </div>
              </div>
              
              <input 
                ref="fileInput"
                type="file" 
                accept="image/*" 
                multiple 
                @change="handleImageUpload"
                class="hidden-input"
              />
            </div>
          </div>

          <!-- 提交按钮 -->
          <button type="submit" class="submit-btn" :disabled="isSubmitting">
            <span v-if="!isSubmitting">提交</span>
            <span v-else>提交中...</span>
          </button>
        </form>
      </div>

      <!-- 历史记录 -->
      <div v-if="activeTab === 'history'" class="history-section">
        <div v-if="complaints.length === 0" class="empty-state">
          <div class="empty-icon">📝</div>
          <div class="empty-text">暂无投诉记录</div>
          <div class="empty-hint">您的投诉和建议记录会显示在这里</div>
        </div>

        <div v-else class="complaints-list">
          <div v-for="complaint in complaints" :key="complaint.id" class="complaint-item">
            <div class="complaint-header">
              <div class="complaint-type">{{ getTypeLabel(complaint.type) }}</div>
              <div class="complaint-status" :class="complaint.status">
                {{ getStatusLabel(complaint.status) }}
              </div>
            </div>
            
            <div class="complaint-content">
              <div class="complaint-description">{{ complaint.description }}</div>
              <div v-if="complaint.orderId" class="complaint-order">
                订单号：{{ complaint.orderId }}
              </div>
            </div>
            
            <div class="complaint-footer">
              <div class="complaint-time">{{ formatDate(complaint.createTime) }}</div>
              <button @click="viewComplaintDetail(complaint)" class="view-detail-btn">
                查看详情
              </button>
            </div>
            
            <!-- 回复内容 -->
            <div v-if="complaint.reply" class="complaint-reply">
              <div class="reply-header">
                <div class="reply-label">客服回复：</div>
                <div class="reply-time">{{ formatDate(complaint.replyTime) }}</div>
              </div>
              <div class="reply-content">{{ complaint.reply }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 详情弹窗 -->
    <div v-if="showDetailModal" class="detail-modal" @click="closeDetailModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3 class="modal-title">投诉详情</h3>
          <button @click="closeDetailModal" class="close-btn">×</button>
        </div>
        
        <div class="modal-body">
          <div class="detail-item">
            <label>投诉类型：</label>
            <span>{{ getTypeLabel(selectedComplaint?.type) }}</span>
          </div>
          
          <div v-if="selectedComplaint?.orderId" class="detail-item">
            <label>订单号：</label>
            <span>{{ selectedComplaint.orderId }}</span>
          </div>
          
          <div class="detail-item">
            <label>提交时间：</label>
            <span>{{ formatDate(selectedComplaint?.createTime) }}</span>
          </div>
          
          <div class="detail-item">
            <label>处理状态：</label>
            <span class="status-badge" :class="selectedComplaint?.status">
              {{ getStatusLabel(selectedComplaint?.status) }}
            </span>
          </div>
          
          <div class="detail-item">
            <label>详细描述：</label>
            <div class="description-content">{{ selectedComplaint?.description }}</div>
          </div>
          
          <div v-if="selectedComplaint?.images?.length > 0" class="detail-item">
            <label>相关图片：</label>
            <div class="detail-images">
              <img 
                v-for="(image, index) in selectedComplaint.images" 
                :key="index"
                :src="image" 
                class="detail-image"
              />
            </div>
          </div>
          
          <div v-if="selectedComplaint?.reply" class="detail-item">
            <label>客服回复：</label>
            <div class="reply-content">{{ selectedComplaint.reply }}</div>
            <div class="reply-time">回复时间：{{ formatDate(selectedComplaint.replyTime) }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'

export default {
  name: 'ComplaintsPage',
  setup() {
    const router = useRouter()
    
    const activeTab = ref('submit')
    const isSubmitting = ref(false)
    const showDetailModal = ref(false)
    const selectedComplaint = ref(null)
    const uploadedImages = ref([])
    const fileInput = ref(null)
    
    // 表单数据
    const formData = reactive({
      type: '',
      orderId: '',
      description: '',
      contact: ''
    })
    
    // 投诉记录
    const complaints = ref([
      {
        id: 1,
        type: 'delivery_issue',
        description: '外卖送达时间严重超时，等了2个小时才到，食物都凉了',
        orderId: '202312150001',
        contact: '138****5678',
        status: 'replied',
        createTime: '2023-12-15 18:30:00',
        reply: '非常抱歉给您带来的不便，我们已对该配送员进行处罚，并为您安排了补偿。',
        replyTime: '2023-12-15 20:15:00',
        images: []
      },
      {
        id: 2,
        type: 'food_quality',
        description: '订购的牛肉面里有异物，疑似头发丝',
        orderId: '202312140002',
        contact: '139****1234',
        status: 'processing',
        createTime: '2023-12-14 12:45:00',
        images: ['http://example.com/image1.jpg']
      },
      {
        id: 3,
        type: 'suggestion',
        description: 'APP界面可以更加简洁一些，搜索功能希望能够优化',
        contact: 'user@example.com',
        status: 'pending',
        createTime: '2023-12-13 09:20:00'
      }
    ])
    
    // 投诉类型标签
    const typeLabels = {
      'food_quality': '食品质量问题',
      'delivery_issue': '配送问题',
      'service_attitude': '服务态度问题',
      'order_error': '订单错误',
      'refund_issue': '退款问题',
      'app_bug': 'APP故障',
      'other': '其他问题',
      'suggestion': '意见建议'
    }
    
    // 状态标签
    const statusLabels = {
      'pending': '待处理',
      'processing': '处理中',
      'replied': '已回复',
      'closed': '已关闭'
    }
    
    // 方法
    const submitComplaint = async () => {
      if (formData.description.length > 500) {
        alert('描述内容不能超过500字')
        return
      }
      
      isSubmitting.value = true
      
      try {
        // 模拟提交
        await new Promise(resolve => setTimeout(resolve, 1500))
        
        // 创建新的投诉记录
        const newComplaint = {
          id: Date.now(),
          type: formData.type,
          description: formData.description,
          orderId: formData.orderId,
          contact: formData.contact,
          status: 'pending',
          createTime: new Date().toLocaleString(),
          images: uploadedImages.value.map(img => img.url)
        }
        
        complaints.value.unshift(newComplaint)
        
        // 重置表单
        Object.assign(formData, {
          type: '',
          orderId: '',
          description: '',
          contact: ''
        })
        uploadedImages.value = []
        
        alert('提交成功！我们会尽快处理您的反馈')
        activeTab.value = 'history'
        
      } catch (error) {
        console.error('提交失败:', error)
        alert('提交失败，请稍后重试')
      } finally {
        isSubmitting.value = false
      }
    }
    
    const handleImageUpload = (event) => {
      const files = Array.from(event.target.files)
      
      files.forEach(file => {
        if (uploadedImages.value.length >= 3) {
          alert('最多只能上传3张图片')
          return
        }
        
        if (file.size > 5 * 1024 * 1024) {
          alert('图片大小不能超过5MB')
          return
        }
        
        const reader = new FileReader()
        reader.onload = (e) => {
          uploadedImages.value.push({
            name: file.name,
            url: e.target.result
          })
        }
        reader.readAsDataURL(file)
      })
      
      event.target.value = ''
    }
    
    const selectImage = () => {
      fileInput.value?.click()
    }
    
    const removeImage = (index) => {
      uploadedImages.value.splice(index, 1)
    }
    
    const viewComplaintDetail = (complaint) => {
      selectedComplaint.value = complaint
      showDetailModal.value = true
    }
    
    const closeDetailModal = () => {
      showDetailModal.value = false
      selectedComplaint.value = null
    }
    
    const getTypeLabel = (type) => {
      return typeLabels[type] || type
    }
    
    const getStatusLabel = (status) => {
      return statusLabels[status] || status
    }
    
    const formatDate = (dateString) => {
      if (!dateString) return ''
      return new Date(dateString).toLocaleString()
    }
    
    const goBack = () => {
      router.back()
    }
    
    return {
      activeTab,
      isSubmitting,
      showDetailModal,
      selectedComplaint,
      uploadedImages,
      fileInput,
      formData,
      complaints,
      submitComplaint,
      handleImageUpload,
      selectImage,
      removeImage,
      viewComplaintDetail,
      closeDetailModal,
      getTypeLabel,
      getStatusLabel,
      formatDate,
      goBack
    }
  }
}
</script>

<style scoped>
.complaints-page {
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
  margin-right: 12px;
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

/* 页面内容 */
.page-content {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

/* 标签页 */
.tabs {
  display: flex;
  background: white;
  border-radius: 12px 12px 0 0;
  border: 1px solid #e9ecef;
  border-bottom: none;
  overflow: hidden;
}

.tab-btn {
  flex: 1;
  padding: 16px;
  background: #f8f9fa;
  border: none;
  font-size: 16px;
  font-weight: 500;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
}

.tab-btn.active {
  background: white;
  color: #333;
}

.tab-btn:hover {
  background: white;
  color: #333;
}

/* 提交表单 */
.submit-section {
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 0 0 12px 12px;
  padding: 24px;
}

.complaint-form {
  max-width: 600px;
}

.form-group {
  margin-bottom: 24px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
}

.form-input,
.form-select,
.form-textarea {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  color: #333;
  transition: all 0.3s ease;
  box-sizing: border-box;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: #4facfe;
  box-shadow: 0 0 0 3px rgba(79, 172, 254, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 120px;
  line-height: 1.5;
}

.char-count {
  text-align: right;
  font-size: 12px;
  color: #999;
  margin-top: 4px;
}

/* 上传区域 */
.upload-area {
  border: 2px dashed #ddd;
  border-radius: 8px;
  padding: 20px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.upload-area:hover {
  border-color: #4facfe;
  background: #f8f9fa;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}

.upload-icon {
  font-size: 32px;
  color: #999;
}

.upload-text {
  font-size: 16px;
  color: #333;
  font-weight: 500;
}

.upload-hint {
  font-size: 12px;
  color: #999;
}

.uploaded-images {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  justify-content: center;
}

.image-item {
  position: relative;
  width: 80px;
  height: 80px;
}

.uploaded-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid #ddd;
}

.remove-image-btn {
  position: absolute;
  top: -8px;
  right: -8px;
  width: 24px;
  height: 24px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.add-image-btn {
  width: 80px;
  height: 80px;
  border: 2px dashed #ddd;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-image-btn:hover {
  border-color: #4facfe;
  background: #f8f9fa;
}

.add-icon {
  font-size: 24px;
  color: #999;
}

.hidden-input {
  display: none;
}

/* 提交按钮 */
.submit-btn {
  width: 100%;
  padding: 16px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* 历史记录 */
.history-section {
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 0 0 12px 12px;
  min-height: 400px;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 18px;
  color: #333;
  margin-bottom: 8px;
}

.empty-hint {
  font-size: 14px;
  color: #999;
}

.complaints-list {
  padding: 20px;
}

.complaint-item {
  border: 1px solid #e9ecef;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 16px;
  transition: all 0.3s ease;
}

.complaint-item:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.complaint-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.complaint-type {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.complaint-status {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.complaint-status.pending {
  background: #fff3cd;
  color: #856404;
}

.complaint-status.processing {
  background: #cce5ff;
  color: #004085;
}

.complaint-status.replied {
  background: #d4edda;
  color: #155724;
}

.complaint-status.closed {
  background: #f8d7da;
  color: #721c24;
}

.complaint-content {
  margin-bottom: 16px;
}

.complaint-description {
  font-size: 14px;
  color: #333;
  line-height: 1.5;
  margin-bottom: 8px;
}

.complaint-order {
  font-size: 12px;
  color: #999;
}

.complaint-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.complaint-time {
  font-size: 12px;
  color: #999;
}

.view-detail-btn {
  padding: 6px 12px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.view-detail-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(79, 172, 254, 0.3);
}

.complaint-reply {
  margin-top: 16px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #4facfe;
}

.reply-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.reply-label {
  font-size: 14px;
  font-weight: 600;
  color: #4facfe;
}

.reply-time {
  font-size: 12px;
  color: #999;
}

.reply-content {
  font-size: 14px;
  color: #333;
  line-height: 1.5;
}

/* 详情弹窗 */
.detail-modal {
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
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #e9ecef;
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
  border-radius: 50%;
  font-size: 20px;
  color: #999;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-btn:hover {
  background: #f8f9fa;
  color: #333;
}

.modal-body {
  padding: 24px;
}

.detail-item {
  margin-bottom: 20px;
}

.detail-item label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.description-content {
  font-size: 14px;
  color: #333;
  line-height: 1.6;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 8px;
}

.detail-images {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.detail-image {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid #ddd;
  cursor: pointer;
  transition: all 0.3s ease;
}

.detail-image:hover {
  transform: scale(1.05);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-content {
    padding: 12px;
  }
  
  .submit-section {
    padding: 20px 16px;
  }
  
  .complaint-item {
    padding: 16px;
  }
  
  .modal-content {
    margin: 10px;
    max-height: calc(100vh - 20px);
  }
  
  .modal-header,
  .modal-body {
    padding: 16px;
  }
  
  .uploaded-images {
    justify-content: flex-start;
  }
}
</style>
