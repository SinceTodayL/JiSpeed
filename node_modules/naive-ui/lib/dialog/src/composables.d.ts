import type { DialogApiInjection, DialogReactive } from './DialogProvider';
import { type Ref } from 'vue';
export declare function useDialog(): DialogApiInjection;
export declare function useDialogReactiveList(): Ref<readonly DialogReactive[]>;
