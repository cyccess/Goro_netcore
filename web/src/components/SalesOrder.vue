<template>
  <div class="sales-box">
    <scroller :on-refresh="refresh" :on-infinite="infinite" ref="myscroller" :no-data-text="noData">
      <div class="sales-wrapper">
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

    </scroller>
  </div>
</template>
<script>
  export default {
    data() {
      return {
        page:0,
        noData:'',
        list: [
          {id: 1, orderNo: '20180511001', customer: '大客户1'},
          {id: 2, orderNo: '20180511002', customer: '大客户2'},
          {id: 3, orderNo: '20180511003', customer: '大客户3'}
        ]
      }
    },
    methods: {
      refresh(done) {
        this.list = [];
        this.page = 0;
        this.noData = '';
        done();
      },
      async infinite(done) {
        if (this.noData) {
          done();
          return;
        }
        this.brandId = this.$route.query.brandId || 0;
        this.page += 1;
        let res = await this.$http.post('/api/Sell/List', {page: this.page, brandId: this.brandId});

        if (res.code === 100) {
          if (res.data.length < 10) {
            this.noData = '没有更多了';
          }

          this.list = [...this.list, ...res.data];
          done()
        }
        else {
          if (this.list.length === 0) {
            this.noData = "暂无数据";
          }
          done(true);
        }
      },
      detail(index) {
        let id = this.list[index].id;
        this.$router.push({path: '/salesOrderDetail', query: {id: id}});
      }
    }
  }
</script>
<style scoped>
  .sales-box {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
  }
  .sales-wrapper{
    padding: .875rem;
    font-size: .875rem;
  }

  .header {
    margin-bottom: 1.2rem;
    color:#3296FA;
  }
</style>
