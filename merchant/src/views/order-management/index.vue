<script setup lang="tsx">
import { reactive, ref, computed } from 'vue';
import { NButton, NPopconfirm, NTag, NCard, NDataTable } from 'naive-ui';
import { fetchGetAllOrders, fetchOrderDetailsBatch, fetchDishDetailsBatch, getOrderStatusText } from '@/service/api';
import { useAppStore } from '@/store/modules/app';
import { useMerchantStore } from '@/store/modules/merchant';
import { $t } from '@/locales';
import TableHeaderOperation from '@/components/advanced/table-header-operation.vue';
import OrderSearch from './modules/order-search.vue';
import OrderOperateDrawer from './modules/order-operate-drawer.vue';

const appStore = useAppStore();
const merchantStore = useMerchantStore();

const loading = ref(false);
const data = ref<Api.Order.OrderDetailItem[]>([]);
const originalData = ref<Api.Order.OrderDetailItem[]>([]); 
const checkedRowKeys = ref<string[]>([]);
const drawerVisible = ref(false);
const operateType = ref<'add' | 'edit'>('add');
const editingData = ref<Api.Order.OrderDetailItem | null>(null);

const searchParams = reactive<{
  orderId: string | null;
  orderStatus: number | null;
}>({
  orderId: null,
  orderStatus: null
});

// 获取数据 - 先获取订单ID列表，再获取详细信息
const getData = async () => {
  loading.value = true;
  try {
    console.log('开始获取订单数据，商家ID:', merchantStore.merchantId);
    
    // 分别获取所有状态的订单ID列表
    const statusesToFetch = [0, 1, 2, 3, 4, 5, 6, 7, 8]; // 未支付、已支付、确认收货、已评价、售后中、售后结束、订单关闭、配送中、待配送
    const allOrderPromises = statusesToFetch.map(status => 
      fetchGetAllOrders(merchantStore.merchantId, { orderStatus: status })
    );
    
    const results = await Promise.all(allOrderPromises);
    
    // 合并所有订单ID
    let allOrderIds: string[] = [];
    results.forEach(result => {
      // @sa/axios的createFlatRequest返回包装对象 {data: [...], error: null, response: {...}}
      // 需要提取data字段
      const orderIds = result?.data;
      if (Array.isArray(orderIds)) {
        allOrderIds = allOrderIds.concat(orderIds);
      }
    });
    
    console.log('获取到订单ID列表:', allOrderIds);
    
    if (allOrderIds.length === 0) {
      console.log('没有找到订单数据');
      originalData.value = [];
      data.value = [];
      return;
    }
    
    // 批量获取订单详细信息
    const orderDetails = await fetchOrderDetailsBatch(allOrderIds);
    
    console.log('获取到订单详情:', orderDetails.length, '条记录');
    
    if (orderDetails.length === 0) {
      console.log('所有订单详情获取失败或无有效订单');
      window.$message?.warning('未能获取到有效的订单详情信息');
      originalData.value = [];
      data.value = [];
      return;
    }
    
    // 转换后端PascalCase为前端camelCase  
    console.log('订单详情数据样例:', orderDetails[0]);
    
    const transformedOrders = orderDetails.map((detail: any) => ({
      orderId: detail.OrderId || detail.orderId || '',
      merchantId: detail.MerchantId || detail.merchantId || merchantStore.merchantId,
      userId: detail.UserId || detail.userId || '',
      addressId: detail.AddressId || detail.addressId || '',
      orderAmount: Number(detail.OrderAmount || detail.orderAmount || 0),
      createAt: detail.CreateAt || detail.createAt || '',
      orderStatus: Number(detail.OrderStatus || detail.orderStatus || 0),
      reconId: detail.ReconId || detail.reconId || '',
      couponId: detail.CouponId || detail.couponId || '',
      assignId: detail.AssignId || detail.assignId || '',
      orderDishes: Array.isArray(detail.OrderDishes) ? detail.OrderDishes.map((od: any) => ({
        dishId: od.DishId || od.dishId || '',
        quantity: Number(od.Quantity || od.quantity || 0)
      })) : (Array.isArray(detail.orderDishes) ? detail.orderDishes : []),
      orderLogIds: detail.OrderLogIds || detail.orderLogIds || [],
      paymentIds: detail.PaymentIds || detail.paymentIds || [],
      refundIds: detail.RefundIds || detail.refundIds || [],
      complaintIds: detail.ComplaintIds || detail.complaintIds || [],
      reviewIds: detail.ReviewIds || detail.reviewIds || []
    }));
    
    // 获取所有涉及的菜品ID
    const allDishIds = new Set<string>();
    transformedOrders.forEach(order => {
      order.orderDishes.forEach((dish: any) => {
        if (dish.dishId) {
          allDishIds.add(dish.dishId);
        }
      });
    });
    
    console.log('订单涉及的菜品ID:', Array.from(allDishIds));
    
    // 批量获取菜品详情
    let dishDetailsMap: Record<string, Api.Goods.DishItem> = {};
    if (allDishIds.size > 0) {
      try {
        dishDetailsMap = await fetchDishDetailsBatch(merchantStore.merchantId, Array.from(allDishIds));
        console.log('获取到菜品详情:', dishDetailsMap);
      } catch (error) {
        console.error('获取菜品详情失败:', error);
        window.$message?.warning('获取菜品详情失败，订单中菜品信息可能不完整');
      }
    }
    
    // 将菜品详情补充到订单数据中
    const ordersWithDishDetails = transformedOrders.map(order => ({
      ...order,
      orderDishes: order.orderDishes.map((dish: any) => ({
        ...dish,
        dishDetails: dishDetailsMap[dish.dishId] || null // 补充菜品详细信息
      }))
    }));
    
    // 按创建时间降序排列
    ordersWithDishDetails.sort((a, b) => new Date(b.createAt).getTime() - new Date(a.createAt).getTime());
    
    originalData.value = ordersWithDishDetails;
    data.value = ordersWithDishDetails;
    console.log('成功获取订单详细数据（包含菜品详情）:', ordersWithDishDetails.length, '条记录');
    console.log('最终数据赋值给data.value:', data.value.length, '条');
    
  } catch (error: any) {
    console.error('获取订单数据失败:', error);
    
    let errorMessage = '获取订单数据失败';
    if (error?.response?.status === 401) {
      errorMessage = '认证失败，请重新登录';
    } else if (error?.response?.status === 403) {
      errorMessage = '权限不足，无法访问订单数据';
    } else if (error?.response?.status === 404) {
      errorMessage = 'API端点不存在，请检查后端服务状态';
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
  
  // 根据订单ID搜索
  if (searchParams.orderId && searchParams.orderId.trim()) {
    filteredData = filteredData.filter(item => 
      item.orderId.toLowerCase().includes(searchParams.orderId!.toLowerCase())
    );
  }
  
  // 根据订单状态搜索（在已有的1-4状态中进一步过滤）
  if (searchParams.orderStatus !== null && searchParams.orderStatus !== undefined) {
    const statusValue = Number(searchParams.orderStatus);
    filteredData = filteredData.filter(item => 
      Number(item.orderStatus) === statusValue
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
    orderId: null,
    orderStatus: null
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
const handleEdit = (orderId: string) => {
  const item = data.value.find(order => order.orderId === orderId);
  if (item) {
    operateType.value = 'edit';
    editingData.value = item;
    drawerVisible.value = true;
  }
};

// 删除
const handleDelete = (orderId: string) => {
  console.log('删除订单:', orderId);
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

// 列配置
const columnChecks = ref([
  { key: 'selection', title: '选择', checked: true },
  { key: 'orderSummary', title: '订单概要', checked: true },
  { key: 'customerInfo', title: '客户信息', checked: true },
  { key: 'dishDetails', title: '菜品详情', checked: true },
  { key: 'orderAmount', title: '订单金额', checked: true },
  { key: 'orderStatus', title: '订单状态', checked: true },
  { key: 'createAt', title: '下单时间', checked: true },
  { key: 'dishCount', title: '菜品数量', checked: false },
  { key: 'paymentInfo', title: '支付信息', checked: false },
  { key: 'addressInfo', title: '配送地址', checked: false },
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
      key: 'orderSummary',
      title: '订单概要',
      align: 'left' as const,
      width: 200,
      render: (row: any) => {
        const dishCount = row.orderDishes?.length || 0;
        const shortOrderId = row.orderId.slice(-6); // 只显示订单ID的后6位
        
        const totalAmount = row.orderAmount ? `¥${row.orderAmount.toFixed(2)}` : '¥0.00';
        
        return (
          <div class="flex-col gap-4px">
            <div class="text-sm font-semibold">#{shortOrderId}</div>
            <div class="text-xs text-gray-600">
              {dishCount > 0 ? `${dishCount}种菜品 | ${totalAmount}` : '订单详情'}
            </div>
          </div>
        );
      }
    },
    {
      key: 'customerInfo',
      title: '客户信息',
      align: 'left' as const,
      width: 120,
      render: (row: any) => {
        const shortUserId = row.userId ? row.userId.slice(-6) : '未知用户'; // 只显示用户ID的后6位
        return (
          <div class="flex-col gap-4px">
            <div class="text-sm">用户{shortUserId}</div>
            {row.couponId && (
              <div class="text-xs text-blue-600">使用优惠券</div>
            )}
          </div>
        );
      }
    },
    {
      key: 'dishDetails',
      title: '菜品详情',
      align: 'left' as const,
      width: 300,
      render: (row: any) => {
        const dishes = row.orderDishes || [];
        if (dishes.length === 0) {
          return <div class="text-sm text-gray-500">无菜品信息</div>;
        }
        
        return (
          <div class="flex-col gap-8px max-h-120px overflow-y-auto">
            {dishes.map((dish: any, index: number) => {
              const dishDetails = dish.dishDetails;
              const dishName = dishDetails?.dishName || `菜品${dish.dishId.slice(-4)}`;
              const price = dishDetails?.price ? `¥${dishDetails.price.toFixed(2)}` : '价格未知';
              
              return (
                <div key={dish.dishId} class="flex justify-between items-center text-sm border-b border-gray-100 pb-4px last:border-b-0">
                  <div class="flex-col flex-1">
                    <div class="font-medium text-gray-800">{dishName}</div>
                    <div class="text-xs text-gray-500">单价: {price}</div>
                  </div>
                  <div class="text-right ml-8px">
                    <div class="font-semibold text-orange-600">x{dish.quantity}</div>
                    <div class="text-xs text-gray-500">
                      {dishDetails?.price ? `¥${(dishDetails.price * dish.quantity).toFixed(2)}` : '-'}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        );
      }
    },
    {
      key: 'orderAmount',
      title: '订单金额',
      align: 'center' as const,
      width: 100,
      render: (row: any) => (
        <div class="font-semibold text-orange-600">
          ¥{row.orderAmount.toFixed(2)}
        </div>
      )
    },
    {
      key: 'orderStatus',
      title: '订单状态',
      align: 'center' as const,
      width: 100,
      render: (row: any) => {
        const statusText = getOrderStatusText(row.orderStatus);
        let type: 'default' | 'success' | 'warning' | 'error' | 'info' = 'default';
        
        switch (row.orderStatus) {
          case 1:
            type = 'success'; // 已支付
            break;
          case 2:
            type = 'info'; // 确认收货
            break;
          case 3:
            type = 'success'; // 已评价
            break;
          case 4:
            type = 'warning'; // 售后中
            break;
          case 7:
            type = 'warning'; // 配送中
            break;
          case 8:
            type = 'info'; // 待配送
            break;
          default:
            type = 'default';
        }
        
        return (
          <NTag type={type}>
            {statusText}
          </NTag>
        );
      }
    },
    {
      key: 'createAt',
      title: '下单时间',
      align: 'center' as const,
      width: 140,
      render: (row: any) => {
        // 格式化时间显示
        const date = new Date(row.createAt);
        return (
          <div class="flex-col gap-4px">
            <div class="text-sm">{date.toLocaleDateString('zh-CN')}</div>
            <div class="text-xs text-gray-500">{date.toLocaleTimeString('zh-CN', {hour: '2-digit', minute: '2-digit'})}</div>
          </div>
        );
      }
    },
    {
      key: 'dishCount',
      title: '菜品数量',
      align: 'center' as const,
      width: 80,
      render: (row: any) => {
        const totalQuantity = row.orderDishes?.reduce((sum: number, dish: any) => sum + (dish.quantity || 0), 0) || 0;
        const dishCount = row.orderDishes?.length || 0;
        return (
          <div class="text-center">
            <div class="text-sm font-semibold">{totalQuantity}</div>
            <div class="text-xs text-gray-500">{dishCount}种菜品 {totalQuantity}份</div>
          </div>
        );
      }
    },
    {
      key: 'paymentInfo',
      title: '支付信息',
      align: 'center' as const,
      width: 120,
      render: (row: any) => {
        const paymentCount = row.paymentIds?.length || 0;
        return paymentCount > 0 ? `${paymentCount}笔支付` : '无支付记录';
      }
    },
    {
      key: 'addressInfo',
      title: '配送地址',
      align: 'left' as const,
      width: 200,
      ellipsis: {
        tooltip: true
      },
      render: (row: any) => {
        const shortAddressId = row.addressId ? row.addressId.slice(-6) : '';
        return shortAddressId ? `地址${shortAddressId}` : '无地址信息';
      }
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center' as const,
      width: 160,
      render: (row: any) => (
        <div class="flex-center gap-8px">
          <NButton type="primary" ghost size="small" onClick={() => handleEdit(row.orderId)}>
            查看详情
          </NButton>
          {/* 根据订单状态显示不同的操作按钮 */}
          {row.orderStatus === 1 && (
            <NButton type="success" ghost size="small">
              确认订单
            </NButton>
          )}
          {row.orderStatus === 4 && (
            <NButton type="warning" ghost size="small">
              处理售后
            </NButton>
          )}
          {row.orderStatus === 7 && (
            <NButton type="info" ghost size="small">
              开始配送
            </NButton>
          )}
          {row.orderStatus === 8 && (
            <NButton type="info" ghost size="small">
              确认送达
            </NButton>
          )}
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
    <OrderSearch v-model:model="searchParams" @search="handleSearch" @reset="resetSearchParams" />
    <NCard title="订单管理" :bordered="false" size="small" class="card-wrapper sm:flex-1-hidden">
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
        :scroll-x="1200"
        :loading="loading"
        remote
        :row-key="(row: Api.Order.OrderDetailItem) => row.orderId"
        class="sm:h-full"
      />
      <OrderOperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        @submitted="handleSubmitted"
      />
    </NCard>
  </div>
</template>

<style scoped></style> 