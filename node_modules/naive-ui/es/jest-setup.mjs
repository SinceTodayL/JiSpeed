import { isBrowser } from "./_utils/index.mjs";
// https://github.com/jsdom/jsdom/issues/1422
if (isBrowser) {
  HTMLDivElement.prototype.scrollTo = () => {};
}