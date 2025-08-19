<script setup lang="ts">
import { reactive, ref, onMounted, computed, nextTick } from 'vue';
import type { FormInst, FormRules } from 'naive-ui';
import { fetchMerchantInfo, updateMerchantInfo } from '@/service/api';
import { useMerchantStore } from '@/store/modules/merchant';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';

defineOptions({
  name: 'ProfileInfo'
});

const merchantStore = useMerchantStore();
const appStore = useAppStore();

// 表单引用
const formRef = ref<FormInst | null>(null);

// 加载状态
const loading = ref(false);
const submitting = ref(false);

// 商家信息表单数据
const formModel = reactive({
  merchantName: '',
  status: 1,
  contactInfo: '',
  location: ''
});

// 原始数据，用于检测是否有变更
const originalData = ref({
  merchantName: '',
  status: 1,
  contactInfo: '',
  location: ''
});

// 表单初始化状态
const formInitialized = ref(false);

// 表单验证规则
const rules: FormRules = {
  merchantName: [
    { required: true, message: $t('page.profile.merchantNamePlaceholder'), trigger: 'blur' }
  ],
  location: [
    { required: true, message: $t('page.profile.locationPlaceholder'), trigger: 'blur' }
  ],
  contactInfo: [
    { required: true, message: $t('page.profile.contactInfoPlaceholder'), trigger: 'blur' }
  ]
};

// 状态选项
const statusOptions = [
  { label: $t('page.profile.statusActive'), value: 1 },
  { label: $t('page.profile.statusInactive'), value: 0 }
];

// 检测是否有变更
const hasChanges = computed(() => {
  return (
    formModel.merchantName !== originalData.value.merchantName ||
    formModel.status !== originalData.value.status ||
    formModel.contactInfo !== originalData.value.contactInfo ||
    formModel.location !== originalData.value.location
  );
});

// 获取商家信息
const loadMerchantInfo = async () => {
  try {
    loading.value = true;
    console.log('开始获取商家信息');
    console.log('merchantId:', merchantStore.merchantId);
    
    if (!merchantStore.merchantId) {
      console.error('merchantId 为空，无法调用API');
      window.$message?.error('商家ID未找到，请重新登录');
      return;
    }
    
    const result = await fetchMerchantInfo(merchantStore.merchantId);
    console.log('🔙 API完整返回结果:', JSON.stringify(result, null, 2));
    
    // 检查不同的数据结构可能性
    let merchantData = null;
    if (result?.data?.data) {
      // 嵌套结构: { data: { data: {...} } }
      merchantData = result.data.data;
      console.log('使用嵌套数据结构 result.data.data:', merchantData);
    } else if (result?.data) {
      // 直接结构: { data: {...} }
      merchantData = result.data;
      console.log('使用直接数据结构 result.data:', merchantData);
    } else if (result && typeof result === 'object') {
      // API直接返回数据
      merchantData = result;
      console.log('使用原始返回数据:', merchantData);
    }
    
    if (merchantData && typeof merchantData === 'object') {
      console.log('找到商家数据:', merchantData);
      
      // 更新表单数据
      const newFormData = {
        merchantName: merchantData.merchantName || '',
        status: merchantData.status ?? 1,
        contactInfo: merchantData.contactInfo || '',
        location: merchantData.location || ''
      };
      
      console.log('🔧 准备更新表单数据:', newFormData);
      
      // 使用 nextTick 确保DOM更新
      await nextTick();
      
      // 更新表单数据
      formModel.merchantName = newFormData.merchantName;
      formModel.status = newFormData.status;
      formModel.contactInfo = newFormData.contactInfo;
      formModel.location = newFormData.location;
      
      console.log('📝 表单数据已更新:', {
        merchantName: formModel.merchantName,
        status: formModel.status,
        contactInfo: formModel.contactInfo,
        location: formModel.location
      });
      
      // 保存原始数据
      originalData.value.merchantName = newFormData.merchantName;
      originalData.value.status = newFormData.status;
      originalData.value.contactInfo = newFormData.contactInfo;
      originalData.value.location = newFormData.location;
      
      // 标记表单已初始化
      formInitialized.value = true;
      
      // 更新store
      merchantStore.setMerchantInfo(merchantData);
      
      console.log('数据加载完成，表单已填充');
    } else {
      console.error('未找到有效的商家数据');
      console.log('完整API响应:', result);
      window.$message?.warning('获取到的商家信息格式不正确');
    }
  } catch (error) {
    console.error('获取商家信息失败:', error);
    window.$message?.error($t('page.profile.loadFailed'));
  } finally {
    loading.value = false;
  }
};

// 保存商家信息
const handleSave = async () => {
  try {
    // 表单验证
    await formRef.value?.validate();
    
    submitting.value = true;
    
    // 只发送有变更的字段
    const updateData: {
      merchantName?: string;
      status?: number;
      contactInfo?: string;
      location?: string;
    } = {};
    
    if (formModel.merchantName !== originalData.value.merchantName) {
      updateData.merchantName = formModel.merchantName;
    }
    if (formModel.status !== originalData.value.status) {
      updateData.status = formModel.status;
    }
    if (formModel.contactInfo !== originalData.value.contactInfo) {
      updateData.contactInfo = formModel.contactInfo;
    }
    if (formModel.location !== originalData.value.location) {
      updateData.location = formModel.location;
    }
    
    const result = await updateMerchantInfo(merchantStore.merchantId, updateData);
    
    if (result?.data) {
      // 更新原始数据
      Object.assign(originalData.value, formModel);
      
      // 更新store中的商家信息
      merchantStore.setMerchantInfo(result.data);
      
      window.$message?.success($t('page.profile.updateSuccess'));
    }
  } catch (error) {
    console.error('更新商家信息失败:', error);
    window.$message?.error($t('page.profile.updateFailed'));
  } finally {
    submitting.value = false;
  }
};

// 取消编辑，恢复原始数据
const handleCancel = () => {
  Object.assign(formModel, originalData.value);
};

// 暴露加载方法给父组件
defineExpose({
  loadMerchantInfo
});

// 组件挂载时获取数据
onMounted(async () => {
  if (!merchantStore.merchantId) {
    window.$message?.warning('商家ID未找到，请重新登录');
    return;
  }
  
  // 直接从API获取最新的商家信息
  await loadMerchantInfo();
});
</script>

<template>
  <div class="profile-info-container">
    <NSpin :show="loading">
      <div class="max-w-600px">
        <NForm
          ref="formRef"
          :model="formModel"
          :rules="rules"
          label-placement="left"
          label-width="120"
          class="profile-form"
        >
          <NFormItem :label="$t('page.profile.merchantName')" path="merchantName">
            <NInput
              v-model:value="formModel.merchantName"
              :placeholder="$t('page.profile.merchantNamePlaceholder')"
              clearable
            />
          </NFormItem>
          
          <NFormItem :label="$t('page.profile.status')" path="status">
            <NSelect
              v-model:value="formModel.status"
              :options="statusOptions"
              :placeholder="$t('page.profile.status')"
            />
          </NFormItem>
          
          <NFormItem :label="$t('page.profile.contactInfo')" path="contactInfo">
            <NInput
              v-model:value="formModel.contactInfo"
              :placeholder="$t('page.profile.contactInfoPlaceholder')"
              clearable
            />
          </NFormItem>
          
          <NFormItem :label="$t('page.profile.location')" path="location">
            <NInput
              v-model:value="formModel.location"
              :placeholder="$t('page.profile.locationPlaceholder')"
              type="textarea"
              :rows="3"
              clearable
            />
          </NFormItem>
          
          <NFormItem class="form-actions">
            <div class="flex gap-12px">
              <NButton
                type="primary"
                :loading="submitting"
                :disabled="!hasChanges"
                @click="handleSave"
              >
                {{ $t('page.profile.save') }}
              </NButton>
              <NButton
                :disabled="!hasChanges || submitting"
                @click="handleCancel"
              >
                {{ $t('page.profile.cancel') }}
              </NButton>
            </div>
          </NFormItem>
        </NForm>
      </div>
    </NSpin>
  </div>
</template>

<style scoped>
.profile-info-container {
  padding: 20px;
}

.profile-form .form-actions {
  padding-top: 16px;
  border-top: 1px solid var(--n-divider-color);
}
</style>
