<script setup lang="tsx">
import { ref, onMounted } from 'vue';
import { NButton, NPopconfirm, NTag, useMessage } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';
import { fetchUserList, formatGender, formatUserLevel } from '@/api/user';

defineOptions({
  name: 'UserManage'
});

const message = useMessage();
const loading = ref(false);
const tableData = ref<any[]>([]);

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
    width: 80
  },
  {
    key: 'userId',
    title: '用户ID',
    align: 'center',
    width: 120,
    ellipsis: {
      tooltip: true
    }
  },
  {
    key: 'nickname',
    title: '用户昵称',
    align: 'center',
    width: 120
  },
  {
    key: 'genderText',
    title: '性别',
    align: 'center',
    width: 80
  },
  {
    key: 'levelText',
    title: '用户等级',
    align: 'center',
    width: 100
  },
  {
    key: 'totalOrdersText',
    title: '总订单数',
    align: 'center',
    width: 100
  },
  {
    key: 'favoriteCountText',
    title: '收藏数',
    align: 'center',
    width: 100
  },
  {
    key: 'hasEmail',
    title: '绑定邮箱',
    align: 'center',
    width: 100,
    render: (row) => {
      const tagType = row.hasEmail === '是' ? 'success' : 'default';
      return <NTag type={tagType}>{row.hasEmail}</NTag>;
    }
  },
  {
    key: 'hasPhone',
    title: '绑定手机',
    align: 'center',
    width: 100,
    render: (row) => {
      const tagType = row.hasPhone === '是' ? 'success' : 'default';
      return <NTag type={tagType}>{row.hasPhone}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small">查看详情</NButton>
        <NPopconfirm onPositiveClick={() => handleUserAction(row.userId, 'disable')}>
          {{
            default: () => '确定要禁用该用户吗？',
            trigger: () => <NButton type="error" size="small">禁用</NButton>,
          }}
        </NPopconfirm>
      </div>
    )
  }
];

// 处理用户操作
function handleUserAction(userId: string, action: string) {
  message.info(`${action} 用户: ${userId}`);
  // 这里可以调用相应的 API
}

// 组件挂载时获取数据
onMounted(() => {
  getUserList();
});
</script>

<template>
  <div class="m-16px">
    <n-card title="用户管理" :bordered="false" class="h-full">
      <n-data-table
        :columns="columns"
        :data="tableData"
        :loading="loading"
        :pagination="{ pageSize: 10 }"
        flex-height
        class="h-full"
      />
    </n-card>
  </div>
</template>

<style scoped></style> 