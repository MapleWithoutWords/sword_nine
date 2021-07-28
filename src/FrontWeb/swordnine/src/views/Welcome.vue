<template>
  <div class="container">
    <el-row>
      <el-col :span="19">
        <el-input v-model="keyword" placeholder="请输入关键字"></el-input>
      </el-col>
      <el-col :span="4" style="margin-left:12px;">
        <el-button type="primary" icon="el-icon-search"
          >搜索</el-button
        >
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      keyword: ''
    }
  },
  methods: {},
  async mounted() {}
}
</script>

<style lang="less" scoped>
.container {
  width: 50%;
  margin-left: 50%;
  margin-top: 30vh;
  transform: translate(-50%, 0);
}
</style>
