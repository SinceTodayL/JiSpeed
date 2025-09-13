import type { ImagePreviewInst } from './ImagePreview';
import type { IntersectionObserverOptions } from './utils';
import { type ImgHTMLAttributes, type PropType, type SlotsType, type VNode } from 'vue';
import { type ExtractPublicPropTypes } from '../../_utils';
export interface ImageInst {
    click: () => void;
}
export declare const imageProps: {
    onPreviewPrev: PropType<() => void>;
    onPreviewNext: PropType<() => void>;
    showToolbar: {
        type: BooleanConstructor;
        default: boolean;
    };
    showToolbarTooltip: BooleanConstructor;
    renderToolbar: PropType<import("./public-types").ImageRenderToolbar>;
    theme: PropType<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    alt: StringConstructor;
    height: PropType<string | number>;
    imgProps: PropType<ImgHTMLAttributes>;
    previewedImgProps: PropType<ImgHTMLAttributes>;
    lazy: BooleanConstructor;
    intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    objectFit: {
        type: PropType<"fill" | "contain" | "cover" | "none" | "scale-down">;
        default: string;
    };
    previewSrc: StringConstructor;
    fallbackSrc: StringConstructor;
    width: PropType<string | number>;
    src: StringConstructor;
    previewDisabled: BooleanConstructor;
    loadDescription: StringConstructor;
    onError: PropType<(e: Event) => void>;
    onLoad: PropType<(e: Event) => void>;
};
export type ImageProps = ExtractPublicPropTypes<typeof imageProps>;
export interface ImageSlots {
    placeholder?: () => VNode[];
    error?: () => VNode[];
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    onPreviewPrev: PropType<() => void>;
    onPreviewNext: PropType<() => void>;
    showToolbar: {
        type: BooleanConstructor;
        default: boolean;
    };
    showToolbarTooltip: BooleanConstructor;
    renderToolbar: PropType<import("./public-types").ImageRenderToolbar>;
    theme: PropType<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    alt: StringConstructor;
    height: PropType<string | number>;
    imgProps: PropType<ImgHTMLAttributes>;
    previewedImgProps: PropType<ImgHTMLAttributes>;
    lazy: BooleanConstructor;
    intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    objectFit: {
        type: PropType<"fill" | "contain" | "cover" | "none" | "scale-down">;
        default: string;
    };
    previewSrc: StringConstructor;
    fallbackSrc: StringConstructor;
    width: PropType<string | number>;
    src: StringConstructor;
    previewDisabled: BooleanConstructor;
    loadDescription: StringConstructor;
    onError: PropType<(e: Event) => void>;
    onLoad: PropType<(e: Event) => void>;
}>, {
    click: () => void;
    mergedClsPrefix: import("vue").Ref<string, string>;
    groupId: string | undefined;
    previewInstRef: import("vue").Ref<{
        setThumbnailEl: (e: HTMLImageElement | null) => void;
        setPreviewSrc: (src?: string) => void;
        toggleShow: () => void;
    } | null, ImagePreviewInst | {
        setThumbnailEl: (e: HTMLImageElement | null) => void;
        setPreviewSrc: (src?: string) => void;
        toggleShow: () => void;
    } | null>;
    imageRef: import("vue").Ref<HTMLImageElement | null, HTMLImageElement | null>;
    showError: import("vue").Ref<boolean, boolean>;
    shouldStartLoading: import("vue").Ref<boolean, boolean>;
    loaded: import("vue").Ref<boolean, boolean>;
    mergedOnClick: (e: MouseEvent) => void;
    mergedOnError: (e: Event) => void;
    mergedOnLoad: (e: Event) => void;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    onPreviewPrev: PropType<() => void>;
    onPreviewNext: PropType<() => void>;
    showToolbar: {
        type: BooleanConstructor;
        default: boolean;
    };
    showToolbarTooltip: BooleanConstructor;
    renderToolbar: PropType<import("./public-types").ImageRenderToolbar>;
    theme: PropType<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>;
    themeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    builtinThemeOverrides: PropType<import("../../_mixins/use-theme").ExtractThemeOverrides<import("../../_mixins").Theme<"Image", {
        toolbarIconColor: string;
        toolbarColor: string;
        toolbarBoxShadow: string;
        toolbarBorderRadius: string;
    }, {
        Tooltip: import("../../_mixins").Theme<"Tooltip", {
            borderRadius: string;
            boxShadow: string;
            color: string;
            textColor: string;
            padding: string;
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
        }>;
    }>>>;
    alt: StringConstructor;
    height: PropType<string | number>;
    imgProps: PropType<ImgHTMLAttributes>;
    previewedImgProps: PropType<ImgHTMLAttributes>;
    lazy: BooleanConstructor;
    intersectionObserverOptions: PropType<IntersectionObserverOptions>;
    objectFit: {
        type: PropType<"fill" | "contain" | "cover" | "none" | "scale-down">;
        default: string;
    };
    previewSrc: StringConstructor;
    fallbackSrc: StringConstructor;
    width: PropType<string | number>;
    src: StringConstructor;
    previewDisabled: BooleanConstructor;
    loadDescription: StringConstructor;
    onError: PropType<(e: Event) => void>;
    onLoad: PropType<(e: Event) => void>;
}>> & Readonly<{}>, {
    objectFit: "fill" | "none" | "contain" | "cover" | "scale-down";
    lazy: boolean;
    showToolbar: boolean;
    showToolbarTooltip: boolean;
    previewDisabled: boolean;
}, SlotsType<ImageSlots>, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
