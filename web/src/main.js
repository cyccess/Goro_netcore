// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import FastClick from 'fastclick'
import VueRouter from 'vue-router'
import App from './App'
import routes from './router/index'
import VueScroller from 'vue-scroller'
import 'bootstrap/dist/css/bootstrap.min.css'
import {getStore} from './utils'

import {AjaxPlugin, AlertPlugin ,ConfirmPlugin, ToastPlugin} from 'vux'

Vue.use(VueRouter);
Vue.use(AjaxPlugin);
Vue.use(VueScroller);
Vue.use(AlertPlugin);
Vue.use(ConfirmPlugin);
Vue.use(ToastPlugin);


const router = new VueRouter({
  routes
});


FastClick.attach(document.body);

Vue.config.productionTip = false;

router.beforeEach((to, from, next) => {
  let sid = getStore("sid");
  if (to.meta.requiresAuth && !sid) {
    next({
      path: '/login',
      query: {redirect: to.fullPath}
    });
  }
  else {
    window.document.title = to.meta.title;
    next();
  }
});


/* eslint-disable no-new */
new Vue({
  router,
  render: h => h(App)
}).$mount('#app-box');
