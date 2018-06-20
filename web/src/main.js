// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import qs from 'qs';
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

// 请求时的拦截
AjaxPlugin.$http.interceptors.request.use((request) => {
  // 发送请求之前做一些处理
  // console.log(request)
  if (request.data) {
    let contentType = request.headers["Content-Type"]
    // contentType参数不是application/json，转换参数格式
    if (!(contentType && contentType.indexOf("application/json") > -1))
      request.data = qs.stringify(request.data);
  }

  return request;
}, error => {
  // 当请求异常时做一些处理
  return Promise.reject(error);
});

// 响应时拦截
AjaxPlugin.$http.interceptors.response.use(response => {
  if (response.status === 200) {
    console.log(response.data)
    return response.data;
  } else {
    return Promise.reject(response);
  }
}, error => {
  // 当响应异常时做一些处理
  return Promise.reject(error);
});

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

Vue.filter('money', function (value) {
  if (!value) return '0.00';
  value = parseFloat(value);
  return value.toFixed(2)
});


/* eslint-disable no-new */
new Vue({
  router,
  render: h => h(App)
}).$mount('#app-box');
