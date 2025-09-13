import { commonLight } from "../../_styles/common/index.mjs";
import commonVariables from "./_common.mjs";
export function self(vars) {
  const {
    boxShadow2,
    popoverColor,
    textColor2,
    borderRadius,
    fontSize,
    dividerColor
  } = vars;
  return Object.assign(Object.assign({}, commonVariables), {
    fontSize,
    borderRadius,
    color: popoverColor,
    dividerColor,
    textColor: textColor2,
    boxShadow: boxShadow2
  });
}
const popoverLight = {
  name: 'Popover',
  common: commonLight,
  self
};
export default popoverLight;