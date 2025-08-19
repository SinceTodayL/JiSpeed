<script setup lang="ts">
import { ref } from 'vue';
import { $t } from '@/locales';
import ProfileInfo from './components/profile-info.vue';
import ApplicationForm from './components/application-form.vue';

defineOptions({
  name: 'Profile'
});

// 当前激活的tab
const activeTab = ref('profile');

// 子组件引用
const profileInfoRef = ref<InstanceType<typeof ProfileInfo>>();
const applicationFormRef = ref<InstanceType<typeof ApplicationForm>>();

// Tab选项
const tabOptions = [
  {
    key: 'profile',
    label: '我的信息',
    icon: 'mdi:account-circle'
  },
  {
    key: 'application',
    label: '入驻申请',
    icon: 'mdi:file-document-edit'
  }
];

// 处理tab切换
const handleTabChange = (tabKey: string) => {
  activeTab.value = tabKey;
  
  // 当切换到对应tab时，刷新数据
  if (tabKey === 'profile' && profileInfoRef.value) {
    console.log('🔄 切换到我的信息页面，刷新商家信息...');
    profileInfoRef.value.loadMerchantInfo?.();
  } else if (tabKey === 'application' && applicationFormRef.value) {
    console.log('🔄 切换到入驻申请页面，刷新申请列表...');
    applicationFormRef.value.loadApplications?.();
  }
};
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <NCard :title="$t('page.profile.title')" :bordered="false" size="small" class="card-wrapper">
      <NTabs
        v-model:value="activeTab"
        type="line"
        animated
        @update:value="handleTabChange"
      >
        <NTabPane
          v-for="tab in tabOptions"
          :key="tab.key"
          :name="tab.key"
          :tab="tab.label"
        >
          <template #tab>
            <div class="flex items-center gap-8px">
              <SvgIcon :icon="tab.icon" class="text-16px" />
              <span>{{ tab.label }}</span>
            </div>
          </template>
          
          <!-- 我的信息页面 -->
          <ProfileInfo 
            v-if="tab.key === 'profile'" 
            ref="profileInfoRef"
          />
          
          <!-- 入驻申请页面 -->
          <ApplicationForm 
            v-if="tab.key === 'application'" 
            ref="applicationFormRef"
          />
        </NTabPane>
      </NTabs>
    </NCard>
  </div>
</template>

<style scoped>
.card-wrapper {
  height: 100%;
  display: flex;
  flex-direction: column;
}

:deep(.n-tabs-content) {
  padding: 0;
  flex: 1;
  overflow: hidden;
}

:deep(.n-tab-pane) {
  padding: 0;
  height: 100%;
  overflow-y: auto;
}

:deep(.n-card__content) {
  height: 100%;
  display: flex;
  flex-direction: column;
}

:deep(.n-tabs) {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
