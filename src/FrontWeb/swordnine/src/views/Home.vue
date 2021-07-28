<template>
  <el-container>
    <!-- 头部区域 -->
    <el-header>
      <div>
        <i
          :class="isCollapse ? 'el-icon-s-unfold' : 'el-icon-s-fold'"
          @click="asideToggle"
        ></i>
        <img src="../assets/logo.png" alt="剑九系统" />
        <span style="font-family: '宋体';">SwordNine</span>
      </div>
      <!-- <el-button type="info" @click="logout">退出登录</el-button> -->
    </el-header>
    <el-container>
      <el-aside :width="isCollapse ? '64px' : '200px'">
        <el-menu
          background-color="#333744"
          text-color="#fff"
          active-text-color="#ffd04b"
          :collapse="isCollapse"
          :collapse-transition="false"
          :router="true"
        >
          <el-menu-item index="welcome">
            <i class="el-icon-s-home"></i>
            <span>首页</span>
          </el-menu-item>

          <el-submenu index="1-1">
            <template slot="title">
              <i class="el-icon-menu"></i>
              <span>数据建模</span>
            </template>
            <el-menu-item index="dataSourceIndex">
              <i class="el-icon-s-data"></i>
              <span>数据源管理</span>
            </el-menu-item>
            <!-- <el-menu-item index="classAttributeIndex">
              <i class="el-icon-user"></i>
              <span>类表管理</span>
            </el-menu-item> -->
            <el-menu-item index="parentClassAttributeIndex">
              <i class="el-icon-user"></i>
              <span>上级类别管理</span>
            </el-menu-item>
            <el-menu-item index="modelManagement">
              <i class="el-icon-s-grid"></i>
              <span>模型管理</span>
            </el-menu-item>
          </el-submenu>

          <el-submenu index="2-1">
            <template slot="title">
              <i class="el-icon-film"></i>
              <span>代码BI</span>
            </template>
            <el-menu-item index="/ruleIndex">
              <i class="el-icon-s-ticket"></i>
              <span>规则管理</span>
            </el-menu-item>
            <el-menu-item index="/templateIndex">
              <i class="el-icon-notebook-2"></i>
              <span>模板管理</span>
            </el-menu-item>
            <el-menu-item index="/codeGenIndex">
              <i class="iconfont icon-iconset0264"></i>
              <span>代码生成</span>
            </el-menu-item>
          </el-submenu>
        </el-menu>
      </el-aside>
      <!-- 主体-->
      <el-main>
        <router-view></router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script>
export default {
  data() {
    return {
      isCollapse: true
    }
  },
  methods: {
    logout: async function() {
      var logRes = await this.$sendAsync({
        url: '/api/logout',
        method: 'get'
      })
      if (logRes !== null) {
        window.sessionStorage.clear()
      }
      location.reload()
    },
    // 侧边栏折叠
    asideToggle() {
      this.isCollapse = !this.isCollapse
    }
  }
}
</script>

<style lang="less" scoped>
.el-container {
  width: 100%;
  height: 100%;
}
.el-header {
  background-color: rgb(55, 61, 65);
  color: #fff !important;
  display: flex;
  justify-content: space-between;
  font-size: 22px;
  align-items: center;
  padding-left: 0;
  > div {
    display: flex;
    align-items: center;
    img {
      height: 50px;
    }
    span {
      margin-left: 15px;
    }
    i {
      width: 80px;
      font-size: 64px;
      cursor: pointer;
    }
  }
}

.el-main {
  background-color: rgb(234, 237, 241);
}

.el-aside {
  background-color: rgb(51, 55, 68);
}
</style>
