<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import type { FormInst, FormRules } from 'naive-ui';
import { Icon } from '@iconify/vue';
import { getRiderInfo, updateRiderInfo } from '@/service/api/rider';
import { useAuthStore } from '../../store/modules/auth';
import { useRiderStore } from '../../store/modules/rider';

const authStore = useAuthStore();
const riderStore = useRiderStore();

// 骑手ID将通过currentRiderId计算属性动态获取

const formRef = ref<FormInst | null>(null);
const loading = ref(false);
const submitting = ref(false);

const formModel = reactive({
  applicationUserId: '',
  name: '',
  phoneNumber: '',
  riderId: '',
  vehicleNumber: ''
});

// 原始数据，用于检测是否有变更
const originalData = ref({
  applicationUserId: '',
  name: '',
  phoneNumber: '',
  riderId: '',
  vehicleNumber: ''
});

// 获取当前登录的骑手ID
const currentRiderId = computed(() => {
  return authStore.userInfo.userId || riderStore.riderId || '';
});

// 计算表单完善状态
const isFormComplete = computed(() => {
  const { name, phoneNumber, vehicleNumber } = formModel;
  return Boolean(name && phoneNumber && vehicleNumber);
});

// 检测是否有变更
const hasChanges = computed(() => {
  return (
    formModel.name !== originalData.value.name ||
    formModel.phoneNumber !== originalData.value.phoneNumber ||
    formModel.vehicleNumber !== originalData.value.vehicleNumber
  );
});

// 表单验证规则
const rules: FormRules = {
  name: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { max: 50, message: '姓名长度不能超过50个字符', trigger: 'blur' }
  ],
  phoneNumber: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号格式', trigger: 'blur' }
  ],
  vehicleNumber: [
    { required: true, message: '请输入车辆编号', trigger: 'blur' },
    { max: 20, message: '车辆编号长度不能超过20个字符', trigger: 'blur' }
  ]
};


async function fetchRiderInfo() {
  try {
    console.log('🔄 开始获取骑手信息...');
    console.log('🆔 骑手ID：', currentRiderId.value);
    
    const response = await getRiderInfo(currentRiderId.value);
    console.log('📥 获取骑手信息API响应：', response);
    
    const { data } = response;
    console.log('📥 获取到的数据：', data);
    
    if (data) {
      // 更新表单数据
      Object.assign(formModel, data);
      
      // 保存原始数据，用于检测变更
      originalData.value.applicationUserId = data.applicationUserId || '';
      originalData.value.name = data.name || '';
      originalData.value.phoneNumber = data.phoneNumber || '';
      originalData.value.riderId = data.riderId || '';
      originalData.value.vehicleNumber = data.vehicleNumber || '';
      
      console.log('✅ 获取骑手信息成功！');
      console.log('📝 更新后的formModel：', formModel);
      console.log('📝 更新后的originalData：', originalData.value);
    } else {
      console.warn('⚠️ 获取到的数据为空');
    }
  } catch (error) {
    console.error('获取骑手信息失败', error);
    window.$message?.error('获取骑手信息失败，请检查网络连接');
    // 当API调用失败时，使用模拟数据
    const mockData = {
      applicationUserId: 'mock_user_001',
      name: '测试骑手',
      phoneNumber: '13800138000',
      riderId: currentRiderId.value,
      vehicleNumber: '宁A12345'
    };
    
    Object.assign(formModel, mockData);
    
    // 保存模拟数据到原始数据
    originalData.value.applicationUserId = mockData.applicationUserId;
    originalData.value.name = mockData.name;
    originalData.value.phoneNumber = mockData.phoneNumber;
    originalData.value.riderId = mockData.riderId;
    originalData.value.vehicleNumber = mockData.vehicleNumber;
  }
}

onMounted(() => {
  fetchRiderInfo();
});

async function handleSave() {
  formRef.value?.validate(async errors => {
    if (errors) {
      return;
    }

    submitting.value = true;
    const { name, phoneNumber, vehicleNumber } = formModel;
    
    try {
      console.log('🚀 开始保存骑手信息...');
      console.log('🆔 骑手ID：', formModel.riderId);
      console.log('📤 发送的数据：', {
        riderId: formModel.riderId,
        name,
        phoneNumber,
        vehicleNumber
      });
      
      console.log('🌐 调用updateRiderInfo API...');
      const response = await updateRiderInfo(formModel.riderId, {
        name,
        phoneNumber,
        vehicleNumber
      });
      
      console.log('📥 完整API响应：', response);
      console.log('📥 响应类型：', typeof response);
      console.log('📥 响应结构：', Object.keys(response));
      
      const { data: updatedData } = response;
      console.log('📥 响应数据：', updatedData);
      console.log('📥 数据类型：', typeof updatedData);

      if (updatedData) {
        window.$message?.success('个人信息保存成功！');
        
        // 更新表单数据
        Object.assign(formModel, updatedData);
        
        // 更新原始数据，确保hasChanges计算属性正确工作
        originalData.value.applicationUserId = updatedData.applicationUserId || '';
        originalData.value.name = updatedData.name || '';
        originalData.value.phoneNumber = updatedData.phoneNumber || '';
        originalData.value.riderId = updatedData.riderId || '';
        originalData.value.vehicleNumber = updatedData.vehicleNumber || '';
        
        console.log('✅ 保存成功！');
        console.log('📝 更新后的formModel：', formModel);
        console.log('📝 更新后的originalData：', originalData.value);
      } else {
        console.warn('⚠️ API返回数据为空');
        window.$message?.warning('保存成功，但未返回更新后的数据');
      }
    } catch (error: any) {
      console.error('保存骑手信息失败', error);
      const errorMessage = error?.response?.data?.message || '保存失败，请稍后重试';
      window.$message?.error(errorMessage);
    } finally {
      submitting.value = false;
    }
  });
}

// 取消编辑，恢复原始数据
const handleCancel = () => {
  // 使用逐个赋值确保响应式更新
  formModel.applicationUserId = originalData.value.applicationUserId;
  formModel.name = originalData.value.name;
  formModel.phoneNumber = originalData.value.phoneNumber;
  formModel.riderId = originalData.value.riderId;
  formModel.vehicleNumber = originalData.value.vehicleNumber;
};
</script>

<template>
  <div class="h-full p-24px bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
          <Icon icon="mdi:account-circle" class="text-2xl text-white" />
        </div>
        <div>
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">个人信息管理</h1>
          <p class="mt-2px text-gray-600 dark:text-gray-400">管理您的个人基本信息和账户设置，确保信息准确无误</p>
        </div>
      </div>
    </NCard>

    <!-- 个人信息表单 -->
    <div>
      <NCard :bordered="false" class="rounded-16px shadow-lg bg-white/90 dark:bg-gray-800/90 backdrop-blur-sm">
        <template #header>
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-cyan-500 rounded-lg flex items-center justify-center">
                <Icon icon="mdi:account-edit" class="text-lg text-white" />
              </div>
              <span class="text-lg font-semibold text-gray-800 dark:text-gray-200">基本信息</span>
            </div>
            <NTag :type="isFormComplete ? 'success' : 'warning'" size="small" round>
              <template #icon>
                <Icon :icon="isFormComplete ? 'mdi:check-circle' : 'mdi:alert-circle'" />
              </template>
              {{ isFormComplete ? '已完善' : '待完善' }}
            </NTag>
          </div>
        </template>

        <NForm
          ref="formRef"
          :model="formModel"
          :rules="rules"
          label-placement="top"
          label-width="120px"
          require-mark-placement="right-hanging"
          size="large"
        >
          <div class="space-y-20px">
            <!-- 第一行：姓名和手机号 -->
            <div class="flex gap-24px">
              <div class="flex-1">
                <NFormItem label="姓名" path="name">
                  <NInput v-model:value="formModel.name" placeholder="请输入您的真实姓名" clearable>
                    <template #prefix>
                      <Icon icon="mdi:account" class="text-gray-400" />
                    </template>
                  </NInput>
                </NFormItem>
              </div>
              <div class="flex-1">
                <NFormItem label="手机号" path="phoneNumber">
                  <NInput v-model:value="formModel.phoneNumber" placeholder="请输入手机号码（必填）" clearable>
                    <template #prefix>
                      <Icon icon="mdi:phone" class="text-gray-400" />
                    </template>
                  </NInput>
                </NFormItem>
              </div>
            </div>

            <!-- 第二行：车辆编号 -->
            <div class="flex gap-24px">
              <div class="flex-1">
                <NFormItem label="车辆编号" path="vehicleNumber">
                  <NInput v-model:value="formModel.vehicleNumber" placeholder="请输入车辆编号" clearable>
                    <template #prefix>
                      <Icon icon="mdi:car" class="text-gray-400" />
                    </template>
                  </NInput>
                </NFormItem>
              </div>
              <div class="flex-1"></div>
            </div>
          </div>

          <!-- 操作按钮 -->
          <div class="mt-24px flex justify-center border-t border-gray-200 pt-16px dark:border-gray-700">
            <div class="flex gap-3">
              <NButton type="default" size="large" @click="fetchRiderInfo" :disabled="submitting">
                <template #icon>
                  <Icon icon="mdi:refresh" />
                </template>
                刷新
              </NButton>
              <NButton
                v-if="hasChanges"
                type="default"
                size="large"
                @click="handleCancel"
                :disabled="submitting"
              >
                <template #icon>
                  <Icon icon="mdi:close" />
                </template>
                取消
              </NButton>
              <NButton
                type="primary"
                size="large"
                :loading="submitting"
                :disabled="!hasChanges"
                class="bg-gradient-to-r from-blue-500 to-purple-600 border-0"
                @click="handleSave"
              >
                <template #icon>
                  <Icon icon="mdi:content-save" />
                </template>
                保存更改
              </NButton>
            </div>
          </div>
        </NForm>
      </NCard>
    </div>
  </div>
</template>

<style scoped>
/* 页面整体动画 */
.h-full {
  animation: fadeIn 0.6s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 卡片样式增强 */
.n-card {
  background-color: var(--n-color);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.n-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1), 0 0 0 1px rgba(255, 255, 255, 0.05);
}

/* 输入框样式增强 */
.n-input {
  transition: all 0.3s ease;
  border-radius: 12px;
}

.n-input:hover {
  border-color: var(--n-primary-color);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.n-input:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
}

/* 按钮样式增强 */
.n-button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  border-radius: 12px;
  font-weight: 600;
  position: relative;
  overflow: hidden;
}

.n-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.n-button:hover::before {
  left: 100%;
}

.n-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.n-button:active {
  transform: translateY(0);
}

/* 渐变按钮特殊样式 */
.n-button.bg-gradient-to-r {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
}

.n-button.bg-gradient-to-r:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
  gap: 8px;
}

/* 更具体的按钮样式覆盖 */
.n-button .n-button__content .n-button__text {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

/* 表单样式优化 */
.n-form-item {
  margin-bottom: 0 !important;
}

.n-form-item .n-form-item-label {
  font-weight: 600;
  color: #374151;
  margin-bottom: 8px;
}

.dark .n-form-item .n-form-item-label {
  color: #d1d5db;
}

.n-form-item .n-form-item-blank {
  flex: 1 !important;
}

/* 确保输入框宽度一致 */
.n-input {
  width: 100% !important;
}

/* 标签样式增强 */
.n-tag {
  border-radius: 20px;
  font-weight: 500;
  padding: 4px 12px;
}

/* 统计卡片动画 */
.grid > div {
  transition: all 0.3s ease;
  cursor: pointer;
}

.grid > div:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
}

/* 头像卡片特殊效果 */
.bg-gradient-to-br {
  position: relative;
  overflow: hidden;
}

.bg-gradient-to-br::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%) translateY(-100%) rotate(45deg);
  }
  100% {
    transform: translateX(100%) translateY(100%) rotate(45deg);
  }
}

/* 自定义滚动条 */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
}

/* 暗色模式滚动条 */
.dark ::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.05);
}

.dark ::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #4c51bf 0%, #553c9a 100%);
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #434190 0%, #4c1d95 100%);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .n-grid {
    grid-template-columns: 1fr !important;
  }

  .n-gi {
    grid-column: span 24 !important;
  }

  .flex.gap-24px {
    flex-direction: column;
    gap: 16px;
  }
}

/* 加载状态动画 */
.n-button[loading] {
  position: relative;
}

.n-button[loading]::after {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
