import type { DropPosition, TreeOption } from './interface';
import { type VNode } from 'vue';
export declare function renderDropMark({ position, offsetLevel, indent, el }: {
    position: 'before' | 'inside' | 'after';
    offsetLevel: number;
    indent: number;
    el: HTMLElement;
}): VNode;
export declare function defaultAllowDrop({ dropPosition, node }: {
    dropPosition: DropPosition;
    node: TreeOption;
}): boolean;
