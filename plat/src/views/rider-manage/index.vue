<script setup lang="tsx">
import { NButton, NPopconfirm, NTag } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';

defineOptions({
  name: 'RiderManage'
});

const tableData = [
  { id: 1, riderName: '骑手小明', phone: '13700137001', realName: '明石', idCard: '440***********001X', status: '接单中' },
  { id: 2, riderName: '骑手小红', phone: '13700137002', realName: '红花', idCard: '440***********002X', status: '休息中' },
  { id: 3, riderName: '骑手小强', phone: '13700137003', realName: '强子', idCard: '440***********003X', status: '接单中' },
  { id: 4, riderName: '骑手小刚', phone: '13700137004', realName: '刚子', idCard: '440***********004X', status: '已拉黑' },
];

const columns: DataTableColumns = [
  { key: 'id', title: '骑手ID', align: 'center' },
  { key: 'riderName', title: '骑手昵称', align: 'center' },
  { key: 'realName', title: '真实姓名', align: 'center' },
  { key: 'phone', title: '联系电话', align: 'center' },
  { key: 'idCard', title: '身份证号', align: 'center' },
  {
    key: 'status',
    title: '状态',
    align: 'center',
    render: (row) => {
      let tagType: 'success' | 'warning' | 'error' = 'success';
      if (row.status === '休息中') tagType = 'warning';
      if (row.status === '已拉黑') tagType = 'error';
      return <NTag type={tagType}>{row.status}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small">审核信息</NButton>
        <NPopconfirm onPositiveClick={() => console.log(`拉黑骑手: ${row.id}`)}>
          {{
            default: () => '确定要拉黑该骑手吗？',
            trigger: () => <NButton type="error" size="small">拉黑</NButton>,
          }}
        </NPopconfirm>
      </div>
    )
  }
];
</script>

<template>
  <div class="m-16px">
    <n-card title="骑手管理" :bordered="false" class="h-full">
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