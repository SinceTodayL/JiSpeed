import{d as I,O as s,ag as f,ah as v,af as S,aj as n,ak as H,bd as W,an as T,r as k,a as _,as as G,ap as j,be as K,b3 as F,at as q,V as Z,ay as J,av as E,bf as Q,aa as X,bg as Y,ar as O,aU as ee,a3 as z,bh as re,bi as ae,bj as te,W as oe,ae as se,ax as A,al as ne,bk as le,bl as D}from"./index-Bzxd5AZU.js";import{a as ie,g as de,h as ce}from"./goods-iMeECYPt.js";import{_ as pe,f as me,b as fe}from"./FormItem-DfEbt8I2.js";const ue=I({name:"ChevronLeft",render(){return s("svg",{viewBox:"0 0 16 16",fill:"none",xmlns:"http://www.w3.org/2000/svg"},s("path",{d:"M10.3536 3.14645C10.5488 3.34171 10.5488 3.65829 10.3536 3.85355L6.20711 8L10.3536 12.1464C10.5488 12.3417 10.5488 12.6583 10.3536 12.8536C10.1583 13.0488 9.84171 13.0488 9.64645 12.8536L5.14645 8.35355C4.95118 8.15829 4.95118 7.84171 5.14645 7.64645L9.64645 3.14645C9.84171 2.95118 10.1583 2.95118 10.3536 3.14645Z",fill:"currentColor"}))}}),ge=f("collapse","width: 100%;",[f("collapse-item",`
 font-size: var(--n-font-size);
 color: var(--n-text-color);
 transition:
 color .3s var(--n-bezier),
 border-color .3s var(--n-bezier);
 margin: var(--n-item-margin);
 `,[v("disabled",[n("header","cursor: not-allowed;",[n("header-main",`
 color: var(--n-title-text-color-disabled);
 `),f("collapse-item-arrow",`
 color: var(--n-arrow-color-disabled);
 `)])]),f("collapse-item","margin-left: 32px;"),S("&:first-child","margin-top: 0;"),S("&:first-child >",[n("header","padding-top: 0;")]),v("left-arrow-placement",[n("header",[f("collapse-item-arrow","margin-right: 4px;")])]),v("right-arrow-placement",[n("header",[f("collapse-item-arrow","margin-left: 4px;")])]),n("content-wrapper",[n("content-inner","padding-top: 16px;"),W({duration:"0.15s"})]),v("active",[n("header",[v("active",[f("collapse-item-arrow","transform: rotate(90deg);")])])]),S("&:not(:first-child)","border-top: 1px solid var(--n-divider-color);"),H("disabled",[v("trigger-area-main",[n("header",[n("header-main","cursor: pointer;"),f("collapse-item-arrow","cursor: default;")])]),v("trigger-area-arrow",[n("header",[f("collapse-item-arrow","cursor: pointer;")])]),v("trigger-area-extra",[n("header",[n("header-extra","cursor: pointer;")])])]),n("header",`
 font-size: var(--n-title-font-size);
 display: flex;
 flex-wrap: nowrap;
 align-items: center;
 transition: color .3s var(--n-bezier);
 position: relative;
 padding: var(--n-title-padding);
 color: var(--n-title-text-color);
 `,[n("header-main",`
 display: flex;
 flex-wrap: nowrap;
 align-items: center;
 font-weight: var(--n-title-font-weight);
 transition: color .3s var(--n-bezier);
 flex: 1;
 color: var(--n-title-text-color);
 `),n("header-extra",`
 display: flex;
 align-items: center;
 transition: color .3s var(--n-bezier);
 color: var(--n-text-color);
 `),f("collapse-item-arrow",`
 display: flex;
 transition:
 transform .15s var(--n-bezier),
 color .3s var(--n-bezier);
 font-size: 18px;
 color: var(--n-arrow-color);
 `)])])]),he=Object.assign(Object.assign({},j.props),{defaultExpandedNames:{type:[Array,String],default:null},expandedNames:[Array,String],arrowPlacement:{type:String,default:"left"},accordion:{type:Boolean,default:!1},displayDirective:{type:String,default:"if"},triggerAreas:{type:Array,default:()=>["main","extra","arrow"]},onItemHeaderClick:[Function,Array],"onUpdate:expandedNames":[Function,Array],onUpdateExpandedNames:[Function,Array],onExpandedNamesChange:{type:[Function,Array],validator:()=>!0,default:void 0}}),U=Z("n-collapse"),_e=I({name:"Collapse",props:he,slots:Object,setup(e,{slots:d}){const{mergedClsPrefixRef:l,inlineThemeDisabled:r,mergedRtlRef:i}=T(e),t=k(e.defaultExpandedNames),g=_(()=>e.expandedNames),x=G(g,t),w=j("Collapse","-collapse",ge,K,e,l);function c(p){const{"onUpdate:expandedNames":o,onUpdateExpandedNames:m,onExpandedNamesChange:y}=e;m&&E(m,p),o&&E(o,p),y&&E(y,p),t.value=p}function h(p){const{onItemHeaderClick:o}=e;o&&E(o,p)}function a(p,o,m){const{accordion:y}=e,{value:R}=x;if(y)p?(c([o]),h({name:o,expanded:!0,event:m})):(c([]),h({name:o,expanded:!1,event:m}));else if(!Array.isArray(R))c([o]),h({name:o,expanded:!0,event:m});else{const b=R.slice(),P=b.findIndex($=>o===$);~P?(b.splice(P,1),c(b),h({name:o,expanded:!1,event:m})):(b.push(o),c(b),h({name:o,expanded:!0,event:m}))}}J(U,{props:e,mergedClsPrefixRef:l,expandedNamesRef:x,slots:d,toggleItem:a});const u=F("Collapse",i,l),N=_(()=>{const{common:{cubicBezierEaseInOut:p},self:{titleFontWeight:o,dividerColor:m,titlePadding:y,titleTextColor:R,titleTextColorDisabled:b,textColor:P,arrowColor:$,fontSize:V,titleFontSize:B,arrowColorDisabled:L,itemMargin:M}}=w.value;return{"--n-font-size":V,"--n-bezier":p,"--n-text-color":P,"--n-divider-color":m,"--n-title-padding":y,"--n-title-font-size":B,"--n-title-text-color":R,"--n-title-text-color-disabled":b,"--n-title-font-weight":o,"--n-arrow-color":$,"--n-arrow-color-disabled":L,"--n-item-margin":M}}),C=r?q("collapse",void 0,N,e):void 0;return{rtlEnabled:u,mergedTheme:w,mergedClsPrefix:l,cssVars:r?void 0:N,themeClass:C?.themeClass,onRender:C?.onRender}},render(){var e;return(e=this.onRender)===null||e===void 0||e.call(this),s("div",{class:[`${this.mergedClsPrefix}-collapse`,this.rtlEnabled&&`${this.mergedClsPrefix}-collapse--rtl`,this.themeClass],style:this.cssVars},this.$slots)}}),ve=I({name:"CollapseItemContent",props:{displayDirective:{type:String,required:!0},show:Boolean,clsPrefix:{type:String,required:!0}},setup(e){return{onceTrue:Y(O(e,"show"))}},render(){return s(Q,null,{default:()=>{const{show:e,displayDirective:d,onceTrue:l,clsPrefix:r}=this,i=d==="show"&&l,t=s("div",{class:`${r}-collapse-item__content-wrapper`},s("div",{class:`${r}-collapse-item__content-inner`},this.$slots));return i?X(t,[[ee,e]]):e?t:null}})}}),xe={title:String,name:[String,Number],disabled:Boolean,displayDirective:String},Ie=I({name:"CollapseItem",props:xe,setup(e){const{mergedRtlRef:d}=T(e),l=ae(),r=te(()=>{var a;return(a=e.name)!==null&&a!==void 0?a:l}),i=oe(U);i||se("collapse-item","`n-collapse-item` must be placed inside `n-collapse`.");const{expandedNamesRef:t,props:g,mergedClsPrefixRef:x,slots:w}=i,c=_(()=>{const{value:a}=t;if(Array.isArray(a)){const{value:u}=r;return!~a.findIndex(N=>N===u)}else if(a){const{value:u}=r;return u!==a}return!0});return{rtlEnabled:F("Collapse",d,x),collapseSlots:w,randomName:l,mergedClsPrefix:x,collapsed:c,triggerAreas:O(g,"triggerAreas"),mergedDisplayDirective:_(()=>{const{displayDirective:a}=e;return a||g.displayDirective}),arrowPlacement:_(()=>g.arrowPlacement),handleClick(a){let u="main";A(a,"arrow")&&(u="arrow"),A(a,"extra")&&(u="extra"),g.triggerAreas.includes(u)&&i&&!e.disabled&&i.toggleItem(c.value,r.value,a)}}},render(){const{collapseSlots:e,$slots:d,arrowPlacement:l,collapsed:r,mergedDisplayDirective:i,mergedClsPrefix:t,disabled:g,triggerAreas:x}=this,w=z(d.header,{collapsed:r},()=>[this.title]),c=d["header-extra"]||e["header-extra"],h=d.arrow||e.arrow;return s("div",{class:[`${t}-collapse-item`,`${t}-collapse-item--${l}-arrow-placement`,g&&`${t}-collapse-item--disabled`,!r&&`${t}-collapse-item--active`,x.map(a=>`${t}-collapse-item--trigger-area-${a}`)]},s("div",{class:[`${t}-collapse-item__header`,!r&&`${t}-collapse-item__header--active`]},s("div",{class:`${t}-collapse-item__header-main`,onClick:this.handleClick},l==="right"&&w,s("div",{class:`${t}-collapse-item-arrow`,key:this.rtlEnabled?0:1,"data-arrow":!0},z(h,{collapsed:r},()=>[s(ne,{clsPrefix:t},{default:()=>this.rtlEnabled?s(ue,null):s(le,null)})])),l==="left"&&w),re(c,{collapsed:r},a=>s("div",{class:`${t}-collapse-item__header-extra`,onClick:this.handleClick,"data-extra":!0},a))),s(ve,{clsPrefix:t,displayDirective:i,show:!r},d))}}),we=Object.assign(Object.assign({},ce),fe),Ne=I({__GRID_ITEM__:!0,name:"FormItemGridItem",alias:["FormItemGi"],props:we,setup(){const e=k(null);return{formItemInstRef:e,validate:(...r)=>{const{value:i}=e;if(i)return i.validate(...r)},restoreValidation:()=>{const{value:r}=e;r&&r.restoreValidation()}}},render(){return s(ie,D(this.$.vnode.props||{},de),{default:()=>{const e=D(this.$props,me);return s(pe,Object.assign({ref:"formItemInstRef"},e),this.$slots)}})}});export{_e as _,Ie as a,Ne as b};
