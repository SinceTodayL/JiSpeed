import type { TreeNode } from 'treemate';
import type { MenuGroupOption, MenuIgnoredOption, MenuMixedOption, MenuOption } from './interface';
import type { MenuSetupProps } from './Menu';
import { type VNode } from 'vue';
export declare function isIgnoredNode(rawNode: MenuMixedOption): rawNode is MenuIgnoredOption;
export declare function isDividerNode(rawNode: MenuMixedOption): rawNode is MenuIgnoredOption;
export declare function itemRenderer(tmNode: TreeNode<MenuOption, MenuGroupOption, MenuIgnoredOption>, menuProps: MenuSetupProps): VNode | null;
