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
  toLogin();
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
        console.log('开始注销流程...');
        
        // 重置认证状态（不触发内部跳转）
        await authStore.resetStoreWithoutRedirect();
        
        // 清除所有本地存储
        console.log('清除localStorage...');
        localStorage.clear();
        
        console.log('清除sessionStorage...');
        sessionStorage.clear();
        
        // 清除所有Cookie（包括不同路径的）
        console.log('清除Cookies...');
        const cookies = document.cookie.split(";");
        cookies.forEach((cookie) => {
          const eqPos = cookie.indexOf("=");
          const name = eqPos > -1 ? cookie.substr(0, eqPos).trim() : cookie.trim();
          
          // 清除不同路径的cookie
          const paths = ["/", "/plat", "/login"];
          paths.forEach(path => {
            document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=${path}`;
            document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=${path};domain=${window.location.hostname}`;
          });
        });
        
        // 清除IndexedDB数据（如果有的话）
        if ('indexedDB' in window && indexedDB.databases) {
          try {
            console.log('清除IndexedDB...');
            const databases = await indexedDB.databases?.() || [];
            await Promise.all(
              databases.map(db => {
                if (db.name) {
                  const deleteRequest = indexedDB.deleteDatabase(db.name);
                  return new Promise((resolve, reject) => {
                    deleteRequest.onsuccess = () => resolve(db.name);
                    deleteRequest.onerror = () => reject(deleteRequest.error);
                  });
                }
                return Promise.resolve();
              })
            );
            console.log('IndexedDB清除完成');
          } catch (error) {
            console.warn('清除IndexedDB时出错:', error);
          }
        }
        
        // 清除缓存（如果支持）
        if ('caches' in window) {
          try {
            console.log('清除缓存...');
            const cacheNames = await caches.keys();
            await Promise.all(
              cacheNames.map(name => caches.delete(name))
            );
            console.log('缓存清除完成');
          } catch (error) {
            console.warn('清除缓存时出错:', error);
          }
        }
        
        console.log('数据清除完成，准备跳转到外部登录页面...');
        
        // 显示注销成功提示
        window.$notification?.success({
          title: '注销成功',
          content: '即将跳转到登录页面...',
          duration: 2000
        });
        
        // 延迟跳转，让用户看到提示信息
        setTimeout(() => {
          console.log('跳转到外部登录页面: http://121.4.90.75/login/');
          // 跳转到外部登录页面
          window.location.href = 'http://121.4.90.75/login/';
        }, 1000);
        
      } catch (error) {
        console.error('注销过程中出错:', error);
        window.$notification?.error({
          title: '注销失败',
          content: '注销过程中出现错误，请手动刷新页面',
          duration: 4500
        });
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
