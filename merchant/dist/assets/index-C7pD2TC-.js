import{X as I,Y as x,aD as te,aE as ae,an as D,am as b,d as z,P as u,ad as F,aF as L,W as j,aG as ne,al as re,a7 as oe,a as w,ae as G,V as se,ac as ie,aH as le,aI as E,aJ as ce,aK as de,F as P,at as ue,m as H,aw as J,aj as K,c as k,o as h,K as B,w as m,f as p,e as v,b as $,s as fe,t as S,h as R,A as O,aL as ve,r as T,aM as me,aN as ge,p as pe,aO as he,aP as _e,J as xe,L as be,_ as ye,aQ as $e,g as N,aA as U,aR as we,y as Se,B as ze}from"./index-BxzqPXmU.js";import{f as Ce,a as ke,b as Re}from"./merchant-Bt-CI2i4.js";import{a as W,_ as Q}from"./Grid-D_d4zmmE.js";import{N as q}from"./Space-BuoyC2Rm.js";import{_ as X}from"./Spin-DEQ8D7pC.js";const Ae=I([x("list",`
 --n-merged-border-color: var(--n-border-color);
 --n-merged-color: var(--n-color);
 --n-merged-color-hover: var(--n-color-hover);
 margin: 0;
 font-size: var(--n-font-size);
 transition:
 background-color .3s var(--n-bezier),
 color .3s var(--n-bezier),
 border-color .3s var(--n-bezier);
 padding: 0;
 list-style-type: none;
 color: var(--n-text-color);
 background-color: var(--n-merged-color);
 `,[D("show-divider",[x("list-item",[I("&:not(:last-child)",[b("divider",`
 background-color: var(--n-merged-border-color);
 `)])])]),D("clickable",[x("list-item",`
 cursor: pointer;
 `)]),D("bordered",`
 border: 1px solid var(--n-merged-border-color);
 border-radius: var(--n-border-radius);
 `),D("hoverable",[x("list-item",`
 border-radius: var(--n-border-radius);
 `,[I("&:hover",`
 background-color: var(--n-merged-color-hover);
 `,[b("divider",`
 background-color: transparent;
 `)])])]),D("bordered, hoverable",[x("list-item",`
 padding: 12px 20px;
 `),b("header, footer",`
 padding: 12px 20px;
 `)]),b("header, footer",`
 padding: 12px 0;
 box-sizing: border-box;
 transition: border-color .3s var(--n-bezier);
 `,[I("&:not(:last-child)",`
 border-bottom: 1px solid var(--n-merged-border-color);
 `)]),x("list-item",`
 position: relative;
 padding: 12px 0; 
 box-sizing: border-box;
 display: flex;
 flex-wrap: nowrap;
 align-items: center;
 transition:
 background-color .3s var(--n-bezier),
 border-color .3s var(--n-bezier);
 `,[b("prefix",`
 margin-right: 20px;
 flex: 0;
 `),b("suffix",`
 margin-left: 20px;
 flex: 0;
 `),b("main",`
 flex: 1;
 `),b("divider",`
 height: 1px;
 position: absolute;
 bottom: 0;
 left: 0;
 right: 0;
 background-color: transparent;
 transition: background-color .3s var(--n-bezier);
 pointer-events: none;
 `)])]),te(x("list",`
 --n-merged-color-hover: var(--n-color-hover-modal);
 --n-merged-color: var(--n-color-modal);
 --n-merged-border-color: var(--n-border-color-modal);
 `)),ae(x("list",`
 --n-merged-color-hover: var(--n-color-hover-popover);
 --n-merged-color: var(--n-color-popover);
 --n-merged-border-color: var(--n-border-color-popover);
 `))]),Ie=Object.assign(Object.assign({},j.props),{size:{type:String,default:"medium"},bordered:Boolean,clickable:Boolean,hoverable:Boolean,showDivider:{type:Boolean,default:!0}}),Y=se("n-list"),Ne=z({name:"List",props:Ie,slots:Object,setup(a){const{mergedClsPrefixRef:e,inlineThemeDisabled:o,mergedRtlRef:f}=F(a),_=L("List",f,e),d=j("List","-list",Ae,ne,a,e);re(Y,{showDividerRef:oe(a,"showDivider"),mergedClsPrefixRef:e});const r=w(()=>{const{common:{cubicBezierEaseInOut:l},self:{fontSize:s,textColor:t,color:n,colorModal:i,colorPopover:g,borderColor:y,borderColorModal:C,borderColorPopover:A,borderRadius:V,colorHover:M,colorHoverModal:Z,colorHoverPopover:ee}}=d.value;return{"--n-font-size":s,"--n-bezier":l,"--n-text-color":t,"--n-color":n,"--n-border-radius":V,"--n-border-color":y,"--n-border-color-modal":C,"--n-border-color-popover":A,"--n-color-modal":i,"--n-color-popover":g,"--n-color-hover":M,"--n-color-hover-modal":Z,"--n-color-hover-popover":ee}}),c=o?G("list",void 0,r,a):void 0;return{mergedClsPrefix:e,rtlEnabled:_,cssVars:o?void 0:r,themeClass:c?.themeClass,onRender:c?.onRender}},render(){var a;const{$slots:e,mergedClsPrefix:o,onRender:f}=this;return f?.(),u("ul",{class:[`${o}-list`,this.rtlEnabled&&`${o}-list--rtl`,this.bordered&&`${o}-list--bordered`,this.showDivider&&`${o}-list--show-divider`,this.hoverable&&`${o}-list--hoverable`,this.clickable&&`${o}-list--clickable`,this.themeClass],style:this.cssVars},e.header?u("div",{class:`${o}-list__header`},e.header()):null,(a=e.default)===null||a===void 0?void 0:a.call(e),e.footer?u("div",{class:`${o}-list__footer`},e.footer()):null)}}),Te=z({name:"ListItem",slots:Object,setup(){const a=ie(Y,null);return a||le("list-item","`n-list-item` must be placed in `n-list`."),{showDivider:a.showDividerRef,mergedClsPrefix:a.mergedClsPrefixRef}},render(){const{$slots:a,mergedClsPrefix:e}=this;return u("li",{class:`${e}-list-item`},a.prefix?u("div",{class:`${e}-list-item__prefix`},a.prefix()):null,a.default?u("div",{class:`${e}-list-item__main`},a):null,a.suffix?u("div",{class:`${e}-list-item__suffix`},a.suffix()):null,this.showDivider&&u("div",{class:`${e}-list-item__divider`}))}}),je=x("statistic",[b("label",`
 font-weight: var(--n-label-font-weight);
 transition: .3s color var(--n-bezier);
 font-size: var(--n-label-font-size);
 color: var(--n-label-text-color);
 `),x("statistic-value",`
 margin-top: 4px;
 font-weight: var(--n-value-font-weight);
 `,[b("prefix",`
 margin: 0 4px 0 0;
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-prefix-text-color);
 `,[x("icon",{verticalAlign:"-0.125em"})]),b("content",`
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-text-color);
 `),b("suffix",`
 margin: 0 0 0 4px;
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-suffix-text-color);
 `,[x("icon",{verticalAlign:"-0.125em"})])])]),De=Object.assign(Object.assign({},j.props),{tabularNums:Boolean,label:String,value:[String,Number]}),Pe=z({name:"Statistic",props:De,slots:Object,setup(a){const{mergedClsPrefixRef:e,inlineThemeDisabled:o,mergedRtlRef:f}=F(a),_=j("Statistic","-statistic",je,ce,a,e),d=L("Statistic",f,e),r=w(()=>{const{self:{labelFontWeight:l,valueFontSize:s,valueFontWeight:t,valuePrefixTextColor:n,labelTextColor:i,valueSuffixTextColor:g,valueTextColor:y,labelFontSize:C},common:{cubicBezierEaseInOut:A}}=_.value;return{"--n-bezier":A,"--n-label-font-size":C,"--n-label-font-weight":l,"--n-label-text-color":i,"--n-value-font-weight":t,"--n-value-font-size":s,"--n-value-prefix-text-color":n,"--n-value-suffix-text-color":g,"--n-value-text-color":y}}),c=o?G("statistic",void 0,r,a):void 0;return{rtlEnabled:d,mergedClsPrefix:e,cssVars:o?void 0:r,themeClass:c?.themeClass,onRender:c?.onRender}},render(){var a;const{mergedClsPrefix:e,$slots:{default:o,label:f,prefix:_,suffix:d}}=this;return(a=this.onRender)===null||a===void 0||a.call(this),u("div",{class:[`${e}-statistic`,this.themeClass,this.rtlEnabled&&`${e}-statistic--rtl`],style:this.cssVars},E(f,r=>u("div",{class:`${e}-statistic__label`},this.label||r)),u("div",{class:`${e}-statistic-value`,style:{fontVariantNumeric:this.tabularNums?"tabular-nums":""}},E(_,r=>r&&u("span",{class:`${e}-statistic-value__prefix`},r)),this.value!==void 0?u("span",{class:`${e}-statistic-value__content`},this.value):E(o,r=>r&&u("span",{class:`${e}-statistic-value__content`},r)),E(d,r=>r&&u("span",{class:`${e}-statistic-value__suffix`},r))))}}),Me=x("thing",`
 display: flex;
 transition: color .3s var(--n-bezier);
 font-size: var(--n-font-size);
 color: var(--n-text-color);
`,[x("thing-avatar",`
 margin-right: 12px;
 margin-top: 2px;
 `),x("thing-avatar-header-wrapper",`
 display: flex;
 flex-wrap: nowrap;
 `,[x("thing-header-wrapper",`
 flex: 1;
 `)]),x("thing-main",`
 flex-grow: 1;
 `,[x("thing-header",`
 display: flex;
 margin-bottom: 4px;
 justify-content: space-between;
 align-items: center;
 `,[b("title",`
 font-size: 16px;
 font-weight: var(--n-title-font-weight);
 transition: color .3s var(--n-bezier);
 color: var(--n-title-text-color);
 `)]),b("description",[I("&:not(:last-child)",`
 margin-bottom: 4px;
 `)]),b("content",[I("&:not(:first-child)",`
 margin-top: 12px;
 `)]),b("footer",[I("&:not(:first-child)",`
 margin-top: 12px;
 `)]),b("action",[I("&:not(:first-child)",`
 margin-top: 12px;
 `)])])]),Ee=Object.assign(Object.assign({},j.props),{title:String,titleExtra:String,description:String,descriptionClass:String,descriptionStyle:[String,Object],content:String,contentClass:String,contentStyle:[String,Object],contentIndented:Boolean}),Be=z({name:"Thing",props:Ee,slots:Object,setup(a,{slots:e}){const{mergedClsPrefixRef:o,inlineThemeDisabled:f,mergedRtlRef:_}=F(a),d=j("Thing","-thing",Me,de,a,o),r=L("Thing",_,o),c=w(()=>{const{self:{titleTextColor:s,textColor:t,titleFontWeight:n,fontSize:i},common:{cubicBezierEaseInOut:g}}=d.value;return{"--n-bezier":g,"--n-font-size":i,"--n-text-color":t,"--n-title-font-weight":n,"--n-title-text-color":s}}),l=f?G("thing",void 0,c,a):void 0;return()=>{var s;const{value:t}=o,n=r?r.value:!1;return(s=l?.onRender)===null||s===void 0||s.call(l),u("div",{class:[`${t}-thing`,l?.themeClass,n&&`${t}-thing--rtl`],style:f?void 0:c.value},e.avatar&&a.contentIndented?u("div",{class:`${t}-thing-avatar`},e.avatar()):null,u("div",{class:`${t}-thing-main`},!a.contentIndented&&(e.header||a.title||e["header-extra"]||a.titleExtra||e.avatar)?u("div",{class:`${t}-thing-avatar-header-wrapper`},e.avatar?u("div",{class:`${t}-thing-avatar`},e.avatar()):null,e.header||a.title||e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header-wrapper`},u("div",{class:`${t}-thing-header`},e.header||a.title?u("div",{class:`${t}-thing-header__title`},e.header?e.header():a.title):null,e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header__extra`},e["header-extra"]?e["header-extra"]():a.titleExtra):null),e.description||a.description?u("div",{class:[`${t}-thing-main__description`,a.descriptionClass],style:a.descriptionStyle},e.description?e.description():a.description):null):null):u(P,null,e.header||a.title||e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header`},e.header||a.title?u("div",{class:`${t}-thing-header__title`},e.header?e.header():a.title):null,e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header__extra`},e["header-extra"]?e["header-extra"]():a.titleExtra):null):null,e.description||a.description?u("div",{class:[`${t}-thing-main__description`,a.descriptionClass],style:a.descriptionStyle},e.description?e.description():a.description):null),e.default||a.content?u("div",{class:[`${t}-thing-main__content`,a.contentClass],style:a.contentStyle},e.default?e.default():a.content):null,e.footer?u("div",{class:`${t}-thing-main__footer`},e.footer()):null,e.action?u("div",{class:`${t}-thing-main__action`},e.action()):null))}}});function Oe(a,e){const o=new URLSearchParams;e?.size!==void 0&&o.append("size",e.size.toString()),e?.page!==void 0&&o.append("page",e.page.toString());const f=o.toString(),_=`/api/announcements/activate/${a}${f?`?${f}`:""}`;return ue({url:_,method:"get"})}const Ve="/merchant/assets/avatar-C3nF7gXh.svg",Fe={class:"flex-y-center"},Le={class:"pl-12px"},Ge={class:"text-18px font-semibold"},He={class:"text-#999 leading-30px"},Ke={key:0,class:"text-#666 text-12px mt-1"},We=z({name:"HeaderBanner",__name:"header-banner",setup(a){const e=H(),o=J(),f=w(()=>e.isMobile?0:16),_=w(()=>{const{salesStats:l}=o;if(!l||l.length===0)return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:"¥0.00"},{id:2,label:"日均销量",value:"0份"}];l.reduce((t,n)=>t+(Number(n.salesQty)||0),0);const s=l.reduce((t,n)=>t+(Number(n.salesAmount)||0),0);return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:`¥${s.toFixed(2)}`}]}),d=w(()=>`你好，${o.merchantInfo?.merchantName||"商家"} !`),r=w(()=>{const l=o.merchantInfo?.status,s=o.merchantInfo?.location||"",t=o.merchantInfo?.contactInfo||"";let n="🟢 营业中";l===0?n="🔴 暂停营业":l&&l>0&&(n="🟢 营业中");const i=s?`🏢${s}`:"",g=t?`📞${t}`:"";return[n,i,g].filter(Boolean).join(" | ")}),c=async()=>{const{merchantId:l}=o;if(!l){console.log("merchantId is empty");return}try{const s=await Ce(l);console.log("fetchMerchantInfo result",s);let t=null;const n=s;n?.data?.data?(t=n.data.data,console.log("使用嵌套数据结构 result.data.data:",t)):n?.data?(t=n.data,console.log("使用直接数据结构 result.data:",t)):s&&typeof s=="object"&&(t=s,console.log("使用原始返回数据:",t)),t&&typeof t=="object"?(o.setMerchantInfo(t),console.log("merchantStore.merchantInfo",o.merchantInfo)):(console.error("未找到有效的商家数据"),console.log("完整API响应:",s),window.$message?.warning("获取到的商家信息格式不正确"))}catch(s){console.error("加载商家基本信息失败:",s),window.$message?.warning("暂时无法获取商家信息，请稍后刷新页面")}try{const s=new Date().toISOString(),t=new Date(Date.now()-30*24*60*60*1e3).toISOString(),n=await ke(l,{startTime:t,endTime:s});console.log("fetchMerchantSalesStats result",n),n&&Array.isArray(n.response.data.data)?(o.setSalesStats(n.response.data.data),console.log("成功加载销售统计数据:",n.response.data.data)):console.warn("销售统计数据格式异常:",n.response.data.data)}catch(s){console.error("加载商家销售数据失败:",s),window.$message?.error("获取商家销售数据失败")}};return K(()=>{o.triggerAuthUpdate(),c()}),(l,s)=>{const t=W,n=Pe,i=q,g=Q,y=B;return h(),k(y,{bordered:!1,class:"card-wrapper"},{default:m(()=>[p(g,{"x-gap":f.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:m(()=>[p(t,{span:"24 s:24 m:18"},{default:m(()=>[v("div",Fe,[s[0]||(s[0]=v("div",{class:"size-72px shrink-0 overflow-hidden rd-1/2"},[v("img",{src:Ve,class:"size-full"})],-1)),v("div",Le,[v("h3",Ge,S(d.value),1),v("p",He,S(r.value),1),R(o).merchantInfo?.contactInfo?(h(),$("p",Ke," 📞 "+S(R(o).merchantInfo.contactInfo),1)):fe("",!0)])])]),_:1}),p(t,{span:"24 s:24 m:6"},{default:m(()=>[p(i,{size:24,justify:"end"},{default:m(()=>[(h(!0),$(P,null,O(_.value,C=>(h(),k(n,ve({key:C.id,class:"whitespace-nowrap"},{ref_for:!0},C),null,16))),128))]),_:1})]),_:1})]),_:1},8,["x-gap"])]),_:1})}}}),Qe=z({name:"CountTo",__name:"count-to",props:{startValue:{default:0},endValue:{default:2021},duration:{default:1500},autoplay:{type:Boolean,default:!0},decimals:{default:0},prefix:{default:""},suffix:{default:""},separator:{default:","},decimal:{default:"."},useEasing:{type:Boolean,default:!0},transition:{default:"linear"}},setup(a){const e=a,o=T(e.startValue),f=w(()=>e.useEasing?me[e.transition]:void 0),_=ge(o,{disabled:!1,duration:e.duration,transition:f.value}),d=w(()=>r(_.value));function r(l){const{decimals:s,decimal:t,separator:n,suffix:i,prefix:g}=e;let y=l.toFixed(s);y=String(y);const C=y.split(".");let A=C[0];const V=C.length>1?t+C[1]:"",M=/(\d+)(\d{3})/;if(n)for(;M.test(A);)A=A.replace(M,`$1${n}$2`);return g+A+V+i}async function c(){await he(),o.value=e.endValue}return pe([()=>e.startValue,()=>e.endValue],()=>{e.autoplay&&c()},{immediate:!0}),(l,s)=>(h(),$("span",null,S(d.value),1))}}),Ue={class:"text-16px"},Je={class:"flex justify-between pt-12px"},qe=z({name:"CardData",__name:"card-data",setup(a){const e=J(),o=w(()=>{const{salesStats:r}=e;if(!r||r.length===0)return[{key:"totalSales",title:"总销量",value:0,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:0,unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:0,unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"maxDaySales",title:"单日最高销量",value:0,unit:"份",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}];const c=r.reduce((n,i)=>n+(Number(i.salesQty)||0),0),l=r.reduce((n,i)=>n+(Number(i.salesAmount)||0),0),s=l/7;return r.reduce((n,i)=>Math.max(n,Number(i.salesQty)||0),0),[{key:"totalSales",title:"总销量",value:c,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:Math.round(l),unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:Math.round(s),unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"goodRate",title:"好评率",value:100,unit:"%",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}]}),[f,_]=_e();function d(r){return`linear-gradient(to bottom right, ${r.start}, ${r.end})`}return(r,c)=>{const l=ye,s=Qe,t=W,n=Q,i=B;return h(),k(i,{bordered:!1,size:"small",class:"card-wrapper"},{default:m(()=>[p(R(f),null,{default:m(({$slots:g,gradientColor:y})=>[v("div",{class:"rd-8px px-16px pb-4px pt-8px text-white",style:be({backgroundImage:y})},[(h(),k(xe(g.default)))],4)]),_:1}),p(n,{cols:"s:1 m:2 l:4",responsive:"screen","x-gap":16,"y-gap":16},{default:m(()=>[(h(!0),$(P,null,O(o.value,g=>(h(),k(t,{key:g.key},{default:m(()=>[p(R(_),{"gradient-color":d(g.color),class:"flex-1"},{default:m(()=>[v("h3",Ue,S(g.title),1),v("div",Je,[p(l,{icon:g.icon,class:"text-32px"},null,8,["icon"]),p(s,{prefix:g.unit==="¥"?g.unit:"",suffix:g.unit==="¥"?"":g.unit,"start-value":1,"end-value":g.value,class:"text-30px text-white dark:text-dark"},null,8,["prefix","suffix","end-value"])])]),_:2},1032,["gradient-color"])]),_:2},1024))),128))]),_:1})]),_:1})}}}),Xe={key:0,class:"space-y-3"},Ye={class:"flex-shrink-0 w-12 text-center"},Ze={class:"flex-1 ml-3"},et={class:"flex items-center justify-between"},tt={class:"font-semibold text-gray-900 truncate max-w-32"},at={class:"text-sm text-gray-500 truncate max-w-40"},nt={class:"text-right flex-shrink-0"},rt={class:"text-blue-600 font-semibold"},ot={class:"mt-1"},st={key:1,class:"h-60 flex items-center justify-center"},it={key:2,class:"h-60 flex items-center justify-center"},lt=z({name:"MerchantRanking",__name:"merchant-ranking",setup(a){H();const e=T([]),o=T(!1),f={0:{label:"未审核",type:"warning"},1:{label:"正常营业",type:"success"},2:{label:"暂停营业",type:"info"},3:{label:"停业",type:"error"}},_=async()=>{o.value=!0;try{console.log("=== 开始获取商家排行榜 ===");const t=await Re({size:10,status:1});console.log("商家排行榜API响应:",t);const n=t;let i=[];n?.data&&Array.isArray(n.data)?i=n.data:Array.isArray(n)?i=n:t&&Array.isArray(t)&&(i=t),console.log("解析后的商家数据:",i),e.value=i}catch(t){console.error("获取商家排行榜失败:",t),window.$message?.error("获取商家排行榜失败"),e.value=[]}finally{o.value=!1}},d=w(()=>!e.value||e.value.length===0?[]:[...e.value].sort((n,i)=>(i.OrdersCount||0)-(n.OrdersCount||0)).map((n,i)=>({...n,rank:i+1}))),r=t=>f[t]||{label:"未知",type:"error"},c=t=>t===0?"暂无订单":t>=1e4?`${(t/1e4).toFixed(1)}万单`:t>=1e3?`${(t/1e3).toFixed(1)}K单`:`${t}单`,l=t=>{switch(t){case 1:return"text-yellow-500 font-bold text-lg";case 2:return"text-gray-400 font-bold text-lg";case 3:return"text-orange-600 font-bold text-lg";default:return"text-gray-600 font-semibold"}},s=t=>{switch(t){case 1:return"🥇";case 2:return"🥈";case 3:return"🥉";default:return`${t}`}};return K(()=>{_()}),(t,n)=>(h(),k(R(B),{bordered:!1,class:"card-wrapper",title:"商家订单量排行榜"},{"header-extra":m(()=>[p(R(U),{type:"info",size:"small"},{default:m(()=>n[0]||(n[0]=[N("Top 10")])),_:1,__:[0]})]),default:m(()=>[p(R(X),{show:o.value},{default:m(()=>[d.value.length>0?(h(),$("div",Xe,[(h(!0),$(P,null,O(d.value,i=>(h(),$("div",{key:i.merchantId,class:"flex items-center p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"},[v("div",Ye,[v("span",{class:we(l(i.rank))},S(s(i.rank)),3)]),v("div",Ze,[v("div",et,[v("div",null,[v("h4",tt,S(i.merchantName),1),v("p",at," 📍 "+S(i.location),1)]),v("div",nt,[v("div",rt,S(c(i.OrdersCount||0)),1),v("div",ot,[p(R(U),{type:r(i.status).type,size:"small"},{default:m(()=>[N(S(r(i.status).label),1)]),_:2},1032,["type"])])])])])]))),128))])):o.value?(h(),$("div",it,n[2]||(n[2]=[v("div",{class:"text-gray-400"},"正在加载排行榜...",-1)]))):(h(),$("div",st,[p(R($e),{description:"暂无排行榜数据"},{icon:m(()=>n[1]||(n[1]=[v("div",{class:"text-6xl"},"📊",-1)])),_:1})]))]),_:1},8,["show"])]),_:1}))}}),ct=Se(lt,[["__scopeId","data-v-6fa23491"]]),dt="/merchant/assets/soybean-JC38yUrs.jpg",ut={class:"size-72px overflow-hidden rd-1/2"},ft=z({name:"SoybeanAvatar",__name:"soybean-avatar",setup(a){return(e,o)=>(h(),$("div",ut,o[0]||(o[0]=[v("img",{src:dt,class:"size-full"},null,-1)])))}}),vt={key:0,class:"flex justify-center items-center py-4"},mt={key:1,class:"text-center py-4"},gt={class:"text-red-500 mb-2"},pt={key:3,class:"text-center py-8 text-gray-500"},ht=z({name:"ProjectNews",__name:"project-news",setup(a){const e=T([]),o=T(!1),f=T(""),_=async()=>{try{o.value=!0,f.value="";const d=await Oe("merchant",{size:10,page:1});console.log("Full API Response:",d);let r=[];Array.isArray(d)?(r=d,console.log("Response is direct array:",r)):d&&typeof d=="object"&&(d.data&&d.data.data&&Array.isArray(d.data.data)?r=d.data.data:d.data&&Array.isArray(d.data)&&(r=d.data)),r&&Array.isArray(r)&&r.length>0?e.value=r.map((c,l)=>{const s=c.announceId||c.AnnounceId||`temp-${l}`,t=c.title||c.Title||"无标题",n=c.content||c.Content||t,i=c.startAt||c.StartAt,g=c.targetRole||c.TargetRole;return{id:s,content:n||t,title:t,time:i?new Date(i).toLocaleString("zh-CN",{year:"numeric",month:"2-digit",day:"2-digit",hour:"2-digit",minute:"2-digit",second:"2-digit",hour12:!1}):"无时间"}}):e.value=[]}catch{f.value="加载公告失败",e.value=[]}finally{o.value=!1}};return K(()=>{_()}),(d,r)=>{const c=X,l=ze,s=ft,t=Be,n=Te,i=Ne,g=B;return h(),k(g,{title:"平台资讯",bordered:!1,size:"small",segmented:"",class:"card-wrapper"},{"header-extra":m(()=>[v("a",{class:"text-primary",href:"javascript:;",onClick:_},"刷新")]),default:m(()=>[o.value?(h(),$("div",vt,[p(c,{size:"medium"}),r[0]||(r[0]=v("div",{class:"ml-2 text-gray-500"},"正在加载公告...",-1))])):f.value?(h(),$("div",mt,[v("div",gt,S(f.value),1),p(l,{onClick:_,size:"small",type:"primary"},{default:m(()=>r[1]||(r[1]=[N("重试")])),_:1,__:[1]})])):e.value.length>0?(h(),k(i,{key:2},{default:m(()=>[(h(!0),$(P,null,O(e.value,y=>(h(),k(n,{key:y.id},{prefix:m(()=>[p(s,{class:"size-48px!"})]),default:m(()=>[p(t,{title:y.title,description:`${y.content} - ${y.time}`},null,8,["title","description"])]),_:2},1024))),128))]),_:1})):(h(),$("div",pt,[r[3]||(r[3]=v("div",{class:"mb-2"},"暂无公告信息",-1)),r[4]||(r[4]=v("div",{class:"text-xs text-gray-400 mb-3"},[N(" • 检查数据库是否有 TargetRole='merchant' 或 null 的记录"),v("br"),N(" • 检查公告时间是否在有效期内 ")],-1)),p(l,{onClick:_,size:"small"},{default:m(()=>r[2]||(r[2]=[N("重新加载")])),_:1,__:[2]})]))]),_:1})}}}),wt=z({name:"home",__name:"index",setup(a){const e=H(),o=w(()=>e.isMobile?0:16);return(f,_)=>{const d=W,r=Q,c=q;return h(),k(c,{vertical:"",size:16},{default:m(()=>[p(We),p(qe),p(r,{"x-gap":o.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:m(()=>[p(d,{span:"24 s:24 m:14"},{default:m(()=>[p(ht)]),_:1}),p(d,{span:"24 s:24 m:10"},{default:m(()=>[p(ct)]),_:1})]),_:1},8,["x-gap"])]),_:1})}}});export{wt as default};
