<template>
  <div class="account-box">
    <div class="card text-center">
      <div class="logo">
        <img src="../assets/goro.jpg" alt="">
      </div>

      <form>
        <div class="form-group" title="首次登录，需要输入手机号码绑定账户">
          <input class="form-control" type="text" v-model.trim="mobilePhone" :maxlength="11" placeholder="请输入手机号码">
          <small>首次登录，需要输入手机号码绑定账户</small>
        </div>

        <button type="button" class="btn btn-primary btn-block" @click="onSubmit">确定绑定</button>
      </form>
    </div>

  </div>
</template>

<script>
  export default {
    data() {
      return {
        mobilePhone: '',
        isSubmit:false,
      }
    },
    methods: {
      onSubmit() {
        let valid = this.validate();
        if (valid) {

        }
      },
      validate() {
        let reg = /^1[0-9]{10}$/;
        if (!this.mobilePhone) {
          this.$vux.toast.text('请输入手机号');
          return false;
        }

        if (!reg.test(this.mobilePhone)) {
          this.$vux.toast.text('请输入正确的手机号');
          return false;
        }
        return true;
      }
    }
  }
</script>

<style scoped>
  .account-box {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
  }

  .logo {
    padding: 5rem;
  }

  .card {
    border: 0;
    padding: 1rem;
  }

  .btn {
    border-radius: 0;
  }

  .form-control {
    border-radius: 0;
  }
</style>
