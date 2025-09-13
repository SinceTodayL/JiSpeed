import type { ExtractPublicPropTypes } from '../../_utils';
import { type PropType, type VNodeChild } from 'vue';
export interface CountdownTimeInfo {
    hours: number;
    minutes: number;
    seconds: number;
    milliseconds: number;
}
export interface CountdownInst {
    reset: () => void;
}
export declare const countdownProps: {
    duration: {
        type: NumberConstructor;
        default: number;
    };
    active: {
        type: BooleanConstructor;
        default: boolean;
    };
    precision: {
        type: PropType<0 | 1 | 2 | 3>;
        default: number;
    };
    render: PropType<(props: CountdownTimeInfo) => VNodeChild>;
    onFinish: PropType<() => void>;
};
export type CountdownProps = ExtractPublicPropTypes<typeof countdownProps>;
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    duration: {
        type: NumberConstructor;
        default: number;
    };
    active: {
        type: BooleanConstructor;
        default: boolean;
    };
    precision: {
        type: PropType<0 | 1 | 2 | 3>;
        default: number;
    };
    render: PropType<(props: CountdownTimeInfo) => VNodeChild>;
    onFinish: PropType<() => void>;
}>, CountdownInst & {
    distance: import("vue").Ref<number, number>;
    getTimeInfo: (distance: number) => CountdownTimeInfo;
    getDisplayValue: (info: CountdownTimeInfo) => string;
}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    duration: {
        type: NumberConstructor;
        default: number;
    };
    active: {
        type: BooleanConstructor;
        default: boolean;
    };
    precision: {
        type: PropType<0 | 1 | 2 | 3>;
        default: number;
    };
    render: PropType<(props: CountdownTimeInfo) => VNodeChild>;
    onFinish: PropType<() => void>;
}>> & Readonly<{}>, {
    active: boolean;
    duration: number;
    precision: 0 | 1 | 2 | 3;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
