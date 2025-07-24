<script setup lang="tsx">
import { reactive, ref, computed } from 'vue';
import { NButton, NPopconfirm, NTag, NImage } from 'naive-ui';
import { fetchGetAllDishes } from '@/service/api';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';
import GoodsSearch from './modules/goods-search.vue';
import GoodsOperateDrawer from './modules/goods-operate-drawer.vue';
import DishReviewsModal from './modules/dish-reviews-modal.vue';

const appStore = useAppStore();

// 使用硬编码的商家ID进行测试
const MERCHANT_ID = 'MER1234567890001';

const loading = ref(false);
const data = ref<Api.Goods.DishItem[]>([]);
const originalData = ref<Api.Goods.DishItem[]>([]); // 保存原始数据用于搜索
const checkedRowKeys = ref<string[]>([]);
const drawerVisible = ref(false);
const operateType = ref<'add' | 'edit'>('add');
const editingData = ref<Api.Goods.DishItem | null>(null);

// 评论相关状态
const reviewsModalVisible = ref(false);
const selectedDishForReviews = ref<{
  dishId: string;
  dishName: string;
}>({
  dishId: '',
  dishName: ''
});

const searchParams = reactive<{
  dishName: string | null;
  categoryId: string | null;
  onSale: number | null;
}>({
  dishName: null,
  categoryId: null,
  onSale: null
});

// 获取数据
const getData = async () => {
  loading.value = true;
  try {
    const result = await fetchGetAllDishes(MERCHANT_ID);
    console.log('API响应:', result);
    
    // API已经经过 transformBackendResponse 处理，直接是 data 数组
    if (result && Array.isArray(result)) {
      originalData.value = result; // 保存原始数据
      data.value = result; // 显示数据
      console.log('成功获取商品数据:', result);
    } else {
      console.warn('API响应格式不正确:', result);
      originalData.value = [];
      data.value = [];
    }
  } catch (error) {
    console.error('获取商品数据失败:', error);
    window.$message?.error('获取商品数据失败，请检查网络连接');
    originalData.value = [];
    data.value = [];
  } finally {
    loading.value = false;
  }
};

// 本地搜索过滤
const filterData = () => {
  let filteredData = [...originalData.value];
  
  // 根据菜品名称搜索
  if (searchParams.dishName && searchParams.dishName.trim()) {
    filteredData = filteredData.filter(item => 
      item.dishName.toLowerCase().includes(searchParams.dishName!.toLowerCase())
    );
  }
  
  // 根据分类ID搜索
  if (searchParams.categoryId && searchParams.categoryId.trim()) {
    filteredData = filteredData.filter(item => 
      item.categoryId.includes(searchParams.categoryId!)
    );
  }
  
  // 根据销售状态搜索
  if (searchParams.onSale !== null && searchParams.onSale !== undefined) {
    const onSaleValue = Number(searchParams.onSale);
    filteredData = filteredData.filter(item => 
      Number(item.onSale) === onSaleValue
    );
  }
  
  data.value = filteredData;
};

// 搜索
const handleSearch = () => {
  filterData();
};

// 重置搜索
const resetSearchParams = () => {
  Object.assign(searchParams, {
    dishName: null,
    categoryId: null,
    onSale: null
  });
  data.value = [...originalData.value]; // 重置为原始数据
};

// 新增
const handleAdd = () => {
  operateType.value = 'add';
  editingData.value = null;
  drawerVisible.value = true;
};

// 编辑
const handleEdit = (dishId: string) => {
  const item = data.value.find(dish => dish.dishId === dishId);
  if (item) {
    operateType.value = 'edit';
    editingData.value = item;
    drawerVisible.value = true;
  }
};

// 删除
const handleDelete = (dishId: string) => {
  console.log('删除商品:', dishId);
  // TODO: 实现删除功能
  getData();
};

// 批量删除
const handleBatchDelete = () => {
  console.log('批量删除:', checkedRowKeys.value);
  // TODO: 实现批量删除功能
  checkedRowKeys.value = [];
  getData();
};

// 刷新
const refresh = () => {
  getData();
};

// 提交后重新加载
const handleSubmitted = () => {
  getData();
};

// 查看评论
const handleViewReviews = (dishId: string) => {
  const item = data.value.find(dish => dish.dishId === dishId);
  if (item) {
    selectedDishForReviews.value = {
      dishId: item.dishId,
      dishName: item.dishName
    };
    reviewsModalVisible.value = true;
  }
};

// 列配置
const columnChecks = ref([
  { key: 'selection', title: '选择', checked: true },
  { key: 'coverUrl', title: '商品图片', checked: true },
  { key: 'dishName', title: '菜品名称', checked: true },
  { key: 'price', title: '价格', checked: true },
  { key: 'originPrice', title: '原价', checked: true },
  { key: 'monthlySales', title: '月销量', checked: true },
  { key: 'rating', title: '评分', checked: true },
  { key: 'onSale', title: '销售状态', checked: true },
  { key: 'quantity', title: '库存数量', checked: true },
  { key: 'operate', title: '操作', checked: true }
]);

const columns = computed(() => {
  const allColumns = [
    {
      type: 'selection' as const,
      align: 'center' as const,
      width: 48
    },
    {
      key: 'coverUrl',
      title: '商品图片',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => (
        <NImage
          width="60"
          height="60"
          src={row.coverUrl}
          fallback-src="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHJlY3Qgd2lkdGg9IjYwIiBoZWlnaHQ9IjYwIiBmaWxsPSIjRjVGNUY1Ii8+CjxwYXRoIGQ9Ik0yNSAyNUMyNSAyNy43NjE0IDI3LjIzODYgMzAgMzAgMzBDMzIuNzYxNCAzMCAzNSAyNy43NjE0IDM1IDI1QzM1IDIyLjIzODYgMzIuNzYxNCAyMCAzMCAyMEMyNy4yMzg2IDIwIDI1IDIyLjIzODYgMjUgMjVaIiBmaWxsPSIjQzNDM0MzIi8+CjxwYXRoIGQ9Ik0yMCAzNUwyNS44NTc5IDI5LjE0MjFDMjYuMjQ4NCAyOC43NTE2IDI2Ljg4MTYgMjguNzUxNiAyNy4yNzIxIDI5LjE0MjFMMzAgMzJMMzIuNzI3OSAyOS4yNzIxQzMzLjExODQgMjguODgxNiAzMy43NTE2IDI4Ljg4MTYgMzQuMTQyMSAyOS4yNzIxTDQwIDM1VjQwSDE5VjM1eiIgZmlsbD0iI0MzQzNDMyIvPgo8L3N2Zz4K"
          style="border-radius: 4px; object-fit: cover;"
        />
      )
    },
    {
      key: 'dishName',
      title: '菜品名称',
      align: 'center' as const,
      width: 120
    },
    {
      key: 'price',
      title: '价格',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => `¥${row.price.toFixed(2)}`
    },
    {
      key: 'originPrice',
      title: '原价',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => `¥${row.originPrice.toFixed(2)}`
    },
    {
      key: 'monthlySales',
      title: '月销量',
      align: 'center' as const,
      width: 100
    },
    {
      key: 'rating',
      title: '评分',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => `${row.rating.toFixed(1)}%`
    },
    {
      key: 'onSale',
      title: '销售状态',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => {
        // 确保转换为数字类型
        const status = Number(row.onSale);
        const isOnSale = status === 1;
        
        return (
          <NTag type={isOnSale ? 'success' : 'error'}>
            {isOnSale ? '在售' : '停售'}
          </NTag>
        );
      }
    },
    {
      key: 'quantity',
      title: '库存数量',
      align: 'center' as const,
      width: 100
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center' as const,
      width: 180,
      render: (row: Api.Goods.DishItem) => (
        <div class="flex-center gap-8px">
          <NButton type="info" ghost size="small" onClick={() => handleViewReviews(row.dishId)}>
            查看评论
          </NButton>
          <NButton type="primary" ghost size="small" onClick={() => handleEdit(row.dishId)}>
            编辑
          </NButton>
          <NPopconfirm onPositiveClick={() => handleDelete(row.dishId)}>
            {{
              default: () => '确认删除吗？',
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

  return allColumns.filter(column => {
    const checkItem = columnChecks.value.find(item => item.key === column.key);
    return checkItem?.checked !== false;
  });
});

// 初始化数据
getData();
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <GoodsSearch v-model:model="searchParams" @search="handleSearch" @reset="resetSearchParams" />
    <NCard title="商品管理" :bordered="false" size="small" class="card-wrapper sm:flex-1-hidden">
      <template #header-extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          @add="handleAdd"
          @delete="handleBatchDelete"
          @refresh="refresh"
        />
      </template>
      <NDataTable
        v-model:checked-row-keys="checkedRowKeys"
        :columns="columns"
        :data="data"
        size="small"
        :flex-height="!appStore.isMobile"
        :scroll-x="1012"
        :loading="loading"
        remote
        :row-key="(row: Api.Goods.DishItem) => row.dishId"
        class="sm:h-full"
      />
      <GoodsOperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        @submitted="handleSubmitted"
      />
      
      <!-- 评论模态框 -->
      <DishReviewsModal
        v-model:visible="reviewsModalVisible"
        :merchant-id="MERCHANT_ID"
        :dish-id="selectedDishForReviews.dishId"
        :dish-name="selectedDishForReviews.dishName"
      />
    </NCard>
  </div>
</template>

<style scoped></style> 