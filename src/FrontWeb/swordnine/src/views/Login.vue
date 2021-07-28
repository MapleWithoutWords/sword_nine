<template>
  <div id="login">
    <div class="login_box">
      <div class="avatar_box">
        <img src="../assets/logo.png" alt />
      </div>
      <el-form
        ref="login_form_ref"
        :model="login"
        :rules="rules"
        label-width="80px"
        class="login_form"
      >
        <el-form-item label-width="0" prop="account">
          <el-input
            v-model="login.account"
            prefix-icon="el-icon-user-solid"
          ></el-input>
        </el-form-item>
        <el-form-item label-width="0" prop="password">
          <el-input
            v-model="login.password"
            prefix-icon="el-icon-s-goods"
            type="password"
          ></el-input>
        </el-form-item>
        <el-form-item label-width="0" class="login_btn_item">
          <el-button
            type="primary"
            v-on:click="loginUp"
            size="medium"
            class="login_btn"
            >登录</el-button
          >
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      login: {
        account: '',
        password: '',
        appId: this.$appKey,
        time: Math.round(new Date().getTime() / 1000)
      },
      rules: {
        account: [{ required: true, message: '请输入账号', trigger: 'blur' }],
        password: [
          { required: true, message: '请输入请输入密码', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    // eslint-disable-next-line vue/no-dupe-keys
    loginUp: function() {
      this.$refs.login_form_ref.validate(async result => {
        if (!result) {
          return false
        }
        var pubkey = await this.$getRSAPubKey()
        this.$encrypt.setPublicKey(pubkey)
        this.login.time = Math.round(new Date().getTime() / 1000)
        var str = this.$encrypt.encrypt(JSON.stringify(this.login))
        // const that = this
        // eslint-disable-next-line no-unused-vars
        var res = await this.$http({
          url: '/api/rsalogin',
          method: 'post',
          data: { enStr: str }
        })
        if (!res) {
          this.$message.error('网络错误')
          return false
        }
        if (res.code !== 0) {
          this.$message.error(res.msg)
          return false
        }
        window.sessionStorage.setItem('token', JSON.stringify(res))
        var userRes = await this.$http({
          url: '/api/getuserinfo',
          method: 'get'
        })
        if (!userRes) {
          this.$message.error('网络错误')
          return false
        }
        if (userRes.code !== 0) {
          this.$message.error(userRes.msg)
          return false
        }
        window.sessionStorage.setItem('userId', userRes.data)
        this.$router.push('/home')
      })
    }
  }
}
</script>

<style lang="less" scoped>
#login {
  height: 100%;
  background-color: #2b4b6b;
}
.login_box {
  width: 30vw;
  height: 30vh;
  background-color: #fff;
  box-shadow: 0 0 1px #fff;
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  border-radius: 1em;
}
.avatar_box {
  width: 130px;
  height: 130px;
  border-radius: 50%;
  border: 1px solid #eee;
  padding: 10px;
  box-shadow: 0 0 10px #ddd;
  position: absolute;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: #fff;
  img {
    width: 100%;
    height: 100%;
    border-radius: 50%;
    background-color: #eee;
  }
}
.login_form {
  position: absolute;
  bottom: 0;
  padding: 0 20px;
  width: 100%;
  box-sizing: border-box;
}
.login_btn_item {
  width: 100%;
}
.login_btn {
  // display: flex;
  // justify-content: flex-end;
  width: 100%;
}
</style>
