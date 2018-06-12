import HelloWorld from '@/components/HelloWorld'
import Account from '@/components/Account'
import SalesOrder from '@/components/SalesOrder'
import SalesOrderDetail from '@/components/SalesOrderDetail'

export default [
  {
    path: '/',
    name: 'HelloWorld',
    component: HelloWorld
  }, {
    path: '/account',
    name: 'Account',
    component: Account
  }, {
    path: '/salesOrder',
    name: 'SalesOrder',
    component: SalesOrder,
    meta: {title: '待审核销售订单', requiresAuth: false}
  }, {
    path: '/salesOrderDetail',
    name: 'SalesOrderDetail',
    component: SalesOrderDetail,
    meta: {title: '待审核销售订单详情', requiresAuth: false}
  }
]

