<script setup lang="tsx">
import { ref, onMounted, computed } from 'vue';
import { 
  NButton, 
  NTag, 
  NModal, 
  NGrid, 
  NGi, 
  NCard, 
  NForm, 
  NFormItem, 
  NText, 
  NSpin, 
  NInput,
  NSelect,
  NSpace,
  NStatistic,
  NIcon,
  NAvatar,
  NBadge,
  NDivider,
  NDataTable,
  useMessage 
} from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { 
  PersonOutline, 
  SearchOutline, 
  RefreshOutline,
  PeopleOutline,
  MailOutline,
  CallOutline,
  TrophyOutline,
  StatsChartOutline,
  EyeOutline
} from '@vicons/ionicons5';
import { fetchUserList, fetchUserInfo, formatGender, formatUserLevel } from '@/api/user';

defineOptions({
  name: 'UserManage'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);

// 用户详情相关状态
const showDetailModal = ref(false);
const detailLoading = ref(false);
const userDetail = ref<any>({});

// 搜索和筛选
const searchForm = ref({
  nickname: '',
  gender: null,
  level: null,
  hasEmail: null,
  hasPhone: null
});

// 统计数据
const userStats = computed(() => {
  const total = tableData.value.length;
  const maleCount = tableData.value.filter(user => user.gender === 1).length;
  const femaleCount = tableData.value.filter(user => user.gender === 2).length;
  const withEmail = tableData.value.filter(user => user.email).length;
  const withPhone = tableData.value.filter(user => user.phoneNumber).length;
  
  return {
    total,
    maleCount,
    femaleCount,
    withEmail,
    withPhone,
    emailRate: total > 0 ? Math.round((withEmail / total) * 100) : 0,
    phoneRate: total > 0 ? Math.round((withPhone / total) * 100) : 0
  };
});

// 筛选选项
const genderOptions = [
  { label: '全部性别', value: null },
  { label: '男', value: 1 },
  { label: '女', value: 2 },
  { label: '未知', value: 0 }
];

const levelOptions = [
  { label: '全部等级', value: null },
  { label: '普通用户', value: 1 },
  { label: 'VIP用户', value: 2 },
  { label: '黄金用户', value: 3 }
];

const bindingOptions = [
  { label: '全部', value: null },
  { label: '已绑定', value: true },
  { label: '未绑定', value: false }
];

// 过滤后的表格数据
const filteredTableData = computed(() => {
  return tableData.value.filter(user => {
    if (searchForm.value.nickname && !user.nickname?.toLowerCase().includes(searchForm.value.nickname.toLowerCase())) {
      return false;
    }
    if (searchForm.value.gender !== null && user.gender !== searchForm.value.gender) {
      return false;
    }
    if (searchForm.value.level !== null && user.level !== searchForm.value.level) {
      return false;
    }
    if (searchForm.value.hasEmail !== null) {
      const hasEmail = !!user.email;
      if (hasEmail !== searchForm.value.hasEmail) return false;
    }
    if (searchForm.value.hasPhone !== null) {
      const hasPhone = !!user.phoneNumber;
      if (hasPhone !== searchForm.value.hasPhone) return false;
    }
    return true;
  });
});

// 搜索功能
function handleSearch() {
  // 搜索是通过 computed 属性实时进行的，这里可以添加额外逻辑
  message.success(`找到 ${filteredTableData.value.length} 个用户`);
}

// 重置搜索
function handleReset() {
  searchForm.value = {
    nickname: '',
    gender: null,
    level: null,
    hasEmail: null,
    hasPhone: null
  };
  message.info('搜索条件已重置');
}

// 刷新数据
function handleRefresh() {
  getUserList();
}

// 获取用户列表数据
async function getUserList() {
  try {
    loading.value = true;
    const response = await fetchUserList();
    
    if (response && Array.isArray(response.data)) {
      tableData.value = response.data.map((user, index) => ({
        ...user,
        index: index + 1,
        genderText: formatGender(user.gender),
        levelText: formatUserLevel(user.level),
        totalOrdersText: user.stats?.totalOrders || 0,
        favoriteCountText: user.stats?.favoriteCount || 0,
        hasEmail: user.email ? '是' : '否',
        hasPhone: user.phoneNumber ? '是' : '否'
      }));
      message.success(`成功加载 ${response.data.length} 个用户数据`);
    } else {
      message.error(response.message || '获取用户列表失败');
      tableData.value = [];
    }
  } catch (error) {
    message.error('获取用户列表失败: ' + error.message);
    tableData.value = [];
    console.error('Error fetching user list:', error);
  } finally {
    loading.value = false;
  }
}

const columns: DataTableColumns = [
  {
    key: 'index',
    title: '序号',
    align: 'center',
    width: 70,
    render: (row, index) => (
      <n-badge value={index + 1} type="info" />
    )
  },
  {
    key: 'userInfo',
    title: '用户信息',
    width: 200,
    render: (row) => (
      <div class="flex items-center gap-12px">
        <n-avatar
          size="medium"
          style={{
            backgroundColor: row.gender === 1 ? '#409eff' : row.gender === 2 ? '#f56c6c' : '#909399'
          }}
        >
          <n-icon>
            <PersonOutline />
          </n-icon>
        </n-avatar>
        <div class="flex flex-col">
          <n-text strong>{row.nickname || '未设置昵称'}</n-text>
          <n-text depth="3" style="font-size: 12px;">
            ID: {row.userId?.slice(-8) || '-'}
          </n-text>
        </div>
      </div>
    )
  },
  {
    key: 'genderText',
    title: '性别',
    align: 'center',
    width: 80,
    render: (row) => {
      const genderConfig = {
        1: { type: 'info', color: '#409eff' },
        2: { type: 'error', color: '#f56c6c' },
        0: { type: 'default', color: '#909399' }
      };
      const config = genderConfig[row.gender] || genderConfig[0];
      return <n-tag type={config.type} size="small">{row.genderText}</n-tag>;
    }
  },
  {
    key: 'levelText',
    title: '用户等级',
    align: 'center',
    width: 120,
    render: (row) => {
      const levelConfig = {
        1: { type: 'default', icon: null },
        2: { type: 'warning', icon: TrophyOutline },
        3: { type: 'success', icon: TrophyOutline }
      };
      const config = levelConfig[row.level] || levelConfig[1];
      return (
        <n-tag type={config.type} size="small">
          {config.icon && <n-icon component={config.icon} style="margin-right: 4px;" />}
          {row.levelText}
        </n-tag>
      );
    }
  },
  {
    key: 'stats',
    title: '用户统计',
    align: 'center',
    width: 120,
    render: (row) => (
      <div class="flex flex-col gap-4px">
        <div class="flex items-center justify-center gap-4px">
          <NIcon color="#409eff" size="14">
            <StatsChartOutline />
          </NIcon>
          <n-text style="font-size: 12px;">订单: {row.totalOrdersText}</n-text>
        </div>
        <div class="flex items-center justify-center gap-4px">
          <NIcon color="#f56c6c" size="14">
            <TrophyOutline />
          </NIcon>
          <n-text style="font-size: 12px;">收藏: {row.favoriteCountText}</n-text>
        </div>
      </div>
    )
  },
  {
    key: 'contact',
    title: '联系方式',
    align: 'center',
    width: 120,
    render: (row) => (
      <div class="flex flex-col gap-4px">
        <div class="flex items-center justify-center gap-4px">
          <n-icon color={row.email ? '#67c23a' : '#dcdfe6'} size="14">
            <MailOutline />
          </n-icon>
          <n-tag size="small" type={row.email ? 'success' : 'default'}>
            {row.hasEmail}
          </n-tag>
        </div>
        <div class="flex items-center justify-center gap-4px">
          <n-icon color={row.phoneNumber ? '#67c23a' : '#dcdfe6'} size="14">
            <CallOutline />
          </n-icon>
          <n-tag size="small" type={row.phoneNumber ? 'success' : 'default'}>
            {row.hasPhone}
          </n-tag>
        </div>
      </div>
    )
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    width: 120,
    render: (row) => (
      <n-button 
        type="primary" 
        size="small" 
        onClick={() => handleViewDetail(row.userId)}
        style="border-radius: 6px;"
      >
        <div style="display: flex; align-items: center; gap: 4px;">
          <n-icon size="14">
            <EyeOutline />
          </n-icon>
          查看详情
        </div>
      </n-button>
    )
  }
];

// 查看用户详情
async function handleViewDetail(userId: string) {
  try {
    detailLoading.value = true;
    showDetailModal.value = true;
    userDetail.value = {};
    
    console.log(`获取用户详情，ID: ${userId}`);
    
    const response = await fetchUserInfo(userId);
    
    if (response && response.data) {
      userDetail.value = {
        ...response.data,
        genderText: formatGender(response.data.gender),
        levelText: formatUserLevel(response.data.level),
        hasEmail: response.data.email ? '是' : '否',
        hasPhone: response.data.phoneNumber ? '是' : '否'
      };
      console.log('用户详情:', userDetail.value);
    } else {
      message.error(response?.message || '获取用户详情失败');
      showDetailModal.value = false;
    }
    
  } catch (error) {
    console.error('获取用户详情失败:', error);
    message.error('获取用户详情失败: ' + (error.message || '未知错误'));
    showDetailModal.value = false;
  } finally {
    detailLoading.value = false;
  }
}

// 组件挂载时获取数据
onMounted(() => {
  getUserList();
});
</script>

<template>
  <div class="p-6 min-h-full bg-gray-50">
    <!-- 页面标题 -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-2 flex items-center gap-2">
        <n-icon size="24" color="#409eff">
          <PeopleOutline />
        </n-icon>
        用户管理中心
      </h1>
      <p class="text-gray-600">管理和查看所有用户信息</p>
    </div>

    <!-- 统计卡片区域 -->
    <n-grid :cols="4" :x-gap="16" :y-gap="16" class="mb-6">
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="总用户数"
            :value="userStats.total"
            value-style="color: #409eff; font-weight: bold;"
          >
            <template #prefix>
              <n-icon size="20" color="#409eff">
                <PeopleOutline />
              </n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="男性用户"
            :value="userStats.maleCount"
            value-style="color: #409eff; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">人</span>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="女性用户"
            :value="userStats.femaleCount"
            value-style="color: #f56c6c; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">人</span>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card :bordered="false" class="shadow-sm hover:shadow-lg transition-shadow duration-300">
          <n-statistic
            label="邮箱绑定率"
            :value="userStats.emailRate"
            value-style="color: #67c23a; font-weight: bold;"
          >
            <template #suffix>
              <span class="text-sm text-gray-500">%</span>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- 搜索筛选区域 -->
    <n-card title="筛选条件" class="mb-6 shadow-sm" :bordered="false">
      <template #header-extra>
        <n-space>
          <n-button size="small" @click="handleSearch" type="primary">
            <template #icon>
              <n-icon>
                <SearchOutline />
              </n-icon>
            </template>
            搜索
          </n-button>
          <n-button size="small" @click="handleReset">
            <template #icon>
              <n-icon>
                <RefreshOutline />
              </n-icon>
            </template>
            重置
          </n-button>
        </n-space>
      </template>
      
      <n-form :model="searchForm" inline label-placement="left" label-width="80">
        <n-form-item label="用户昵称">
          <n-input
            v-model:value="searchForm.nickname"
            placeholder="请输入用户昵称"
            clearable
            style="width: 180px"
          />
        </n-form-item>
        
        <n-form-item label="性别">
          <n-select
            v-model:value="searchForm.gender"
            :options="genderOptions"
            placeholder="选择性别"
            clearable
            style="width: 120px"
          />
        </n-form-item>
        
        <n-form-item label="用户等级">
          <n-select
            v-model:value="searchForm.level"
            :options="levelOptions"
            placeholder="选择等级"
            clearable
            style="width: 140px"
          />
        </n-form-item>
        
        <n-form-item label="邮箱绑定">
          <n-select
            v-model:value="searchForm.hasEmail"
            :options="bindingOptions"
            placeholder="邮箱状态"
            clearable
            style="width: 120px"
          />
        </n-form-item>
        
        <n-form-item label="手机绑定">
          <n-select
            v-model:value="searchForm.hasPhone"
            :options="bindingOptions"
            placeholder="手机状态"
            clearable
            style="width: 120px"
          />
        </n-form-item>
      </n-form>
    </n-card>

    <!-- 用户列表 -->
    <n-card :bordered="false" class="shadow-sm">
      <template #header>
        <div class="flex items-center gap-2">
          <n-icon size="18" color="#409eff">
            <PeopleOutline />
          </n-icon>
          <span class="font-medium">用户列表</span>
        </div>
      </template>
      
      <template #header-extra>
        <n-space align="center">
          <n-text depth="3">
            显示 {{ filteredTableData.length }} / {{ tableData.length }} 条记录
          </n-text>
          <n-button size="small" @click="handleRefresh" :loading="loading">
            <template #icon>
              <n-icon>
                <RefreshOutline />
              </n-icon>
            </template>
            刷新
          </n-button>
        </n-space>
      </template>
      
      <n-data-table
        :columns="columns"
        :data="filteredTableData"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizePicker: true, pageSizes: [10, 20, 50] }"
        flex-height
        class="min-h-400px"
        :row-class-name="() => 'hover:bg-blue-50 transition-colors duration-200'"
      />
    </n-card>
    
    <!-- 用户详情弹窗 -->
    <n-modal 
      v-model:show="showDetailModal" 
      preset="card" 
      style="width: 900px; max-height: 80vh;" 
      class="rounded-2xl"
      :mask-closable="false"
    >
      <template #header>
        <div class="flex items-center gap-3">
          <n-avatar 
            :size="40"
            :style="{
              backgroundColor: userDetail.gender === 1 ? '#409eff' : userDetail.gender === 2 ? '#f56c6c' : '#909399'
            }"
          >
            <n-icon size="20">
              <PersonOutline />
            </n-icon>
          </n-avatar>
          <div>
            <h3 class="text-lg font-semibold text-gray-800">
              {{ userDetail.nickname || userDetail.nickName || '用户详情' }}
            </h3>
            <p class="text-sm text-gray-500">
              ID: {{ userDetail.userId || '-' }}
            </p>
          </div>
        </div>
      </template>

      <div v-if="!detailLoading" class="space-y-6">
        <!-- 用户概览 -->
        <div class="bg-gradient-to-r from-blue-50 to-indigo-50 p-4 rounded-lg">
          <n-grid :cols="4" :x-gap="20">
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-blue-600">{{ userDetail.stats?.totalOrders || 0 }}</div>
                <div class="text-sm text-gray-600">总订单数</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-purple-600">{{ userDetail.stats?.favoriteCount || 0 }}</div>
                <div class="text-sm text-gray-600">收藏商品</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-green-600">{{ userDetail.levelText || '-' }}</div>
                <div class="text-sm text-gray-600">用户等级</div>
              </div>
            </n-gi>
            <n-gi>
              <div class="text-center">
                <div class="text-2xl font-bold text-orange-600">
                  {{ userDetail.createdAt ? Math.floor((Date.now() - new Date(userDetail.createdAt).getTime()) / (1000 * 60 * 60 * 24)) : '-' }}
                </div>
                <div class="text-sm text-gray-600">注册天数</div>
              </div>
            </n-gi>
          </n-grid>
        </div>

        <n-divider />

        <!-- 详细信息 -->
        <n-grid :cols="2" :x-gap="24" :y-gap="20">
          <!-- 基本信息 -->
          <n-gi>
            <n-card title="基本信息" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#409eff">
                  <PersonOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">用户ID</span>
                  <n-text code style="word-break: break-all; max-width: 200px;">{{ userDetail.userId || '-' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">用户昵称</span>
                  <n-text strong>{{ userDetail.nickName || userDetail.nickname || '未设置' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">性别</span>
                  <n-tag 
                    :type="userDetail.gender === 1 ? 'info' : userDetail.gender === 2 ? 'error' : 'default'"
                    size="small"
                  >
                    {{ userDetail.genderText || '未知' }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">用户等级</span>
                  <n-tag type="warning" size="small">
                    <template #icon>
                      <n-icon v-if="userDetail.level >= 2">
                        <TrophyOutline />
                      </n-icon>
                    </template>
                    {{ userDetail.levelText || '普通用户' }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">生日</span>
                  <n-text>{{ userDetail.birthday || '未设置' }}</n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
          
          <!-- 联系信息 -->
          <n-gi>
            <n-card title="联系信息" size="small" class="h-full">
              <template #header-extra>
                <n-icon color="#67c23a">
                  <MailOutline />
                </n-icon>
              </template>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#409eff">
                      <MailOutline />
                    </n-icon>
                    邮箱地址
                  </span>
                  <n-text>{{ userDetail.email || '未绑定' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium flex items-center gap-2">
                    <n-icon size="16" color="#67c23a">
                      <CallOutline />
                    </n-icon>
                    手机号码
                  </span>
                  <n-text>{{ userDetail.phoneNumber || '未绑定' }}</n-text>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">邮箱验证</span>
                  <n-tag 
                    :type="userDetail.email ? 'success' : 'default'" 
                    size="small"
                  >
                    {{ userDetail.hasEmail }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2 border-b border-gray-100">
                  <span class="text-gray-600 font-medium">手机验证</span>
                  <n-tag 
                    :type="userDetail.phoneNumber ? 'success' : 'default'" 
                    size="small"
                  >
                    {{ userDetail.hasPhone }}
                  </n-tag>
                </div>
                
                <div class="flex justify-between items-center py-2">
                  <span class="text-gray-600 font-medium">注册时间</span>
                  <n-text depth="3">{{ userDetail.createdAt ? new Date(userDetail.createdAt).toLocaleString() : '未知' }}</n-text>
                </div>
              </div>
            </n-card>
          </n-gi>
        </n-grid>
      </div>
      
      <!-- 加载状态 -->
      <div v-else class="flex justify-center items-center h-80">
        <n-spin size="large">
          <template #description>
            <div class="text-center mt-4">
              <p class="text-gray-600">正在加载用户详情...</p>
              <p class="text-sm text-gray-400 mt-1">请稍候片刻</p>
            </div>
          </template>
        </n-spin>
      </div>
    </n-modal>
  </div>
</template>

<style scoped>
/* 页面整体样式 */
.user-manage-container {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

/* 统计卡片动画 */
.n-card {
  transition: all 0.3s ease;
}

.n-card:hover {
  transform: translateY(-2px);
}

/* 表格行悬停效果 */
:deep(.n-data-table-tr:hover) {
  background-color: rgba(24, 144, 255, 0.05) !important;
}

/* 搜索表单样式优化 */
:deep(.n-form-item-label__text) {
  font-weight: 500;
  color: #4a5568;
}

/* 用户头像渐变效果 */
.user-avatar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: 2px solid #fff;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

/* 状态标签样式 */
:deep(.n-tag) {
  border-radius: 12px;
  font-weight: 500;
}

/* 模态框圆角优化 */
:deep(.n-modal) {
  border-radius: 16px;
  overflow: hidden;
}

/* 加载动画优化 */
:deep(.n-spin) {
  color: #409eff;
}

/* 按钮圆角优化 */
:deep(.n-button) {
  border-radius: 8px;
  font-weight: 500;
}

/* 输入框圆角优化 */
:deep(.n-input) {
  border-radius: 8px;
}

:deep(.n-select) {
  border-radius: 8px;
}

/* 数据表格优化 */
:deep(.n-data-table) {
  border-radius: 12px;
  overflow: hidden;
}

:deep(.n-data-table-th) {
  background-color: #f8fafc;
  font-weight: 600;
  color: #4a5568;
}

/* 渐变背景 */
.gradient-bg {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

/* 卡片阴影效果 */
.card-shadow {
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.card-shadow:hover {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

/* 统计数字动画 */
.stat-number {
  transition: all 0.3s ease;
}

.stat-number:hover {
  transform: scale(1.05);
}

/* 用户详情弹窗优化 */
.user-detail-header {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  color: white;
  padding: 1rem;
  margin: -1rem -1rem 1rem -1rem;
  border-radius: 12px 12px 0 0;
}

/* 信息行样式 */
.info-row {
  transition: background-color 0.2s ease;
  padding: 0.75rem 0;
  border-radius: 6px;
}

.info-row:hover {
  background-color: rgba(24, 144, 255, 0.05);
  padding-left: 0.5rem;
  padding-right: 0.5rem;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .p-6 {
    padding: 1rem;
  }
  
  .n-grid[cols="4"] {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .n-modal {
    width: 95vw !important;
    max-width: none !important;
  }
}

/* 页面进入动画 */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

/* 数据加载骨架屏效果 */
.skeleton-loading {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: loading 1.5s infinite;
}

@keyframes loading {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

/* 成功/错误状态指示器 */
.status-indicator {
  position: relative;
}

.status-indicator::before {
  content: '';
  position: absolute;
  top: 50%;
  left: -8px;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  transform: translateY(-50%);
}

.status-success::before {
  background-color: #52c41a;
}

.status-error::before {
  background-color: #f5222d;
}

.status-warning::before {
  background-color: #faad14;
}
</style> 