declare const presetProps: {
    icon: import("vue").PropType<() => import("vue").VNodeChild>;
    type: {
        readonly type: import("vue").PropType<"info" | "success" | "warning" | "error" | "default">;
        readonly default: "default";
    };
    title: import("vue").PropType<string | (() => import("vue").VNodeChild)>;
    closable: {
        readonly type: BooleanConstructor;
        readonly default: true;
    };
    negativeText: StringConstructor;
    positiveText: StringConstructor;
    positiveButtonProps: import("vue").PropType<import("../..").ButtonProps>;
    negativeButtonProps: import("vue").PropType<import("../..").ButtonProps>;
    content: import("vue").PropType<string | (() => import("vue").VNodeChild)>;
    action: import("vue").PropType<() => import("vue").VNodeChild>;
    showIcon: {
        readonly type: BooleanConstructor;
        readonly default: true;
    };
    loading: BooleanConstructor;
    bordered: BooleanConstructor;
    iconPlacement: import("vue").PropType<import("../../dialog/src/interface").IconPlacement>;
    titleClass: import("vue").PropType<string | Array<string | undefined>>;
    titleStyle: import("vue").PropType<string | import("vue").CSSProperties>;
    contentClass: import("vue").PropType<string | Array<string | undefined>>;
    contentStyle: import("vue").PropType<string | import("vue").CSSProperties>;
    actionClass: import("vue").PropType<string | Array<string | undefined>>;
    actionStyle: import("vue").PropType<string | import("vue").CSSProperties>;
    onPositiveClick: import("vue").PropType<(e: MouseEvent) => void>;
    onNegativeClick: import("vue").PropType<(e: MouseEvent) => void>;
    onClose: import("vue").PropType<() => void>;
    headerClass: StringConstructor;
    headerStyle: import("vue").PropType<import("vue").CSSProperties | string>;
    headerExtraClass: StringConstructor;
    headerExtraStyle: import("vue").PropType<import("vue").CSSProperties | string>;
    footerClass: StringConstructor;
    footerStyle: import("vue").PropType<import("vue").CSSProperties | string>;
    embedded: BooleanConstructor;
    segmented: {
        readonly type: import("vue").PropType<boolean | import("../../card/src/Card").CardSegmented>;
        readonly default: false;
    };
    size: {
        readonly type: import("vue").PropType<"small" | "medium" | "large" | "huge">;
        readonly default: "medium";
    };
    hoverable: BooleanConstructor;
    role: StringConstructor;
    tag: {
        readonly type: import("vue").PropType<keyof HTMLElementTagNameMap>;
        readonly default: "div";
    };
    cover: import("vue").PropType<() => import("vue").VNodeChild>;
    footer: import("vue").PropType<() => import("vue").VNodeChild>;
    headerExtra: import("vue").PropType<() => import("vue").VNodeChild>;
};
declare const presetPropsKeys: ("type" | "tag" | "content" | "size" | "footer" | "title" | "cover" | "icon" | "role" | "onClose" | "action" | "loading" | "positiveText" | "negativeText" | "bordered" | "showIcon" | "contentClass" | "contentStyle" | "closable" | "headerClass" | "headerStyle" | "footerClass" | "footerStyle" | "iconPlacement" | "hoverable" | "embedded" | "headerExtraClass" | "headerExtraStyle" | "segmented" | "headerExtra" | "positiveButtonProps" | "negativeButtonProps" | "titleClass" | "titleStyle" | "actionClass" | "actionStyle" | "onPositiveClick" | "onNegativeClick")[];
export { presetProps, presetPropsKeys };
