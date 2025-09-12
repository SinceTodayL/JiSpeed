<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getActiveAnnouncements, postAnnouncement } from '@/api/announcement';
import { NCard, NList, NListItem, NThing, NButton, NModal, NForm, NFormItem, NInput, NDatePicker, NSelect, useMessage } from 'naive-ui';
import { $t } from '@/locales';

defineOptions({
  name: 'Announcement'
});

const message = useMessage();
const announcements = ref([]);
const showModal = ref(false);
const formValue = ref({
  title: '',
  content: '',
  targetRole: 'all',
  timeRange: null
});

const roleOptions = [
  { label: '所有人', value: 'all' },
  { label: '用户', value: 'user' },
  { label: '商家', value: 'merchant' },
  { label: '骑手', value: 'rider' }
];

async function fetchAnnouncements() {
  try {
    const { data } = await getActiveAnnouncements('all'); // 默认获取所有角色的公告
    if (data) {
      announcements.value = data;
    }
  } catch (error) {
    console.error('Failed to fetch announcements:', error);
  }
}

function formatDate(dateStr) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString('zh-CN', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
}

function getRoleLabel(role) {
  const roleMap = {
    all: '全部',
    user: '用户',
    merchant: '商家',
    rider: '骑手'
  };
  return roleMap[role] || '全部';
}

async function handlePublish() {
  if (!formValue.value.title || !formValue.value.content || !formValue.value.timeRange) {
    message.warning('请填写所有必填项');
    return;
  }
  
  const adminId = '6f7af74d972c481c91f19596e07aae3a'; // 真实管理员ID
  const announcementData = {
    Title: formValue.value.title,
    Content: formValue.value.content,
    TargetRole: formValue.value.targetRole,
    StartAt: new Date(formValue.value.timeRange[0]).toISOString(),
    EndAt: new Date(formValue.value.timeRange[1]).toISOString()
  };
  
  try {
    await postAnnouncement(adminId, announcementData);
    message.success('公告发布成功');
    showModal.value = false;
    // 重置表单
    formValue.value = {
      title: '',
      content: '',
      targetRole: 'all',
      timeRange: null
    };
    fetchAnnouncements(); // 刷新列表
  } catch (error) {
    message.error('公告发布失败');
    console.error('Failed to post announcement:', error);
  }
}

onMounted(() => {
  fetchAnnouncements();
});
</script>

<template>
  <NCard 
    :title="$t('page.home.announcement.title')" 
    :bordered="false" 
    class="card-wrapper"
  >
    <template #header-extra>
      <NButton 
        type="primary" 
        size="small" 
        @click="showModal = true"
        class="bg-gradient-to-r from-blue-500 to-purple-500 border-none hover:from-blue-600 hover:to-purple-600"
      >
        <template #icon>
          <SvgIcon icon="material-symbols:add" />
        </template>
        {{ $t('page.home.announcement.publish') }}
      </NButton>
    </template>

    <div v-if="announcements.length === 0" class="text-center py-8 text-gray-500">
      <SvgIcon icon="material-symbols:announcement-outline" class="text-4xl mb-2" />
      <p>{{ $t('page.home.announcement.noAnnouncements') }}</p>
    </div>

    <div v-else class="space-y-3">
      <div 
        v-for="item in announcements" 
        :key="item.announceId"
        class="p-4 rounded-lg border border-gray-200 hover:border-blue-300 hover:shadow-md transition-all duration-200 bg-gradient-to-r from-white to-blue-50 hover:from-blue-50 hover:to-purple-50"
      >
        <div class="flex items-start space-x-3">
          <div class="flex-shrink-0 mt-1">
            <div class="w-2 h-2 bg-blue-500 rounded-full"></div>
          </div>
          <div class="flex-1 min-w-0">
            <h4 class="text-sm font-medium text-gray-900 truncate">
              {{ item.title }}
            </h4>
            <p class="text-sm text-gray-600 mt-1 line-clamp-2">
              {{ item.content }}
            </p>
            <div class="flex items-center justify-between mt-2 text-xs text-gray-500">
              <span>{{ formatDate(item.startAt) }}</span>
              <NBadge 
                :value="getRoleLabel(item.targetRole)" 
                type="info"
                class="text-xs"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <NModal 
      v-model:show="showModal" 
      preset="card" 
      style="width: 600px" 
      :title="$t('page.home.announcement.publishModalTitle')"
      class="rounded-xl"
    >
      <NForm :model="formValue" label-placement="left" label-width="80">
        <NFormItem label="标题" required>
          <NInput 
            v-model:value="formValue.title" 
            placeholder="请输入公告标题"
            class="rounded-lg"
          />
        </NFormItem>
        <NFormItem label="内容" required>
          <NInput 
            v-model:value="formValue.content" 
            type="textarea" 
            placeholder="请输入公告内容"
            :rows="4"
            class="rounded-lg"
          />
        </NFormItem>
        <NFormItem label="目标角色">
          <NSelect 
            v-model:value="formValue.targetRole" 
            :options="roleOptions"
            class="rounded-lg"
          />
        </NFormItem>
        <NFormItem label="有效期" required>
          <NDatePicker 
            v-model:value="formValue.timeRange" 
            type="datetimerange" 
            clearable
            class="w-full rounded-lg"
          />
        </NFormItem>
        <div class="flex space-x-3 mt-6 pt-4">
          <NButton @click="showModal = false" class="flex-1 rounded-lg">
            取消
          </NButton>
          <NButton 
            type="primary" 
            @click="handlePublish"
            class="flex-1 bg-gradient-to-r from-blue-500 to-purple-500 border-none rounded-lg"
          >
            立即发布
          </NButton>
        </div>
      </NForm>
    </NModal>
  </NCard>
</template>