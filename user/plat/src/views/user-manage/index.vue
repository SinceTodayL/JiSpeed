<script setup lang="tsx">
import { NButton, NPopconfirm, NTag } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';

defineOptions({
  name: 'UserManage'
});

const tableData = [
  { id: 1, userName: 'User001', phone: '13800138001', registrationTime: '2023-01-01 10:00:00', status: '正常' },
  { id: 2, userName: 'User002', phone: '13800138002', registrationTime: '2023-01-02 11:00:00', status: '正常' },
  { id: 3, userName: 'User003', phone: '13800138003', registrationTime: '2023-01-03 12:00:00', status: '禁用' },
  { id: 4, userName: 'User004', phone: '13800138004', registrationTime: '2023-01-04 13:00:00', status: '正常' },
];

const columns: DataTableColumns = [
  {
    key: 'id',
    title: '用户ID',
    align: 'center'
  },
  {
    key: 'userName',
    title: '用户昵称',
    align: 'center'
  },
  {
    key: 'phone',
    title: '手机号',
    align: 'center'
  },
  {
    key: 'registrationTime',
    title: '注册时间',
    align: 'center'
  },
  {
    key: 'status',
    title: '状态',
    align: 'center',
    render: (row) => {
      const tagType = row.status === '正常' ? 'success' : 'error';
      return <NTag type={tagType}>{row.status}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small">编辑</NButton>
        <NPopconfirm onPositiveClick={() => console.log(`禁用用户: ${row.id}`)}>
          {{
            default: () => '确定要禁用该用户吗？',
            trigger: () => <NButton type="error" size="small">禁用</NButton>,
          }}
        </NPopconfirm>
      </div>
    )
  }
];
</script>

<template>
  <div class="m-16px">
    <n-card title="用户管理" :bordered="false" class="h-full">
      <n-data-table
        :columns="columns"
        :data="tableData"
        :pagination="{ pageSize: 10 }"
        flex-height
        class="h-full"
      />
    </n-card>
  </div>
</template>

<style scoped></style> 