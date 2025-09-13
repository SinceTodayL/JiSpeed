import type { ExtractPublicPropTypes } from '../../_utils';
import type { ScrollbarProps } from '../../scrollbar/src/Scrollbar';
import { type CSSProperties, type PropType } from 'vue';
import { type VirtualListInst, type VirtualListItemData, type VirtualListScrollToOptions } from 'vueuc';
import { type ScrollbarInst } from '../../_internal';
export { type VirtualListInst } from 'vueuc';
export declare const virtualListProps: {
    readonly scrollbarProps: PropType<ScrollbarProps>;
    readonly items: {
        readonly type: PropType<VirtualListItemData[]>;
        readonly default: () => never[];
    };
    readonly itemSize: {
        readonly type: NumberConstructor;
        readonly required: true;
    };
    readonly itemResizable: BooleanConstructor;
    readonly itemsStyle: PropType<string | CSSProperties>;
    readonly visibleItemsTag: {
        readonly type: PropType<string | object>;
        readonly default: "div";
    };
    readonly visibleItemsProps: ObjectConstructor;
    readonly ignoreItemResize: BooleanConstructor;
    readonly onScroll: PropType<(event: Event) => void>;
    readonly onWheel: PropType<(event: WheelEvent) => void>;
    readonly onResize: PropType<(entry: ResizeObserverEntry) => void>;
    readonly defaultScrollKey: PropType<number | string>;
    readonly defaultScrollIndex: NumberConstructor;
    readonly keyField: {
        readonly type: StringConstructor;
        readonly default: "key";
    };
    readonly paddingTop: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
    readonly paddingBottom: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
};
export type VirtualListProps = ExtractPublicPropTypes<typeof virtualListProps>;
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    readonly scrollbarProps: PropType<ScrollbarProps>;
    readonly items: {
        readonly type: PropType<VirtualListItemData[]>;
        readonly default: () => never[];
    };
    readonly itemSize: {
        readonly type: NumberConstructor;
        readonly required: true;
    };
    readonly itemResizable: BooleanConstructor;
    readonly itemsStyle: PropType<string | CSSProperties>;
    readonly visibleItemsTag: {
        readonly type: PropType<string | object>;
        readonly default: "div";
    };
    readonly visibleItemsProps: ObjectConstructor;
    readonly ignoreItemResize: BooleanConstructor;
    readonly onScroll: PropType<(event: Event) => void>;
    readonly onWheel: PropType<(event: WheelEvent) => void>;
    readonly onResize: PropType<(entry: ResizeObserverEntry) => void>;
    readonly defaultScrollKey: PropType<number | string>;
    readonly defaultScrollIndex: NumberConstructor;
    readonly keyField: {
        readonly type: StringConstructor;
        readonly default: "key";
    };
    readonly paddingTop: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
    readonly paddingBottom: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
}>, {
    scrollTo: (options: VirtualListScrollToOptions | number, y?: number) => void;
    scrollbarInstRef: import("vue").Ref<{
        $el: HTMLElement;
        containerRef: HTMLElement | null;
        contentRef: HTMLElement | null;
        containerScrollTop: number;
        syncUnifiedContainer: () => void;
        scrollTo: import("../../_internal/scrollbar/src/Scrollbar").ScrollTo;
        scrollBy: import("../../_internal/scrollbar/src/Scrollbar").ScrollBy;
        sync: () => void;
        handleMouseEnterWrapper: () => void;
        handleMouseLeaveWrapper: () => void;
    } | null, ScrollbarInst | {
        $el: HTMLElement;
        containerRef: HTMLElement | null;
        contentRef: HTMLElement | null;
        containerScrollTop: number;
        syncUnifiedContainer: () => void;
        scrollTo: import("../../_internal/scrollbar/src/Scrollbar").ScrollTo;
        scrollBy: import("../../_internal/scrollbar/src/Scrollbar").ScrollBy;
        sync: () => void;
        handleMouseEnterWrapper: () => void;
        handleMouseLeaveWrapper: () => void;
    } | null>;
    virtualListInstRef: import("vue").Ref<{
        listElRef: HTMLElement;
        itemsElRef: HTMLElement | null;
        scrollTo: import("vueuc").VVirtualListScrollTo;
    } | null, VirtualListInst | {
        listElRef: HTMLElement;
        itemsElRef: HTMLElement | null;
        scrollTo: import("vueuc").VVirtualListScrollTo;
    } | null>;
    getScrollContainer: () => HTMLElement | null | undefined;
    getScrollContent: () => HTMLElement | null | undefined;
    handleScroll: (e: Event) => void;
    handleResize: (e: ResizeObserverEntry) => void;
    handleWheel: (e: WheelEvent) => void;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    readonly scrollbarProps: PropType<ScrollbarProps>;
    readonly items: {
        readonly type: PropType<VirtualListItemData[]>;
        readonly default: () => never[];
    };
    readonly itemSize: {
        readonly type: NumberConstructor;
        readonly required: true;
    };
    readonly itemResizable: BooleanConstructor;
    readonly itemsStyle: PropType<string | CSSProperties>;
    readonly visibleItemsTag: {
        readonly type: PropType<string | object>;
        readonly default: "div";
    };
    readonly visibleItemsProps: ObjectConstructor;
    readonly ignoreItemResize: BooleanConstructor;
    readonly onScroll: PropType<(event: Event) => void>;
    readonly onWheel: PropType<(event: WheelEvent) => void>;
    readonly onResize: PropType<(entry: ResizeObserverEntry) => void>;
    readonly defaultScrollKey: PropType<number | string>;
    readonly defaultScrollIndex: NumberConstructor;
    readonly keyField: {
        readonly type: StringConstructor;
        readonly default: "key";
    };
    readonly paddingTop: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
    readonly paddingBottom: {
        readonly type: PropType<number | string>;
        readonly default: 0;
    };
}>> & Readonly<{}>, {
    readonly paddingBottom: string | number;
    readonly paddingTop: string | number;
    readonly items: VirtualListItemData[];
    readonly itemResizable: boolean;
    readonly visibleItemsTag: string | object;
    readonly ignoreItemResize: boolean;
    readonly keyField: string;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
