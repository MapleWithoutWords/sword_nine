<template>
  <div>
    <el-form
      ref="dialogForm"
      :model="formData"
      width="50%"
      label-width="120px"
      label-position="right"
      :hide-required-asterisk="false"
      label-suffix="："
      :rules="formRule"
    >
      <el-form-item label="数据源">
        <el-input
          disabled
          size="small"
          v-model="formData.dataSourceName"
        ></el-input>
      </el-form-item>
      <el-form-item label="上级目录" prop="parentId">
        <select-tree
          :options="allDirectoryData"
          :data="treeData"
          :clearable="false"
          :accordion="true"
          @getValue="getParentValue($event)"
        />
      </el-form-item>
      <el-form-item label="名称" :required="true" prop="name">
        <el-input size="small" v-model="formData.name"></el-input>
      </el-form-item>
      <el-form-item label="编码" :required="true" prop="code">
        <el-input size="small" v-model="formData.code"></el-input>
      </el-form-item>
      <el-form-item label="序号" :required="true" prop="seqNo">
        <el-input
          size="small"
          v-model.number="formData.seqNo"
          autocomplete="off"
        ></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="formSubmit">确认</el-button>
        <el-button @click="$emit('cancel')">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import ClassDirectoryService from '@/services/dataSourceService/classDirectoryService'

import SelectTree from '@/components/ElementUI/SelectTree'
export default {
  components: { SelectTree },
  props: ['type', 'data'],

  watch: {
    async data(n, o) {
      await this.initData(n)
    },
    type(n, o) {
      // n为新值,o为旧值;
      this.type = n
    }
  },
  data() {
    return {
      treeData: {},
      formData: {
        name: '',
        code: '',
        dataSourceId: '',
        parentId: '',
        seqNo: 0,
        dataSourceName: '',
        content: ''
      },
      formRule: {
        name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        code: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        dataSourceId: [
          { required: true, message: '数据源不能为空', trigger: 'blur' }
        ]
      },
      formType: '',
      allDirectoryData: []
    }
  },
  methods: {
    async initData(data) {
      // eslint-disable-next-line no-undef
      this.formData = _.cloneDeep(data)
      this.treeData.id = data.parentId
      this.treeData.name = data.parentName
      this.selTreeKey = !this.selTreeKey
      await this.loadDirectoryData()
    },
    async loadDirectoryData() {
      var dataSourceId = this.formData.dataSourceId
      if (!dataSourceId) {
        console.error('数据源id不能为空')
        this.$message.error('数据源id不能为空')
        return false
      }
      var result = await ClassDirectoryService.getDirectoryTree({
        dataSourceId: dataSourceId
      })
      var ret = this.$validResponse(result)
      if (ret) {
        this.allDirectoryData = ret.data
      }
    },
    formSubmit() {
      const that = this
      this.$refs.dialogForm.validate(async valid => {
        if (!valid) {
          return false
        }
        var res
        if (that.type === 'add') {
          // 新增
          res = await ClassDirectoryService.addNew(that.formData)
        } else if (that.type === 'editor') {
          // 编辑
          res = await ClassDirectoryService.update(that.formData)
        } else {
          that.$message.error('未知的type')
        }
        var ret = that.$validResponse(res)
        if (!ret) {
          return false
        }
        this.$emit('ok', ret.data)
        return true
      })
    },
    getParentValue(data) {
      console.log(data)
      this.formData.parentId = data.id
    }
  },
  mounted: async function() {
    await this.initData(this.data)
  }
}
</script>

<style lang="less" scoped></style>
