<script setup lang="ts">
import { ref, watch } from 'vue';
import { NEmpty, NInput, NSpin } from 'naive-ui';
import { Icon } from '@iconify/vue';
import { type InputTipsResult, getInputTips } from '@/service/api/amap';

interface Props {
  placeholder?: string;
  city?: string;
  location?: string;
  modelValue?: string;
}

interface Emits {
  (e: 'update:modelValue', value: string): void;
  (e: 'select', suggestion: InputTipsResult): void;
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请输入地址',
  city: '',
  location: '',
  modelValue: ''
});

const emit = defineEmits<Emits>();

// 响应式数据
const searchKeyword = ref(props.modelValue);
const suggestions = ref<InputTipsResult[]>([]);
const showSuggestions = ref(false);
const selectedIndex = ref(-1);
const loading = ref(false);
const searchTimer = ref<NodeJS.Timeout | null>(null);

// 处理搜索
const performSearch = async () => {
  const keyword = searchKeyword.value.trim();
  if (!keyword) {
    suggestions.value = [];
    showSuggestions.value = false;
    return;
  }

  try {
    loading.value = true;
    const { data } = await getInputTips({
      keywords: keyword,
      city: props.city,
      location: props.location,
      citylimit: Boolean(props.city)
    });

    if (data && data.tips) {
      suggestions.value = data.tips;
      showSuggestions.value = true;
      selectedIndex.value = -1;
    } else {
      suggestions.value = [];
      showSuggestions.value = true;
    }
  } catch {
    // 地址搜索失败
    suggestions.value = [];
    showSuggestions.value = true;
  } finally {
    loading.value = false;
  }
};

// 监听输入变化
watch(searchKeyword, newValue => {
  emit('update:modelValue', newValue);

  if (searchTimer.value) {
    clearTimeout(searchTimer.value);
  }

  if (newValue.trim()) {
    searchTimer.value = setTimeout(() => {
      performSearch();
    }, 300); // 300ms 防抖
  } else {
    suggestions.value = [];
    showSuggestions.value = false;
  }
});

// 监听外部值变化
watch(
  () => props.modelValue,
  newValue => {
    searchKeyword.value = newValue;
  }
);

// 处理输入框获得焦点
const handleFocus = () => {
  if (suggestions.value.length > 0) {
    showSuggestions.value = true;
  }
};

// 处理输入框失去焦点
const handleBlur = () => {
  // 延迟隐藏，让点击事件能够触发
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
};

// 选择建议项
const selectSuggestion = (suggestion: InputTipsResult) => {
  searchKeyword.value = suggestion.name;
  suggestions.value = [];
  showSuggestions.value = false;
  selectedIndex.value = -1;
  emit('select', suggestion);
};

// 键盘导航
const handleKeydown = (event: KeyboardEvent) => {
  if (!showSuggestions.value || suggestions.value.length === 0) {
    return;
  }

  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault();
      selectedIndex.value = Math.min(selectedIndex.value + 1, suggestions.value.length - 1);
      break;
    case 'ArrowUp':
      event.preventDefault();
      selectedIndex.value = Math.max(selectedIndex.value - 1, -1);
      break;
    case 'Enter':
      event.preventDefault();
      if (selectedIndex.value >= 0 && selectedIndex.value < suggestions.value.length) {
        selectSuggestion(suggestions.value[selectedIndex.value]);
      }
      break;
    case 'Escape':
      showSuggestions.value = false;
      selectedIndex.value = -1;
      break;
    default:
      break;
  }
};

// 暴露方法
defineExpose({
  focus: () => {
    // 可以添加聚焦方法
  },
  clear: () => {
    searchKeyword.value = '';
    suggestions.value = [];
    showSuggestions.value = false;
  }
});
</script>

<template>
  <div class="address-search">
    <div class="search-input-container">
      <NInput
        v-model:value="searchKeyword"
        :placeholder="placeholder"
        size="large"
        clearable
        @input="performSearch"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
      >
        <template #prefix>
          <Icon icon="ion:search" />
        </template>
      </NInput>

      <!-- 搜索结果下拉列表 -->
      <div v-if="showSuggestions && suggestions.length > 0" class="suggestions-dropdown">
        <div
          v-for="(suggestion, index) in suggestions"
          :key="suggestion.id"
          class="suggestion-item"
          :class="{ 'is-active': selectedIndex === index }"
          @click="selectSuggestion(suggestion)"
          @mouseenter="selectedIndex = index"
        >
          <div class="suggestion-main">
            <div class="suggestion-name">{{ suggestion.name }}</div>
            <div class="suggestion-address">{{ suggestion.address }}</div>
          </div>
          <div class="suggestion-district">{{ suggestion.district }}</div>
        </div>
      </div>

      <!-- 无结果提示 -->
      <div v-if="showSuggestions && suggestions.length === 0 && searchKeyword && !loading" class="no-results">
        <NEmpty description="未找到相关地址" />
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <NSpin size="small" />
        <span class="ml-8px">搜索中...</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.address-search {
  position: relative;
  width: 100%;
}

.search-input-container {
  position: relative;
  width: 100%;
}

.suggestions-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #e0e0e6;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  max-height: 300px;
  overflow-y: auto;
  margin-top: 4px;
}

.suggestion-item {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  cursor: pointer;
  border-bottom: 1px solid #f0f0f0;
  transition: background-color 0.2s;
}

.suggestion-item:last-child {
  border-bottom: none;
}

.suggestion-item:hover,
.suggestion-item.is-active {
  background-color: #f5f5f5;
}

.suggestion-main {
  flex: 1;
  min-width: 0;
}

.suggestion-name {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  margin-bottom: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.suggestion-address {
  font-size: 12px;
  color: #666;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.suggestion-district {
  font-size: 12px;
  color: #999;
  margin-left: 8px;
  flex-shrink: 0;
}

.no-results {
  padding: 20px;
  text-align: center;
}

.loading-container {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #e0e0e6;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  padding: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 4px;
}
</style>
