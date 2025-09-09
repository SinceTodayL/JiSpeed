<script setup lang="tsx">
import { reactive, ref, computed } from 'vue';
import { NButton, NPopconfirm, NTag, NImage } from 'naive-ui';
import { fetchGetAllDishes, deleteDish, batchDeleteDishes, createDish, updateDish } from '@/service/api';
import { localStg } from '@/utils/storage';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';
import GoodsSearch from './modules/goods-search.vue';
import GoodsOperateDrawer from './modules/goods-operate-drawer.vue';
import MerchantReviewsModal from './modules/dish-reviews-modal.vue';
import { useMerchantStore } from '@/store/modules/merchant';

const merchantStore = useMerchantStore();
const appStore = useAppStore();


const loading = ref(false);
const data = ref<Api.Goods.DishItem[]>([]);
const originalData = ref<Api.Goods.DishItem[]>([]); // 保存原始数据用于搜索
const checkedRowKeys = ref<string[]>([]);
const drawerVisible = ref(false);
const operateType = ref<'add' | 'edit'>('add');
const editingData = ref<Api.Goods.DishItem | null>(null);

// Reviews related state
const reviewsModalVisible = ref(false);

const searchParams = reactive<{
  dishName: string | null;
  categoryId: string | null;
  onSale: number | null;
}>({
  dishName: null,
  categoryId: null,
  onSale: null
});

// Get dishes data from backend API
const getData = async () => {
  loading.value = true;
  try {
    // Check authentication status
    const token = localStg.get('token');
    const merchantId = merchantStore.merchantId;
    
    if (!token) {
      window.$message?.error('未找到认证令牌，请重新登录');
      return;
    }
    
    if (!merchantId) {
      window.$message?.error('未找到商家ID，请检查登录状态');
      return;
    }
    
    // Fetch dishes for the current merchant
    const response: any = await fetchGetAllDishes(merchantStore.merchantId);
    
    // Extract actual data from the response
    // Backend returns: ApiResponse<List<DishesDto>> wrapped by axios
    let actualData: any[] = [];
    
    if (Array.isArray(response)) {
      // Case 1: response is already the dishes array (after transformBackendResponse)
      actualData = response;
    } else if (response && typeof response === 'object') {
      // Case 2: response is axios response object containing ApiResponse
      if (response.data && response.data.data && Array.isArray(response.data.data)) {
        actualData = response.data.data;
      } else if (response.data && Array.isArray(response.data)) {
        actualData = response.data;
      }
    }

    // Transform backend C# PascalCase to frontend camelCase
    if (actualData && Array.isArray(actualData) && actualData.length >= 0) {
      const transformedData = actualData.map((dish: any, index: number) => {
        // Map backend PascalCase fields to frontend camelCase
        const transformedDish: Api.Goods.DishItem = {
          dishId: dish.DishId || dish.dishId || `temp-${index}`,
          categoryId: dish.CategoryId || dish.categoryId || '',
          dishName: dish.DishName || dish.dishName || '未命名菜品',
          price: Number(dish.Price || dish.price || 0),
          originPrice: Number(dish.OriginPrice || dish.originPrice || 0),
          coverUrl: dish.CoverUrl || dish.coverUrl || '',
          monthlySales: dish.MonthlySales || dish.monthlySales || 0,
          rating: dish.Rating || dish.rating || 0,
          onSale: dish.OnSale !== undefined ? dish.OnSale : (dish.onSale !== undefined ? dish.onSale : 1),
          merchantId: dish.MerchantId || dish.merchantId || merchantStore.merchantId,
          StockQuantity: dish.StockQuantity || dish.stockQuantity || 0, // 正确映射库存数量
          reviewQuantity: dish.ReviewQuantity || dish.reviewQuantity || 0,
          categoryName: dish.CategoryName || dish.categoryName || undefined,
          Description: dish.Description || dish.description || undefined // 添加菜品描述映射
        };
        
        return transformedDish;
      });
      
      originalData.value = transformedData; // Save original data for search
      data.value = transformedData; // Display data
    } else {
      originalData.value = [];
      data.value = [];
    }
  } catch (error: any) {
    
    // Enhanced error handling
    let errorMessage = '获取商品数据失败';
    
    if (error?.response?.status === 401) {
      errorMessage = '认证失败，请重新登录';
    } else if (error?.response?.status === 403) {
      errorMessage = '权限不足，无法访问商品数据';
    } else if (error?.response?.status === 404) {
      errorMessage = 'API端点不存在，请检查后端服务状态';
    } else if (error?.response?.status === 500) {
      errorMessage = '后端服务器错误，请联系管理员';
    } else if (error?.code === 'ERR_NETWORK') {
      errorMessage = '网络连接失败，请检查后端服务是否运行';
    }
    
    window.$message?.error(errorMessage);
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

// Delete single dish
const handleDelete = async (dishId: string) => {
  try {
    const result = await deleteDish(merchantStore.merchantId, dishId);
    
    window.$message?.success('商品删除成功');
    getData(); // Refresh data after successful deletion
  } catch (error) {
    window.$message?.error('删除商品失败，请重试');
  }
};

// Batch delete multiple dishes
const handleBatchDelete = async () => {
  if (checkedRowKeys.value.length === 0) {
    window.$message?.warning('请选择要删除的商品');
    return;
  }
  
  try {
    
    // Delete all selected dishes in parallel
    await batchDeleteDishes(merchantStore.merchantId, checkedRowKeys.value);
    
    window.$message?.success(`成功删除 ${checkedRowKeys.value.length} 个商品`);
    checkedRowKeys.value = []; // Clear selection
    getData(); // Refresh data after successful deletion
  } catch (error) {
    window.$message?.error('批量删除商品失败，请重试');
  }
};

// 刷新
const refresh = () => {
  getData();
};

// 提交后重新加载
const handleSubmitted = () => {
  getData();
};

// View reviews
const handleViewReviews = () => {
  reviewsModalVisible.value = true;
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
  { key: 'StockQuantity', title: '库存数量', checked: true },
  { key: 'Description', title: '菜品描述', checked: true },
  { key: 'operate', title: '操作', checked: true },
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
          src={row.coverUrl || undefined}
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
      render: (row: Api.Goods.DishItem) => `${(row.rating || 0).toFixed(1)}%`
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
      key: 'StockQuantity',
      title: '库存数量',
      align: 'center' as const,
      width: 100,
      render: (row: Api.Goods.DishItem) => row.StockQuantity || 0
    },
    {
      key: 'Description',
      title: '菜品描述',
      align: 'center' as const,
      width: 150,
      render: (row: Api.Goods.DishItem) => row.Description || '-'
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center' as const,
      width: 180,
      render: (row: Api.Goods.DishItem) => (
        <div class="flex-center gap-8px">
          <NButton type="info" ghost size="small" onClick={() => handleViewReviews()}>
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
        :scroll-x="1162"
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
      
      <MerchantReviewsModal
        v-model:visible="reviewsModalVisible"
        :merchant-id="merchantStore.merchantId"
      />
    </NCard>
  </div>
</template>

<style scoped></style> 