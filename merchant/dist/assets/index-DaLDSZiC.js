import{X as N,Y as b,aD as te,aE as ae,an as P,am as $,d as k,P as m,ad as F,aF as L,W as M,aG as ne,al as oe,a7 as re,a as z,ae as G,V as se,ac as le,aH as ie,aI as B,aJ as ce,aK as de,F as j,at as ue,m as H,aw as J,aj as W,c as R,o as _,K as V,w as h,f as p,e as g,b as S,s as fe,t as C,h as I,A as O,aL as me,r as T,aM as ve,aN as ge,p as he,aO as pe,aP as _e,J as xe,L as be,_ as ye,aQ as $e,g as D,aA as U,aR as we,y as Se,B as ze}from"./index-CmHymj64.js";import{f as Ce,a as ke,b as Re,c as Ae}from"./merchant-DIPCyJvq.js";import{a as K,_ as Q}from"./Grid-BAEOqfoE.js";import{N as q}from"./Space-DdW0Hcsn.js";import{_ as X}from"./Spin-DgLu0RLm.js";const Ie=N([b("list",`
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
 `,[P("show-divider",[b("list-item",[N("&:not(:last-child)",[$("divider",`
 background-color: var(--n-merged-border-color);
 `)])])]),P("clickable",[b("list-item",`
 cursor: pointer;
 `)]),P("bordered",`
 border: 1px solid var(--n-merged-border-color);
 border-radius: var(--n-border-radius);
 `),P("hoverable",[b("list-item",`
 border-radius: var(--n-border-radius);
 `,[N("&:hover",`
 background-color: var(--n-merged-color-hover);
 `,[$("divider",`
 background-color: transparent;
 `)])])]),P("bordered, hoverable",[b("list-item",`
 padding: 12px 20px;
 `),$("header, footer",`
 padding: 12px 20px;
 `)]),$("header, footer",`
 padding: 12px 0;
 box-sizing: border-box;
 transition: border-color .3s var(--n-bezier);
 `,[N("&:not(:last-child)",`
 border-bottom: 1px solid var(--n-merged-border-color);
 `)]),b("list-item",`
 position: relative;
 padding: 12px 0; 
 box-sizing: border-box;
 display: flex;
 flex-wrap: nowrap;
 align-items: center;
 transition:
 background-color .3s var(--n-bezier),
 border-color .3s var(--n-bezier);
 `,[$("prefix",`
 margin-right: 20px;
 flex: 0;
 `),$("suffix",`
 margin-left: 20px;
 flex: 0;
 `),$("main",`
 flex: 1;
 `),$("divider",`
 height: 1px;
 position: absolute;
 bottom: 0;
 left: 0;
 right: 0;
 background-color: transparent;
 transition: background-color .3s var(--n-bezier);
 pointer-events: none;
 `)])]),te(b("list",`
 --n-merged-color-hover: var(--n-color-hover-modal);
 --n-merged-color: var(--n-color-modal);
 --n-merged-border-color: var(--n-border-color-modal);
 `)),ae(b("list",`
 --n-merged-color-hover: var(--n-color-hover-popover);
 --n-merged-color: var(--n-color-popover);
 --n-merged-border-color: var(--n-border-color-popover);
 `))]),Ne=Object.assign(Object.assign({},M.props),{size:{type:String,default:"medium"},bordered:Boolean,clickable:Boolean,hoverable:Boolean,showDivider:{type:Boolean,default:!0}}),Y=se("n-list"),Te=k({name:"List",props:Ne,slots:Object,setup(t){const{mergedClsPrefixRef:e,inlineThemeDisabled:o,mergedRtlRef:v}=F(t),x=L("List",v,e),f=M("List","-list",Ie,ne,t,e);oe(Y,{showDividerRef:re(t,"showDivider"),mergedClsPrefixRef:e});const a=z(()=>{const{common:{cubicBezierEaseInOut:c},self:{fontSize:i,textColor:n,color:l,colorModal:s,colorPopover:r,borderColor:d,borderColorModal:y,borderColorPopover:w,borderRadius:A,colorHover:E,colorHoverModal:Z,colorHoverPopover:ee}}=f.value;return{"--n-font-size":i,"--n-bezier":c,"--n-text-color":n,"--n-color":l,"--n-border-radius":A,"--n-border-color":d,"--n-border-color-modal":y,"--n-border-color-popover":w,"--n-color-modal":s,"--n-color-popover":r,"--n-color-hover":E,"--n-color-hover-modal":Z,"--n-color-hover-popover":ee}}),u=o?G("list",void 0,a,t):void 0;return{mergedClsPrefix:e,rtlEnabled:x,cssVars:o?void 0:a,themeClass:u?.themeClass,onRender:u?.onRender}},render(){var t;const{$slots:e,mergedClsPrefix:o,onRender:v}=this;return v?.(),m("ul",{class:[`${o}-list`,this.rtlEnabled&&`${o}-list--rtl`,this.bordered&&`${o}-list--bordered`,this.showDivider&&`${o}-list--show-divider`,this.hoverable&&`${o}-list--hoverable`,this.clickable&&`${o}-list--clickable`,this.themeClass],style:this.cssVars},e.header?m("div",{class:`${o}-list__header`},e.header()):null,(t=e.default)===null||t===void 0?void 0:t.call(e),e.footer?m("div",{class:`${o}-list__footer`},e.footer()):null)}}),De=k({name:"ListItem",slots:Object,setup(){const t=le(Y,null);return t||ie("list-item","`n-list-item` must be placed in `n-list`."),{showDivider:t.showDividerRef,mergedClsPrefix:t.mergedClsPrefixRef}},render(){const{$slots:t,mergedClsPrefix:e}=this;return m("li",{class:`${e}-list-item`},t.prefix?m("div",{class:`${e}-list-item__prefix`},t.prefix()):null,t.default?m("div",{class:`${e}-list-item__main`},t):null,t.suffix?m("div",{class:`${e}-list-item__suffix`},t.suffix()):null,this.showDivider&&m("div",{class:`${e}-list-item__divider`}))}}),Me=b("statistic",[$("label",`
 font-weight: var(--n-label-font-weight);
 transition: .3s color var(--n-bezier);
 font-size: var(--n-label-font-size);
 color: var(--n-label-text-color);
 `),b("statistic-value",`
 margin-top: 4px;
 font-weight: var(--n-value-font-weight);
 `,[$("prefix",`
 margin: 0 4px 0 0;
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-prefix-text-color);
 `,[b("icon",{verticalAlign:"-0.125em"})]),$("content",`
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-text-color);
 `),$("suffix",`
 margin: 0 0 0 4px;
 font-size: var(--n-value-font-size);
 transition: .3s color var(--n-bezier);
 color: var(--n-value-suffix-text-color);
 `,[b("icon",{verticalAlign:"-0.125em"})])])]),Pe=Object.assign(Object.assign({},M.props),{tabularNums:Boolean,label:String,value:[String,Number]}),je=k({name:"Statistic",props:Pe,slots:Object,setup(t){const{mergedClsPrefixRef:e,inlineThemeDisabled:o,mergedRtlRef:v}=F(t),x=M("Statistic","-statistic",Me,ce,t,e),f=L("Statistic",v,e),a=z(()=>{const{self:{labelFontWeight:c,valueFontSize:i,valueFontWeight:n,valuePrefixTextColor:l,labelTextColor:s,valueSuffixTextColor:r,valueTextColor:d,labelFontSize:y},common:{cubicBezierEaseInOut:w}}=x.value;return{"--n-bezier":w,"--n-label-font-size":y,"--n-label-font-weight":c,"--n-label-text-color":s,"--n-value-font-weight":n,"--n-value-font-size":i,"--n-value-prefix-text-color":l,"--n-value-suffix-text-color":r,"--n-value-text-color":d}}),u=o?G("statistic",void 0,a,t):void 0;return{rtlEnabled:f,mergedClsPrefix:e,cssVars:o?void 0:a,themeClass:u?.themeClass,onRender:u?.onRender}},render(){var t;const{mergedClsPrefix:e,$slots:{default:o,label:v,prefix:x,suffix:f}}=this;return(t=this.onRender)===null||t===void 0||t.call(this),m("div",{class:[`${e}-statistic`,this.themeClass,this.rtlEnabled&&`${e}-statistic--rtl`],style:this.cssVars},B(v,a=>m("div",{class:`${e}-statistic__label`},this.label||a)),m("div",{class:`${e}-statistic-value`,style:{fontVariantNumeric:this.tabularNums?"tabular-nums":""}},B(x,a=>a&&m("span",{class:`${e}-statistic-value__prefix`},a)),this.value!==void 0?m("span",{class:`${e}-statistic-value__content`},this.value):B(o,a=>a&&m("span",{class:`${e}-statistic-value__content`},a)),B(f,a=>a&&m("span",{class:`${e}-statistic-value__suffix`},a))))}}),Ee=b("thing",`
 display: flex;
 transition: color .3s var(--n-bezier);
 font-size: var(--n-font-size);
 color: var(--n-text-color);
`,[b("thing-avatar",`
 margin-right: 12px;
 margin-top: 2px;
 `),b("thing-avatar-header-wrapper",`
 display: flex;
 flex-wrap: nowrap;
 `,[b("thing-header-wrapper",`
 flex: 1;
 `)]),b("thing-main",`
 flex-grow: 1;
 `,[b("thing-header",`
 display: flex;
 margin-bottom: 4px;
 justify-content: space-between;
 align-items: center;
 `,[$("title",`
 font-size: 16px;
 font-weight: var(--n-title-font-weight);
 transition: color .3s var(--n-bezier);
 color: var(--n-title-text-color);
 `)]),$("description",[N("&:not(:last-child)",`
 margin-bottom: 4px;
 `)]),$("content",[N("&:not(:first-child)",`
 margin-top: 12px;
 `)]),$("footer",[N("&:not(:first-child)",`
 margin-top: 12px;
 `)]),$("action",[N("&:not(:first-child)",`
 margin-top: 12px;
 `)])])]),Be=Object.assign(Object.assign({},M.props),{title:String,titleExtra:String,description:String,descriptionClass:String,descriptionStyle:[String,Object],content:String,contentClass:String,contentStyle:[String,Object],contentIndented:Boolean}),Ve=k({name:"Thing",props:Be,slots:Object,setup(t,{slots:e}){const{mergedClsPrefixRef:o,inlineThemeDisabled:v,mergedRtlRef:x}=F(t),f=M("Thing","-thing",Ee,de,t,o),a=L("Thing",x,o),u=z(()=>{const{self:{titleTextColor:i,textColor:n,titleFontWeight:l,fontSize:s},common:{cubicBezierEaseInOut:r}}=f.value;return{"--n-bezier":r,"--n-font-size":s,"--n-text-color":n,"--n-title-font-weight":l,"--n-title-text-color":i}}),c=v?G("thing",void 0,u,t):void 0;return()=>{var i;const{value:n}=o,l=a?a.value:!1;return(i=c?.onRender)===null||i===void 0||i.call(c),m("div",{class:[`${n}-thing`,c?.themeClass,l&&`${n}-thing--rtl`],style:v?void 0:u.value},e.avatar&&t.contentIndented?m("div",{class:`${n}-thing-avatar`},e.avatar()):null,m("div",{class:`${n}-thing-main`},!t.contentIndented&&(e.header||t.title||e["header-extra"]||t.titleExtra||e.avatar)?m("div",{class:`${n}-thing-avatar-header-wrapper`},e.avatar?m("div",{class:`${n}-thing-avatar`},e.avatar()):null,e.header||t.title||e["header-extra"]||t.titleExtra?m("div",{class:`${n}-thing-header-wrapper`},m("div",{class:`${n}-thing-header`},e.header||t.title?m("div",{class:`${n}-thing-header__title`},e.header?e.header():t.title):null,e["header-extra"]||t.titleExtra?m("div",{class:`${n}-thing-header__extra`},e["header-extra"]?e["header-extra"]():t.titleExtra):null),e.description||t.description?m("div",{class:[`${n}-thing-main__description`,t.descriptionClass],style:t.descriptionStyle},e.description?e.description():t.description):null):null):m(j,null,e.header||t.title||e["header-extra"]||t.titleExtra?m("div",{class:`${n}-thing-header`},e.header||t.title?m("div",{class:`${n}-thing-header__title`},e.header?e.header():t.title):null,e["header-extra"]||t.titleExtra?m("div",{class:`${n}-thing-header__extra`},e["header-extra"]?e["header-extra"]():t.titleExtra):null):null,e.description||t.description?m("div",{class:[`${n}-thing-main__description`,t.descriptionClass],style:t.descriptionStyle},e.description?e.description():t.description):null),e.default||t.content?m("div",{class:[`${n}-thing-main__content`,t.contentClass],style:t.contentStyle},e.default?e.default():t.content):null,e.footer?m("div",{class:`${n}-thing-main__footer`},e.footer()):null,e.action?m("div",{class:`${n}-thing-main__action`},e.action()):null))}}});function Oe(t,e){const o=new URLSearchParams;e?.size!==void 0&&o.append("size",e.size.toString()),e?.page!==void 0&&o.append("page",e.page.toString());const v=o.toString(),x=`/api/announcements/activate/${t}${v?`?${v}`:""}`;return ue({url:x,method:"get"})}const Fe="/merchant/assets/avatar-C3nF7gXh.svg",Le={class:"flex-y-center"},Ge={class:"pl-12px"},He={class:"text-18px font-semibold"},We={class:"text-#999 leading-30px"},Ke={key:0,class:"text-#666 text-12px mt-1"},Qe=k({name:"HeaderBanner",__name:"header-banner",setup(t){const e=H(),o=J(),v=z(()=>e.isMobile?0:16),x=z(()=>{const{salesStats:c}=o;if(!c||c.length===0)return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:"¥0.00"},{id:2,label:"日均销量",value:"0份"}];c.reduce((n,l)=>n+(Number(l.salesQty)||0),0);const i=c.reduce((n,l)=>n+(Number(l.salesAmount)||0),0);return[{id:0,label:"统计周期",value:"最近30天"},{id:1,label:"总销售额",value:`¥${i.toFixed(2)}`}]}),f=z(()=>`你好，${o.merchantInfo?.merchantName||"商家"} !`),a=z(()=>{const c=o.merchantInfo?.status,i=o.merchantInfo?.location||"",n=o.merchantInfo?.contactInfo||"";let l="🟢 营业中";c===0?l="🔴 暂停营业":c&&c>0&&(l="🟢 营业中");const s=i?`🏢${i}`:"",r=n?`📞${n}`:"";return[l,s,r].filter(Boolean).join(" | ")}),u=async()=>{const{merchantId:c}=o;if(!c){console.log("merchantId is empty");return}try{const i=await Ce(c);console.log("fetchMerchantInfo result",i);let n=null;const l=i;l?.data?.data?(n=l.data.data,console.log("使用嵌套数据结构 result.data.data:",n)):l?.data?(n=l.data,console.log("使用直接数据结构 result.data:",n)):i&&typeof i=="object"&&(n=i,console.log("使用原始返回数据:",n)),n&&typeof n=="object"?(o.setMerchantInfo(n),console.log("merchantStore.merchantInfo",o.merchantInfo)):(console.error("未找到有效的商家数据"),console.log("完整API响应:",i),window.$message?.warning("获取到的商家信息格式不正确"))}catch(i){console.error("加载商家基本信息失败:",i),window.$message?.warning("暂时无法获取商家信息，请稍后刷新页面")}try{const i=new Date().toISOString(),n=new Date(Date.now()-30*24*60*60*1e3).toISOString(),l=await ke(c,{startTime:n,endTime:i});console.log("fetchMerchantSalesStats result",l),l&&Array.isArray(l.response.data.data)?(o.setSalesStats(l.response.data.data),console.log("成功加载销售统计数据:",l.response.data.data)):console.warn("销售统计数据格式异常:",l.response.data.data)}catch(i){console.error("加载商家销售数据失败:",i),window.$message?.error("获取商家销售数据失败")}};return W(()=>{o.triggerAuthUpdate(),u()}),(c,i)=>{const n=K,l=je,s=q,r=Q,d=V;return _(),R(d,{bordered:!1,class:"card-wrapper"},{default:h(()=>[p(r,{"x-gap":v.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:h(()=>[p(n,{span:"24 s:24 m:18"},{default:h(()=>[g("div",Le,[i[0]||(i[0]=g("div",{class:"size-72px shrink-0 overflow-hidden rd-1/2"},[g("img",{src:Fe,class:"size-full"})],-1)),g("div",Ge,[g("h3",He,C(f.value),1),g("p",We,C(a.value),1),I(o).merchantInfo?.contactInfo?(_(),S("p",Ke," 📞 "+C(I(o).merchantInfo.contactInfo),1)):fe("",!0)])])]),_:1}),p(n,{span:"24 s:24 m:6"},{default:h(()=>[p(s,{size:24,justify:"end"},{default:h(()=>[(_(!0),S(j,null,O(x.value,y=>(_(),R(l,me({key:y.id,class:"whitespace-nowrap"},{ref_for:!0},y),null,16))),128))]),_:1})]),_:1})]),_:1},8,["x-gap"])]),_:1})}}}),Ue=k({name:"CountTo",__name:"count-to",props:{startValue:{default:0},endValue:{default:2021},duration:{default:1500},autoplay:{type:Boolean,default:!0},decimals:{default:0},prefix:{default:""},suffix:{default:""},separator:{default:","},decimal:{default:"."},useEasing:{type:Boolean,default:!0},transition:{default:"linear"}},setup(t){const e=t,o=T(e.startValue),v=z(()=>e.useEasing?ve[e.transition]:void 0),x=ge(o,{disabled:!1,duration:e.duration,transition:v.value}),f=z(()=>a(x.value));function a(c){const{decimals:i,decimal:n,separator:l,suffix:s,prefix:r}=e;let d=c.toFixed(i);d=String(d);const y=d.split(".");let w=y[0];const A=y.length>1?n+y[1]:"",E=/(\d+)(\d{3})/;if(l)for(;E.test(w);)w=w.replace(E,`$1${l}$2`);return r+w+A+s}async function u(){await pe(),o.value=e.endValue}return he([()=>e.startValue,()=>e.endValue],()=>{e.autoplay&&u()},{immediate:!0}),(c,i)=>(_(),S("span",null,C(f.value),1))}}),Je={class:"text-16px"},qe={class:"flex justify-between pt-12px"},Xe=k({name:"CardData",__name:"card-data",setup(t){const e=J(),o=z(()=>{const{salesStats:a}=e;if(!a||a.length===0)return[{key:"totalSales",title:"总销量",value:0,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:0,unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:0,unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"maxDaySales",title:"单日最高销量",value:0,unit:"份",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}];const u=a.reduce((l,s)=>l+(Number(s.salesQty)||0),0),c=a.reduce((l,s)=>l+(Number(s.salesAmount)||0),0),i=c/7;return a.reduce((l,s)=>Math.max(l,Number(s.salesQty)||0),0),[{key:"totalSales",title:"总销量",value:u,unit:"份",color:{start:"#ec4786",end:"#b955a4"},icon:"mdi:food"},{key:"totalRevenue",title:"总营收",value:Math.round(c),unit:"¥",color:{start:"#865ec0",end:"#5144b4"},icon:"ant-design:money-collect-outlined"},{key:"avgDailyRevenue",title:"日均营收",value:Math.round(i),unit:"¥",color:{start:"#56cdf3",end:"#719de3"},icon:"mdi:trending-up"},{key:"goodRate",title:"好评率",value:100,unit:"%",color:{start:"#fcbc25",end:"#f68057"},icon:"mdi:crown"}]}),[v,x]=_e();function f(a){return`linear-gradient(to bottom right, ${a.start}, ${a.end})`}return(a,u)=>{const c=ye,i=Ue,n=K,l=Q,s=V;return _(),R(s,{bordered:!1,size:"small",class:"card-wrapper"},{default:h(()=>[p(I(v),null,{default:h(({$slots:r,gradientColor:d})=>[g("div",{class:"rd-8px px-16px pb-4px pt-8px text-white",style:be({backgroundImage:d})},[(_(),R(xe(r.default)))],4)]),_:1}),p(l,{cols:"s:1 m:2 l:4",responsive:"screen","x-gap":16,"y-gap":16},{default:h(()=>[(_(!0),S(j,null,O(o.value,r=>(_(),R(n,{key:r.key},{default:h(()=>[p(I(x),{"gradient-color":f(r.color),class:"flex-1"},{default:h(()=>[g("h3",Je,C(r.title),1),g("div",qe,[p(c,{icon:r.icon,class:"text-32px"},null,8,["icon"]),p(i,{prefix:r.unit==="¥"?r.unit:"",suffix:r.unit==="¥"?"":r.unit,"start-value":1,"end-value":r.value,class:"text-30px text-white dark:text-dark"},null,8,["prefix","suffix","end-value"])])]),_:2},1032,["gradient-color"])]),_:2},1024))),128))]),_:1})]),_:1})}}}),Ye={key:0,class:"space-y-3"},Ze={class:"flex-shrink-0 w-12 text-center"},et={class:"flex-1 ml-3"},tt={class:"flex items-center justify-between"},at={class:"font-semibold text-gray-900 truncate max-w-32"},nt={class:"text-sm text-gray-500 truncate max-w-40"},ot={class:"text-right flex-shrink-0"},rt={class:"text-green-600 font-semibold"},st={class:"mt-1"},lt={key:1,class:"h-60 flex items-center justify-center"},it={key:2,class:"h-60 flex items-center justify-center"},ct=k({name:"MerchantRanking",__name:"merchant-ranking",setup(t){H();const e=T([]),o=T(!1),v=T(new Map),x={0:{label:"未审核",type:"warning"},1:{label:"正常营业",type:"success"},2:{label:"暂停营业",type:"info"},3:{label:"停业",type:"error"}},f=async()=>{o.value=!0;try{console.log("=== 开始获取商家排行榜 ===");const s=await Re({size:10,status:1});console.log("商家排行榜API响应:",s);const r=s;let d=[];r?.data&&Array.isArray(r.data)?d=r.data:Array.isArray(r)?d=r:s&&Array.isArray(s)&&(d=s),console.log("解析后的商家数据:",d),e.value=d,d.length>0&&await a(d)}catch(s){console.error("获取商家排行榜失败:",s),window.$message?.error("获取商家排行榜失败"),e.value=[]}finally{o.value=!1}},a=async s=>{try{console.log("=== 开始批量获取商家销售数据 ===");const r=s.map(w=>Ae(w.merchantId).then(A=>({merchantId:w.merchantId,salesAmount:A})).catch(A=>(console.warn(`获取商家${w.merchantId}销售数据失败:`,A),{merchantId:w.merchantId,salesAmount:0}))),d=await Promise.all(r);console.log("批量获取销售数据结果:",d);const y=new Map;d.forEach(({merchantId:w,salesAmount:A})=>{y.set(w,A)}),v.value=y,console.log("更新后的销售数据Map:",v.value)}catch(r){console.error("批量获取商家销售数据失败:",r)}},u=z(()=>!e.value||e.value.length===0?[]:e.value.map(d=>({...d,salesAmount:v.value.get(d.merchantId)||0})).sort((d,y)=>y.salesAmount-d.salesAmount).map((d,y)=>({...d,rank:y+1}))),c=s=>x[s]||{label:"未知",type:"error"},i=s=>{if(s===0)return"暂无数据";const r=s/100;return r>=1e4?`¥${(r/1e4).toFixed(1)}万`:r>=1e3?`¥${(r/1e3).toFixed(1)}K`:`¥${r.toFixed(2)}`},n=s=>{switch(s){case 1:return"text-yellow-500 font-bold text-lg";case 2:return"text-gray-400 font-bold text-lg";case 3:return"text-orange-600 font-bold text-lg";default:return"text-gray-600 font-semibold"}},l=s=>{switch(s){case 1:return"🥇";case 2:return"🥈";case 3:return"🥉";default:return`${s}`}};return W(()=>{f()}),(s,r)=>(_(),R(I(V),{bordered:!1,class:"card-wrapper",title:"商家销售排行榜"},{"header-extra":h(()=>[p(I(U),{type:"info",size:"small"},{default:h(()=>r[0]||(r[0]=[D("Top 10")])),_:1,__:[0]})]),default:h(()=>[p(I(X),{show:o.value},{default:h(()=>[u.value.length>0?(_(),S("div",Ye,[(_(!0),S(j,null,O(u.value,d=>(_(),S("div",{key:d.merchantId,class:"flex items-center p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"},[g("div",Ze,[g("span",{class:we(n(d.rank))},C(l(d.rank)),3)]),g("div",et,[g("div",tt,[g("div",null,[g("h4",at,C(d.merchantName),1),g("p",nt," 📍 "+C(d.location),1)]),g("div",ot,[g("div",rt,C(i(d.salesAmount)),1),g("div",st,[p(I(U),{type:c(d.status).type,size:"small"},{default:h(()=>[D(C(c(d.status).label),1)]),_:2},1032,["type"])])])])])]))),128))])):o.value?(_(),S("div",it,r[2]||(r[2]=[g("div",{class:"text-gray-400"},"正在加载排行榜...",-1)]))):(_(),S("div",lt,[p(I($e),{description:"暂无排行榜数据"},{icon:h(()=>r[1]||(r[1]=[g("div",{class:"text-6xl"},"📊",-1)])),_:1})]))]),_:1},8,["show"])]),_:1}))}}),dt=Se(ct,[["__scopeId","data-v-f7bf1547"]]),ut="/merchant/assets/soybean-JC38yUrs.jpg",ft={class:"size-72px overflow-hidden rd-1/2"},mt=k({name:"SoybeanAvatar",__name:"soybean-avatar",setup(t){return(e,o)=>(_(),S("div",ft,o[0]||(o[0]=[g("img",{src:ut,class:"size-full"},null,-1)])))}}),vt={key:0,class:"flex justify-center items-center py-4"},gt={key:1,class:"text-center py-4"},ht={class:"text-red-500 mb-2"},pt={key:3,class:"text-center py-8 text-gray-500"},_t=k({name:"ProjectNews",__name:"project-news",setup(t){const e=T([]),o=T(!1),v=T(""),x=async()=>{try{o.value=!0,v.value="";const f=await Oe("merchant",{size:10,page:1});console.log("Full API Response:",f);let a=[];Array.isArray(f)?(a=f,console.log("Response is direct array:",a)):f&&typeof f=="object"&&(f.data&&f.data.data&&Array.isArray(f.data.data)?(a=f.data.data,console.log("Extracted data from response.data.data:",a)):f.data&&Array.isArray(f.data)?(a=f.data,console.log("Extracted data from response.data:",a)):console.log("Could not find announcements array in response:",f)),a&&Array.isArray(a)&&a.length>0?(console.log("Processing announcements:",a.length),e.value=a.map((u,c)=>{console.log(`Announcement ${c}:`,u);const i=u.announceId||u.AnnounceId||`temp-${c}`,n=u.title||u.Title||"无标题",l=u.content||u.Content||n,s=u.startAt||u.StartAt,r=u.targetRole||u.TargetRole;return console.log(`Processing: ID=${i}, Title=${n}, TargetRole=${r}, StartAt=${s}`),{id:i,content:l||n,title:n,time:s?new Date(s).toLocaleString("zh-CN",{year:"numeric",month:"2-digit",day:"2-digit",hour:"2-digit",minute:"2-digit",second:"2-digit",hour12:!1}):"无时间"}}),console.log("Successfully processed announcements:",e.value)):(console.log("No announcements found or invalid data structure:",a),e.value=[])}catch(f){v.value="加载公告失败",console.error("Error fetching announcements:",f),e.value=[]}finally{o.value=!1}};return W(()=>{x()}),(f,a)=>{const u=X,c=ze,i=mt,n=Ve,l=De,s=Te,r=V;return _(),R(r,{title:"平台资讯",bordered:!1,size:"small",segmented:"",class:"card-wrapper"},{"header-extra":h(()=>[g("a",{class:"text-primary",href:"javascript:;",onClick:x},"刷新")]),default:h(()=>[o.value?(_(),S("div",vt,[p(u,{size:"medium"}),a[0]||(a[0]=g("div",{class:"ml-2 text-gray-500"},"正在加载公告...",-1))])):v.value?(_(),S("div",gt,[g("div",ht,C(v.value),1),p(c,{onClick:x,size:"small",type:"primary"},{default:h(()=>a[1]||(a[1]=[D("重试")])),_:1,__:[1]})])):e.value.length>0?(_(),R(s,{key:2},{default:h(()=>[(_(!0),S(j,null,O(e.value,d=>(_(),R(l,{key:d.id},{prefix:h(()=>[p(i,{class:"size-48px!"})]),default:h(()=>[p(n,{title:d.title,description:`${d.content} - ${d.time}`},null,8,["title","description"])]),_:2},1024))),128))]),_:1})):(_(),S("div",pt,[a[3]||(a[3]=g("div",{class:"mb-2"},"暂无公告信息",-1)),a[4]||(a[4]=g("div",{class:"text-xs text-gray-400 mb-3"},[D(" • 检查数据库是否有 TargetRole='merchant' 或 null 的记录"),g("br"),D(" • 检查公告时间是否在有效期内 ")],-1)),p(c,{onClick:x,size:"small"},{default:h(()=>a[2]||(a[2]=[D("重新加载")])),_:1,__:[2]})]))]),_:1})}}}),St=k({name:"home",__name:"index",setup(t){const e=H(),o=z(()=>e.isMobile?0:16);return(v,x)=>{const f=K,a=Q,u=q;return _(),R(u,{vertical:"",size:16},{default:h(()=>[p(Qe),p(Xe),p(a,{"x-gap":o.value,"y-gap":16,responsive:"screen","item-responsive":""},{default:h(()=>[p(f,{span:"24 s:24 m:14"},{default:h(()=>[p(_t)]),_:1}),p(f,{span:"24 s:24 m:10"},{default:h(()=>[p(dt)]),_:1})]),_:1},8,["x-gap"])]),_:1})}}});export{St as default};
