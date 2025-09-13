import { type PropType, type VNode } from 'vue';
export interface ImagePreviewInst {
    setThumbnailEl: (e: HTMLImageElement | null) => void;
    setPreviewSrc: (src?: string) => void;
    toggleShow: () => void;
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    onNext: PropType<() => void>;
    onPrev: PropType<() => void>;
    clsPrefix: {
        type: StringConstructor;
        required: true;
    };
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
}>, {
    setThumbnailEl: (e: HTMLImageElement | null) => void;
    setPreviewSrc: (src?: string) => void;
    toggleShow: () => void;
    previewRef: import("vue").Ref<HTMLImageElement | null, HTMLImageElement | null>;
    previewWrapperRef: import("vue").Ref<HTMLDivElement | null, HTMLDivElement | null>;
    previewSrc: import("vue").Ref<string | undefined, string | undefined>;
    show: import("vue").Ref<boolean, boolean>;
    appear: Readonly<import("vue").Ref<boolean, boolean>>;
    displayed: import("vue").Ref<boolean, boolean>;
    previewedImgProps: import("vue").Ref<import("vue").ImgHTMLAttributes | undefined, import("vue").ImgHTMLAttributes | undefined> | undefined;
    handleWheel(e: WheelEvent): void;
    handlePreviewMousedown: (e: MouseEvent) => void;
    handlePreviewDblclick: (e: MouseEvent) => void;
    syncTransformOrigin: () => void;
    handleAfterLeave: () => void;
    handleDragStart: (e: DragEvent) => void;
    zoomIn: () => void;
    zoomOut: () => void;
    handleDownloadClick: () => void;
    rotateCounterclockwise: () => void;
    rotateClockwise: () => void;
    handleSwitchPrev: () => void;
    handleSwitchNext: () => void;
    withTooltip: (node: VNode, tooltipKey: "tipPrevious" | "tipNext" | "tipCounterclockwise" | "tipClockwise" | "tipZoomOut" | "tipZoomIn" | "tipDownload" | "tipClose" | "tipOriginalSize") => VNode;
    resizeToOrignalImageSize: () => void;
    cssVars: import("vue").ComputedRef<{
        '--n-bezier': string;
        '--n-toolbar-icon-color': string;
        '--n-toolbar-color': string;
        '--n-toolbar-border-radius': string;
        '--n-toolbar-box-shadow': string;
    }> | undefined;
    themeClass: import("vue").Ref<string, string> | undefined;
    onRender: (() => void) | undefined;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    onNext: PropType<() => void>;
    onPrev: PropType<() => void>;
    clsPrefix: {
        type: StringConstructor;
        required: true;
    };
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
}>> & Readonly<{}>, {
    showToolbar: boolean;
    showToolbarTooltip: boolean;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
