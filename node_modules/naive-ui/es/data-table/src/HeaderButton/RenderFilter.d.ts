import type { RenderFilter } from '../interface';
import { type PropType } from 'vue';
declare const _default: import("vue").DefineComponent<import("vue").ExtractPropTypes<{
    render: {
        type: PropType<RenderFilter>;
        required: true;
    };
    active: {
        type: BooleanConstructor;
        default: boolean;
    };
    show: {
        type: BooleanConstructor;
        default: boolean;
    };
}>, {}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {}, string, import("vue").PublicProps, Readonly<import("vue").ExtractPropTypes<{
    render: {
        type: PropType<RenderFilter>;
        required: true;
    };
    active: {
        type: BooleanConstructor;
        default: boolean;
    };
    show: {
        type: BooleanConstructor;
        default: boolean;
    };
}>> & Readonly<{}>, {
    show: boolean;
    active: boolean;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, true, {}, any>;
export default _default;
