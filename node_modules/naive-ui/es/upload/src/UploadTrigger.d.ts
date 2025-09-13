import type { UploadTriggerDefaultSlotOptions } from './interface';
import { type SlotsType, type VNode } from 'vue';
export interface UploadTriggerSlots {
    default?: (options: UploadTriggerDefaultSlotOptions) => VNode[];
}
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    abstract: BooleanConstructor;
}>, () => VNode<import("vue").RendererNode, import("vue").RendererElement, {
    [key: string]: any;
}>[] | JSX.Element | undefined, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    abstract: BooleanConstructor;
}>> & Readonly<{}>, {
    abstract: boolean;
}, SlotsType<UploadTriggerSlots>, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
