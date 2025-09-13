import { createId } from 'seemly';
import { defineComponent, getCurrentInstance, h, provide, ref, toRef } from 'vue';
import { useConfig } from "../../_mixins/index.mjs";
import { createInjectionKey } from "../../_utils/index.mjs";
import NImagePreview from "./ImagePreview.mjs";
import { imagePreviewSharedProps } from "./interface.mjs";
export const imageGroupInjectionKey = createInjectionKey('n-image-group');
export const imageGroupProps = imagePreviewSharedProps;
export default defineComponent({
  name: 'ImageGroup',
  props: imageGroupProps,
  setup(props) {
    let currentSrc;
    const {
      mergedClsPrefixRef
    } = useConfig(props);
    const groupId = `c${createId()}`;
    const vm = getCurrentInstance();
    const previewInstRef = ref(null);
    const setPreviewSrc = src => {
      var _a;
      currentSrc = src;
      (_a = previewInstRef.value) === null || _a === void 0 ? void 0 : _a.setPreviewSrc(src);
    };
    function go(step) {
      var _a, _b;
      if (!(vm === null || vm === void 0 ? void 0 : vm.proxy)) return;
      const container = vm.proxy.$el.parentElement;
      // use dom api since we can't get the correct order before all children are rendered
      const imgs = container.querySelectorAll(`[data-group-id=${groupId}]:not([data-error=true])`);
      if (!imgs.length) return;
      const index = Array.from(imgs).findIndex(img => img.dataset.previewSrc === currentSrc);
      if (~index) {
        setPreviewSrc(imgs[(index + step + imgs.length) % imgs.length].dataset.previewSrc);
      } else {
        setPreviewSrc(imgs[0].dataset.previewSrc);
      }
      if (step === 1) {
        (_a = props.onPreviewNext) === null || _a === void 0 ? void 0 : _a.call(props);
      } else {
        (_b = props.onPreviewPrev) === null || _b === void 0 ? void 0 : _b.call(props);
      }
    }
    provide(imageGroupInjectionKey, {
      mergedClsPrefixRef,
      setPreviewSrc,
      setThumbnailEl: el => {
        var _a;
        (_a = previewInstRef.value) === null || _a === void 0 ? void 0 : _a.setThumbnailEl(el);
      },
      toggleShow: () => {
        var _a;
        (_a = previewInstRef.value) === null || _a === void 0 ? void 0 : _a.toggleShow();
      },
      groupId,
      renderToolbarRef: toRef(props, 'renderToolbar')
    });
    return {
      mergedClsPrefix: mergedClsPrefixRef,
      previewInstRef,
      next: () => {
        go(1);
      },
      prev: () => {
        go(-1);
      }
    };
  },
  render() {
    return h(NImagePreview, {
      theme: this.theme,
      themeOverrides: this.themeOverrides,
      clsPrefix: this.mergedClsPrefix,
      ref: "previewInstRef",
      onPrev: this.prev,
      onNext: this.next,
      showToolbar: this.showToolbar,
      showToolbarTooltip: this.showToolbarTooltip,
      renderToolbar: this.renderToolbar
    }, this.$slots);
  }
});