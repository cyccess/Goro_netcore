<template>
  <div class="sales-box">
    <scroller :on-refresh="refresh" :on-infinite="infinite" ref="myscroller" :no-data-text="noData">
      <div class="sales-list">
        <div @click="detail(index)" class="sales-item" v-for="(item,index) in list" :key="index">
          <div class="bill-no vux-1px-b">单据编号：{{item.fBillNo}}</div>
          <div class="custom">
            {{item.fCustName}}
          </div>
        </div>
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
        list: []
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
        this.page += 1;
        let res = await this.$http.post('/api/SalesReturnNotice', {page: this.page});

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
        let fBillNo = this.list[index].fBillNo;
        this.$router.push({path: '/salesReturnNoticeDetail', query: {billNo: fBillNo}});
      }
    }
  }
</script>
<style scoped lang="less">
  .sales-box {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
    background-color:#f1f1f1;
  }

  .sales-list{

  }
  .sales-item{
    margin-top: .56667rem;
    padding-left: .75rem;
    background-color: #fff;
    font-size: .87333rem;
    .bill-no{
      height: 3.06667rem;
      line-height: 3.06667rem;
      margin-right: .4rem;
    }
    .custom{
      padding: .93333rem 0;
    }
  }
</style>
