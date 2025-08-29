import{Y as B,an as g,am as d,a1 as A,X as R,V as ae,ac as ne,ao as G,r as F,a7 as j,ap as O,a_ as N,ad as H,as as I,d as M,P as y,aI as ie,W as D,b$ as K,a as $,aq as V,aF as W,ae as L,be as de,al as se}from"./index-Bojb7anf.js";import{g as le}from"./Space-D__Tmt69.js";const ce=B("radio",`
 line-height: var(--n-label-line-height);
 outline: none;
 position: relative;
 user-select: none;
 -webkit-user-select: none;
 display: inline-flex;
 align-items: flex-start;
 flex-wrap: nowrap;
 font-size: var(--n-font-size);
 word-break: break-word;
`,[g("checked",[d("dot",`
 background-color: var(--n-color-active);
 `)]),d("dot-wrapper",`
 position: relative;
 flex-shrink: 0;
 flex-grow: 0;
 width: var(--n-radio-size);
 `),B("radio-input",`
 position: absolute;
 border: 0;
 width: 0;
 height: 0;
 opacity: 0;
 margin: 0;
 `),d("dot",`
 position: absolute;
 top: 50%;
 left: 0;
 transform: translateY(-50%);
 height: var(--n-radio-size);
 width: var(--n-radio-size);
 background: var(--n-color);
 box-shadow: var(--n-box-shadow);
 border-radius: 50%;
 transition:
 background-color .3s var(--n-bezier),
 box-shadow .3s var(--n-bezier);
 `,[R("&::before",`
 content: "";
 opacity: 0;
 position: absolute;
 left: 4px;
 top: 4px;
 height: calc(100% - 8px);
 width: calc(100% - 8px);
 border-radius: 50%;
 transform: scale(.8);
 background: var(--n-dot-color-active);
 transition: 
 opacity .3s var(--n-bezier),
 background-color .3s var(--n-bezier),
 transform .3s var(--n-bezier);
 `),g("checked",{boxShadow:"var(--n-box-shadow-active)"},[R("&::before",`
 opacity: 1;
 transform: scale(1);
 `)])]),d("label",`
 color: var(--n-text-color);
 padding: var(--n-label-padding);
 font-weight: var(--n-label-font-weight);
 display: inline-block;
 transition: color .3s var(--n-bezier);
 `),A("disabled",`
 cursor: pointer;
 `,[R("&:hover",[d("dot",{boxShadow:"var(--n-box-shadow-hover)"})]),g("focus",[R("&:not(:active)",[d("dot",{boxShadow:"var(--n-box-shadow-focus)"})])])]),g("disabled",`
 cursor: not-allowed;
 `,[d("dot",{boxShadow:"var(--n-box-shadow-disabled)",backgroundColor:"var(--n-color-disabled)"},[R("&::before",{backgroundColor:"var(--n-dot-color-disabled)"}),g("checked",`
 opacity: 1;
 `)]),d("label",{color:"var(--n-text-color-disabled)"}),B("radio-input",`
 cursor: not-allowed;
 `)])]),ue={name:String,value:{type:[String,Number,Boolean],default:"on"},checked:{type:Boolean,default:void 0},defaultChecked:Boolean,disabled:{type:Boolean,default:void 0},label:String,size:String,onUpdateChecked:[Function,Array],"onUpdate:checked":[Function,Array],checkedValue:{type:Boolean,default:void 0}},Y=ae("n-radio-group");function be(o){const e=ne(Y,null),t=G(o,{mergedSize(r){const{size:s}=o;if(s!==void 0)return s;if(e){const{mergedSizeRef:{value:c}}=e;if(c!==void 0)return c}return r?r.mergedSize.value:"medium"},mergedDisabled(r){return!!(o.disabled||e?.disabledRef.value||r?.disabled.value)}}),{mergedSizeRef:i,mergedDisabledRef:a}=t,u=F(null),b=F(null),h=F(o.defaultChecked),n=j(o,"checked"),p=O(n,h),m=N(()=>e?e.valueRef.value===o.value:p.value),w=N(()=>{const{name:r}=o;if(r!==void 0)return r;if(e)return e.nameRef.value}),f=F(!1);function C(){if(e){const{doUpdateValue:r}=e,{value:s}=o;I(r,s)}else{const{onUpdateChecked:r,"onUpdate:checked":s}=o,{nTriggerFormInput:c,nTriggerFormChange:l}=t;r&&I(r,!0),s&&I(s,!0),c(),l(),h.value=!0}}function x(){a.value||m.value||C()}function k(){x(),u.value&&(u.value.checked=m.value)}function z(){f.value=!1}function S(){f.value=!0}return{mergedClsPrefix:e?e.mergedClsPrefixRef:H(o).mergedClsPrefixRef,inputRef:u,labelRef:b,mergedName:w,mergedDisabled:a,renderSafeChecked:m,focus:f,mergedSize:i,handleRadioInputChange:k,handleRadioInputBlur:z,handleRadioInputFocus:S}}const he=Object.assign(Object.assign({},D.props),ue),xe=M({name:"Radio",props:he,setup(o){const e=be(o),t=D("Radio","-radio",ce,K,o,e.mergedClsPrefix),i=$(()=>{const{mergedSize:{value:p}}=e,{common:{cubicBezierEaseInOut:m},self:{boxShadow:w,boxShadowActive:f,boxShadowDisabled:C,boxShadowFocus:x,boxShadowHover:k,color:z,colorDisabled:S,colorActive:r,textColor:s,textColorDisabled:c,dotColorActive:l,dotColorDisabled:v,labelPadding:_,labelLineHeight:T,labelFontWeight:P,[V("fontSize",p)]:E,[V("radioSize",p)]:U}}=t.value;return{"--n-bezier":m,"--n-label-line-height":T,"--n-label-font-weight":P,"--n-box-shadow":w,"--n-box-shadow-active":f,"--n-box-shadow-disabled":C,"--n-box-shadow-focus":x,"--n-box-shadow-hover":k,"--n-color":z,"--n-color-active":r,"--n-color-disabled":S,"--n-dot-color-active":l,"--n-dot-color-disabled":v,"--n-font-size":E,"--n-radio-size":U,"--n-text-color":s,"--n-text-color-disabled":c,"--n-label-padding":_}}),{inlineThemeDisabled:a,mergedClsPrefixRef:u,mergedRtlRef:b}=H(o),h=W("Radio",b,u),n=a?L("radio",$(()=>e.mergedSize.value[0]),i,o):void 0;return Object.assign(e,{rtlEnabled:h,cssVars:a?void 0:i,themeClass:n?.themeClass,onRender:n?.onRender})},render(){const{$slots:o,mergedClsPrefix:e,onRender:t,label:i}=this;return t?.(),y("label",{class:[`${e}-radio`,this.themeClass,this.rtlEnabled&&`${e}-radio--rtl`,this.mergedDisabled&&`${e}-radio--disabled`,this.renderSafeChecked&&`${e}-radio--checked`,this.focus&&`${e}-radio--focus`],style:this.cssVars},y("div",{class:`${e}-radio__dot-wrapper`}," ",y("div",{class:[`${e}-radio__dot`,this.renderSafeChecked&&`${e}-radio__dot--checked`]}),y("input",{ref:"inputRef",type:"radio",class:`${e}-radio-input`,value:this.value,name:this.mergedName,checked:this.renderSafeChecked,disabled:this.mergedDisabled,onChange:this.handleRadioInputChange,onFocus:this.handleRadioInputFocus,onBlur:this.handleRadioInputBlur})),ie(o.default,a=>!a&&!i?null:y("div",{ref:"labelRef",class:`${e}-radio__label`},a||i)))}}),ve=B("radio-group",`
 display: inline-block;
 font-size: var(--n-font-size);
`,[d("splitor",`
 display: inline-block;
 vertical-align: bottom;
 width: 1px;
 transition:
 background-color .3s var(--n-bezier),
 opacity .3s var(--n-bezier);
 background: var(--n-button-border-color);
 `,[g("checked",{backgroundColor:"var(--n-button-border-color-active)"}),g("disabled",{opacity:"var(--n-opacity-disabled)"})]),g("button-group",`
 white-space: nowrap;
 height: var(--n-height);
 line-height: var(--n-height);
 `,[B("radio-button",{height:"var(--n-height)",lineHeight:"var(--n-height)"}),d("splitor",{height:"var(--n-height)"})]),B("radio-button",`
 vertical-align: bottom;
 outline: none;
 position: relative;
 user-select: none;
 -webkit-user-select: none;
 display: inline-block;
 box-sizing: border-box;
 padding-left: 14px;
 padding-right: 14px;
 white-space: nowrap;
 transition:
 background-color .3s var(--n-bezier),
 opacity .3s var(--n-bezier),
 border-color .3s var(--n-bezier),
 color .3s var(--n-bezier);
 background: var(--n-button-color);
 color: var(--n-button-text-color);
 border-top: 1px solid var(--n-button-border-color);
 border-bottom: 1px solid var(--n-button-border-color);
 `,[B("radio-input",`
 pointer-events: none;
 position: absolute;
 border: 0;
 border-radius: inherit;
 left: 0;
 right: 0;
 top: 0;
 bottom: 0;
 opacity: 0;
 z-index: 1;
 `),d("state-border",`
 z-index: 1;
 pointer-events: none;
 position: absolute;
 box-shadow: var(--n-button-box-shadow);
 transition: box-shadow .3s var(--n-bezier);
 left: -1px;
 bottom: -1px;
 right: -1px;
 top: -1px;
 `),R("&:first-child",`
 border-top-left-radius: var(--n-button-border-radius);
 border-bottom-left-radius: var(--n-button-border-radius);
 border-left: 1px solid var(--n-button-border-color);
 `,[d("state-border",`
 border-top-left-radius: var(--n-button-border-radius);
 border-bottom-left-radius: var(--n-button-border-radius);
 `)]),R("&:last-child",`
 border-top-right-radius: var(--n-button-border-radius);
 border-bottom-right-radius: var(--n-button-border-radius);
 border-right: 1px solid var(--n-button-border-color);
 `,[d("state-border",`
 border-top-right-radius: var(--n-button-border-radius);
 border-bottom-right-radius: var(--n-button-border-radius);
 `)]),A("disabled",`
 cursor: pointer;
 `,[R("&:hover",[d("state-border",`
 transition: box-shadow .3s var(--n-bezier);
 box-shadow: var(--n-button-box-shadow-hover);
 `),A("checked",{color:"var(--n-button-text-color-hover)"})]),g("focus",[R("&:not(:active)",[d("state-border",{boxShadow:"var(--n-button-box-shadow-focus)"})])])]),g("checked",`
 background: var(--n-button-color-active);
 color: var(--n-button-text-color-active);
 border-color: var(--n-button-border-color-active);
 `),g("disabled",`
 cursor: not-allowed;
 opacity: var(--n-opacity-disabled);
 `)])]);function fe(o,e,t){var i;const a=[];let u=!1;for(let b=0;b<o.length;++b){const h=o[b],n=(i=h.type)===null||i===void 0?void 0:i.name;n==="RadioButton"&&(u=!0);const p=h.props;if(n!=="RadioButton"){a.push(h);continue}if(b===0)a.push(h);else{const m=a[a.length-1].props,w=e===m.value,f=m.disabled,C=e===p.value,x=p.disabled,k=(w?2:0)+(f?0:1),z=(C?2:0)+(x?0:1),S={[`${t}-radio-group__splitor--disabled`]:f,[`${t}-radio-group__splitor--checked`]:w},r={[`${t}-radio-group__splitor--disabled`]:x,[`${t}-radio-group__splitor--checked`]:C},s=k<z?r:S;a.push(y("div",{class:[`${t}-radio-group__splitor`,s]}),h)}}return{children:a,isButtonGroup:u}}const ge=Object.assign(Object.assign({},D.props),{name:String,value:[String,Number,Boolean],defaultValue:{type:[String,Number,Boolean],default:null},size:String,disabled:{type:Boolean,default:void 0},"onUpdate:value":[Function,Array],onUpdateValue:[Function,Array]}),Re=M({name:"RadioGroup",props:ge,setup(o){const e=F(null),{mergedSizeRef:t,mergedDisabledRef:i,nTriggerFormChange:a,nTriggerFormInput:u,nTriggerFormBlur:b,nTriggerFormFocus:h}=G(o),{mergedClsPrefixRef:n,inlineThemeDisabled:p,mergedRtlRef:m}=H(o),w=D("Radio","-radio-group",ve,K,o,n),f=F(o.defaultValue),C=j(o,"value"),x=O(C,f);function k(l){const{onUpdateValue:v,"onUpdate:value":_}=o;v&&I(v,l),_&&I(_,l),f.value=l,a(),u()}function z(l){const{value:v}=e;v&&(v.contains(l.relatedTarget)||h())}function S(l){const{value:v}=e;v&&(v.contains(l.relatedTarget)||b())}se(Y,{mergedClsPrefixRef:n,nameRef:j(o,"name"),valueRef:x,disabledRef:i,mergedSizeRef:t,doUpdateValue:k});const r=W("Radio",m,n),s=$(()=>{const{value:l}=t,{common:{cubicBezierEaseInOut:v},self:{buttonBorderColor:_,buttonBorderColorActive:T,buttonBorderRadius:P,buttonBoxShadow:E,buttonBoxShadowFocus:U,buttonBoxShadowHover:q,buttonColor:X,buttonColorActive:J,buttonTextColor:Q,buttonTextColorActive:Z,buttonTextColorHover:ee,opacityDisabled:oe,[V("buttonHeight",l)]:te,[V("fontSize",l)]:re}}=w.value;return{"--n-font-size":re,"--n-bezier":v,"--n-button-border-color":_,"--n-button-border-color-active":T,"--n-button-border-radius":P,"--n-button-box-shadow":E,"--n-button-box-shadow-focus":U,"--n-button-box-shadow-hover":q,"--n-button-color":X,"--n-button-color-active":J,"--n-button-text-color":Q,"--n-button-text-color-hover":ee,"--n-button-text-color-active":Z,"--n-height":te,"--n-opacity-disabled":oe}}),c=p?L("radio-group",$(()=>t.value[0]),s,o):void 0;return{selfElRef:e,rtlEnabled:r,mergedClsPrefix:n,mergedValue:x,handleFocusout:S,handleFocusin:z,cssVars:p?void 0:s,themeClass:c?.themeClass,onRender:c?.onRender}},render(){var o;const{mergedValue:e,mergedClsPrefix:t,handleFocusin:i,handleFocusout:a}=this,{children:u,isButtonGroup:b}=fe(de(le(this)),e,t);return(o=this.onRender)===null||o===void 0||o.call(this),y("div",{onFocusin:i,onFocusout:a,ref:"selfElRef",class:[`${t}-radio-group`,this.rtlEnabled&&`${t}-radio-group--rtl`,this.themeClass,b&&`${t}-radio-group--button-group`],style:this.cssVars},u)}});export{Re as N,xe as a};
