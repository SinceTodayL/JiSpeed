<script setup lang="ts">
import { computed } from 'vue';
import type { VNode } from 'vue';
import { useAuthStore } from '@/store/modules/auth';
import { useRouterPush } from '@/hooks/common/router';
import { useSvgIcon } from '@/hooks/common/icon';
import { $t } from '@/locales';

defineOptions({
  name: 'UserAvatar'
});

const authStore = useAuthStore();
const { routerPushByKey, toLogin } = useRouterPush();
const { SvgIconVNode } = useSvgIcon();

function loginOrRegister() {
  // 移除登录跳转，不执行任何操作
  console.log('登录按钮被点击，但登录功能已禁用');
}

type DropdownKey = 'logout';

type DropdownOption =
  | {
      key: DropdownKey;
      label: string;
      icon?: () => VNode;
    }
  | {
      type: 'divider';
      key: string;
    };

const options = computed(() => {
  const opts: DropdownOption[] = [
    {
      label: $t('common.logout'),
      key: 'logout',
      icon: SvgIconVNode({ icon: 'ph:sign-out', fontSize: 18 })
    }
  ];

  return opts;
});

function logout() {
  window.$dialog?.info({
    title: $t('common.tip'),
    content: $t('common.logoutConfirm'),
    positiveText: $t('common.confirm'),
    negativeText: $t('common.cancel'),
    onPositiveClick: async () => {
      try {
        // 重置认证状态
        await authStore.resetStore();
        
        // 清除所有本地存储
        localStorage.clear();
        sessionStorage.clear();
        
        // 清除所有Cookie（包括不同路径的）
        const cookies = document.cookie.split(";");
        cookies.forEach((cookie) => {
          const eqPos = cookie.indexOf("=");
          const name = eqPos > -1 ? cookie.substr(0, eqPos).trim() : cookie.trim();
          
          // 清除不同路径的cookie
          const paths = ["/", "/merchant", "/login"];
          paths.forEach(path => {
            document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=${path}`;
            document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=${path};domain=${window.location.hostname}`;
          });
        });
        
        // 清除IndexedDB数据（如果有的话）
        if ('indexedDB' in window) {
          try {
            const databases = await indexedDB.databases?.() || [];
            await Promise.all(
              databases.map(db => {
                if (db.name) {
                  return new Promise((resolve) => {
                    const deleteReq = indexedDB.deleteDatabase(db.name);
                    deleteReq.onsuccess = () => resolve(true);
                    deleteReq.onerror = () => resolve(false);
                  });
                }
                return Promise.resolve();
              })
            );
          } catch (error) {
            console.warn('清除IndexedDB失败:', error);
          }
        }
        
        
        
        // 显示注销成功消息
        window.$message?.success('注销成功，正在跳转...');
        
        // 短暂延迟后跳转，让用户看到成功消息
        setTimeout(() => {
          toLogin();
        }, 1000);
        
      } catch (error) {
        console.error('注销过程中发生错误:', error);
        window.$message?.error('注销失败，请重试');
      }
    }
  });
}

function handleDropdown(key: DropdownKey) {
  if (key === 'logout') {
    logout();
  } else {
    // If your other options are jumps from other routes, they will be directly supported here
    routerPushByKey(key);
  }
}
</script>

<template>
  <NButton v-if="!authStore.isLogin" quaternary @click="loginOrRegister">
    {{ $t('page.login.common.loginOrRegister') }}
  </NButton>
  <NDropdown v-else placement="bottom" trigger="click" :options="options" @select="handleDropdown">
    <div>
      <ButtonIcon>
        <SvgIcon icon="ph:user-circle" class="text-icon-large" />
        <span class="text-16px font-medium">{{ authStore.userInfo.userName }}</span>
      </ButtonIcon>
    </div>
  </NDropdown>
</template>

<style scoped></style>
