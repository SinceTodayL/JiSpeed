<script setup lang="tsx">
import { NButton, NPopconfirm, NTag } from 'naive-ui';
import type { DataTableColumns } from 'naive-ui';

defineOptions({
  name: 'MerchantManage'
});

const tableData = [
  { id: 1, shopName: '美味汉堡店', owner: '张三', phone: '13900139001', address: '商业街1号', status: '营业中' },
  { id: 2, shopName: '开心奶茶铺', owner: '李四', phone: '13900139002', address: '大学路2号', status: '休息中' },
  { id: 3, shopName: '正宗兰州拉面', owner: '王五', phone: '13900139003', address: '美食城3号', status: '营业中' },
  { id: 4, shopName: '深夜烧烤摊', owner: '赵六', phone: '13900139004', address: '夜市4号', status: '已封禁' },
];

const columns: DataTableColumns = [
  { key: 'id', title: '商家ID', align: 'center' },
  { key: 'shopName', title: '店铺名称', align: 'center' },
  { key: 'owner', title: '负责人', align: 'center' },
  { key: 'phone', title: '联系电话', align: 'center' },
  { key: 'address', title: '店铺地址', align: 'center' },
  {
    key: 'status',
    title: '状态',
    align: 'center',
    render: (row) => {
      let tagType: 'success' | 'warning' | 'error' = 'success';
      if (row.status === '休息中') tagType = 'warning';
      if (row.status === '已封禁') tagType = 'error';
      return <NTag type={tagType}>{row.status}</NTag>;
    }
  },
  {
    key: 'actions',
    title: '操作',
    align: 'center',
    render: (row) => (
      <div class="flex-center gap-10px">
        <NButton type="primary" size="small">详情</NButton>
        <NPopconfirm onPositiveClick={() => console.log(`封禁商家: ${row.id}`)}>
          {{
            default: () => '确定要封禁该商家吗？',
            trigger: () => <NButton type="error" size="small">封禁</NButton>,
          }}
        </NPopconfirm>
      </div>
    )
  }
];
</script>

<template>
  <div class="m-16px">
    <n-card title="商家管理" :bordered="false" class="h-full">
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