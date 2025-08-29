import{X as f,Y as c,Z as C,an as v,d as S,P as o,bt as x,T as k,ad as T,W as h,bu as $,a as m,bc as w,aq as R,ae as _,bv as B,r as O,ak as P}from"./index-BAcIoLG0.js";const W=f([f("@keyframes spin-rotate",`
 from {
 transform: rotate(0);
 }
 to {
 transform: rotate(360deg);
 }
 `),c("spin-container",`
 position: relative;
 `,[c("spin-body",`
 position: absolute;
 top: 50%;
 left: 50%;
 transform: translateX(-50%) translateY(-50%);
 `,[C()])]),c("spin-body",`
 display: inline-flex;
 align-items: center;
 justify-content: center;
 flex-direction: column;
 `),c("spin",`
 display: inline-flex;
 height: var(--n-size);
 width: var(--n-size);
 font-size: var(--n-size);
 color: var(--n-color);
 `,[v("rotate",`
 animation: spin-rotate 2s linear infinite;
 `)]),c("spin-description",`
 display: inline-block;
 font-size: var(--n-font-size);
 color: var(--n-text-color);
 transition: color .3s var(--n-bezier);
 margin-top: 8px;
 `),c("spin-content",`
 opacity: 1;
 transition: opacity .3s var(--n-bezier);
 pointer-events: all;
 `,[v("spinning",`
 user-select: none;
 -webkit-user-select: none;
 pointer-events: none;
 opacity: var(--n-opacity-spinning);
 `)])]),j={small:20,medium:18,large:16},N=Object.assign(Object.assign({},h.props),{contentClass:String,contentStyle:[Object,String],description:String,stroke:String,size:{type:[String,Number],default:"medium"},show:{type:Boolean,default:!0},strokeWidth:Number,rotate:{type:Boolean,default:!0},spinning:{type:Boolean,validator:()=>!0,default:void 0},delay:Number}),I=S({name:"Spin",props:N,slots:Object,setup(e){const{mergedClsPrefixRef:r,inlineThemeDisabled:t}=T(e),n=h("Spin","-spin",W,$,e,r),d=m(()=>{const{size:s}=e,{common:{cubicBezierEaseInOut:a},self:u}=n.value,{opacitySpinning:y,color:b,textColor:g}=u,z=typeof s=="number"?w(s):u[R("size",s)];return{"--n-bezier":a,"--n-opacity-spinning":y,"--n-size":z,"--n-color":b,"--n-text-color":g}}),i=t?_("spin",m(()=>{const{size:s}=e;return typeof s=="number"?String(s):s[0]}),d,e):void 0,p=B(e,["spinning","show"]),l=O(!1);return P(s=>{let a;if(p.value){const{delay:u}=e;if(u){a=window.setTimeout(()=>{l.value=!0},u),s(()=>{clearTimeout(a)});return}}l.value=p.value}),{mergedClsPrefix:r,active:l,mergedStrokeWidth:m(()=>{const{strokeWidth:s}=e;if(s!==void 0)return s;const{size:a}=e;return j[typeof a=="number"?"medium":a]}),cssVars:t?void 0:d,themeClass:i?.themeClass,onRender:i?.onRender}},render(){var e,r;const{$slots:t,mergedClsPrefix:n,description:d}=this,i=t.icon&&this.rotate,p=(d||t.description)&&o("div",{class:`${n}-spin-description`},d||((e=t.description)===null||e===void 0?void 0:e.call(t))),l=t.icon?o("div",{class:[`${n}-spin-body`,this.themeClass]},o("div",{class:[`${n}-spin`,i&&`${n}-spin--rotate`],style:t.default?"":this.cssVars},t.icon()),p):o("div",{class:[`${n}-spin-body`,this.themeClass]},o(x,{clsPrefix:n,style:t.default?"":this.cssVars,stroke:this.stroke,"stroke-width":this.mergedStrokeWidth,class:`${n}-spin`}),p);return(r=this.onRender)===null||r===void 0||r.call(this),t.default?o("div",{class:[`${n}-spin-container`,this.themeClass],style:this.cssVars},o("div",{class:[`${n}-spin-content`,this.active&&`${n}-spin-content--spinning`,this.contentClass],style:this.contentStyle},t),o(k,{name:"fade-in-transition"},{default:()=>this.active?l:null})):l}});export{I as _};
