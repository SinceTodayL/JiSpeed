"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.lightTheme = void 0;
const styles_1 = require("../_internal/scrollbar/styles");
// The file is for internal usage, do not export it, since all the components
// have default light theme.
const common_1 = require("../_styles/common");
const styles_2 = require("../alert/styles");
const styles_3 = require("../anchor/styles");
const styles_4 = require("../auto-complete/styles");
const styles_5 = require("../avatar-group/styles");
const styles_6 = require("../avatar/styles");
const styles_7 = require("../back-top/styles");
const styles_8 = require("../badge/styles");
const styles_9 = require("../breadcrumb/styles");
const styles_10 = require("../button-group/styles");
const styles_11 = require("../button/styles");
const styles_12 = require("../calendar/styles");
const styles_13 = require("../card/styles");
const styles_14 = require("../carousel/styles");
const styles_15 = require("../cascader/styles");
const styles_16 = require("../checkbox/styles");
const styles_17 = require("../code/styles");
const styles_18 = require("../collapse-transition/styles");
const styles_19 = require("../collapse/styles");
const styles_20 = require("../color-picker/styles");
const styles_21 = require("../data-table/styles");
const styles_22 = require("../date-picker/styles");
const styles_23 = require("../descriptions/styles");
const styles_24 = require("../dialog/styles");
const styles_25 = require("../divider/styles");
const styles_26 = require("../drawer/styles");
const styles_27 = require("../dropdown/styles");
const styles_28 = require("../dynamic-input/styles");
const styles_29 = require("../dynamic-tags/styles");
const styles_30 = require("../element/styles");
const styles_31 = require("../ellipsis/styles");
const styles_32 = require("../empty/styles");
const styles_33 = require("../equation/styles");
const styles_34 = require("../flex/styles");
const styles_35 = require("../float-button-group/styles");
const styles_36 = require("../float-button/styles");
const styles_37 = require("../form/styles");
const styles_38 = require("../gradient-text/styles");
const styles_39 = require("../icon-wrapper/styles");
const styles_40 = require("../icon/styles");
const styles_41 = require("../image/styles");
const styles_42 = require("../input-number/styles");
const styles_43 = require("../input-otp/styles");
const styles_44 = require("../input/styles");
const styles_45 = require("../layout/styles");
const styles_46 = require("../legacy-grid/styles");
const styles_47 = require("../legacy-transfer/styles");
const styles_48 = require("../list/styles");
const styles_49 = require("../loading-bar/styles");
const styles_50 = require("../log/styles");
const styles_51 = require("../marquee/styles");
const styles_52 = require("../mention/styles");
const styles_53 = require("../menu/styles");
const styles_54 = require("../message/styles");
const styles_55 = require("../modal/styles");
const styles_56 = require("../notification/styles");
const styles_57 = require("../page-header/styles");
const styles_58 = require("../pagination/styles");
const styles_59 = require("../popconfirm/styles");
const styles_60 = require("../popover/styles");
const styles_61 = require("../popselect/styles");
const styles_62 = require("../progress/styles");
const styles_63 = require("../qr-code/styles");
const styles_64 = require("../radio/styles");
const styles_65 = require("../rate/styles");
const styles_66 = require("../result/styles");
const styles_67 = require("../select/styles");
const styles_68 = require("../skeleton/styles");
const styles_69 = require("../slider/styles");
const styles_70 = require("../space/styles");
const styles_71 = require("../spin/styles");
const styles_72 = require("../split/styles");
const styles_73 = require("../statistic/styles");
const styles_74 = require("../steps/styles");
const styles_75 = require("../switch/styles");
const styles_76 = require("../table/styles");
const styles_77 = require("../tabs/styles");
const styles_78 = require("../tag/styles");
const styles_79 = require("../thing/styles");
const styles_80 = require("../time-picker/styles");
const styles_81 = require("../timeline/styles");
const styles_82 = require("../tooltip/styles");
const styles_83 = require("../transfer/styles");
const styles_84 = require("../tree-select/styles");
const styles_85 = require("../tree/styles");
const styles_86 = require("../typography/styles");
const styles_87 = require("../upload/styles");
const styles_88 = require("../watermark/styles");
exports.lightTheme = {
    name: 'light',
    common: common_1.commonLight,
    Alert: styles_2.alertLight,
    Anchor: styles_3.anchorLight,
    AutoComplete: styles_4.autoCompleteLight,
    Avatar: styles_6.avatarLight,
    AvatarGroup: styles_5.avatarGroupLight,
    BackTop: styles_7.backTopLight,
    Badge: styles_8.badgeLight,
    Breadcrumb: styles_9.breadcrumbLight,
    Button: styles_11.buttonLight,
    ButtonGroup: styles_10.buttonGroupLight,
    Calendar: styles_12.calendarLight,
    Card: styles_13.cardLight,
    Carousel: styles_14.carouselLight,
    Cascader: styles_15.cascaderLight,
    Checkbox: styles_16.checkboxLight,
    Code: styles_17.codeLight,
    Collapse: styles_19.collapseLight,
    CollapseTransition: styles_18.collapseTransitionLight,
    ColorPicker: styles_20.colorPickerLight,
    DataTable: styles_21.dataTableLight,
    DatePicker: styles_22.datePickerLight,
    Descriptions: styles_23.descriptionsLight,
    Dialog: styles_24.dialogLight,
    Divider: styles_25.dividerLight,
    Drawer: styles_26.drawerLight,
    Dropdown: styles_27.dropdownLight,
    DynamicInput: styles_28.dynamicInputLight,
    DynamicTags: styles_29.dynamicTagsLight,
    Element: styles_30.elementLight,
    Empty: styles_32.emptyLight,
    Equation: styles_33.equationLight,
    Ellipsis: styles_31.ellipsisLight,
    Flex: styles_34.flexLight,
    Form: styles_37.formLight,
    GradientText: styles_38.gradientTextLight,
    Icon: styles_40.iconLight,
    IconWrapper: styles_39.iconWrapperLight,
    Image: styles_41.imageLight,
    Input: styles_44.inputLight,
    InputNumber: styles_42.inputNumberLight,
    InputOtp: styles_43.inputOtpLight,
    Layout: styles_45.layoutLight,
    LegacyTransfer: styles_47.legacyTransferLight,
    List: styles_48.listLight,
    LoadingBar: styles_49.loadingBarLight,
    Log: styles_50.logLight,
    Menu: styles_53.menuLight,
    Mention: styles_52.mentionLight,
    Message: styles_54.messageLight,
    Modal: styles_55.modalLight,
    Notification: styles_56.notificationLight,
    PageHeader: styles_57.pageHeaderLight,
    Pagination: styles_58.paginationLight,
    Popconfirm: styles_59.popconfirmLight,
    Popover: styles_60.popoverLight,
    Popselect: styles_61.popselectLight,
    Progress: styles_62.progressLight,
    QrCode: styles_63.qrcodeLight,
    Radio: styles_64.radioLight,
    Rate: styles_65.rateLight,
    Row: styles_46.rowLight,
    Result: styles_66.resultLight,
    Scrollbar: styles_1.scrollbarLight,
    Skeleton: styles_68.skeletonLight,
    Select: styles_67.selectLight,
    Slider: styles_69.sliderLight,
    Space: styles_70.spaceLight,
    Spin: styles_71.spinLight,
    Statistic: styles_73.statisticLight,
    Steps: styles_74.stepsLight,
    Switch: styles_75.switchLight,
    Table: styles_76.tableLight,
    Tabs: styles_77.tabsLight,
    Tag: styles_78.tagLight,
    Thing: styles_79.thingLight,
    TimePicker: styles_80.timePickerLight,
    Timeline: styles_81.timelineLight,
    Tooltip: styles_82.tooltipLight,
    Transfer: styles_83.transferLight,
    Tree: styles_85.treeLight,
    TreeSelect: styles_84.treeSelectLight,
    Typography: styles_86.typographyLight,
    Upload: styles_87.uploadLight,
    Watermark: styles_88.watermarkLight,
    Split: styles_72.splitLight,
    FloatButton: styles_36.floatButtonLight,
    FloatButtonGroup: styles_35.floatButtonGroupLight,
    Marquee: styles_51.marqueeLight
};
