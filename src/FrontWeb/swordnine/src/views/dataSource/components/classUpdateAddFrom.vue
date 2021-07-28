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
      <el-form-item
        label="所属目录"
        :required="true"
        prop="classDirectoryId"
        v-if="formData.type === 0"
      >
        <select-tree
          :key="selTreeKey"
          :options="allDirectoryData"
          :data="treeData"
          :clearable="false"
          :accordion="true"
          @getValue="getParentValue($event)"
        />
      </el-form-item>
      <el-form-item label="上级类别" v-if="formData.type === 0">
        <el-select v-model="formData.parentId" placeholder="请选择上级类别">
          <el-option
            v-for="item in parentClassList"
            :key="item.id"
            :label="item.code"
            :value="item.id"
          >
            <span style="float: left">{{ item.code }}</span>
            <span style="float: right; color: #8492a6; font-size: 13px">{{
              item.name
            }}</span>
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="名称" :required="true" prop="name">
        <el-input size="small" v-model="formData.name"></el-input>
      </el-form-item>
      <el-form-item label="编码" :required="true" prop="code">
        <el-input size="small" v-model="formData.code"></el-input>
      </el-form-item>
      <el-form-item label="表名" :required="true" prop="tableName">
        <el-input size="small" v-model="formData.tableName"></el-input>
      </el-form-item>
      <el-form-item label="序号" :required="true" prop="seqNo">
        <el-input
          size="small"
          v-model.number="formData.seqNo"
          autocomplete="off"
        ></el-input>
      </el-form-item>
      <el-form-item label="备注" prop="remark">
        <el-input
          type="textarea"
          size="small"
          v-model="formData.remark"
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
import ClassService from '@/services/dataSourceService/classService'

import SelectTree from '@/components/ElementUI/SelectTree'
export default {
  components: { SelectTree },
  props: ['type', 'data'],

  watch: {
    data: {
      immediate: true,
      deep: true,
      async handler(newval) {
        // eslint-disable-next-line no-undef
        await this.initData(_.cloneDeep(newval))
      }
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
        classDirectoryId: '',
        code: '',
        dataSourceId: '',
        dataSourceName: '',
        name: '',
        seqNo: 0,
        tableName: '',
        remark: '',
        parentId: '',
        type: 0
      },
      formRule: {
        name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        code: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        dataSourceId: [
          { required: true, message: '数据源不能为空', trigger: 'blur' }
        ]
      },
      formType: '',
      allDirectoryData: [],
      selTreeKey: false,
      parentClassList: []
    }
  },
  methods: {
    async initData(data) {
      // eslint-disable-next-line no-undef
      this.formData = data
      this.treeData.id = data.classDirectoryId
      this.treeData.name = data.directoryName
      this.selTreeKey = !this.selTreeKey
      await this.loadDirectoryData()
      await this.loadParentClassData()
    },
    async loadDirectoryData() {
      var dataSourceId = this.formData.dataSourceId
      if (!dataSourceId) {
        console.error('数据源id不能为空')
        return false
      }
      var result = await ClassDirectoryService.getDirectoryTree({
        dataSourceId
      })
      var ret = this.$validResponse(result)
      if (ret) {
        this.allDirectoryData = ret.data
      }
    },
    async loadParentClassData() {
      var res = await ClassService.getList({
        dataSourceId: this.formData.dataSourceId,
        type: 1
      })
      var ret = this.$validResponse(res)
      if (ret) {
        this.parentClassList = ret.data
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
          res = await ClassService.addNew(that.formData)
        } else if (that.type === 'editor') {
          // 编辑
          res = await ClassService.update(that.formData)
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
      this.formData.classDirectoryId = data.id
      this.formData.parentId = data.id
    }
  },
  created: async function() {
    // await this.initData(this.data)
  }
}
</script>

<style lang="less" scoped></style>
