<template>
  <div v-if="visible" class="address-dialog-mask">
    <div class="address-dialog">
      <div class="dialog-header">
        <span>选择配送地址</span>
        <button class="close-btn" @click="$emit('close')">×</button>
      </div>
      <div class="dialog-body">
        <div v-if="loading" class="loading">正在加载地址...</div>
        <div v-else>
          <div v-if="addresses.length === 0" class="empty">暂无地址，请先添加</div>
          <ul v-else class="address-list">
            <li v-for="address in addresses" :key="address.addressId" :class="['address-item', {selected: address.addressId === selectedId}]" @click="select(address)">
              <div class="address-main">
                <span class="receiver">{{ address.receiverName }} {{ address.receiverPhone }}</span>
                <span class="detail">{{ address.detailedAddress }}</span>
              </div>
              <span v-if="address.addressId === selectedId" class="selected-icon">✔</span>
            </li>
          </ul>
        </div>
      </div>
      <div class="dialog-footer">
        <button class="add-btn" @click="goAdd">添加新地址</button>
        <button class="manage-btn" @click="goManage">管理地址</button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { addressAPI } from '@/api/user.js'

export default {
  name: 'AddressSelectDialog',
  props: {
    visible: Boolean,
    userId: {
      type: [String, Number],
      required: true
    },
    selectedId: {
      type: [String, Number],
      default: null
    }
  },
  emits: ['close', 'select'],
  setup(props, { emit }) {
    const router = useRouter()
    const addresses = ref([])
    const loading = ref(false)

    const fetchAddresses = async () => {
      loading.value = true
      try {
        console.log('AddressSelectDialog props.userId:', props.userId)
        const res = await addressAPI.getUserAddresses(props.userId)
        console.log('getUserAddresses 返回:', res)
        if (res && res.code === 0) {
          addresses.value = res.data || []
        } else {
          addresses.value = []
        }
      } catch (e) {
        addresses.value = []
      } finally {
        loading.value = false
      }
    }

    const select = (address) => {
      emit('select', address)
      emit('close')
    }

    const goAdd = () => {
      emit('close')
      router.push('/addresses?mode=add')
    }
    const goManage = () => {
      emit('close')
      router.push('/addresses')
    }

    watch(() => props.userId, (val) => {
      if (props.visible && val) fetchAddresses()
    })
    watch(() => props.visible, (val) => {
      if (val && props.userId) fetchAddresses()
    })
    onMounted(() => {
      if (props.visible && props.userId) fetchAddresses()
    })

    return { addresses, loading, select, goAdd, goManage }
  }
}
</script>

<style scoped>
.address-dialog-mask {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.3);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}
.address-dialog {
  background: #fff;
  border-radius: 12px;
  width: 90vw;
  max-width: 400px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.18);
  overflow: hidden;
}
.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  font-size: 18px;
  font-weight: 600;
  border-bottom: 1px solid #eee;
}
.close-btn {
  background: none;
  border: none;
  font-size: 22px;
  cursor: pointer;
}
.dialog-body {
  padding: 16px;
  min-height: 120px;
}
.loading {
  text-align: center;
  color: #888;
}
.empty {
  text-align: center;
  color: #aaa;
}
.address-list {
  list-style: none;
  padding: 0;
  margin: 0;
}
.address-item {
  padding: 12px;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.address-item.selected {
  background: #e6f7ff;
}
.selected-icon {
  color: #007BFF;
  font-size: 18px;
}
.address-main {
  display: flex;
  flex-direction: column;
}
.receiver {
  font-weight: 600;
  color: #333;
}
.detail {
  color: #666;
  font-size: 13px;
}
.dialog-footer {
  display: flex;
  justify-content: space-between;
  padding: 12px 16px;
  border-top: 1px solid #eee;
}
.add-btn, .manage-btn {
  background: #007BFF;
  color: #fff;
  border: none;
  border-radius: 20px;
  padding: 8px 18px;
  font-size: 14px;
  cursor: pointer;
}
.add-btn {
  background: #00D4FF;
  margin-right: 8px;
}
</style>
