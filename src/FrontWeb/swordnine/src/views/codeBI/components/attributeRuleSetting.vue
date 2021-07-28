<template>
  <div>
    <el-card>
      <!--搜索-->
      <el-row :gutter="12">
        <el-col :span="4">
          <el-input
            size="small"
            v-model="queryClassAttributeInfo.keyword"
            placeholder="请输入名称或编码"
          ></el-input>
        </el-col>
        <el-col :span="2">
          <el-button
            type="primary"
            @click="loadClassAttributeData()"
            icon="el-icon-search"
            size="small"
            >搜索</el-button
          >
        </el-col>
        <el-col :span="2" :offset="13">
          <el-button
            type="success"
            @click="
              () => {
                codeGenDialog.visible = true
              }
            "
            >生成代码</el-button
          >
        </el-col>
        <el-col :span="2" :offset="1">
          <el-button
            v-if="queryClassAttributeInfo.classId"
            type="primary"
            @click="saveRuleSetting"
            >保存</el-button
          >
        </el-col>
      </el-row>

      <!-- 表格区域-->
      <el-table :data="classAttributeDatas" :stripe="true" :border="true">
        <el-table-column label="所属类别">{{
          queryClassAttributeInfo.className
        }}</el-table-column>
        <el-table-column prop="attributeName" label="名称"></el-table-column>
        <el-table-column prop="attributeCode" label="编码"></el-table-column>
        <el-table-column
          :key="item.id"
          v-for="(item, index) in ruleList"
          :label="item.name"
        >
          <template slot-scope="scope">
            <el-switch
              v-if="item.valueType == 0"
              v-model="scope.row.ruleValues[index].value"
              active-value="true"
              inactive-value="false"
            ></el-switch>
            <el-input
              v-else-if="item.valueType == 1"
              v-model="scope.row.ruleValues[index].value"
            ></el-input>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 代码生成确认对话框 -->
    <el-dialog
      width="30%"
      :close-on-click-modal="false"
      :title="codeGenDialog.title"
      :visible.sync="codeGenDialog.visible"
    >
      <el-select
        v-model="codeGenDialog.selectedTemplateId"
        placeholder="请选择"
      >
        <el-option
          v-for="item in codeGenDialog.templateList"
          :key="item.id"
          :label="item.name"
          :value="item.id"
        >
          <span style="float: left">{{ item.code }}</span>
          <span style="float: right; color: #8492a6; font-size: 13px">{{
            item.name
          }}</span>
        </el-option>
      </el-select>
      <div style="margin-top:12px;">
        <el-button type="primary" @click="codeGenerateConfirmClick"
          >确认</el-button
        >
        <el-button
          @click="
            () => {
              codeGenDialog.visible = false
            }
          "
          >取消</el-button
        >
      </div>
    </el-dialog>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import RuleAttributeSettingService from '@/services/dataSourceService/ruleAttributeSettingService'
// eslint-disable-next-line no-unused-vars
import RuleService from '@/services/dataSourceService/ruleService'
// eslint-disable-next-line no-unused-vars
import DataSourceService from '@/services/dataSourceService/dataSourceService'
// eslint-disable-next-line no-unused-vars
import TemplateService from '@/services/codeBI/templateService'
export default {
  components: {},
  props: ['data'],
  watch: {
    data: {
      immediate: true,
      deep: true,
      async handler(newval) {
        // eslint-disable-next-line no-undef
        await this.initData(_.cloneDeep(newval))
      }
    }
  },
  data() {
    return {
      // 查询类别属性
      queryClassAttributeInfo: {
        classId: null,
        className: null,
        keyword: null,
        dataSourceId: ''
      },
      classAttributeDatas: [],
      // 属性已配置规则列表数据源
      attributeRuleList: [],
      ruleList: [],
      codeGenDialog: {
        title: '选择模板',
        visible: false,
        templateList: [],
        selectedTemplateId: ''
      }
    }
  },
  methods: {
    async initData(data) {
      await this.loadRuleData()
      this.queryClassAttributeInfo.classId = data.classId
      this.queryClassAttributeInfo.className = data.className
      this.queryClassAttributeInfo.dataSourceId = data.dataSourceId
      await this.loadClassAttributeData()
    },
    async loadTemplateList() {
      var result = await TemplateService.getList({})
      var ret = this.$validResponse(result)
      if (ret) {
        this.codeGenDialog.templateList = ret.data
      }
    },
    // 加载类别属性
    async loadClassAttributeData() {
      if (!this.queryClassAttributeInfo.classId) {
        // this.$message.error('请选择一个类别')
        this.classAttributeDatas = []
        return false
      }
      var result = await RuleAttributeSettingService.queryAttrRuleSetting(
        this.queryClassAttributeInfo
      )
      var ret = this.$validResponse(result)
      if (ret) {
        this.classAttributeDatas = ret.data
      }
    },
    // 加载所有规则
    async loadRuleData() {
      var result = await RuleService.getList({})
      var ret = this.$validResponse(result)
      if (ret) {
        this.ruleList = ret.data
      }
    },
    async saveRuleSetting() {
      console.log(this.classAttributeDatas)
      var result = await RuleAttributeSettingService.updateOrInsert(
        this.classAttributeDatas
      )
      var ret = this.$validResponse(result)
      if (ret) {
        this.$message({
          showClose: true,
          message: '操作成功',
          type: 'success'
        })
        this.loadClassAttributeData()
      }
    },
    async codeGenerateConfirmClick() {
      if (!this.codeGenDialog.selectedTemplateId) {
        this.$message.error('请选择模板')
        return false
      }
      var result = await DataSourceService.generatorCode({
        dataSourceId: this.queryClassAttributeInfo.dataSourceId,
        templateId: this.codeGenDialog.selectedTemplateId
      })
      var ret = this.$validResponse(result)
      if (ret) {
        this.$message.success('生成成功')
        debugger
        window.open(window.apiInstance.defaults.baseURL + ret.data)
        this.codeGenDialog.visible = false
      }
    }
  },
  created: async function() {
    // eslint-disable-next-line no-undef
    await this.initData(_.cloneDeep(this.data))
    await this.loadTemplateList()
  }
}
</script>

<style lang="less" scoped>
.moduleTreeDiv {
  height: 70vh;
  border-radius: 3px;
  border: 1px solid gainsboro;
  padding-top: 24px;
  margin-bottom: 12px;
  background-color: white;
}

.treeTitle {
  background-color: #f2f2f2;
  height: 39px;
  line-height: 36px;
  color: black !important;
}
.el-table {
  height: 72vh;
  overflow: scroll;
}
</style>
