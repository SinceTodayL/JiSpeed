<script setup lang="ts">
import { computed, ref } from 'vue';
import type { Component } from 'vue';
// 不再需要颜色混合函数，因为使用纯白背景
// import { getPaletteColorByNumber, mixColor } from '@sa/color';
import { loginModuleRecord } from '@/constants/app';
import { useAppStore } from '@/store/modules/app';
import { useThemeStore } from '@/store/modules/theme';
import { $t } from '@/locales';
import { ROLE_CONFIG } from '@/constants/login-defaults';
import PwdLogin from './modules/pwd-login.vue';
import CodeLogin from './modules/code-login.vue';
import Register from './modules/register.vue';
import ResetPwd from './modules/reset-pwd.vue';
import BindWechat from './modules/bind-wechat.vue';

interface Props {
  /** The login module */
  module?: UnionKey.LoginModule;
}

const props = defineProps<Props>();

const appStore = useAppStore();
const themeStore = useThemeStore();

// 角色选择状态
const selectedRole = ref<'user' | 'rider' | 'merchant' | 'admin'>('user');

// 轮播图数据
const carouselImages = [
  'http://121.4.90.75/assets/beijingtu1.jpg',
  'http://121.4.90.75/assets/beijingtu2.jpg',
  'http://121.4.90.75/assets/beijingtu3.jpg',
  'http://121.4.90.75/assets/beijingtu4.jpg',
  'http://121.4.90.75/assets/beijingtu5.jpg'
];

// 图片加载错误处理
function handleImageError(event: Event) {
  const img = event.target as HTMLImageElement;
  console.warn('图片加载失败:', img.src);
  // 可以设置一个默认图片
  img.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTAwIiBoZWlnaHQ9IjUwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjVmNWY1Ii8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIxOCIgZmlsbD0iIzk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPuWbvueJh+WKoOi9veWksei0pTwvdGV4dD48L3N2Zz4=';
}

interface LoginModule {
  label: string;
  component: Component;
}

const moduleMap: Record<UnionKey.LoginModule, LoginModule> = {
  'pwd-login': { label: loginModuleRecord['pwd-login'], component: PwdLogin },
  'code-login': { label: loginModuleRecord['code-login'], component: CodeLogin },
  register: { label: loginModuleRecord.register, component: Register },
  'reset-pwd': { label: loginModuleRecord['reset-pwd'], component: ResetPwd },
  'bind-wechat': { label: loginModuleRecord['bind-wechat'], component: BindWechat }
};

// 角色选项配置
const roleOptions = computed(() => [
  { key: 'user', label: $t('page.login.pwdLogin.user'), icon: 'mdi:account' },
  { key: 'rider', label: $t('page.login.pwdLogin.rider'), icon: 'mdi:motorbike' },
  { key: 'merchant', label: $t('page.login.pwdLogin.merchant'), icon: 'mdi:store' },
  { key: 'admin', label: $t('page.login.pwdLogin.admin'), icon: 'mdi:shield-account' }
]);

const activeModule = computed(() => moduleMap[props.module || 'pwd-login']);

// 不再需要主题背景色，因为已移除 WaveBg 组件
// const bgThemeColor = computed(() =>
//   themeStore.darkMode ? getPaletteColorByNumber(themeStore.themeColor, 600) : themeStore.themeColor
// );

const bgColor = computed(() => {
  // 使用纯白色背景，去掉橙色底色
  return '#ffffff';
});
</script>

<template>
  <div class="relative size-full flex-center overflow-hidden" :style="{ backgroundColor: bgColor }">
    <!-- 移除 WaveBg 组件以去掉橙色背景 -->
    <!-- <WaveBg :theme-color="bgThemeColor" /> -->
    
    <!-- 主容器：轮播图 + 登录框 -->
    <div class="relative z-4 flex rd-12px overflow-hidden shadow-2xl min-h-500px">
      <!-- 左侧轮播图 -->
      <div class="w-500px" style="min-height: 500px;">
        <NCarousel 
          autoplay 
          :interval="4000"
          :show-dots="true"
          :show-arrow="false"
          class="h-full rd-l-12px overflow-hidden"
        >
          <img
            v-for="(image, index) in carouselImages"
            :key="index"
            :src="image"
            class="w-full h-full object-cover"
            :alt="`Background ${index + 1}`"
            @error="handleImageError"
          />
        </NCarousel>
      </div>
      
      <!-- 右侧登录框 -->
      <NCard :bordered="false" style="width: 490px;" class="min-h-full">
        <div class="w-450px lt-sm:w-300px">
          <header class="flex-y-center justify-between">
            <SystemLogo class="text-64px text-primary lt-sm:text-48px" />
            <h3 class="text-28px text-primary font-600 lt-sm:text-22px">{{ $t('system.loginTitle') }}</h3>
            <div class="i-flex-col">
              <ThemeSchemaSwitch
                :theme-schema="themeStore.themeScheme"
                :show-tooltip="false"
                class="text-20px lt-sm:text-18px"
                @switch="themeStore.toggleThemeScheme"
              />
              <LangSwitch
                v-if="themeStore.header.multilingual.visible"
                :lang="appStore.locale"
                :lang-options="appStore.localeOptions"
                :show-tooltip="false"
                @change-lang="appStore.changeLocale"
              />
            </div>
          </header>
          
          <!-- 角色选择区域 -->
          <div class="pt-24px">
            <h4 class="text-16px text-primary font-medium mb-16px">{{ $t('page.login.pwdLogin.loginRole') }}</h4>
            <div class="grid grid-cols-2 gap-12px mb-24px">
              <NButton
                v-for="role in roleOptions"
                :key="role.key"
                :type="selectedRole === role.key ? 'primary' : 'default'"
                size="large"
                class="h-60px"
                @click="selectedRole = role.key as 'user' | 'rider' | 'merchant' | 'admin'"
              >
                <div class="flex-col items-center">
                  <SvgIcon :icon="role.icon" class="text-20px mb-4px" />
                  <span class="text-14px">{{ role.label }}</span>
                </div>
              </NButton>
            </div>
          </div>
          
          <main class="pt-12px">
            <h3 class="text-18px text-primary font-medium">{{ $t(activeModule.label) }}</h3>
            <div class="pt-24px">
              <Transition :name="themeStore.page.animateMode" mode="out-in" appear>
                <component :is="activeModule.component" :selected-role="selectedRole" />
              </Transition>
            </div>
          </main>
        </div>
      </NCard>
    </div>
  </div>
</template>

<style scoped></style>
