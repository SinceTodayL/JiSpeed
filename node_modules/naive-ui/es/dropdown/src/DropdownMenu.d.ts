import type { TreeNode } from 'treemate';
import type { DropdownGroupOption, DropdownIgnoredOption, DropdownOption } from './interface';
import { type CSSProperties, type PropType, type Ref } from 'vue';
export interface NDropdownMenuInjection {
    showIconRef: Ref<boolean>;
    hasSubmenuRef: Ref<boolean>;
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    scrollable: BooleanConstructor;
    showArrow: BooleanConstructor;
    arrowStyle: PropType<string | CSSProperties>;
    clsPrefix: {
        type: StringConstructor;
        required: true;
    };
    tmNodes: {
        type: PropType<Array<TreeNode<DropdownOption, DropdownGroupOption, DropdownIgnoredOption>>>;
        default: () => never[];
    };
    parentKey: {
        type: (StringConstructor | NumberConstructor)[];
        default: null;
    };
}>, {
    bodyRef: Ref<HTMLElement | null, HTMLElement | null>;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    scrollable: BooleanConstructor;
    showArrow: BooleanConstructor;
    arrowStyle: PropType<string | CSSProperties>;
    clsPrefix: {
        type: StringConstructor;
        required: true;
    };
    tmNodes: {
        type: PropType<Array<TreeNode<DropdownOption, DropdownGroupOption, DropdownIgnoredOption>>>;
        default: () => never[];
    };
    parentKey: {
        type: (StringConstructor | NumberConstructor)[];
        default: null;
    };
}>> & Readonly<{}>, {
    scrollable: boolean;
    showArrow: boolean;
    parentKey: string | number;
    tmNodes: TreeNode<import("../..").MenuOption, import("../..").MenuGroupOption, import("../../menu/src/interface").MenuIgnoredOption>[];
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
