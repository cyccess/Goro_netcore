<template>
  <div class="sales-box">

    <div class="orderInfo">
      <div class="info-title">订单信息</div>
      <div class="info-text">销售订单号：</div>
      <div class="info-text">日期：</div>
      <div class="info-text">客户：</div>
      <div class="info-text">备注：</div>
    </div>

    <div class="table-responsive">
      <table class="table">
        <thead>
        <tr>
          <th>序号</th>
          <th>物料代码</th>
          <th>物料名称</th>
          <th>规格型号</th>
          <th>数量</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="(item,index) in list" :key="index">
          <td>{{item.id+index}}</td>
          <td>{{item.code + index}}</td>
          <td>{{item.name+index}}</td>
          <td>{{item.modal}}</td>
          <td>{{item.num}}</td>
        </tr>
        </tbody>
      </table>
    </div>

    <div class="btn-wrapper">

      <button @click="modalShow=true" class="btn btn-default" type="submit">不同意</button>
      <button @click="agree" class="btn btn-primary" type="submit">同意</button>
    </div>

    <x-dialog v-model="modalShow" class="dialog-disagree" @on-show="onShow">
      <div class="card">
        <div class="dialog-close" @click="modalShow=false">
          <span class="vux-close"></span>
        </div>
        <div class="card-body">
          <h6 class="card-subtitle mb-2 text-muted">不同意原因</h6>
          <textarea class="form-control" v-model="reason"></textarea>
          <div class="btn-box">
            <button @click="disagree" type="button" class="btn btn-sm btn-primary">确定</button>
            <button @click="modalShow=false" type="button" class="btn btn-sm btn-default">取消</button>
          </div>
        </div>
      </div>

    </x-dialog>
  </div>
</template>

<script>
  import { XDialog } from 'vux'
  export default {
    components: {
      XDialog
    },
    data() {
      return {
        list: [
          {id: 1, code: '880001', name: '物料名称', modal: 'UC2', num: 2},
          {id: 1, code: '880001', name: '物料名称', modal: 'UC2', num: 2},
          {id: 1, code: '880001', name: '物料名称', modal: 'UC2', num: 2},
          {id: 1, code: '880001', name: '物料名称', modal: 'UC2', num: 2},
        ],
        modalShow: false,
        reason: '',
      }
    },
    created(){

    },
    methods: {
      agree() {
        this.$vux.alert.show({
          title: '提示',
          content: '审核通过！',
          onHide () {

          }
        })
      },
      disagree() {
        let vm = this;
        this.modalShow = false;
        this.$vux.alert.show({
          title: '提示',
          content: '操作成功！',
          onHide () {
            vm.$router.push({path: '/salesOrder'});
          }
        })
      },
      onShow(){

      }
    }
  }
</script>

<style lang="less" scoped>
  @import '~vux/src/styles/close';
  .sales-box {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
    padding: .875rem;
    font-size: .875rem;
  }

  .btn-wrapper {
    display: flex;
    position: absolute;
    left: 0;
    bottom: 0;
    width: 100%;
    .btn{
      flex: 1;
      border-radius: 0;
    }
  }

  .orderInfo {
    margin-bottom: 1rem;
  }

  .info-title {
    font-weight: 600;
    margin-bottom: .5rem;
  }

  .info-text {
    line-height: 1.5rem;
  }

  .table-responsive > .table {
    margin-bottom: 0
  }

  .table-responsive > .table > tbody > tr > td, .table-responsive > .table > tbody > tr > th, .table-responsive > .table > tfoot > tr > td, .table-responsive > .table > tfoot > tr > th, .table-responsive > .table > thead > tr > td, .table-responsive > .table > thead > tr > th {
    white-space: nowrap
  }

  .weui-dialog{
    text-align: left;
    .card-subtitle{
      text-align: left;
    }
    .dialog-close{
      position: absolute;
      top: .75rem;
      right: .75rem;
    }
    .btn-box{
      margin-top: .5rem;
      text-align: right;
    }
  }
</style>
