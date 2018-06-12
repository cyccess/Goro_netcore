<template>
  <div class="sales-box">

    <div class="header">待(工艺/交期/信用额度)审核销售订单</div>

    <table class="table">
      <thead>
      <tr>
        <th>序号</th>
        <th>销售订单号</th>
        <th>客户</th>
      </tr>
      </thead>
      <tbody>
      <tr @click="detail(index)" v-for="(item,index) in list" :key="index">
        <td>{{item.id}}</td>
        <td>{{item.orderNo}}</td>
        <td>{{item.customer}}</td>
      </tr>
      </tbody>
    </table>

  </div>
</template>
<script>
  export default {
    data() {
      return {
        list: [
          {id: 1, orderNo: '20180511001', customer: '大客户1'},
          {id: 2, orderNo: '20180511002', customer: '大客户2'},
          {id: 3, orderNo: '20180511003', customer: '大客户3'}
        ]
      }
    },
    methods: {
      detail(index) {
        let id = this.list[index].id;
        this.$router.push({path: '/salesOrderDetail', query: {id: id}});
      }
    }
  }
</script>
<style scoped>
  .sales-box {
    padding: .875rem;
    font-size: .875rem;
  }

  .header {
    margin-bottom: 1.2rem;
    color:#3296FA;
  }
</style>
