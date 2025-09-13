import type { ExtractPublicPropTypes } from '../../_utils';
import type { CollapseItemArrowSlotProps, CollapseItemHeaderExtraSlotProps, CollapseItemHeaderSlotProps } from './interface';
import { type PropType, type VNode } from 'vue';
export declare const collapseItemProps: {
    readonly title: StringConstructor;
    readonly name: PropType<string | number>;
    readonly disabled: BooleanConstructor;
    readonly displayDirective: PropType<"if" | "show">;
};
export type CollapseItemProps = ExtractPublicPropTypes<typeof collapseItemProps>;
export interface CollapseItemSlots {
    default?: () => VNode[];
    header?: (props: CollapseItemHeaderSlotProps) => VNode[];
    'header-extra'?: (props: CollapseItemHeaderExtraSlotProps) => VNode[];
    arrow?: (props: CollapseItemArrowSlotProps) => VNode[];
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    readonly title: StringConstructor;
    readonly name: PropType<string | number>;
    readonly disabled: BooleanConstructor;
    readonly displayDirective: PropType<"if" | "show">;
}>, {
    rtlEnabled: import("vue").Ref<import("../../config-provider/src/internal-interface").RtlItem | undefined, import("../../config-provider/src/internal-interface").RtlItem | undefined> | undefined;
    collapseSlots: import("./Collapse").CollapseSlots;
    randomName: string;
    mergedClsPrefix: import("vue").Ref<string, string>;
    collapsed: import("vue").ComputedRef<boolean>;
    triggerAreas: import("vue").Ref<("extra" | "main" | "arrow")[], ("extra" | "main" | "arrow")[]>;
    mergedDisplayDirective: import("vue").ComputedRef<"show" | "if">;
    arrowPlacement: import("vue").ComputedRef<"left" | "right">;
    handleClick(e: MouseEvent): void;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    readonly title: StringConstructor;
    readonly name: PropType<string | number>;
    readonly disabled: BooleanConstructor;
    readonly displayDirective: PropType<"if" | "show">;
}>> & Readonly<{}>, {
    readonly disabled: boolean;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
