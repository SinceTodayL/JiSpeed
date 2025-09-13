import type { RtlEnabledState, RtlItem } from '../config-provider/src/internal-interface';
import { type Ref } from 'vue';
export declare function useRtl(mountId: string, rtlStateRef: Ref<RtlEnabledState | undefined> | undefined, clsPrefixRef: Ref<string>): Ref<RtlItem | undefined> | undefined;
