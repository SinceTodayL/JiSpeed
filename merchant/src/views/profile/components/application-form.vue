<script setup lang="ts">
import { reactive, ref, computed } from 'vue';
import type { FormInst, FormRules } from 'naive-ui';
import { submitApplication, fetchApplications } from '@/service/api';
import { useMerchantStore } from '@/store/modules/merchant';
import { $t } from '@/locales';

defineOptions({
  name: 'ApplicationForm'
});

const merchantStore = useMerchantStore();

// 表单引用
const formRef = ref<FormInst | null>(null);

// 加载状态
const submitting = ref(false);
const loading = ref(false);

// 申请表单数据
const formModel = reactive({
  companyName: '',
  applicationMaterials: ''
});

// 申请记录
const applications = ref<Api.Application.ApplicationResponse[]>([]);

// 表单验证规则
const rules: FormRules = {
  companyName: [
    { required: true, message: '请输入公司名称', trigger: 'blur' },
    { min: 2, max: 100, message: '公司名称长度应为2-100个字符', trigger: 'blur' }
  ],
  applicationMaterials: [
    { required: true, message: '请输入申请理由', trigger: 'blur' },
    { min: 10, max: 1000, message: '申请理由长度应为10-1000个字符', trigger: 'blur' }
  ]
};

// 状态标签映射
const statusMap = {
  '0': { label: '待审核', type: 'info' as const },
  '1': { label: '已通过', type: 'success' as const },
  '2': { label: '已拒绝', type: 'error' as const }
};

// 检查是否可以提交新申请（没有待审核的申请）
const canSubmitApplication = computed(() => {
  return !applications.value.some(app => app.auditStatus === '0');
});

// 提交申请
const handleSubmit = async () => {
  try {
    // 表单验证
    await formRef.value?.validate();
    
    if (!merchantStore.merchantId) {
      window.$message?.error('商家ID未找到，请重新登录');
      return;
    }

    if (!canSubmitApplication.value) {
      window.$message?.warning('您有待审核的申请，请等待审核结果后再提交新申请');
      return;
    }
    
    submitting.value = true;
    
    const result = await submitApplication(merchantStore.merchantId, {
      companyName: formModel.companyName,
      applicationMaterials: formModel.applicationMaterials
    });
    
    if (result?.data) {
      window.$message?.success('申请提交成功，请等待管理员审核');
      
      // 清空表单
      formModel.companyName = '';
      formModel.applicationMaterials = '';
      
      // 重新加载申请列表以显示新提交的申请
      console.log('申请提交成功 !');
      await loadApplications();
    }
  } catch (error) {
    console.error('提交申请失败:', error);
    window.$message?.error('提交申请失败，请稍后再试');
  } finally {
    submitting.value = false;
  }
};

// 重置表单
const handleReset = () => {
  formModel.companyName = '';
  formModel.applicationMaterials = '';
};

// 加载申请列表
const loadApplications = async () => {
  try {
    if (!merchantStore.merchantId) {
      console.warn('merchantId 为空，无法加载申请列表');
      return;
    }

    loading.value = true;
    console.log('开始获取申请列表, merchantId:', merchantStore.merchantId);
    
    const result = await fetchApplications(merchantStore.merchantId, {
      size: 20,
      page: 1
      // 不传递 auditStatus 参数，获取所有状态的申请记录
    });
    
    console.log('API完整返回结果:', JSON.stringify(result, null, 2));
    
    // 根据实际API返回结构，申请列表直接在 result.data 中
    const apiResponse = result as any;
    if (apiResponse?.data && Array.isArray(apiResponse.data)) {
      applications.value = apiResponse.data;
      // console.log('申请列表加载成功，记录数量:', apiResponse.data.length);
      /*
      console.log('申请记录详情:', apiResponse.data.map((app: any) => ({
        applyId: app.applyId,
        companyName: app.companyName,
        auditStatus: app.auditStatus,
        auditAt: app.auditAt,
        rejectReason: app.rejectReason,
        applicationMaterials: app.applicationMaterials
      })));
      */
    } else {
      console.warn('API返回数据格式异常:', result);
      console.log('检查数据路径 result.data:', apiResponse?.data);
      console.log('数据类型:', typeof apiResponse?.data, 'is Array:', Array.isArray(apiResponse?.data));
      applications.value = [];
    }
  } catch (error) {
    console.error('加载申请列表失败:', error);
    window.$message?.error('加载申请列表失败，请稍后重试');
    applications.value = [];
  } finally {
    loading.value = false;
  }
};

// 格式化时间
const formatTime = (timeStr: string) => {
  return new Date(timeStr).toLocaleString('zh-CN');
};

// 暴露加载方法给父组件
defineExpose({
  loadApplications
});

// 组件挂载时加载申请列表
import { onMounted } from 'vue';
onMounted(async () => {
  console.log('申请表单组件挂载，开始加载申请列表...');
  await loadApplications();
});
</script>

<template>
  <div class="application-form-container">
    <div class="mb-24px">
      <h3 class="text-18px font-semibold mb-16px">商家经营资格申请</h3>
      
      <!-- 申请表单 -->
      <NCard title="提交新申请" :bordered="true" size="small">
        <template #header-extra>
          <NTag v-if="!canSubmitApplication" type="warning" size="small">
            有待审核申请
          </NTag>
        </template>
        
        <NForm
          ref="formRef"
          :model="formModel"
          :rules="rules"
          label-placement="left"
          label-width="120"
          class="application-form"
        >
          <NFormItem label="公司名称" path="companyName">
            <NInput
              v-model:value="formModel.companyName"
              placeholder="请输入公司名称"
              :disabled="!canSubmitApplication"
              clearable
            />
          </NFormItem>
          
          <NFormItem label="申请理由" path="applicationMaterials">
            <NInput
              v-model:value="formModel.applicationMaterials"
              placeholder="请详细说明您的申请理由，包括业务范围、运营计划等"
              type="textarea"
              :rows="4"
              :disabled="!canSubmitApplication"
              clearable
            />
          </NFormItem>
          
          <NFormItem>
            <div class="flex gap-12px">
              <NButton
                type="primary"
                :loading="submitting"
                :disabled="!canSubmitApplication"
                @click="handleSubmit"
              >
                提交申请
              </NButton>
              <NButton
                :disabled="submitting || !canSubmitApplication"
                @click="handleReset"
              >
                重置表单
              </NButton>
            </div>
          </NFormItem>
        </NForm>
        
        <template v-if="!canSubmitApplication">
          <NAlert type="warning" show-icon class="mt-16px">
            <template #header>温馨提示</template>
            您当前有待审核的申请，请等待管理员审核结果后再提交新申请。
          </NAlert>
        </template>
      </NCard>
    </div>

    <!-- 申请记录 -->
    <div>
      <div class="flex items-center justify-between mb-16px">
        <h3 class="text-18px font-semibold">申请记录</h3>
        <NButton
          size="small"
          type="primary"
          :loading="loading"
          @click="loadApplications"
        >
          <template #icon>
            <SvgIcon icon="mdi:refresh" />
          </template>
          刷新记录
        </NButton>
      </div>
      
      <NSpin :show="loading">
        <div v-if="applications.length > 0" class="space-y-20px">
          <NCard
            v-for="app in applications"
            :key="app.applyId"
            :bordered="true"
            size="medium"
            class="application-record"
          >
            <template #header>
              <div class="flex items-start justify-between gap-16px">
                <div class="flex-1 min-w-0">
                  <h4 class="text-18px font-semibold text-gray-800 mb-4px">{{ app.companyName }}</h4>
                  <p class="text-12px text-gray-500">申请ID: {{ app.applyId }}</p>
                </div>
                <NTag
                  :type="statusMap[app.auditStatus as keyof typeof statusMap]?.type"
                  size="medium"
                >
                  {{ statusMap[app.auditStatus as keyof typeof statusMap]?.label }}
                </NTag>
              </div>
            </template>
            
            <div class="application-content">
              <!-- 基本信息区域 -->
              <div class="info-section">
                <h5 class="section-title">基本信息</h5>
                <div class="info-grid">
                  <div class="info-item">
                    <span class="info-label">提交时间</span>
                    <span class="info-value">{{ formatTime(app.submittedAt) }}</span>
                  </div>
                  
                  <div v-if="app.auditAt" class="info-item">
                    <span class="info-label">审核时间</span>
                    <span class="info-value">{{ formatTime(app.auditAt) }}</span>
                  </div>
                  
                  <div v-if="app.adminId" class="info-item">
                    <span class="info-label">审核管理员</span>
                    <span class="info-value">{{ app.adminId }}</span>
                  </div>
                </div>
              </div>

              <!-- 申请理由区域 -->
              <div v-if="app.applicationMaterials" class="info-section">
                <h5 class="section-title">申请理由</h5>
                <div class="content-box content-normal">
                  {{ app.applicationMaterials }}
                </div>
              </div>
              
              <!-- 拒绝原因区域 -->
              <div v-if="app.rejectReason && app.auditStatus === '2'" class="info-section">
                <h5 class="section-title text-red-600">拒绝原因</h5>
                <div class="content-box content-error">
                  {{ app.rejectReason }}
                </div>
              </div>
            </div>
          </NCard>
        </div>
        
        <NEmpty
          v-else
          description="暂无申请记录"
          class="py-40px"
        />
      </NSpin>
    </div>
  </div>
</template>

<style scoped>
.application-form-container {
  padding: 20px;
  /* 删除底部栏后，不需要额外的高度限制和滚动 */
}

.application-form {
  max-width: 700px;
}

.application-record {
  transition: all 0.2s ease;
  margin-bottom: 20px;
}

.application-record:hover {
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.12);
  transform: translateY(-1px);
}

.application-content {
  padding: 4px 0;
}

.info-section {
  margin-bottom: 20px;
}

.info-section:last-child {
  margin-bottom: 0;
}

.section-title {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 12px;
  padding-bottom: 6px;
  border-bottom: 2px solid #f0f0f0;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.info-label {
  font-size: 12px;
  color: #666;
  font-weight: 500;
}

.info-value {
  font-size: 14px;
  color: #333;
  word-break: break-all;
}

.content-box {
  padding: 16px;
  border-radius: 8px;
  font-size: 14px;
  line-height: 1.6;
  word-wrap: break-word;
  white-space: pre-wrap;
}

.content-normal {
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  color: #495057;
}

.content-error {
  background-color: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .application-form-container {
    padding: 16px;
  }
  
  .info-grid {
    grid-template-columns: 1fr;
  }
  
  .application-form {
    max-width: 100%;
  }
}
</style>
