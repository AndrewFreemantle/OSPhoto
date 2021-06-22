require('../node_modules/@fortawesome/fontawesome-free/css/all.min.css');
require('../node_modules/milligram/dist/milligram.min.css');

import { createApp } from 'vue';
import { createRouter, createWebHashHistory } from 'vue-router';
import App from './App.vue';
import Gallery from './components/Gallery';

const routes = [
  // matches /a/one, /one/two/, /one/two/three, etc
  { path: '/:album?', name: 'album', component: Gallery },
];

const router = createRouter({
	history: createWebHashHistory(),
	routes: routes,
});

createApp(App)
	.use(router)
	.mount('#app');
