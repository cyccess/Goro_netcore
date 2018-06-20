<template>
  <div class="sales-box">

    <div class="orderInfo">
      <div class="info-title">销售订单详情</div>
      <div class="info-text" v-for="(item,index) in field" :key="index" :class="[index===2 ? 'vux-1px-b line' : '']">
        <span>{{item.fFieldDescription}}：{{model[item.fFieldName]}}</span>
      </div>
    </div>

    <div class="btn-wrapper">
      <button @click="modalShow=true" class="btn btn-default" type="submit">不同意</button>
      <button @click="agree" class="btn btn-primary" type="submit">同意</button>
    </div>

    <!--<div class="btn-wrapper">-->
      <!--<button @click="modelReply=true" class="btn btn-primary" type="submit">回复</button>-->
    <!--</div>-->

    <x-dialog v-model="modalShow" class="dialog-disagree" @on-show="onShow">
      <div class="card">
        <div class="dialog-close" @click="modalShow=false">
          <span class="vux-close"></span>
        </div>
        <div class="card-body">
          <h6 class="card-subtitle mb-2 text-muted">选择组</h6>
          <div class="check-group">
            <check-icon :value.sync="isMe">工艺组</check-icon>
            <check-icon :value.sync="isPo">供应组</check-icon>
          </div>

          <h6 class="card-subtitle mb-2 text-muted">不同意原因</h6>
          <textarea class="form-control" v-model="reason"></textarea>
          <div class="btn-box">
            <button @click="disagree" type="button" class="btn btn-sm btn-primary">确定</button>
            <button @click="modalShow=false" type="button" class="btn btn-sm btn-default">取消</button>
          </div>
        </div>
      </div>
    </x-dialog>

    <x-dialog v-model="modelReply" class="dialog-reply">
      <div class="card">
        <div class="dialog-close" @click="modelReply=false">
          <span class="vux-close"></span>
        </div>
        <div class="card-body">
          <h6 class="card-subtitle mb-2 text-muted">回复内容</h6>
          <textarea class="form-control" v-model="reason"></textarea>
          <div class="btn-box">
            <button @click="disagree" type="button" class="btn btn-sm btn-primary">确定</button>
            <button @click="modelReply=false" type="button" class="btn btn-sm btn-default">取消</button>
          </div>
        </div>
      </div>
    </x-dialog>
  </div>
</template>

<script>
  import {XDialog, CheckIcon} from 'vux'

  export default {
    components: {
      XDialog, CheckIcon
    },
    data() {
      return {
        modalShow: false,
        modelReply:false,
        reason: '',
        billNo: '',
        model: {},
        field: [],
        isMe: false, //工艺是否审核
        isPo: false  //供应是否审核

      }
    },
    created() {
      this.billNo = this.$route.query.billNo;
      this.getData();
    },
    methods: {
      async getData() {
        let res = await this.$http.post('/api/SalesOrderDetail', {fBillNo: this.billNo});

        this.model = res.data.order[0];
        this.field = res.data.field
      },
      async agree() {
        this.update("Y");
      },
      async disagree() {
        this.modalShow = false;
        this.update("N");
      },
      async update(result) {
        let args = {
          billNo: this.billNo,
          result: result,
          reason: this.reason
        };

        // 工艺/供应是否审核
        if (true) {
          args.isMe = this.isMe ? "1" : "0";
          args.isPo = this.isPo ? "1" : "0";
        }

        let res = await this.$http.post('/api/UpdateSalesOrder', args);
        let message = res.message;
        if (result === "Y") {
          if (res.message === "OK") {
            message = "审核通过！";
          }
        } else {
          if (res.message === "OK") {
            message = "操作成功！";
          }
        }
        let vm = this;
        this.$vux.alert.show({
          title: '提示',
          content: message,
          onHide() {
            vm.$router.push({path: '/salesReturnNotice'});
          }
        });
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
    .btn {
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

  .line {
    padding-bottom: .85rem;
    margin-bottom: .85rem;
  }

  .weui-dialog {
    text-align: left;
    .card-subtitle {
      text-align: left;
    }
    .check-group {
      text-align: left;
      margin: 1rem 0;
    }
    .dialog-close {
      position: absolute;
      top: .75rem;
      right: .75rem;
    }
    .btn-box {
      margin-top: .5rem;
      text-align: right;
    }
  }
</style>
