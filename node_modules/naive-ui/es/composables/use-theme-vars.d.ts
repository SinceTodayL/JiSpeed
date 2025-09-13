import type { ThemeCommonVars } from '../_styles/common';
import type { CustomThemeCommonVars } from '../config-provider';
import { type ComputedRef } from 'vue';
export declare function useThemeVars(): ComputedRef<ThemeCommonVars & CustomThemeCommonVars>;
