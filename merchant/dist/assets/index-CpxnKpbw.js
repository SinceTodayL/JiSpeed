import{X as I,Y as x,aD as te,aE as ae,an as D,am as b,d as z,P as u,ad as F,aF as L,W as j,aG as ne,al as oe,a7 as re,a as w,ae as G,V as se,ac as le,aH as ie,aI as M,aJ as ce,aK as de,F as P,at as ue,m as H,aw as J,aj as K,c as k,o as h,K as B,w as g,f as p,e as v,b as $,s as fe,t as S,h as R,A as V,aL as ve,r as T,aM as me,aN as ge,p as pe,aO as he,aP as _e,J as xe,L as be,_ as ye,aQ as $e,g as N,aA as U,aR as we,y as Se,B as ze}from"./index-D6ldqbnU.js";import{f as Ce,a as ke,b as Re}from"./merchant-Z65Hux2M.js";import{a as W,_ as Q}from"./Grid-DrqdTGpB.js";import{N as q}from"./Space-VDiFdcNC.js";import{_ as X}from"./Spin-BLgfA2f8.js";const Ae=I([x("list",`
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
 `))]),Ie=Object.assign(Object.assign({},j.props),{size:{type:String,default:"medium"},bordered:Boolean,clickable:Boolean,hoverable:Boolean,showDivider:{type:Boolean,default:!0}}),Y=se("n-list"),Ne=z({name:"List",props:Ie,slots:Object,setup(a){const{mergedClsPrefixRef:e,inlineThemeDisabled:r,mergedRtlRef:f}=F(a),_=L("List",f,e),d=j("List","-list",Ae,ne,a,e);oe(Y,{showDividerRef:re(a,"showDivider"),mergedClsPrefixRef:e});const n=w(()=>{const{common:{cubicBezierEaseInOut:i},self:{fontSize:s,textColor:t,color:o,colorModal:l,colorPopover:m,borderColor:y,borderColorModal:C,borderColorPopover:A,borderRadius:O,colorHover:E,colorHoverModal:Z,colorHoverPopover:ee}}=d.value;return{"--n-font-size":s,"--n-bezier":i,"--n-text-color":t,"--n-color":o,"--n-border-radius":O,"--n-border-color":y,"--n-border-color-modal":C,"--n-border-color-popover":A,"--n-color-modal":l,"--n-color-popover":m,"--n-color-hover":E,"--n-color-hover-modal":Z,"--n-color-hover-popover":ee}}),c=r?G("list",void 0,n,a):void 0;return{mergedClsPrefix:e,rtlEnabled:_,cssVars:r?void 0:n,themeClass:c?.themeClass,onRender:c?.onRender}},render(){var a;const{$slots:e,mergedClsPrefix:r,onRender:f}=this;return f?.(),u("ul",{class:[`${r}-list`,this.rtlEnabled&&`${r}-list--rtl`,this.bordered&&`${r}-list--bordered`,this.showDivider&&`${r}-list--show-divider`,this.hoverable&&`${r}-list--hoverable`,this.clickable&&`${r}-list--clickable`,this.themeClass],style:this.cssVars},e.header?u("div",{class:`${r}-list__header`},e.header()):null,(a=e.default)===null||a===void 0?void 0:a.call(e),e.footer?u("div",{class:`${r}-list__footer`},e.footer()):null)}}),Te=z({name:"ListItem",slots:Object,setup(){const a=le(Y,null);return a||ie("list-item","`n-list-item` must be placed in `n-list`."),{showDivider:a.showDividerRef,mergedClsPrefix:a.mergedClsPrefixRef}},render(){const{$slots:a,mergedClsPrefix:e}=this;return u("li",{class:`${e}-list-item`},a.prefix?u("div",{class:`${e}-list-item__prefix`},a.prefix()):null,a.default?u("div",{class:`${e}-list-item__main`},a):null,a.suffix?u("div",{class:`${e}-list-item__suffix`},a.suffix()):null,this.showDivider&&u("div",{class:`${e}-list-item__divider`}))}}),je=x("statistic",[b("label",`
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
 `,[x("icon",{verticalAlign:"-0.125em"})])])]),De=Object.assign(Object.assign({},j.props),{tabularNums:Boolean,label:String,value:[String,Number]}),Pe=z({name:"Statistic",props:De,slots:Object,setup(a){const{mergedClsPrefixRef:e,inlineThemeDisabled:r,mergedRtlRef:f}=F(a),_=j("Statistic","-statistic",je,ce,a,e),d=L("Statistic",f,e),n=w(()=>{const{self:{labelFontWeight:i,valueFontSize:s,valueFontWeight:t,valuePrefixTextColor:o,labelTextColor:l,valueSuffixTextColor:m,valueTextColor:y,labelFontSize:C},common:{cubicBezierEaseInOut:A}}=_.value;return{"--n-bezier":A,"--n-label-font-size":C,"--n-label-font-weight":i,"--n-label-text-color":l,"--n-value-font-weight":t,"--n-value-font-size":s,"--n-value-prefix-text-color":o,"--n-value-suffix-text-color":m,"--n-value-text-color":y}}),c=r?G("statistic",void 0,n,a):void 0;return{rtlEnabled:d,mergedClsPrefix:e,cssVars:r?void 0:n,themeClass:c?.themeClass,onRender:c?.onRender}},render(){var a;const{mergedClsPrefix:e,$slots:{default:r,label:f,prefix:_,suffix:d}}=this;return(a=this.onRender)===null||a===void 0||a.call(this),u("div",{class:[`${e}-statistic`,this.themeClass,this.rtlEnabled&&`${e}-statistic--rtl`],style:this.cssVars},M(f,n=>u("div",{class:`${e}-statistic__label`},this.label||n)),u("div",{class:`${e}-statistic-value`,style:{fontVariantNumeric:this.tabularNums?"tabular-nums":""}},M(_,n=>n&&u("span",{class:`${e}-statistic-value__prefix`},n)),this.value!==void 0?u("span",{class:`${e}-statistic-value__content`},this.value):M(r,n=>n&&u("span",{class:`${e}-statistic-value__content`},n)),M(d,n=>n&&u("span",{class:`${e}-statistic-value__suffix`},n))))}}),Ee=x("thing",`
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
 `)])])]),Me=Object.assign(Object.assign({},j.props),{title:String,titleExtra:String,description:String,descriptionClass:String,descriptionStyle:[String,Object],content:String,contentClass:String,contentStyle:[String,Object],contentIndented:Boolean}),Be=z({name:"Thing",props:Me,slots:Object,setup(a,{slots:e}){const{mergedClsPrefixRef:r,inlineThemeDisabled:f,mergedRtlRef:_}=F(a),d=j("Thing","-thing",Ee,de,a,r),n=L("Thing",_,r),c=w(()=>{const{self:{titleTextColor:s,textColor:t,titleFontWeight:o,fontSize:l},common:{cubicBezierEaseInOut:m}}=d.value;return{"--n-bezier":m,"--n-font-size":l,"--n-text-color":t,"--n-title-font-weight":o,"--n-title-text-color":s}}),i=f?G("thing",void 0,c,a):void 0;return()=>{var s;const{value:t}=r,o=n?n.value:!1;return(s=i?.onRender)===null||s===void 0||s.call(i),u("div",{class:[`${t}-thing`,i?.themeClass,o&&`${t}-thing--rtl`],style:f?void 0:c.value},e.avatar&&a.contentIndented?u("div",{class:`${t}-thing-avatar`},e.avatar()):null,u("div",{class:`${t}-thing-main`},!a.contentIndented&&(e.header||a.title||e["header-extra"]||a.titleExtra||e.avatar)?u("div",{class:`${t}-thing-avatar-header-wrapper`},e.avatar?u("div",{class:`${t}-thing-avatar`},e.avatar()):null,e.header||a.title||e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header-wrapper`},u("div",{class:`${t}-thing-header`},e.header||a.title?u("div",{class:`${t}-thing-header__title`},e.header?e.header():a.title):null,e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header__extra`},e["header-extra"]?e["header-extra"]():a.titleExtra):null),e.description||a.description?u("div",{class:[`${t}-thing-main__description`,a.descriptionClass],style:a.descriptionStyle},e.description?e.description():a.description):null):null):u(P,null,e.header||a.title||e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header`},e.header||a.title?u("div",{class:`${t}-thing-header__title`},e.header?e.header():a.title):null,e["header-extra"]||a.titleExtra?u("div",{class:`${t}-thing-header__extra`},e["header-extra"]?e["header-extra"]():a.titleExtra):null):null,e.description||a.description?u("div",{class:[`${t}-thing-main__description`,a.descriptionClass],style:a.descriptionStyle},e.description?e.description():a.description):null),e.default||a.content?u("div",{class:[`${t}-thing-main__content`,a.contentClass],style:a.contentStyle},e.default?e.default():a.content):null,e.footer?u("div",{class:`${t}-thing-main__footer`},e.footer()):null,e.action?u("div",{class:`${t}-thing-main__action`},e.action()):null))}}});function Ve(a,e){const r=new URLSearchParams;e?.size!==void 0&&r.append("size",e.size.toString()),e?.page!==void 0&&r.append("page",e.page.toString());const f=r.toString(),_=`/api/announcements/activate/${a}${f?`?${f}`:""}`;return ue({url:_,method:"get"})}const Oe="/merchant/assets/avatar-C3nF7gXh.svg",Fe={class:"flex-y-center"},Le={class:"pl-12px"},Ge={class:"text-18px font-semibold"},He={class:"text-#999 leading-30px"},Ke={key:0,class:"text-#666 text-12px mt-1"},We=z({name:"HeaderBanner",__name:"header-banner",setup(a){const e=H(),r=J(),f=w(()=>e.isMobile?0:16),_=w(()=>{const{salesStats:i}=r;if(!i||i.length===0)return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:"¥0.00"},{id:2,label:"日均销量",value:"0份"}];i.reduce((t,o)=>t+(Number(o.salesQty)||0),0);const s=i.reduce((t,o)=>t+(Number(o.salesAmount)||0),0);return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:`¥${s.toFixed(2)}`}]}),d=w(()=>`你好，${r.merchantInfo?.merchantName||"商家"} !`),n=w(()=>{const i=r.merchantInfo?.status,s=r.merchantInfo?.location||"",t=r.merchantInfo?.contactInfo||"";let o="🟢 营业中";i===0?o="🔴 暂停营业":i&&i>0&&(o="🟢 营业中");const l=s?`🏢${s}`:"",m=t?`📞${t}`:"";return[o,l,m].filter(Boolean).join(" | ")}),c=async()=>{const{merchantId:i}=r;if(!i){console.log("merchantId is empty");return}try{const s=await Ce(i);console.log("fetchMerchantInfo result",s);let t=null;const o=s;o?.data?.data?(t=o.data.data,console.log("使用嵌套数据结构 result.data.data:",t)):o?.data?(t=o.data,console.log("使用直接数据结构 result.data:",t)):s&&typeof s=="object"&&(t=s,console.log("使用原始返回数据:",t)),t&&typeof t=="object"?(r.setMerchantInfo(t),console.log("merchantStore.merchantInfo",r.merchantInfo)):(console.error("未找到有效的商家数据"),console.log("完整API响应:",s),window.$message?.warning("获取到的商家信息格式不正确"))}catch(s){console.error("加载商家基本信息失败:",s),window.$message?.warning("暂时无法获取商家信息，请稍后刷新页面")}try{const s=new Date().toISOString(),t=new Date(Date.now()-30*24*60*60*1e3).toISOString(),o=await ke(i,{startTime:t,endTime:s});console.log("fetchMerchantSalesStats result",o),o&&Array.isArray(o.response.data.data)?(r.setSalesStats(o.response.data.data),console.log("成功加载销售统计数据:",o.response.data.data)):console.warn("销售统计数据格式异常:",o.response.data.data)}catch(s){console.error("加载商家销售数据失败:",s),window.$message?.error("获取商家销售数据失败")}};return K(()=>{r.triggerAuthUpdate(),c()}),(i,s)=>{const t=W,o=Pe,l=q,m=Q,y=B;return h(),k(y,{bordered:!1,class:"card-wrapper"},{default:g(()=>[p(m,{"x-gap":f.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:g(()=>[p(t,{span:"24 s:24 m:18"},{default:g(()=>[v("div",Fe,[s[0]||(s[0]=v("div",{class:"size-72px shrink-0 overflow-hidden rd-1/2"},[v("img",{src:Oe,class:"size-full"})],-1)),v("div",Le,[v("h3",Ge,S(d.value),1),v("p",He,S(n.value),1),R(r).merchantInfo?.contactInfo?(h(),$("p",Ke," 📞 "+S(R(r).merchantInfo.contactInfo),1)):fe("",!0)])])]),_:1}),p(t,{span:"24 s:24 m:6"},{default:g(()=>[p(l,{size:24,justify:"end"},{default:g(()=>[(h(!0),$(P,null,V(_.value,C=>(h(),k(o,ve({key:C.id,class:"whitespace-nowrap"},{ref_for:!0},C),null,16))),128))]),_:1})]),_:1})]),_:1},8,["x-gap"])]),_:1})}}}),Qe=z({name:"CountTo",__name:"count-to",props:{startValue:{default:0},endValue:{default:2021},duration:{default:1500},autoplay:{type:Boolean,default:!0},decimals:{default:0},prefix:{default:""},suffix:{default:""},separator:{default:","},decimal:{default:"."},useEasing:{type:Boolean,default:!0},transition:{default:"linear"}},setup(a){const e=a,r=T(e.startValue),f=w(()=>e.useEasing?me[e.transition]:void 0),_=ge(r,{disabled:!1,duration:e.duration,transition:f.value}),d=w(()=>n(_.value));function n(i){const{decimals:s,decimal:t,separator:o,suffix:l,prefix:m}=e;let y=i.toFixed(s);y=String(y);const C=y.split(".");let A=C[0];const O=C.length>1?t+C[1]:"",E=/(\d+)(\d{3})/;if(o)for(;E.test(A);)A=A.replace(E,`$1${o}$2`);return m+A+O+l}async function c(){await he(),r.value=e.endValue}return pe([()=>e.startValue,()=>e.endValue],()=>{e.autoplay&&c()},{immediate:!0}),(i,s)=>(h(),$("span",null,S(d.value),1))}}),Ue={class:"text-16px"},Je={class:"flex justify-between pt-12px"},qe=z({name:"CardData",__name:"card-data",setup(a){const e=J(),r=w(()=>{const{salesStats:n}=e;if(!n||n.length===0)return[{key:"totalSales",title:"总销量",value:0,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:0,unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:0,unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"maxDaySales",title:"单日最高销量",value:0,unit:"份",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}];const c=n.reduce((o,l)=>o+(Number(l.salesQty)||0),0),i=n.reduce((o,l)=>o+(Number(l.salesAmount)||0),0),s=i/7;return n.reduce((o,l)=>Math.max(o,Number(l.salesQty)||0),0),[{key:"totalSales",title:"总销量",value:c,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:Math.round(i),unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:Math.round(s),unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"goodRate",title:"好评率",value:100,unit:"%",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}]}),[f,_]=_e();function d(n){return`linear-gradient(to bottom right, ${n.start}, ${n.end})`}return(n,c)=>{const i=ye,s=Qe,t=W,o=Q,l=B;return h(),k(l,{bordered:!1,size:"small",class:"card-wrapper"},{default:g(()=>[p(R(f),null,{default:g(({$slots:m,gradientColor:y})=>[v("div",{class:"rd-8px px-16px pb-4px pt-8px text-white",style:be({backgroundImage:y})},[(h(),k(xe(m.default)))],4)]),_:1}),p(o,{cols:"s:1 m:2 l:4",responsive:"screen","x-gap":16,"y-gap":16},{default:g(()=>[(h(!0),$(P,null,V(r.value,m=>(h(),k(t,{key:m.key},{default:g(()=>[p(R(_),{"gradient-color":d(m.color),class:"flex-1"},{default:g(()=>[v("h3",Ue,S(m.title),1),v("div",Je,[p(i,{icon:m.icon,class:"text-32px"},null,8,["icon"]),p(s,{prefix:m.unit==="¥"?m.unit:"",suffix:m.unit==="¥"?"":m.unit,"start-value":1,"end-value":m.value,class:"text-30px text-white dark:text-dark"},null,8,["prefix","suffix","end-value"])])]),_:2},1032,["gradient-color"])]),_:2},1024))),128))]),_:1})]),_:1})}}}),Xe={key:0,class:"space-y-3"},Ye={class:"flex-shrink-0 w-12 text-center"},Ze={class:"flex-1 ml-3"},et={class:"flex items-center justify-between"},tt={class:"font-semibold text-gray-900 truncate max-w-32"},at={class:"text-sm text-gray-500 truncate max-w-40"},nt={class:"text-right flex-shrink-0"},ot={class:"text-blue-600 font-semibold"},rt={class:"mt-1"},st={key:1,class:"h-60 flex items-center justify-center"},lt={key:2,class:"h-60 flex items-center justify-center"},it=z({name:"MerchantRanking",__name:"merchant-ranking",setup(a){H();const e=T([]),r=T(!1),f={0:{label:"未审核",type:"warning"},1:{label:"正常营业",type:"success"},2:{label:"暂停营业",type:"info"},3:{label:"停业",type:"error"}},_=async()=>{r.value=!0;try{console.log("=== 开始获取商家排行榜 ===");const t=await Re({size:10,status:1});console.log("商家排行榜API响应:",t);const o=t;let l=[];o?.data&&Array.isArray(o.data)?l=o.data:Array.isArray(o)?l=o:t&&Array.isArray(t)&&(l=t),console.log("解析后的商家数据:",l),e.value=l}catch(t){console.error("获取商家排行榜失败:",t),window.$message?.error("获取商家排行榜失败"),e.value=[]}finally{r.value=!1}},d=w(()=>!e.value||e.value.length===0?[]:[...e.value].sort((o,l)=>(l.ordersCount||0)-(o.ordersCount||0)).map((o,l)=>({...o,rank:l+1}))),n=t=>f[t]||{label:"未知",type:"error"},c=t=>t===0?"暂无订单":t>=1e4?`${(t/1e4).toFixed(1)}万单`:t>=1e3?`${(t/1e3).toFixed(1)}K单`:`${t}单`,i=t=>{switch(t){case 1:return"text-yellow-500 font-bold text-lg";case 2:return"text-gray-400 font-bold text-lg";case 3:return"text-orange-600 font-bold text-lg";default:return"text-gray-600 font-semibold"}},s=t=>{switch(t){case 1:return"🥇";case 2:return"🥈";case 3:return"🥉";default:return`${t}`}};return K(()=>{_()}),(t,o)=>(h(),k(R(B),{bordered:!1,class:"card-wrapper",title:"商家订单量排行榜"},{"header-extra":g(()=>[p(R(U),{type:"info",size:"small"},{default:g(()=>o[0]||(o[0]=[N("Top 10")])),_:1,__:[0]})]),default:g(()=>[p(R(X),{show:r.value},{default:g(()=>[d.value.length>0?(h(),$("div",Xe,[(h(!0),$(P,null,V(d.value,l=>(h(),$("div",{key:l.merchantId,class:"flex items-center p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"},[v("div",Ye,[v("span",{class:we(i(l.rank))},S(s(l.rank)),3)]),v("div",Ze,[v("div",et,[v("div",null,[v("h4",tt,S(l.merchantName),1),v("p",at," 📍 "+S(l.location),1)]),v("div",nt,[v("div",ot,S(c(l.ordersCount||0)),1),v("div",rt,[p(R(U),{type:n(l.status).type,size:"small"},{default:g(()=>[N(S(n(l.status).label),1)]),_:2},1032,["type"])])])])])]))),128))])):r.value?(h(),$("div",lt,o[2]||(o[2]=[v("div",{class:"text-gray-400"},"正在加载排行榜...",-1)]))):(h(),$("div",st,[p(R($e),{description:"暂无排行榜数据"},{icon:g(()=>o[1]||(o[1]=[v("div",{class:"text-6xl"},"📊",-1)])),_:1})]))]),_:1},8,["show"])]),_:1}))}}),ct=Se(it,[["__scopeId","data-v-0bb4e746"]]),dt="/merchant/assets/soybean-JC38yUrs.jpg",ut={class:"size-72px overflow-hidden rd-1/2"},ft=z({name:"SoybeanAvatar",__name:"soybean-avatar",setup(a){return(e,r)=>(h(),$("div",ut,r[0]||(r[0]=[v("img",{src:dt,class:"size-full"},null,-1)])))}}),vt={key:0,class:"flex justify-center items-center py-4"},mt={key:1,class:"text-center py-4"},gt={class:"text-red-500 mb-2"},pt={key:3,class:"text-center py-8 text-gray-500"},ht=z({name:"ProjectNews",__name:"project-news",setup(a){const e=T([]),r=T(!1),f=T(""),_=async()=>{try{r.value=!0,f.value="";const d=await Ve("merchant",{size:10,page:1});console.log("Full API Response:",d);let n=[];Array.isArray(d)?(n=d,console.log("Response is direct array:",n)):d&&typeof d=="object"&&(d.data&&d.data.data&&Array.isArray(d.data.data)?(n=d.data.data,console.log("Extracted data from response.data.data:",n)):d.data&&Array.isArray(d.data)?(n=d.data,console.log("Extracted data from response.data:",n)):console.log("Could not find announcements array in response:",d)),n&&Array.isArray(n)&&n.length>0?(console.log("Processing announcements:",n.length),e.value=n.map((c,i)=>{console.log(`Announcement ${i}:`,c);const s=c.announceId||c.AnnounceId||`temp-${i}`,t=c.title||c.Title||"无标题",o=c.content||c.Content||t,l=c.startAt||c.StartAt,m=c.targetRole||c.TargetRole;return console.log(`Processing: ID=${s}, Title=${t}, TargetRole=${m}, StartAt=${l}`),{id:s,content:o||t,title:t,time:l?new Date(l).toLocaleString("zh-CN",{year:"numeric",month:"2-digit",day:"2-digit",hour:"2-digit",minute:"2-digit",second:"2-digit",hour12:!1}):"无时间"}}),console.log("Successfully processed announcements:",e.value)):(console.log("No announcements found or invalid data structure:",n),e.value=[])}catch(d){f.value="加载公告失败",console.error("Error fetching announcements:",d),e.value=[]}finally{r.value=!1}};return K(()=>{_()}),(d,n)=>{const c=X,i=ze,s=ft,t=Be,o=Te,l=Ne,m=B;return h(),k(m,{title:"平台资讯",bordered:!1,size:"small",segmented:"",class:"card-wrapper"},{"header-extra":g(()=>[v("a",{class:"text-primary",href:"javascript:;",onClick:_},"刷新")]),default:g(()=>[r.value?(h(),$("div",vt,[p(c,{size:"medium"}),n[0]||(n[0]=v("div",{class:"ml-2 text-gray-500"},"正在加载公告...",-1))])):f.value?(h(),$("div",mt,[v("div",gt,S(f.value),1),p(i,{onClick:_,size:"small",type:"primary"},{default:g(()=>n[1]||(n[1]=[N("重试")])),_:1,__:[1]})])):e.value.length>0?(h(),k(l,{key:2},{default:g(()=>[(h(!0),$(P,null,V(e.value,y=>(h(),k(o,{key:y.id},{prefix:g(()=>[p(s,{class:"size-48px!"})]),default:g(()=>[p(t,{title:y.title,description:`${y.content} - ${y.time}`},null,8,["title","description"])]),_:2},1024))),128))]),_:1})):(h(),$("div",pt,[n[3]||(n[3]=v("div",{class:"mb-2"},"暂无公告信息",-1)),n[4]||(n[4]=v("div",{class:"text-xs text-gray-400 mb-3"},[N(" • 检查数据库是否有 TargetRole='merchant' 或 null 的记录"),v("br"),N(" • 检查公告时间是否在有效期内 ")],-1)),p(i,{onClick:_,size:"small"},{default:g(()=>n[2]||(n[2]=[N("重新加载")])),_:1,__:[2]})]))]),_:1})}}}),wt=z({name:"home",__name:"index",setup(a){const e=H(),r=w(()=>e.isMobile?0:16);return(f,_)=>{const d=W,n=Q,c=q;return h(),k(c,{vertical:"",size:16},{default:g(()=>[p(We),p(qe),p(n,{"x-gap":r.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:g(()=>[p(d,{span:"24 s:24 m:14"},{default:g(()=>[p(ht)]),_:1}),p(d,{span:"24 s:24 m:10"},{default:g(()=>[p(ct)]),_:1})]),_:1},8,["x-gap"])]),_:1})}}});export{wt as default};
