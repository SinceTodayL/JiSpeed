<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { fetchActiveAnnouncements } from '@/service/api';

defineOptions({
  name: 'ProjectNews'
});

// Define interface for news/announcement items
interface NewsItem {
  id: string;
  content: string;
  title: string;
  time: string;
}

// Reactive state for storing announcement data
const newses = ref<NewsItem[]>([]);
const loading = ref(false);
const error = ref<string>('');

/**
 * Fetch announcements from backend API
 * Target role is set to 'merchant' as this is merchant dashboard
 * Backend logic: shows announcements where TargetRole='merchant' OR TargetRole=null (for all users)
 * Time filtering: StartAt <= now <= EndAt (only active announcements)
 */
const fetchNews = async () => {
  try {
    loading.value = true;
    error.value = '';
    
    // Fetch active announcements for merchants with pagination (10 items)
    const response: any = await fetchActiveAnnouncements('merchant', {
      size: 10,
      page: 1
    });

    console.log('Full API Response:', response);
    
    // Extract actual data from the response
    // The response structure varies depending on how axios processes it:
    // Case 1: Direct array (after transformBackendResponse) - response is AnnouncementItem[]
    // Case 2: Full axios response - response.data.data is AnnouncementItem[]
    let actualData: any[] = [];
    
    if (Array.isArray(response)) {
      // Case 1: response is already the announcements array
      actualData = response;
      console.log('Response is direct array:', actualData);
    } else if (response && typeof response === 'object') {
      // Case 2: response is axios response object
      if (response.data && response.data.data && Array.isArray(response.data.data)) {
        actualData = response.data.data;
        console.log('Extracted data from response.data.data:', actualData);
      } else if (response.data && Array.isArray(response.data)) {
        actualData = response.data;
        console.log('Extracted data from response.data:', actualData);
      } else {
        console.log('Could not find announcements array in response:', response);
      }
    }

    // Transform backend data to match component interface
    if (actualData && Array.isArray(actualData) && actualData.length > 0) {
      console.log('Processing announcements:', actualData.length);
      
      newses.value = actualData.map((announcement: any, index: number) => {
        console.log(`Announcement ${index}:`, announcement);
        
        // Handle both C# PascalCase and JavaScript camelCase
        const id = announcement.announceId || announcement.AnnounceId || `temp-${index}`;
        const title = announcement.title || announcement.Title || '无标题';
        const content = announcement.content || announcement.Content || title;
        const startAt = announcement.startAt || announcement.StartAt;
        const targetRole = announcement.targetRole || announcement.TargetRole;
        
        console.log(`Processing: ID=${id}, Title=${title}, TargetRole=${targetRole}, StartAt=${startAt}`);
        
        return {
          id,
          content: content || title, // Use content if available, fallback to title
          title,
          time: startAt ? new Date(startAt).toLocaleString('zh-CN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: false
          }) : '无时间'
        };
      });
      
      console.log('Successfully processed announcements:', newses.value);
    } else {
      console.log('No announcements found or invalid data structure:', actualData);
      newses.value = [];
    }
  } catch (err) {
    error.value = '加载公告失败';
    console.error('Error fetching announcements:', err);
    
    // Fallback to empty array if API fails
    newses.value = [];
  } finally {
    loading.value = false;
  }
};

// Fetch announcements on component mount
onMounted(() => {
  fetchNews();
});
</script>

<template>
  <NCard title="平台资讯" :bordered="false" size="small" segmented class="card-wrapper">
    <template #header-extra>
      <a class="text-primary" href="javascript:;" @click="fetchNews">刷新</a>
    </template>
    
    <!-- Loading state -->
    <div v-if="loading" class="flex justify-center items-center py-4">
      <NSpin size="medium" />
      <div class="ml-2 text-gray-500">正在加载公告...</div>
    </div>
    
    <!-- Error state -->
    <div v-else-if="error" class="text-center py-4">
      <div class="text-red-500 mb-2">{{ error }}</div>
      <NButton @click="fetchNews" size="small" type="primary">重试</NButton>
    </div>
    
    <!-- News list -->
    <NList v-else-if="newses.length > 0">
      <NListItem v-for="item in newses" :key="item.id">
        <template #prefix>
          <SoybeanAvatar class="size-48px!" />
        </template>
        <!-- Display both title and content with time -->
        <NThing 
          :title="item.title" 
          :description="`${item.content} - ${item.time}`" 
        />
      </NListItem>
    </NList>
    
    <!-- Empty state -->
    <div v-else class="text-center py-8 text-gray-500">
      <div class="mb-2">暂无公告信息</div>
      <div class="text-xs text-gray-400 mb-3">
        • 检查数据库是否有 TargetRole='merchant' 或 null 的记录<br/>
        • 检查公告时间是否在有效期内
      </div>
      <NButton @click="fetchNews" size="small">重新加载</NButton>
    </div>
  </NCard>
</template>

<style scoped></style>
