<script setup lang="tsx">
import { ref, watch, computed, h } from 'vue';
import { NTag, NRate, NButton, NSpace } from 'naive-ui';
import { fetchMerchantReviews, getReviewStatusText, REVIEW_STATUS_MAP } from '@/service/api/review';
import SvgIcon from '@/components/custom/svg-icon.vue';

defineOptions({
  name: 'MerchantReviewsModal'
});

interface Props {
  /** Merchant ID */
  merchantId: string;
}

const props = defineProps<Props>();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loading = ref(false);
const reviews = ref<Api.Review.ReviewItem[]>([]);

// Review status options
const statusOptions = Object.entries(REVIEW_STATUS_MAP).map(([value, label]) => ({
  label,
  value: Number(value)
}));

// Get reviews data from merchant reviews endpoint
const getReviews = async () => {
  if (!props.merchantId) {
    return;
  }
  
  loading.value = true;
  try {
    const response = await fetchMerchantReviews(props.merchantId);
    console.log('Reviews data:', response);
    
    // Handle response based on backend structure
    let reviewsData: Api.Review.ReviewItem[] = [];
    
    if (response && typeof response === 'object') {
      // If response has data property (standard API response)
      if (response.data && Array.isArray(response.data)) {
        reviewsData = response.data;
      }
      // If response is directly an array (after transformBackendResponse)
      else if (Array.isArray(response)) {
        reviewsData = response;
      }
    }
    
    reviews.value = reviewsData;
  } catch (error) {
    console.error('Failed to fetch reviews data:', error);
    window.$message?.error('Failed to fetch reviews data, please check network connection');
    reviews.value = [];
  } finally {
    loading.value = false;
  }
};

// Close modal
const handleClose = () => {
  visible.value = false;
  reviews.value = [];
};

// No merchant review management operations needed

// Format time
const formatTime = (timeStr?: string) => {
  if (!timeStr) return '-';
  const date = new Date(timeStr);
  return date.toLocaleString('zh-CN');
};

// Get anonymous status text
const getAnonymousText = (isAnonymous?: number) => {
  return isAnonymous === 1 ? '匿名' : '实名';
};

// Get anonymous status tag type
const getAnonymousTagType = (isAnonymous?: number) => {
  return isAnonymous === 1 ? 'warning' : 'success';
};

// Get status tag type
const getStatusTagType = (status?: number) => {
  switch (status) {
    case 1: return 'warning'; // Pending
    case 2: return 'success'; // Approved
    case 3: return 'error';   // Rejected
    default: return 'default';
  }
};

// Watch visibility changes
watch(visible, (newVisible) => {
  if (newVisible) {
    getReviews();
  }
});

// Column configuration
const columns = [
  {
    key: 'index',
    title: '序号',
    align: 'center' as const,
    width: 60,
    render: (_: any, index: number) => index + 1
  },
  {
    key: 'userInfo',
    title: '用户信息',
    align: 'center' as const,
    width: 120,
    render: (row: Api.Review.ReviewItem) => (
      <div class="flex items-center gap-8px">
        {row.userAvatarUrl && (
          <img 
            src={row.userAvatarUrl} 
            alt="用户头像"
            class="w-24px h-24px rounded-full object-cover"
          />
        )}
        <span class="text-sm">
          {row.isAnonymous === 1 ? '匿名用户' : (row.userNickname || '未知用户')}
        </span>
      </div>
    )
  },
  {
    key: 'rating',
    title: '评分',
    align: 'center' as const,
    width: 120,
    render: (row: Api.Review.ReviewItem) => h(NRate, {
      readonly: true,
      value: row.rating || 0,
      size: 'small'
    })
  },
  {
    key: 'content',
    title: '评论内容',
    align: 'left' as const,
    width: 250,
    ellipsis: {
      tooltip: true
    },
    render: (row: Api.Review.ReviewItem) => row.content || '-'
  },
  {
    key: 'isAnonymous',
    title: '匿名状态',
    align: 'center' as const,
    width: 100,
    render: (row: Api.Review.ReviewItem) => h(NTag, {
      type: getAnonymousTagType(row.isAnonymous),
      size: 'small'
    }, {
      default: () => getAnonymousText(row.isAnonymous)
    })
  },
  {
    key: 'reviewAt',
    title: '评论时间',
    align: 'center' as const,
    width: 160,
    render: (row: Api.Review.ReviewItem) => formatTime(row.reviewAt)
  },
  {
    key: 'orderId',
    title: '订单ID',
    align: 'center' as const,
    width: 120,
    ellipsis: {
      tooltip: true
    },
    render: (row: Api.Review.ReviewItem) => row.orderId || '-'
  }
];
</script>

<template>
  <NModal v-model:show="visible" preset="card" style="width: 1200px;" title="商家评论管理">
    <template #header>
        <div class="flex items-center gap-8px">
        <span>商家评论</span>
        <NTag type="info" size="small">所有评论</NTag>
      </div>
    </template>
    
    <div class="min-h-400px">
      <!-- Reviews list -->
      <NDataTable
        :columns="columns"
        :data="reviews"
        :loading="loading"
        size="small"
        :pagination="{
          pageSize: 10,
          showSizePicker: true,
          pageSizes: [10, 20, 50]
        }"
        :row-key="(row: Api.Review.ReviewItem) => row.reviewId || row.timestamp?.toString() || Math.random().toString()"
        :scroll-x="1200"
      />
      
      <!-- No data state -->
      <div v-if="!loading && reviews.length === 0" class="text-center py-40px text-gray-500">
        <div class="mb-8px">
          <SvgIcon icon="mdi:comment-off" class="text-40px" />
        </div>
        <div>暂无评论数据</div>
      </div>
    </div>
    
    <template #footer>
      <div class="flex justify-between items-center">
        <div class="text-gray-500 text-sm">
          商家ID: {{ merchantId }} | 评论总数: {{ reviews.length }}
        </div>
        <NSpace>
          <NButton @click="getReviews" :loading="loading">刷新</NButton>
          <NButton @click="handleClose">关闭</NButton>
        </NSpace>
      </div>
    </template>
  </NModal>
</template>

<style scoped></style> 