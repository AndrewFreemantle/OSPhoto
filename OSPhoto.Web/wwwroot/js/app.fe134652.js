(function(e){function t(t){for(var c,o,i=t[0],u=t[1],l=t[2],b=0,f=[];b<i.length;b++)o=i[b],Object.prototype.hasOwnProperty.call(r,o)&&r[o]&&f.push(r[o][0]),r[o]=0;for(c in u)Object.prototype.hasOwnProperty.call(u,c)&&(e[c]=u[c]);s&&s(t);while(f.length)f.shift()();return n.push.apply(n,l||[]),a()}function a(){for(var e,t=0;t<n.length;t++){for(var a=n[t],c=!0,i=1;i<a.length;i++){var u=a[i];0!==r[u]&&(c=!1)}c&&(n.splice(t--,1),e=o(o.s=a[0]))}return e}var c={},r={app:0},n=[];function o(t){if(c[t])return c[t].exports;var a=c[t]={i:t,l:!1,exports:{}};return e[t].call(a.exports,a,a.exports,o),a.l=!0,a.exports}o.m=e,o.c=c,o.d=function(e,t,a){o.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:a})},o.r=function(e){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},o.t=function(e,t){if(1&t&&(e=o(e)),8&t)return e;if(4&t&&"object"===typeof e&&e&&e.__esModule)return e;var a=Object.create(null);if(o.r(a),Object.defineProperty(a,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var c in e)o.d(a,c,function(t){return e[t]}.bind(null,c));return a},o.n=function(e){var t=e&&e.__esModule?function(){return e["default"]}:function(){return e};return o.d(t,"a",t),t},o.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},o.p="/";var i=window["webpackJsonp"]=window["webpackJsonp"]||[],u=i.push.bind(i);i.push=t,i=i.slice();for(var l=0;l<i.length;l++)t(i[l]);var s=u;n.push([0,"chunk-vendors"]),a()})({0:function(e,t,a){e.exports=a("56d7")},"3c2a":function(e,t,a){"use strict";a("72a5")},"56d7":function(e,t,a){"use strict";a.r(t);a("e260"),a("e6cf"),a("cca6"),a("a79d");var c=a("7a23"),r=a("6c02");function n(e,t,a,r,n,o){var i=Object(c["x"])("Location"),u=Object(c["x"])("router-view"),l=Object(c["x"])("Footer");return Object(c["q"])(),Object(c["d"])(c["a"],null,[Object(c["h"])(i),Object(c["h"])(u),Object(c["h"])(l)],64)}a("b0c0");var o=Object(c["C"])("data-v-24925c6c");Object(c["t"])("data-v-24925c6c");var i={"aria-label":"breadcrumb"},u={class:"breadcrumb"},l={key:0,class:"breadcrumb-item active"},s=Object(c["h"])("i",{class:"fas fa-home"},null,-1),b={class:"breadcrumb-item"},f=Object(c["h"])("i",{class:"fas fa-home"},null,-1),d={key:1,class:"active"};Object(c["r"])();var m=o((function(e,t,a,r,n,m){var p=Object(c["x"])("router-link");return Object(c["q"])(),Object(c["d"])("nav",i,[Object(c["h"])("ol",u,[0===n.data.location.length?(Object(c["q"])(),Object(c["d"])("li",l,[s])):Object(c["e"])("",!0),n.data.location.length>0?(Object(c["q"])(),Object(c["d"])(c["a"],{key:1},[Object(c["h"])("li",b,[Object(c["h"])(p,{to:"/",class:"link-dark text-decoration-none"},{default:o((function(){return[f]})),_:1})]),(Object(c["q"])(!0),Object(c["d"])(c["a"],null,Object(c["w"])(n.data.location,(function(e,t){return Object(c["q"])(),Object(c["d"])("li",{key:e,class:"breadcrumb-item"},[t+1!==n.data.location.length?(Object(c["q"])(),Object(c["d"])(p,{key:0,to:{name:"album",params:{album:e.path}}},{default:o((function(){return[Object(c["g"])(Object(c["z"])(e.name),1)]})),_:2},1032,["to"])):Object(c["e"])("",!0),t+1===n.data.location.length?(Object(c["q"])(),Object(c["d"])("span",d,Object(c["z"])(e.name),1)):Object(c["e"])("",!0)])})),128))],64)):Object(c["e"])("",!0)])])})),p=a("1da1"),O=(a("d3b7"),a("96cf"),{state:Object(c["u"])({location:[],contents:[]}),update:function(e){e&&e.location&&(this.state.location=e.location),e&&e.contents&&(this.state.contents=e.contents)}}),h={name:"Location",data:function(){return{data:O.state}},methods:{updateStore:function(){var e=this;return Object(p["a"])(regeneratorRuntime.mark((function t(){var a;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return a=e.$route.params&&e.$route.params.album?e.$route.params.album:e.getPathFromUrl(),t.t0=O,t.next=4,e.fetchAlbum(a);case 4:t.t1=t.sent,t.t0.update.call(t.t0,t.t1);case 6:case"end":return t.stop()}}),t)})))()},getPathFromUrl:function(){var e=window.location.hash;return null==e||e.length<=2?null:e.substring(2)},fetchAlbum:function(e){return Object(p["a"])(regeneratorRuntime.mark((function t(){var a,c;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return a="http://localhost:5000/album"+(null==e?"":"/".concat(e)),t.next=3,fetch(a);case 3:return c=t.sent,t.next=6,c.json();case 6:return t.abrupt("return",t.sent);case 7:case"end":return t.stop()}}),t)})))()}},created:function(){var e=this;return Object(p["a"])(regeneratorRuntime.mark((function t(){return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return t.next=2,e.updateStore();case 2:e.$watch((function(t){return e.$route.params}),function(){var t=Object(p["a"])(regeneratorRuntime.mark((function t(a){return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return t.next=2,e.updateStore();case 2:return t.abrupt("return",t.sent);case 3:case"end":return t.stop()}}),t)})));return function(e){return t.apply(this,arguments)}}());case 3:case"end":return t.stop()}}),t)})))()}};a("a1b8");h.render=m,h.__scopeId="data-v-24925c6c";var j=h,v=Object(c["C"])("data-v-4b185ecc");Object(c["t"])("data-v-4b185ecc");var g={id:"footer"},y=Object(c["f"])('<div class="left float-left" data-v-4b185ecc><ul data-v-4b185ecc><li data-v-4b185ecc><i class="far fa-images" data-v-4b185ecc></i> OSPhoto</li><li data-v-4b185ecc><a href="https://github.com/AndrewFreemantle/OSPhoto" alt="" data-v-4b185ecc><i class="fab fa-github" data-v-4b185ecc></i> open source</a></li><li data-v-4b185ecc><a href="https://github.com/AndrewFreemantle/OSPhoto/issues" alt="" data-v-4b185ecc><i class="far fa-comment-alt" data-v-4b185ecc></i> feedback</a></li><li data-v-4b185ecc><a href="https://www.buymeacoffee.com/fatlemon" data-v-4b185ecc><i class="fas fa-mug-hot" data-v-4b185ecc></i> buy us a coffee!</a></li></ul></div><div class="right float-right" data-v-4b185ecc> v0.1.0 </div>',2);Object(c["r"])();var w=v((function(e,t,a,r,n,o){return Object(c["q"])(),Object(c["d"])("div",g,[y])})),k={name:"Footer"};a("8046");k.render=w,k.__scopeId="data-v-4b185ecc";var x=k,q={name:"App",components:{Location:j,Footer:x}};q.render=n;var P=q,_=Object(c["C"])("data-v-e627b2a8");Object(c["t"])("data-v-e627b2a8");var C={class:"gallery"};Object(c["r"])();var S=_((function(e,t,a,r,n,o){var i=Object(c["x"])("GalleryItem");return Object(c["q"])(),Object(c["d"])("div",C,[(Object(c["q"])(!0),Object(c["d"])(c["a"],null,Object(c["w"])(n.data.contents,(function(e){return Object(c["q"])(),Object(c["d"])(i,{item:e,key:e},null,8,["item"])})),128))])})),I=Object(c["h"])("figure",{class:"figure figure-dir"},[Object(c["h"])("i",{class:"far fa-5x fa-folder"})],-1),R={class:"figure-caption"},F={class:"figure figure-img"};function $(e,t,a,r,n,o){return Object(c["q"])(),Object(c["d"])(c["a"],null,[a.item.isDirectory?(Object(c["q"])(),Object(c["d"])("div",{key:0,onClick:t[1]||(t[1]=function(){return o.onDirectoryClick&&o.onDirectoryClick.apply(o,arguments)}),class:"gallery-item directory"},[I,Object(c["h"])("figcaption",R,Object(c["z"])(a.item.name),1)])):Object(c["e"])("",!0),a.item.isImage?(Object(c["q"])(),Object(c["d"])("div",{key:1,onClick:t[2]||(t[2]=function(){return o.onImageClick&&o.onImageClick.apply(o,arguments)}),class:"gallery-item image"},[Object(c["h"])("figure",F,[Object(c["h"])("img",{src:o.imagePath(),alt:a.item.name,title:a.item.name},null,8,["src","alt","title"])])])):Object(c["e"])("",!0)],64)}var A={name:"GalleryItem",props:{item:{}},methods:{imagePath:function(){return"https://localhost:5001/image/".concat(this.item.path)},onDirectoryClick:function(){console.log("[dir.click]",this.item.path),this.$router.push({name:"album",params:{album:this.item.path}})},onImageClick:function(){alert("image..")}}};a("3c2a");A.render=$;var D=A,G={name:"Gallery",components:{GalleryItem:D},data:function(){return{data:O.state}}};a("fae0");G.render=S,G.__scopeId="data-v-e627b2a8";var M=G;a("becf"),a("d2c4");var z=[{path:"/:album?",name:"album",component:M}],L=Object(r["a"])({history:Object(r["b"])(),routes:z});Object(c["c"])(P).use(L).mount("#app")},"72a5":function(e,t,a){},"7bec":function(e,t,a){},8046:function(e,t,a){"use strict";a("edf6")},"8b91":function(e,t,a){},a1b8:function(e,t,a){"use strict";a("7bec")},edf6:function(e,t,a){},fae0:function(e,t,a){"use strict";a("8b91")}});
//# sourceMappingURL=app.fe134652.js.map