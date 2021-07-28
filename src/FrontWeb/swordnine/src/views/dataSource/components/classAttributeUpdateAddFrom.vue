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
      <el-form-item label="类别名称">
        <el-input disabled size="small" v-model="formData.className"></el-input>
      </el-form-item>
      <el-form-item label="序号" :required="true" prop="seqNo">
        <el-input
          size="small"
          v-model.number="formData.seqNo"
          autocomplete="off"
        ></el-input>
      </el-form-item>
      <el-form-item label="名称" :required="true" prop="name">
        <el-input size="small" v-model="formData.name"></el-input>
      </el-form-item>
      <el-form-item label="编码" :required="true" prop="code">
        <el-input size="small" v-model="formData.code"></el-input>
      </el-form-item>
      <el-form-item label="列名" :required="true" prop="columnName">
        <el-input size="small" v-model="formData.columnName"></el-input>
      </el-form-item>
      <el-form-item>
        <el-switch
          size="small"
          v-model="formData.isPrimary"
          active-color="#13ce66"
          inactive-color="#ff4949"
          active-text="是否主键"
        ></el-switch>
        <el-switch
        style="margin-left:12px"
          size="small"
          v-model="formData.isNullable"
          active-color="#13ce66"
          inactive-color="#ff4949"
          active-text="非空"
        ></el-switch>
      </el-form-item>
      <el-form-item label="值类型" :required="true" prop="valueType">
        <el-select
          size="small"
          v-model="formData.valueType"
          clearable
          placeholder="请选择"
        >
          <el-option
            v-for="item in valueTypeDataList"
            :key="item.value"
            :label="item.name"
            :value="item.value"
          >
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item
        label="引用数据"
        v-show="formData.valueType === 4"
        prop="referenceId"
      >
        <select-tree
          :props="propsConfig"
          :options="allDirectoryData"
          :data="treeData"
          :clearable="false"
          :accordion="true"
          @getValue="getParentValue($event)"
        />
      </el-form-item>
      <el-form-item
        label="长度"
        :required="formData.valueType === 0"
        prop="length"
      >
        <el-input
          size="small"
          :disabled="formData.valueType !== 0"
          v-model.number="formData.length"
          autocomplete="off"
        ></el-input>
      </el-form-item>
      <el-form-item label="默认值" prop="defaultValue">
        <el-input
          size="small"
          :disabled="formData.valueType === 4"
          v-model="formData.defaultValue"
        ></el-input>
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input
          size="small"
          type="textarea"
          v-model="formData.description"
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
import ClassAttributeService from '@/services/dataSourceService/classAttributeService'
// eslint-disable-next-line no-unused-vars
import ClassService from '@/services/dataSourceService/classService'

import SelectTree from '@/components/ElementUI/SelectTree'
export default {
  props: ['type', 'data'],
  components: { SelectTree },
  watch: {
    data: {
      immediate: true,
      deep: true,
      handler(newval, oldval) {
        // eslint-disable-next-line no-undef
        this.initData(_.cloneDeep(newval))
      }
    },
    type(n, o) {
      // n为新值,o为旧值;
      this.type = n
    },
    async 'formData.valueType'(n, o) {
      console.log(n)
      if (n === 4 || n === 5) {
        this.formData.length = 36
        await this.loadDirectoryData()
      } else {
        this.formData.length = 0
      }
    }
  },
  data() {
    return {
      // 表单数据
      formData: {
        classId: '',
        className: '',
        code: '',
        columnName: '',
        defaultValue: '',
        description: '',
        dataSourceId: '',
        referenceId: '',
        isNullable: true,
        isPrimary: false,
        length: 0,
        name: '',
        remark: '',
        seqNo: 0,
        valueType: 0
      },
      // 表单规则
      formRule: {
        name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        code: [{ required: true, message: '请输入编码', trigger: 'blur' }]
      },
      // 下拉树组件配置
      propsConfig: {
        value: 'id', // ID字段名
        label: (data, node) => {
          if (data.type === 1) {
            return `${data.name}(${data.code})`
          }
          return data.name
        }, // 显示名称
        children: 'childrens', // 子级字段名
        disabled: 'type'
      },
      // 下拉树数据
      allDirectoryData: [],
      // 值类型数据
      valueTypeDataList: [
        { value: 0, name: '字符串' },
        { value: 1, name: '整数' },
        { value: 2, name: '小数' },
        { value: 3, name: '时间' },
        { value: 4, name: '引用' }
      ],
      // 下拉树传入的数据
      treeData: { id: '', name: '' }
    }
  },
  methods: {
    async initData(data) {
      // eslint-disable-next-line no-undef
      this.formData = _.cloneDeep(data)
      this.treeData.id = this.formData.referenceId
      this.treeData.name = `${this.formData.referenceClassName}(${this.formData.referenceClassCode})`
      await this.loadDirectoryData()
    },
    async loadDirectoryData() {
      var dataSourceId = this.formData.dataSourceId
      if (!dataSourceId) {
        console.error('数据源id不能为空')
        this.$message.error('数据源id不能为空')
        return false
      }
      var result = await ClassAttributeService.getDirectoryClass({
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
          res = await ClassAttributeService.addNew(that.formData)
        } else if (that.type === 'editor') {
          // 编辑
          res = await ClassAttributeService.update(that.formData)
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
      this.formData.referenceId = data.id
    }
  },
  created: async function() {
    await this.initData(this.data)
  }
}
</script>

<style lang="less" scoped></style>
