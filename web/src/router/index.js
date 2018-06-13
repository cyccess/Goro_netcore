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
    path: '/salesOrder',
    name: 'SalesOrder',
    component: resolve => require(['@/components/SalesOrder.vue'], resolve),
    meta: {title: '待审核销售订单', requiresAuth: false}
  }, {
    path: '/salesOrderDetail',
    name: 'SalesOrderDetail',
    component: resolve => require(['@/components/SalesOrderDetail.vue'], resolve),
    meta: {title: '待审核销售订单详情', requiresAuth: false}
  }
]

