import type { ExtractPublicPropTypes } from '../../_utils';
import type { ObjectFit, Size } from './interface';
import { type ImgHTMLAttributes, type PropType, type SlotsType, type VNode, type VNodeChild } from 'vue';
import { type IntersectionObserverOptions } from '../../image/src/utils';
export declare const avatarProps: {
    readonly size: PropType<Size>;
    readonly src: StringConstructor;
    readonly circle: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly objectFit: PropType<ObjectFit>;
    readonly round: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly bordered: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly onError: PropType<(e: Event) => void>;
    readonly fallbackSrc: StringConstructor;
    readonly intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    readonly lazy: BooleanConstructor;
    readonly onLoad: PropType<(e: Event) => void>;
    readonly renderPlaceholder: PropType<() => VNodeChild>;
    readonly renderFallback: PropType<() => VNodeChild>;
    readonly imgProps: PropType<ImgHTMLAttributes>;
    /** @deprecated */
    readonly color: StringConstructor;
    readonly theme: PropType<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>;
    readonly themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
    readonly builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
};
export type AvatarProps = ExtractPublicPropTypes<typeof avatarProps>;
export interface AvatarSlots {
    default?: () => VNode[];
    placeholder?: () => VNode[];
    fallback?: () => VNode[];
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    readonly size: PropType<Size>;
    readonly src: StringConstructor;
    readonly circle: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly objectFit: PropType<ObjectFit>;
    readonly round: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly bordered: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly onError: PropType<(e: Event) => void>;
    readonly fallbackSrc: StringConstructor;
    readonly intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    readonly lazy: BooleanConstructor;
    readonly onLoad: PropType<(e: Event) => void>;
    readonly renderPlaceholder: PropType<() => VNodeChild>;
    readonly renderFallback: PropType<() => VNodeChild>;
    readonly imgProps: PropType<ImgHTMLAttributes>;
    /** @deprecated */
    readonly color: StringConstructor;
    readonly theme: PropType<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>;
    readonly themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
    readonly builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
}>, {
    textRef: import("vue").Ref<HTMLElement | null, HTMLElement | null>;
    selfRef: import("vue").Ref<HTMLElement | null, HTMLElement | null>;
    mergedRoundRef: import("vue").ComputedRef<boolean | undefined>;
    mergedClsPrefix: import("vue").Ref<string, string>;
    fitTextTransform: () => void;
    cssVars: import("vue").ComputedRef<{
        '--n-font-size': string;
        '--n-border': string;
        '--n-border-radius': string;
        '--n-color': string;
        '--n-color-modal': string;
        '--n-color-popover': string;
        '--n-bezier': string;
        '--n-merged-size': string;
    }> | undefined;
    themeClass: import("vue").Ref<string, string> | undefined;
    onRender: (() => void) | undefined;
    hasLoadError: import("vue").Ref<boolean, boolean>;
    shouldStartLoading: import("vue").Ref<boolean, boolean>;
    loaded: import("vue").Ref<boolean, boolean>;
    mergedOnError: (e: Event) => void;
    mergedOnLoad: (e: Event) => void;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    readonly size: PropType<Size>;
    readonly src: StringConstructor;
    readonly circle: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly objectFit: PropType<ObjectFit>;
    readonly round: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly bordered: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    readonly onError: PropType<(e: Event) => void>;
    readonly fallbackSrc: StringConstructor;
    readonly intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    readonly lazy: BooleanConstructor;
    readonly onLoad: PropType<(e: Event) => void>;
    readonly renderPlaceholder: PropType<() => VNodeChild>;
    readonly renderFallback: PropType<() => VNodeChild>;
    readonly imgProps: PropType<ImgHTMLAttributes>;
    /** @deprecated */
    readonly color: StringConstructor;
    readonly theme: PropType<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>;
    readonly themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
    readonly builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Avatar", {
        borderRadius: string;
        fontSize: string;
        border: string;
        heightTiny: string;
        heightSmall: string;
        heightMedium: string;
        heightLarge: string;
        heightHuge: string;
        color: string;
        colorModal: string;
        colorPopover: string;
    }, any>>>;
}>> & Readonly<{}>, {
    readonly round: boolean;
    readonly circle: boolean;
    readonly lazy: boolean;
    readonly bordered: boolean;
}, SlotsType<AvatarSlots>, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
