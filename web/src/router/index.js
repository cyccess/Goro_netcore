import HelloWorld from '@/components/HelloWorld'

export default [
  {
    path: '/',
    name: 'HelloWorld',
    component: HelloWorld
  }, {
    path: '/account',
    name: 'Account',
    component: resolve => require(['@/components/Account.vue'], resolve),
  }, {
    path: '/salesReturnNotice',
    name: 'SalesReturnNotice',
    component: resolve => require(['@/components/SalesReturnNotice.vue'], resolve),
    meta: {title: '退货通知单', requiresAuth: false}
  }, {
    path: '/salesReturnNoticeDetail',
    name: 'SalesReturnNoticeDetail',
    component: resolve => require(['@/components/SalesReturnNoticeDetail.vue'], resolve),
    meta: {title: '退货通知单详情', requiresAuth: false}
  }
  , {
    path: '/salesOrder',
    name: 'salesOrder',
    component: resolve => require(['@/components/SalesOrder.vue'], resolve),
    meta: {title: '销售订单', requiresAuth: false}
  }, {
    path: '/salesOrderDetail',
    name: 'salesOrder',
    component: resolve => require(['@/components/SalesOrderDetail.vue'], resolve),
    meta: {title: '销售订单详情', requiresAuth: false}
  }
]

