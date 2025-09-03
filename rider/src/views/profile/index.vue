<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import type { FormInst, FormRules } from 'naive-ui';
import { getRiderInfo, updateRiderInfo } from '@/service/api/rider';

// 骑手ID - 使用真实的后端骑手ID
const riderId = '1663b73718a54c65b32f5b6787972949';

const formRef = ref<FormInst | null>(null);
const loading = ref(false);

const formModel = reactive({
  applicationUserId: '',
  name: '',
  phoneNumber: '',
  riderId: '',
  vehicleNumber: ''
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
  vehicleNumber: [{ max: 20, message: '车辆编号长度不能超过20个字符', trigger: 'blur' }]
};

async function fetchRiderInfo() {
  try {
    const { data } = await getRiderInfo({ riderId });
    if (data) {
      Object.assign(formModel, data);
    }
  } catch (error) {
    console.error('获取骑手信息失败', error);
    window.$message?.error('获取骑手信息失败，请检查网络连接');
    // 当API调用失败时，使用模拟数据
    Object.assign(formModel, {
      applicationUserId: 'mock_user_001',
      name: '测试骑手',
      phoneNumber: '13800138000',
      riderId,
      vehicleNumber: '宁A12345'
    });
  }
}

onMounted(() => {
  fetchRiderInfo();
});

async function handleUpdate() {
  formRef.value?.validate(async errors => {
    if (errors) {
      return;
    }

    loading.value = true;
    const { name, phoneNumber, vehicleNumber } = formModel;

    try {
      const { data: updatedData } = await updateRiderInfo(formModel.riderId, {
        name,
        phoneNumber,
        vehicleNumber
      });

      if (updatedData) {
        window.$message?.success('个人信息更新成功！');
        Object.assign(formModel, updatedData);
      }
    } catch (error: any) {
      console.error('更新骑手信息失败', error);
      const errorMessage = error?.response?.data?.message || '更新失败，请稍后重试';
      window.$message?.error(errorMessage);
    } finally {
      loading.value = false;
    }
  });
}
</script>

<template>
  <div class="h-full p-24px">
    <!-- 页面标题区域 -->
    <NCard :bordered="false" class="mb-24px">
      <div class="flex items-center justify-between">
        <div class="flex-1">
          <h1 class="text-2xl text-gray-800 font-bold dark:text-gray-200">个人信息管理</h1>
          <p class="mt-8px text-gray-600 dark:text-gray-400">管理您的个人基本信息和账户设置，确保信息准确无误。</p>
        </div>
      </div>
    </NCard>

    <NGrid :cols="24" :x-gap="16" :y-gap="16">
      <!-- 左侧：个人信息表单 -->
      <NGi :span="16">
        <NCard :bordered="false" class="rounded-16px shadow-sm">
          <template #header>
            <div class="flex items-center">
              <Icon name="mdi:account-edit" class="text-xl text-blue-500" />
              <span class="text-lg font-semibold">基本信息</span>
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
              <!-- 第一行：用户ID和骑手ID -->
              <div class="flex gap-24px">
                <div class="flex-1">
                  <NFormItem label="用户ID" path="applicationUserId">
                    <NInput
                      v-model:value="formModel.applicationUserId"
                      placeholder="用户ID"
                      disabled
                      class="bg-gray-50 dark:bg-gray-800"
                    />
                  </NFormItem>
                </div>
                <div class="flex-1">
                  <NFormItem label="骑手ID" path="riderId">
                    <NInput
                      v-model:value="formModel.riderId"
                      placeholder="骑手ID"
                      disabled
                      class="bg-gray-50 dark:bg-gray-800"
                    />
                  </NFormItem>
                </div>
              </div>

              <!-- 第二行：姓名和手机号 -->
              <div class="flex gap-24px">
                <div class="flex-1">
                  <NFormItem label="姓名" path="name">
                    <NInput v-model:value="formModel.name" placeholder="请输入您的真实姓名" clearable>
                      <template #prefix>
                        <Icon name="mdi:account" class="text-gray-400" />
                      </template>
                    </NInput>
                  </NFormItem>
                </div>
                <div class="flex-1">
                  <NFormItem label="手机号" path="phoneNumber">
                    <NInput v-model:value="formModel.phoneNumber" placeholder="请输入手机号码" clearable>
                      <template #prefix>
                        <Icon name="mdi:phone" class="text-gray-400" />
                      </template>
                    </NInput>
                  </NFormItem>
                </div>
              </div>

              <!-- 第三行：车辆编号 -->
              <div class="flex gap-24px">
                <div class="flex-1">
                  <NFormItem label="车辆编号" path="vehicleNumber">
                    <NInput v-model:value="formModel.vehicleNumber" placeholder="请输入车辆编号" clearable>
                      <template #prefix>
                        <Icon name="mdi:car" class="text-gray-400" />
                      </template>
                    </NInput>
                  </NFormItem>
                </div>
                <div class="flex-1"></div>
              </div>
            </div>

            <!-- 操作按钮 -->
            <div class="mt-24px flex justify-center border-t border-gray-200 pt-16px dark:border-gray-700">
              <NButton type="primary" size="large" :loading="loading" @click="handleUpdate">
                <template #icon>
                  <Icon name="mdi:content-save" />
                </template>
                保存更改
              </NButton>
            </div>
          </NForm>
        </NCard>
      </NGi>

      <!-- 右侧：信息卡片 -->
      <NGi :span="8">
        <NSpace vertical :size="16">
          <!-- 账户状态卡片 -->
          <NCard :bordered="false">
            <template #header>
              <div class="flex items-center">
                <Icon name="mdi:account-edit" class="text-xl text-blue-500" />
                <span class="text-lg font-semibold">账户状态</span>
              </div>
            </template>
            <NSpace vertical :size="12">
              <div class="flex items-center justify-between">
                <span>账户状态</span>
                <NTag type="success" size="small">正常</NTag>
              </div>
              <div class="flex items-center justify-between">
                <span>注册时间</span>
                <span class="text-sm">{{ new Date().toLocaleDateString('zh-CN') }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span>最后更新</span>
                <span class="text-sm">{{ new Date().toLocaleDateString('zh-CN') }}</span>
              </div>
            </NSpace>
          </NCard>
        </NSpace>
      </NGi>
    </NGrid>
  </div>
</template>

<style scoped>
.n-card {
  background-color: var(--n-color);
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.n-input {
  transition: all 0.3s ease;
}

.n-input:hover {
  border-color: var(--n-primary-color);
}

.n-button {
  transition: all 0.3s ease;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

.n-button:hover {
  transform: translateY(-1px);
}

/* 确保按钮内容居中 */
.n-button .n-button__content {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  width: 100% !important;
}

/* 更具体的按钮样式覆盖 */
.n-button .n-button__content .n-button__text {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

/* 表单对齐样式 */
.n-form-item {
  margin-bottom: 0 !important;
}

.n-form-item .n-form-item-label {
  text-align: right !important;
  padding-right: 12px !important;
}

.n-form-item .n-form-item-blank {
  flex: 1 !important;
}

/* 确保输入框宽度一致 */
.n-input {
  width: 100% !important;
}

/* 自定义滚动条 */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* 暗色模式滚动条 */
.dark ::-webkit-scrollbar-track {
  background: #2d2d2d;
}

.dark ::-webkit-scrollbar-thumb {
  background: #555;
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: #777;
}
</style>
