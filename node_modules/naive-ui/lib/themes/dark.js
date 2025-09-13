"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.darkTheme = void 0;
const styles_1 = require("../_internal/scrollbar/styles");
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
const styles_34 = require("../float-button-group/styles");
const styles_35 = require("../float-button/styles");
const styles_36 = require("../form/styles");
const styles_37 = require("../gradient-text/styles");
const styles_38 = require("../icon-wrapper/styles");
const styles_39 = require("../icon/styles");
const styles_40 = require("../image/styles");
const styles_41 = require("../input-number/styles");
const styles_42 = require("../input/styles");
const styles_43 = require("../layout/styles");
const styles_44 = require("../legacy-grid/styles");
const styles_45 = require("../legacy-transfer/styles");
const styles_46 = require("../list/styles");
const styles_47 = require("../loading-bar/styles");
const styles_48 = require("../log/styles");
const styles_49 = require("../marquee/styles");
const styles_50 = require("../mention/styles");
const styles_51 = require("../menu/styles");
const styles_52 = require("../message/styles");
const styles_53 = require("../modal/styles");
const styles_54 = require("../notification/styles");
const styles_55 = require("../page-header/styles");
const styles_56 = require("../pagination/styles");
const styles_57 = require("../popconfirm/styles");
const styles_58 = require("../popover/styles");
const styles_59 = require("../popselect/styles");
const styles_60 = require("../progress/styles");
const styles_61 = require("../qr-code/styles");
const styles_62 = require("../radio/styles");
const styles_63 = require("../rate/styles");
const styles_64 = require("../result/styles");
const styles_65 = require("../select/styles");
const styles_66 = require("../skeleton/styles");
const styles_67 = require("../slider/styles");
const styles_68 = require("../space/styles");
const styles_69 = require("../spin/styles");
const styles_70 = require("../split/styles");
const styles_71 = require("../statistic/styles");
const styles_72 = require("../steps/styles");
const styles_73 = require("../styles");
const styles_74 = require("../switch/styles");
const styles_75 = require("../table/styles");
const styles_76 = require("../tabs/styles");
const styles_77 = require("../tag/styles");
const styles_78 = require("../thing/styles");
const styles_79 = require("../time-picker/styles");
const styles_80 = require("../timeline/styles");
const styles_81 = require("../tooltip/styles");
const styles_82 = require("../transfer/styles");
const styles_83 = require("../tree-select/styles");
const styles_84 = require("../tree/styles");
const styles_85 = require("../typography/styles");
const styles_86 = require("../upload/styles");
const styles_87 = require("../watermark/styles");
exports.darkTheme = {
    name: 'dark',
    common: common_1.commonDark,
    Alert: styles_2.alertDark,
    Anchor: styles_3.anchorDark,
    AutoComplete: styles_4.autoCompleteDark,
    Avatar: styles_6.avatarDark,
    AvatarGroup: styles_5.avatarGroupDark,
    BackTop: styles_7.backTopDark,
    Badge: styles_8.badgeDark,
    Breadcrumb: styles_9.breadcrumbDark,
    Button: styles_11.buttonDark,
    ButtonGroup: styles_10.buttonGroupDark,
    Calendar: styles_12.calendarDark,
    Card: styles_13.cardDark,
    Carousel: styles_14.carouselDark,
    Cascader: styles_15.cascaderDark,
    Checkbox: styles_16.checkboxDark,
    Code: styles_17.codeDark,
    Collapse: styles_19.collapseDark,
    CollapseTransition: styles_18.collapseTransitionDark,
    ColorPicker: styles_20.colorPickerDark,
    DataTable: styles_21.dataTableDark,
    DatePicker: styles_22.datePickerDark,
    Descriptions: styles_23.descriptionsDark,
    Dialog: styles_24.dialogDark,
    Divider: styles_25.dividerDark,
    Drawer: styles_26.drawerDark,
    Dropdown: styles_27.dropdownDark,
    DynamicInput: styles_28.dynamicInputDark,
    DynamicTags: styles_29.dynamicTagsDark,
    Element: styles_30.elementDark,
    Empty: styles_32.emptyDark,
    Ellipsis: styles_31.ellipsisDark,
    Equation: styles_33.equationDark,
    Flex: styles_73.flexDark,
    Form: styles_36.formDark,
    GradientText: styles_37.gradientTextDark,
    Icon: styles_39.iconDark,
    IconWrapper: styles_38.iconWrapperDark,
    Image: styles_40.imageDark,
    Input: styles_42.inputDark,
    InputNumber: styles_41.inputNumberDark,
    InputOtp: styles_73.inputOtpDark,
    LegacyTransfer: styles_45.legacyTransferDark,
    Layout: styles_43.layoutDark,
    List: styles_46.listDark,
    LoadingBar: styles_47.loadingBarDark,
    Log: styles_48.logDark,
    Menu: styles_51.menuDark,
    Mention: styles_50.mentionDark,
    Message: styles_52.messageDark,
    Modal: styles_53.modalDark,
    Notification: styles_54.notificationDark,
    PageHeader: styles_55.pageHeaderDark,
    Pagination: styles_56.paginationDark,
    Popconfirm: styles_57.popconfirmDark,
    Popover: styles_58.popoverDark,
    Popselect: styles_59.popselectDark,
    Progress: styles_60.progressDark,
    QrCode: styles_61.qrcodeDark,
    Radio: styles_62.radioDark,
    Rate: styles_63.rateDark,
    Result: styles_64.resultDark,
    Row: styles_44.rowDark,
    Scrollbar: styles_1.scrollbarDark,
    Select: styles_65.selectDark,
    Skeleton: styles_66.skeletonDark,
    Slider: styles_67.sliderDark,
    Space: styles_68.spaceDark,
    Spin: styles_69.spinDark,
    Statistic: styles_71.statisticDark,
    Steps: styles_72.stepsDark,
    Switch: styles_74.switchDark,
    Table: styles_75.tableDark,
    Tabs: styles_76.tabsDark,
    Tag: styles_77.tagDark,
    Thing: styles_78.thingDark,
    TimePicker: styles_79.timePickerDark,
    Timeline: styles_80.timelineDark,
    Tooltip: styles_81.tooltipDark,
    Transfer: styles_82.transferDark,
    Tree: styles_84.treeDark,
    TreeSelect: styles_83.treeSelectDark,
    Typography: styles_85.typographyDark,
    Upload: styles_86.uploadDark,
    Watermark: styles_87.watermarkDark,
    Split: styles_70.splitDark,
    FloatButton: styles_35.floatButtonDark,
    FloatButtonGroup: styles_34.floatButtonGroupDark,
    Marquee: styles_49.marqueeDark
};
