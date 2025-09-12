<script setup lang="tsx">
import { NButton, NTag } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';

defineOptions({
  name: 'ComplaintHandle'
});

const tableData = [
  { id: 1, type: '用户投诉商家', content: '点的外卖里有头发', complainant: 'User001', defendant: '美味汉堡店', time: '2023-02-01 12:30:00', status: '待处理' },
  { id: 2, type: '商家投诉用户', content: '用户恶意差评', complainant: '开心奶茶铺', defendant: 'User002', time: '2023-02-02 14:00:00', status: '已处理' },
  { id: 3, type: '用户投诉骑手', content: '骑手提前点击送达', complainant: 'User003', defendant: '骑手小强', time: '2023-02-03 18:00:00', status: '处理中' },
  { id: 4, type: '骑手投诉商家', content: '商家出餐太慢', complainant: '骑手小明', defendant: '正宗兰州拉面', time: '2023-02-04 20:00:00', status: '待处理' },
];

const columns: DataTableColumns = [
  { key: 'id', title: '工单ID', align: 'center' },
  { key: 'type', title: '投诉类型', align: 'center' },
  { key: 'content', title: '投诉内容', align: 'center', width: 200 },
  { key: 'complainant', title: '投诉方', align: 'center' },
  { key: 'defendant', title: '被投诉方', align: 'center' },
  { key: 'time', title: '投诉时间', align: 'center' },
  {
    key: 'status',
    title: '状态',
    align: 'center',
    render: (row) => {
      let tagType: 'warning' | 'success' | 'info' = 'warning';
      if (row.status === '已处理') tagType = 'success';
      if (row.status === '处理中') tagType = 'info';
      return <NTag type={tagType}>{row.status}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: () => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small">处理</NButton>
      </div>
    )
  }
];
</script>

<template>
  <div class="m-16px">
    <n-card title="投诉处理" :bordered="false" class="h-full">
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