import type { ExtractPublicPropTypes } from '../../_utils';
import type { PopoverInst, PopoverTrigger } from '../../popover';
import { type ExtractPropTypes, type PropType, type SlotsType, type VNode } from 'vue';
export declare const popselectProps: {
    multiple: BooleanConstructor;
    value: {
        readonly type: PropType<import("../../select/src/interface").Value | null>;
        readonly default: null;
    };
    cancelable: BooleanConstructor;
    options: {
        readonly type: PropType<import("../../select/src/interface").SelectMixedOption[]>;
        readonly default: () => never[];
    };
    size: {
        readonly type: PropType<import("./interface").PopselectSize>;
        readonly default: "medium";
    };
    scrollable: BooleanConstructor;
    'onUpdate:value': PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onUpdateValue: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onMouseenter: PropType<(e: MouseEvent) => void>;
    onMouseleave: PropType<(e: MouseEvent) => void>;
    renderLabel: PropType<import("../../_internal/select-menu/src/interface").RenderLabel>;
    showCheckmark: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    nodeProps: PropType<import("../..").SelectNodeProps>;
    virtualScroll: BooleanConstructor;
    onChange: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue> | undefined>;
    placement: {
        default: string;
        type: PropType<import("vueuc/lib/binder/src/interface").Placement>;
    };
    trigger: {
        type: PropType<PopoverTrigger>;
        default: string;
    };
    to: {
        type: PropType<HTMLElement | string | boolean>;
        default: undefined;
    };
    disabled: BooleanConstructor;
    show: {
        type: PropType<boolean | undefined>;
        default: undefined;
    };
    flip: {
        type: BooleanConstructor;
        default: boolean;
    };
    x: NumberConstructor;
    y: NumberConstructor;
    maxWidth: NumberConstructor;
    minWidth: NumberConstructor;
    width: {
        type: PropType<number | "trigger">;
        default: undefined;
    };
    zIndex: NumberConstructor;
    duration: {
        type: NumberConstructor;
        default: number;
    };
    raw: BooleanConstructor;
    overlap: BooleanConstructor;
    contentClass: StringConstructor;
    contentStyle: PropType<import("vue").CSSProperties | string>;
    arrowClass: StringConstructor;
    arrowStyle: PropType<string | import("vue").CSSProperties>;
    arrowWrapperClass: StringConstructor;
    arrowWrapperStyle: PropType<string | import("vue").CSSProperties>;
    delay: {
        type: NumberConstructor;
        default: number;
    };
    arrowPointToCenter: BooleanConstructor;
    displayDirective: {
        type: PropType<"if" | "show">;
        default: string;
    };
    keepAliveOnHover: {
        type: BooleanConstructor;
        default: boolean;
    };
    headerClass: StringConstructor;
    headerStyle: PropType<import("vue").CSSProperties | string>;
    footerClass: StringConstructor;
    footerStyle: PropType<import("vue").CSSProperties | string>;
    internalDeactivateImmediately: BooleanConstructor;
    animated: {
        type: BooleanConstructor;
        default: boolean;
    };
    onClickoutside: PropType<(e: MouseEvent) => void>;
    internalTrapFocus: BooleanConstructor;
    defaultShow: BooleanConstructor;
    getDisabled: PropType<() => boolean>;
    'onUpdate:show': PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    onUpdateShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    internalSyncTargetWithParent: BooleanConstructor;
    internalInheritedEventHandlers: {
        type: PropType<import("../../popover/src/Popover").TriggerEventHandlers[]>;
        default: () => never[];
    };
    internalExtraClass: {
        type: PropType<string[]>;
        default: () => never[];
    };
    onShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    onHide: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    theme: PropType<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
};
export type PopselectSetupProps = ExtractPropTypes<typeof popselectProps>;
export type PopselectProps = ExtractPublicPropTypes<typeof popselectProps>;
export interface PopselectSlots {
    default?: () => VNode[];
    header?: () => VNode[];
    action?: () => VNode[];
    empty?: () => VNode[];
}
declare const _default: import("vue").DefineComponent<ExtractPropTypes<{
    multiple: BooleanConstructor;
    value: {
        readonly type: PropType<import("../../select/src/interface").Value | null>;
        readonly default: null;
    };
    cancelable: BooleanConstructor;
    options: {
        readonly type: PropType<import("../../select/src/interface").SelectMixedOption[]>;
        readonly default: () => never[];
    };
    size: {
        readonly type: PropType<import("./interface").PopselectSize>;
        readonly default: "medium";
    };
    scrollable: BooleanConstructor;
    'onUpdate:value': PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onUpdateValue: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onMouseenter: PropType<(e: MouseEvent) => void>;
    onMouseleave: PropType<(e: MouseEvent) => void>;
    renderLabel: PropType<import("../../_internal/select-menu/src/interface").RenderLabel>;
    showCheckmark: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    nodeProps: PropType<import("../..").SelectNodeProps>;
    virtualScroll: BooleanConstructor;
    onChange: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue> | undefined>;
    placement: {
        default: string;
        type: PropType<import("vueuc/lib/binder/src/interface").Placement>;
    };
    trigger: {
        type: PropType<PopoverTrigger>;
        default: string;
    };
    to: {
        type: PropType<HTMLElement | string | boolean>;
        default: undefined;
    };
    disabled: BooleanConstructor;
    show: {
        type: PropType<boolean | undefined>;
        default: undefined;
    };
    flip: {
        type: BooleanConstructor;
        default: boolean;
    };
    x: NumberConstructor;
    y: NumberConstructor;
    maxWidth: NumberConstructor;
    minWidth: NumberConstructor;
    width: {
        type: PropType<number | "trigger">;
        default: undefined;
    };
    zIndex: NumberConstructor;
    duration: {
        type: NumberConstructor;
        default: number;
    };
    raw: BooleanConstructor;
    overlap: BooleanConstructor;
    contentClass: StringConstructor;
    contentStyle: PropType<import("vue").CSSProperties | string>;
    arrowClass: StringConstructor;
    arrowStyle: PropType<string | import("vue").CSSProperties>;
    arrowWrapperClass: StringConstructor;
    arrowWrapperStyle: PropType<string | import("vue").CSSProperties>;
    delay: {
        type: NumberConstructor;
        default: number;
    };
    arrowPointToCenter: BooleanConstructor;
    displayDirective: {
        type: PropType<"if" | "show">;
        default: string;
    };
    keepAliveOnHover: {
        type: BooleanConstructor;
        default: boolean;
    };
    headerClass: StringConstructor;
    headerStyle: PropType<import("vue").CSSProperties | string>;
    footerClass: StringConstructor;
    footerStyle: PropType<import("vue").CSSProperties | string>;
    internalDeactivateImmediately: BooleanConstructor;
    animated: {
        type: BooleanConstructor;
        default: boolean;
    };
    onClickoutside: PropType<(e: MouseEvent) => void>;
    internalTrapFocus: BooleanConstructor;
    defaultShow: BooleanConstructor;
    getDisabled: PropType<() => boolean>;
    'onUpdate:show': PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    onUpdateShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    internalSyncTargetWithParent: BooleanConstructor;
    internalInheritedEventHandlers: {
        type: PropType<import("../../popover/src/Popover").TriggerEventHandlers[]>;
        default: () => never[];
    };
    internalExtraClass: {
        type: PropType<string[]>;
        default: () => never[];
    };
    onShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    onHide: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    theme: PropType<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
}>, {
    popoverInstRef: import("vue").Ref<{
        syncPosition: () => void;
        setShow: (value: boolean) => void;
    } | null, PopoverInst | {
        syncPosition: () => void;
        setShow: (value: boolean) => void;
    } | null>;
    mergedTheme: import("vue").ComputedRef<{
        common: import("../..").ThemeCommonVars;
        self: {
            menuBoxShadow: string;
        };
        peers: {
            Popover: import("../../_mixins").Theme<"Popover", {
                fontSize: string;
                borderRadius: string;
                color: string;
                dividerColor: string;
                textColor: string;
                boxShadow: string;
                space: string;
                spaceArrow: string;
                arrowOffset: string;
                arrowOffsetVertical: string;
                arrowHeight: string;
                padding: string;
            }, any>;
            InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
                optionFontSizeTiny: string;
                optionFontSizeSmall: string;
                optionFontSizeMedium: string;
                optionFontSizeLarge: string;
                optionFontSizeHuge: string;
                optionHeightTiny: string;
                optionHeightSmall: string;
                optionHeightMedium: string;
                optionHeightLarge: string;
                optionHeightHuge: string;
                borderRadius: string;
                color: string;
                groupHeaderTextColor: string;
                actionDividerColor: string;
                optionTextColor: string;
                optionTextColorPressed: string;
                optionTextColorDisabled: string;
                optionTextColorActive: string;
                optionOpacityDisabled: string;
                optionCheckColor: string;
                optionColorPending: string;
                optionColorActive: string;
                optionColorActivePending: string;
                actionTextColor: string;
                loadingColor: string;
                height: string;
                paddingTiny: string;
                paddingSmall: string;
                paddingMedium: string;
                paddingLarge: string;
                paddingHuge: string;
                optionPaddingTiny: string;
                optionPaddingSmall: string;
                optionPaddingMedium: string;
                optionPaddingLarge: string;
                optionPaddingHuge: string;
                loadingSize: string;
            }, {
                Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                    height: string;
                    width: string;
                    borderRadius: string;
                    color: string;
                    colorHover: string;
                    railInsetHorizontalBottom: string;
                    railInsetHorizontalTop: string;
                    railInsetVerticalRight: string;
                    railInsetVerticalLeft: string;
                    railColor: string;
                }, any>;
                Empty: import("../../_mixins").Theme<"Empty", {
                    fontSizeTiny: string;
                    fontSizeSmall: string;
                    fontSizeMedium: string;
                    fontSizeLarge: string;
                    fontSizeHuge: string;
                    textColor: string;
                    iconColor: string;
                    extraTextColor: string;
                    iconSizeTiny: string;
                    iconSizeSmall: string;
                    iconSizeMedium: string;
                    iconSizeLarge: string;
                    iconSizeHuge: string;
                }, any>;
            }>;
        };
        peerOverrides: {
            Popover?: {
                peers?: {
                    [x: string]: any;
                } | undefined;
            } | undefined;
            InternalSelectMenu?: {
                peers?: {
                    Scrollbar?: import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Scrollbar", {
                        height: string;
                        width: string;
                        borderRadius: string;
                        color: string;
                        colorHover: string;
                        railInsetHorizontalBottom: string;
                        railInsetHorizontalTop: string;
                        railInsetVerticalRight: string;
                        railInsetVerticalLeft: string;
                        railColor: string;
                    }, any>> | undefined;
                    Empty?: import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Empty", {
                        fontSizeTiny: string;
                        fontSizeSmall: string;
                        fontSizeMedium: string;
                        fontSizeLarge: string;
                        fontSizeHuge: string;
                        textColor: string;
                        iconColor: string;
                        extraTextColor: string;
                        iconSizeTiny: string;
                        iconSizeSmall: string;
                        iconSizeMedium: string;
                        iconSizeLarge: string;
                        iconSizeHuge: string;
                    }, any>> | undefined;
                } | undefined;
            } | undefined;
        };
    }>;
    syncPosition: () => void;
    setShow: (value: boolean) => void;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<ExtractPropTypes<{
    multiple: BooleanConstructor;
    value: {
        readonly type: PropType<import("../../select/src/interface").Value | null>;
        readonly default: null;
    };
    cancelable: BooleanConstructor;
    options: {
        readonly type: PropType<import("../../select/src/interface").SelectMixedOption[]>;
        readonly default: () => never[];
    };
    size: {
        readonly type: PropType<import("./interface").PopselectSize>;
        readonly default: "medium";
    };
    scrollable: BooleanConstructor;
    'onUpdate:value': PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onUpdateValue: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue>>;
    onMouseenter: PropType<(e: MouseEvent) => void>;
    onMouseleave: PropType<(e: MouseEvent) => void>;
    renderLabel: PropType<import("../../_internal/select-menu/src/interface").RenderLabel>;
    showCheckmark: {
        readonly type: BooleanConstructor;
        readonly default: undefined;
    };
    nodeProps: PropType<import("../..").SelectNodeProps>;
    virtualScroll: BooleanConstructor;
    onChange: PropType<import("../../_utils").MaybeArray<import("../../select/src/interface").OnUpdateValue> | undefined>;
    placement: {
        default: string;
        type: PropType<import("vueuc/lib/binder/src/interface").Placement>;
    };
    trigger: {
        type: PropType<PopoverTrigger>;
        default: string;
    };
    to: {
        type: PropType<HTMLElement | string | boolean>;
        default: undefined;
    };
    disabled: BooleanConstructor;
    show: {
        type: PropType<boolean | undefined>;
        default: undefined;
    };
    flip: {
        type: BooleanConstructor;
        default: boolean;
    };
    x: NumberConstructor;
    y: NumberConstructor;
    maxWidth: NumberConstructor;
    minWidth: NumberConstructor;
    width: {
        type: PropType<number | "trigger">;
        default: undefined;
    };
    zIndex: NumberConstructor;
    duration: {
        type: NumberConstructor;
        default: number;
    };
    raw: BooleanConstructor;
    overlap: BooleanConstructor;
    contentClass: StringConstructor;
    contentStyle: PropType<import("vue").CSSProperties | string>;
    arrowClass: StringConstructor;
    arrowStyle: PropType<string | import("vue").CSSProperties>;
    arrowWrapperClass: StringConstructor;
    arrowWrapperStyle: PropType<string | import("vue").CSSProperties>;
    delay: {
        type: NumberConstructor;
        default: number;
    };
    arrowPointToCenter: BooleanConstructor;
    displayDirective: {
        type: PropType<"if" | "show">;
        default: string;
    };
    keepAliveOnHover: {
        type: BooleanConstructor;
        default: boolean;
    };
    headerClass: StringConstructor;
    headerStyle: PropType<import("vue").CSSProperties | string>;
    footerClass: StringConstructor;
    footerStyle: PropType<import("vue").CSSProperties | string>;
    internalDeactivateImmediately: BooleanConstructor;
    animated: {
        type: BooleanConstructor;
        default: boolean;
    };
    onClickoutside: PropType<(e: MouseEvent) => void>;
    internalTrapFocus: BooleanConstructor;
    defaultShow: BooleanConstructor;
    getDisabled: PropType<() => boolean>;
    'onUpdate:show': PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    onUpdateShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void>>;
    internalSyncTargetWithParent: BooleanConstructor;
    internalInheritedEventHandlers: {
        type: PropType<import("../../popover/src/Popover").TriggerEventHandlers[]>;
        default: () => never[];
    };
    internalExtraClass: {
        type: PropType<string[]>;
        default: () => never[];
    };
    onShow: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    onHide: PropType<import("../../_utils").MaybeArray<(value: boolean) => void> | undefined>;
    theme: PropType<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Popselect", {
        menuBoxShadow: string;
    }, {
        Popover: import("../../_mixins").Theme<"Popover", {
            fontSize: string;
            borderRadius: string;
            color: string;
            dividerColor: string;
            textColor: string;
            boxShadow: string;
            space: string;
            spaceArrow: string;
            arrowOffset: string;
            arrowOffsetVertical: string;
            arrowHeight: string;
            padding: string;
        }, any>;
        InternalSelectMenu: import("../../_mixins").Theme<"InternalSelectMenu", {
            optionFontSizeTiny: string;
            optionFontSizeSmall: string;
            optionFontSizeMedium: string;
            optionFontSizeLarge: string;
            optionFontSizeHuge: string;
            optionHeightTiny: string;
            optionHeightSmall: string;
            optionHeightMedium: string;
            optionHeightLarge: string;
            optionHeightHuge: string;
            borderRadius: string;
            color: string;
            groupHeaderTextColor: string;
            actionDividerColor: string;
            optionTextColor: string;
            optionTextColorPressed: string;
            optionTextColorDisabled: string;
            optionTextColorActive: string;
            optionOpacityDisabled: string;
            optionCheckColor: string;
            optionColorPending: string;
            optionColorActive: string;
            optionColorActivePending: string;
            actionTextColor: string;
            loadingColor: string;
            height: string;
            paddingTiny: string;
            paddingSmall: string;
            paddingMedium: string;
            paddingLarge: string;
            paddingHuge: string;
            optionPaddingTiny: string;
            optionPaddingSmall: string;
            optionPaddingMedium: string;
            optionPaddingLarge: string;
            optionPaddingHuge: string;
            loadingSize: string;
        }, {
            Scrollbar: import("../../_mixins").Theme<"Scrollbar", {
                height: string;
                width: string;
                borderRadius: string;
                color: string;
                colorHover: string;
                railInsetHorizontalBottom: string;
                railInsetHorizontalTop: string;
                railInsetVerticalRight: string;
                railInsetVerticalLeft: string;
                railColor: string;
            }, any>;
            Empty: import("../../_mixins").Theme<"Empty", {
                fontSizeTiny: string;
                fontSizeSmall: string;
                fontSizeMedium: string;
                fontSizeLarge: string;
                fontSizeHuge: string;
                textColor: string;
                iconColor: string;
                extraTextColor: string;
                iconSizeTiny: string;
                iconSizeSmall: string;
                iconSizeMedium: string;
                iconSizeLarge: string;
                iconSizeHuge: string;
            }, any>;
        }>;
    }>>>;
}>> & Readonly<{}>, {
    value: import("../../select/src/interface").Value | null;
    options: import("../../select/src/interface").SelectMixedOption[];
    size: import("./interface").PopselectSize;
    to: string | boolean | HTMLElement;
    disabled: boolean;
    show: boolean | undefined;
    flip: boolean;
    width: number | "trigger";
    multiple: boolean;
    duration: number;
    raw: boolean;
    placement: import("vueuc/lib/binder/src/interface").Placement;
    overlap: boolean;
    scrollable: boolean;
    trigger: PopoverTrigger;
    showCheckmark: boolean;
    virtualScroll: boolean;
    delay: number;
    arrowPointToCenter: boolean;
    displayDirective: "show" | "if";
    keepAliveOnHover: boolean;
    internalDeactivateImmediately: boolean;
    animated: boolean;
    internalTrapFocus: boolean;
    defaultShow: boolean;
    internalSyncTargetWithParent: boolean;
    internalInheritedEventHandlers: import("../../popover/src/Popover").TriggerEventHandlers[];
    internalExtraClass: string[];
    cancelable: boolean;
}, SlotsType<PopselectSlots>, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
