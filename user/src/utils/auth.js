import { reactive } from 'vue';

export const authState = reactive({
  isReady: false,
});

export function setAuthReady() {
  authState.isReady = true;
}

