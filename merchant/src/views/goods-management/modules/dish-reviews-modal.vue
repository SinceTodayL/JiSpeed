<script setup lang="tsx">
import { ref, watch, computed, h } from 'vue';
import { NTag, NRate, NButton, NForm, NFormItem, NInput, NInputNumber, NRadioGroup, NRadio, NSpace, NSpin, NPopconfirm, NSelect } from 'naive-ui';
import { fetchDishReviews, createDishReview, updateDishReview, deleteDishReview, getReviewStatusText, REVIEW_STATUS_MAP } from '@/service/api';
import { useFormRules, useNaiveForm } from '@/hooks/common/form';
import SvgIcon from '@/components/custom/svg-icon.vue';

defineOptions({
  name: 'DishReviewsModal'
});

interface Props {
  /** 商家ID */
  merchantId: string;
  /** 菜品ID */
  dishId: string;
  /** 菜品名称 */
  dishName: string;
}

const props = defineProps<Props>();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loading = ref(false);
const reviews = ref<Api.Review.ReviewItem[]>([]);

// 新增评论相关
const showAddForm = ref(false);
const addLoading = ref(false);
const { formRef, validate, restoreValidation } = useNaiveForm();
const { defaultRequiredRule } = useFormRules();

const newReview = ref<Omit<Api.Review.CreateReviewRequest, 'dishId'>>({
  orderId: '',
  userId: '',
  rating: 5,
  content: '',
  isAnonymous: 1
});

// 编辑评论相关
const editingReview = ref<Api.Review.ReviewItem | null>(null);
const showEditForm = ref(false);
const editLoading = ref(false);

// 重置表单
const resetForm = () => {
  Object.assign(newReview.value, {
    orderId: '',
    userId: '',
    rating: 5,
    content: '',
    isAnonymous: 1
  });
  restoreValidation();
};

// 重置编辑表单
const resetEditForm = () => {
  editingReview.value = null;
  restoreValidation();
};

// 表单验证规则
const rules = {
  orderId: defaultRequiredRule,
  userId: defaultRequiredRule,
  content: defaultRequiredRule
};

// 评论状态选项
const statusOptions = Object.entries(REVIEW_STATUS_MAP).map(([value, label]) => ({
  label,
  value: Number(value)
}));

// 获取评论数据
const getReviews = async () => {
  if (!props.dishId || !props.merchantId) {
    return;
  }
  
  loading.value = true;
  try {
    const result = await fetchDishReviews(props.merchantId, props.dishId);
    console.log('评论数据:', result);
    
    if (result && Array.isArray(result)) {
      reviews.value = result;
    } else {
      console.warn('评论数据格式不正确:', result);
      reviews.value = [];
    }
  } catch (error) {
    console.error('获取评论数据失败:', error);
    window.$message?.error('获取评论数据失败，请检查网络连接');
    reviews.value = [];
  } finally {
    loading.value = false;
  }
};

// 关闭模态框
const handleClose = () => {
  visible.value = false;
  reviews.value = [];
  showAddForm.value = false;
  showEditForm.value = false;
  resetForm();
  resetEditForm();
};

// 显示添加表单
const handleShowAddForm = () => {
  showAddForm.value = true;
  resetForm();
};

// 隐藏添加表单
const handleHideAddForm = () => {
  showAddForm.value = false;
  resetForm();
};

// 显示编辑表单
const handleShowEditForm = (review: Api.Review.ReviewItem) => {
  editingReview.value = { ...review };
  showEditForm.value = true;
  showAddForm.value = false;
};

// 隐藏编辑表单
const handleHideEditForm = () => {
  showEditForm.value = false;
  resetEditForm();
};

// 提交新评论
const handleSubmitReview = async () => {
  try {
    await validate();
    addLoading.value = true;
    
    const reviewData: Api.Review.CreateReviewRequest = {
      ...newReview.value,
      dishId: props.dishId
    };
    
    await createDishReview(props.merchantId, props.dishId, reviewData);
    window.$message?.success('评论提交成功');
    
    // 重新获取评论列表
    await getReviews();
    handleHideAddForm();
  } catch (error) {
    console.error('提交评论失败:', error);
    window.$message?.error('评论提交失败，请重试');
  } finally {
    addLoading.value = false;
  }
};

// 提交编辑评论
const handleSubmitEditReview = async () => {
  if (!editingReview.value) return;
  
  try {
    editLoading.value = true;
    
    const updateData: Api.Review.UpdateReviewRequest = {
      reviewId: editingReview.value.reviewId,
      rating: editingReview.value.rating,
      content: editingReview.value.content,
      isAnonymous: editingReview.value.isAnonymous,
      status: editingReview.value.status || 2
    };
    
    await updateDishReview(props.merchantId, props.dishId, updateData);
    window.$message?.success('评论更新成功');
    
    // 重新获取评论列表
    await getReviews();
    handleHideEditForm();
  } catch (error) {
    console.error('更新评论失败:', error);
    window.$message?.error('评论更新失败，请重试');
  } finally {
    editLoading.value = false;
  }
};

// 删除评论
const handleDeleteReview = async (reviewId: string) => {
  try {
    await deleteDishReview(props.merchantId, props.dishId, reviewId);
    window.$message?.success('评论删除成功');
    
    // 重新获取评论列表
    await getReviews();
  } catch (error) {
    console.error('删除评论失败:', error);
    window.$message?.error('删除评论失败，请重试');
  }
};

// 审核评论
const handleApproveReview = async (reviewId: string, status: number) => {
  try {
    const updateData: Api.Review.UpdateReviewRequest = {
      reviewId,
      status
    };
    
    await updateDishReview(props.merchantId, props.dishId, updateData);
    const statusText = status === 2 ? '通过' : '拒绝';
    window.$message?.success(`评论审核${statusText}成功`);
    
    // 重新获取评论列表
    await getReviews();
  } catch (error) {
    console.error('审核评论失败:', error);
    window.$message?.error('审核评论失败，请重试');
  }
};

// 格式化时间
const formatTime = (timeStr: string) => {
  const date = new Date(timeStr);
  return date.toLocaleString('zh-CN');
};

// 获取匿名状态文本
const getAnonymousText = (isAnonymous: number) => {
  return isAnonymous === 1 ? '匿名' : '实名';
};

// 获取匿名状态标签类型
const getAnonymousTagType = (isAnonymous: number) => {
  return isAnonymous === 1 ? 'warning' : 'success';
};

// 获取状态标签类型
const getStatusTagType = (status: number) => {
  switch (status) {
    case 1: return 'warning'; // 待审核
    case 2: return 'success'; // 已通过
    case 3: return 'error';   // 已拒绝
    default: return 'default';
  }
};

// 监听显示状态变化
watch(visible, (newVisible) => {
  if (newVisible) {
    getReviews();
  }
});

// 列配置
const columns = [
  {
    key: 'index',
    title: '序号',
    align: 'center' as const,
    width: 40,
    render: (_: any, index: number) => index + 1
  },
  {
    key: 'reviewId',
    title: '评论ID',
    align: 'center' as const,
    width: 40,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'user',
    title: '用户',
    align: 'center' as const,
    width: 120,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'rating',
    title: '评分',
    align: 'center' as const,
    width: 120,
    render: (row: Api.Review.ReviewItem) => h(NRate, {
      readonly: true,
      value: row.rating,
      size: 'small'
    })
  },
  {
    key: 'content',
    title: '评论内容',
    align: 'left' as const,
    width: 200,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'isAnonymous',
    title: '匿名状态',
    align: 'center' as const,
    width: 60,
    render: (row: Api.Review.ReviewItem) => h(NTag, {
      type: getAnonymousTagType(row.isAnonymous),
      size: 'small'
    }, {
      default: () => getAnonymousText(row.isAnonymous)
    })
  },
  {
    key: 'status',
    title: '状态',
    align: 'center' as const,
    width: 100,
    render: (row: Api.Review.ReviewItem) => h(NTag, {
      type: getStatusTagType(row.status || 2),
      size: 'small'
    }, {
      default: () => getReviewStatusText(row.status || 2)
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
    key: 'operate',
    title: '操作',
    align: 'center' as const,
    width: 200,
    render: (row: Api.Review.ReviewItem) => (
      <div class="flex-center gap-4px">
        <NButton 
          type="primary" 
          ghost 
          size="small" 
          onClick={() => handleShowEditForm(row)}
        >
          编辑
        </NButton>
        {(row.status === 1 || !row.status) && (
          <>
            <NButton 
              type="success" 
              ghost 
              size="small" 
              onClick={() => handleApproveReview(row.reviewId, 2)}
            >
              通过
            </NButton>
            <NButton 
              type="warning" 
              ghost 
              size="small" 
              onClick={() => handleApproveReview(row.reviewId, 3)}
            >
              拒绝
            </NButton>
          </>
        )}
        <NPopconfirm onPositiveClick={() => handleDeleteReview(row.reviewId)}>
          {{
            default: () => '确认删除这条评论吗？',
            trigger: () => (
              <NButton type="error" ghost size="small">
                删除
              </NButton>
            )
          }}
        </NPopconfirm>
      </div>
    )
  }
];
</script>

<template>
  <NModal v-model:show="visible" preset="card" style="width: 1200px;" title="菜品评论管理">
    <template #header>
      <div class="flex items-center justify-between w-full">
        <div class="flex items-center gap-8px">
          <span>菜品评论</span>
          <NTag type="info" size="small">{{ dishName }}</NTag>
        </div>
        <NButton type="primary" @click="handleShowAddForm" :disabled="showAddForm">
          <template #icon>
            <SvgIcon icon="mdi:plus" />
          </template>
          添加评论
        </NButton>
      </div>
    </template>
    
    <div class="min-h-400px">
      <!-- 新增评论表单 -->
      <div v-if="showAddForm" class="mb-16px p-16px bg-gray-50 rounded-8px">
        <div class="flex items-center justify-between mb-16px">
          <h4 class="text-16px font-medium mb-0">新增评论</h4>
          <NButton text @click="handleHideAddForm">
            <template #icon>
              <SvgIcon icon="mdi:close" />
            </template>
          </NButton>
        </div>
        
        <NSpin :show="addLoading">
          <NForm 
            ref="formRef" 
            :model="newReview" 
            :rules="rules"
            label-placement="left"
            label-width="auto"
            class="max-w-600px"
          >
            <div class="grid grid-cols-2 gap-16px">
              <NFormItem label="订单ID" path="orderId">
                <NInput v-model:value="newReview.orderId" placeholder="请输入订单ID" />
              </NFormItem>
              <NFormItem label="用户ID" path="userId">
                <NInput v-model:value="newReview.userId" placeholder="请输入用户ID" />
              </NFormItem>
            </div>
            
            <NFormItem label="评分" path="rating">
              <NRate v-model:value="newReview.rating" />
              <span class="ml-8px text-gray-500">{{ newReview.rating }} 星</span>
            </NFormItem>
            
            <NFormItem label="评论内容" path="content">
              <NInput
                v-model:value="newReview.content"
                type="textarea"
                placeholder="请输入评论内容"
                :autosize="{ minRows: 3, maxRows: 6 }"
                show-count
                maxlength="500"
              />
            </NFormItem>
            
                         <NFormItem label="匿名状态" path="isAnonymous">
               <NRadioGroup v-model:value="newReview.isAnonymous">
                 <NRadio :value="1">匿名评论</NRadio>
                 <NRadio :value="2">实名评论</NRadio>
               </NRadioGroup>
             </NFormItem>
            
            <div class="flex justify-end gap-8px">
              <NButton @click="handleHideAddForm">取消</NButton>
              <NButton type="primary" @click="handleSubmitReview" :loading="addLoading">
                提交评论
              </NButton>
            </div>
          </NForm>
        </NSpin>
      </div>
      
      <!-- 编辑评论表单 -->
      <div v-if="showEditForm && editingReview" class="mb-16px p-16px bg-blue-50 rounded-8px">
        <div class="flex items-center justify-between mb-16px">
          <h4 class="text-16px font-medium mb-0">编辑评论</h4>
          <NButton text @click="handleHideEditForm">
            <template #icon>
              <SvgIcon icon="mdi:close" />
            </template>
          </NButton>
        </div>
        
        <NSpin :show="editLoading">
          <div class="max-w-600px">
            <div class="grid grid-cols-2 gap-16px mb-16px">
              <div>
                <label class="text-sm text-gray-600 mb-4px block">订单ID</label>
                <NInput :value="editingReview.orderId" readonly />
              </div>
              <div>
                <label class="text-sm text-gray-600 mb-4px block">用户ID</label>
                <NInput :value="editingReview.userId" readonly />
              </div>
            </div>
            
            <div class="mb-16px">
              <label class="text-sm text-gray-600 mb-4px block">评分</label>
              <div class="flex items-center gap-8px">
                <NRate v-model:value="editingReview.rating" />
                <span class="text-gray-500">{{ editingReview.rating }} 星</span>
              </div>
            </div>
            
            <div class="mb-16px">
              <label class="text-sm text-gray-600 mb-4px block">评论内容</label>
              <NInput
                v-model:value="editingReview.content"
                type="textarea"
                placeholder="请输入评论内容"
                :autosize="{ minRows: 3, maxRows: 6 }"
                show-count
                maxlength="500"
              />
            </div>
            
            <div class="grid grid-cols-2 gap-16px mb-16px">
              <div>
                <label class="text-sm text-gray-600 mb-4px block">匿名状态</label>
                <NRadioGroup v-model:value="editingReview.isAnonymous">
                  <NRadio :value="1">匿名</NRadio>
                  <NRadio :value="2">实名</NRadio>
                </NRadioGroup>
              </div>
              <div>
                <label class="text-sm text-gray-600 mb-4px block">审核状态</label>
                <NSelect
                  v-model:value="editingReview.status"
                  :options="statusOptions"
                  placeholder="请选择状态"
                />
              </div>
            </div>
            
            <div class="flex justify-end gap-8px">
              <NButton @click="handleHideEditForm">取消</NButton>
              <NButton type="primary" @click="handleSubmitEditReview" :loading="editLoading">
                保存修改
              </NButton>
            </div>
          </div>
        </NSpin>
      </div>
      
      <!-- 评论列表 -->
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
        :row-key="(row: Api.Review.ReviewItem) => row.reviewId"
        :scroll-x="1300"
      />
      
      <!-- 无数据状态 -->
      <div v-if="!loading && reviews.length === 0 && !showAddForm" class="text-center py-40px text-gray-500">
        <div class="mb-8px">
          <SvgIcon icon="mdi:comment-off" class="text-40px" />
        </div>
        <div>暂无评论数据</div>
        <NButton type="primary" class="mt-16px" @click="handleShowAddForm">
          添加第一条评论
        </NButton>
      </div>
    </div>
    
    <template #footer>
      <div class="flex justify-between items-center">
        <div class="text-gray-500 text-sm">
          菜品ID: {{ dishId }} | 商家ID: {{ merchantId }} | 共 {{ reviews.length }} 条评论
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