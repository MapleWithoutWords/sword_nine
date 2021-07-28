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
            type="primary"
            size="small"
            @click="addRow()"
            icon="el-icon-circle-plus-outline"
            >添加一行</el-button
          >
        </el-col>
        <!-- <el-col :span="2">
          <el-button
            type="primary"
            size="small"
            @click="createClassAttributeClick()"
            icon="el-icon-circle-plus-outline"
            >添加</el-button
          >
        </el-col> -->
        <el-col :span="2" style="margin-left:12px">
          <el-button
            type="primary"
            size="small"
            @click="saveAllData()"
            icon="el-icon-save"
            >保存</el-button
          >
        </el-col>
      </el-row>

      <!-- 表格区域-->
      <el-table
        :data="classAttributeDatas"
        :stripe="true"
        :border="true"
        max-height="600"
      >
        <el-table-column type="selection"></el-table-column>
        <el-table-column label="所属类别" fixed>{{
          queryClassAttributeInfo.className
        }}</el-table-column>
        <el-table-column label="序号">
          <template slot-scope="scope">
            <div>
              <el-input v-model.number="scope.row.seqNo"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="名称">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.name" :disabled="scope.row.inheritId!==''"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="编码">
          <template slot-scope="scope">
            <div>
              <el-input
               :disabled="scope.row.inheritId!==''"
                v-model="scope.row.code"
                @change="
                  newval => {
                    codeChange(scope.row, newval)
                  }
                "
              ></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="列名">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.columnName" :disabled="scope.row.inheritId!==''"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="主键?">
          <template slot-scope="scope">
            <div>
              <el-switch
               :disabled="scope.row.inheritId!==''"
                v-model="scope.row.isPrimary"
                active-color="#13ce66"
                inactive-color="#ff4949"
              >
              </el-switch>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="非空?">
          <template slot-scope="scope">
            <div>
              <el-switch
               :disabled="scope.row.inheritId!==''"
                v-model="scope.row.isNullable"
                active-color="#13ce66"
                inactive-color="#ff4949"
              >
              </el-switch>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="值类型">
          <template slot-scope="scope">
            <div>
              <el-select v-model="scope.row.valueType" placeholder="请选择"  :disabled="scope.row.inheritId!==''">
                <el-option
                  v-for="item in valueTypeDataList"
                  :key="item.value"
                  :label="item.name"
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="引用类别">
          <template slot-scope="scope">
            <div>
              <select-tree
                v-show="scope.row.valueType === 4"
                 :disabled="scope.row.inheritId!==''"
                :props="propsConfig"
                :options="allDirectoryData"
                :data="{
                  id: scope.row.referenceId,
                  name: scope.row.referenceClassName
                }"
                :clearable="false"
                :accordion="true"
                @getValue="$event => getParentValue($event, scope.row)"
              />
            </div>
          </template>
        </el-table-column>
        <el-table-column label="长度">
          <template slot-scope="scope">
            <div>
              <el-input
                :disabled="
                  scope.row.inheritId!==''||(scope.row.valueType !== 0 && scope.row.valueType !== 4)
                "
                v-model.number="scope.row.length"
              ></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="默认值">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.defaultValue" :disabled="scope.row.inheritId!==''"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="备注">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.remark" :disabled="scope.row.inheritId!==''"></el-input>
            </div>
          </template>
        </el-table-column>
        <!--操作列-->
        <el-table-column label="操作" fixed="right">
          <template slot-scope="scope">
            <el-button
            :disabled="scope.row.inheritId!==''"
              type="danger"
              size="mini"
              @click="
                () => {
                  classAttributeDatas = classAttributeDatas.filter(
                    e => e.id != scope.row.id
                  )
                }
              "
              icon="el-icon-delete"
              circle
            ></el-button>
            <!-- <el-button
              type="primary"
              size="mini"
              icon="el-icon-edit"
              @click="editorClassAttributeClick(scope.$index, scope.row)"
              circle
            ></el-button>
            <el-button
              type="danger"
              size="mini"
              @click="delClassAttributeData(scope.$index, scope.row)"
              icon="el-icon-delete"
              circle
            ></el-button> -->
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增编辑属性对话框 -->
    <el-dialog
      :close-on-click-modal="false"
      :title="updateInsertClassAttributeDialog.title"
      :visible.sync="updateInsertClassAttributeDialog.visible"
    >
      <classAttributeUpdateAddFrom
        :data="updateInsertClassAttributeDialog.formData"
        :type="updateInsertClassAttributeDialog.type"
        @ok="updateInsertClassAttributeOkClick"
        @cancel="updateInsertClassAttributeDialog.visible = false"
      ></classAttributeUpdateAddFrom>
    </el-dialog>
  </div>
</template>

<script>
// 类别属性服务
import ClassAttributeService from '@/services/dataSourceService/classAttributeService'
import ClassService from '@/services/dataSourceService/classService'
import classAttributeUpdateAddFrom from './classAttributeUpdateAddFrom.vue'
import SelectTree from '@/components/ElementUI/SelectTree'
export default {
  // eslint-disable-next-line vue/no-unused-components
  components: {
    // eslint-disable-next-line vue/no-unused-components
    classAttributeUpdateAddFrom: classAttributeUpdateAddFrom,
    SelectTree
  },
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
        dataSourceId: '',
        sort: 'seqNo asc'
      },
      // 修改或更新属性数据弹窗
      updateInsertClassAttributeDialog: {
        title: '新增属性',
        visible: false,
        type: 'add',
        formData: {
          classId: '',
          className: '',
          code: '',
          columnName: '',
          dataSourceId: '',
          defaultValue: '',
          description: '',
          referenceId: '',
          referenceClassName: '',
          referenceClassCode: '',
          isNullable: true,
          isPrimary: false,
          length: 0,
          name: '',
          remark: '',
          inheritId: '',
          seqNo: 0,
          valueType: 0
        }
      },
      // 下拉树数据
      allDirectoryData: [],
      // 类别属性数据
      classAttributeDatas: [],
      // 值类型数据
      valueTypeDataList: [
        { value: 0, name: '字符串' },
        { value: 1, name: '整数' },
        { value: 2, name: '小数' },
        { value: 3, name: '时间' },
        { value: 4, name: '引用' },
        { value: 5, name: '长整数' },
        { value: 6, name: '双精度小数' },
        { value: 7, name: '布尔' },
        { value: 8, name: 'Decimal' }
      ],
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
      }
    }
  },
  methods: {
    async initData(data) {
      this.queryClassAttributeInfo.classId = data.classId
      this.queryClassAttributeInfo.className = data.className
      this.queryClassAttributeInfo.dataSourceId = data.dataSourceId
      await this.loadClassAttributeData()
      await this.loadDirectoryData()
    },
    async loadDirectoryData() {
      var dataSourceId = this.queryClassAttributeInfo.dataSourceId
      if (!dataSourceId) {
        console.error('数据源id不能为空')
        // this.$message.error('数据源id不能为空')
        return false
      }
      var result = await ClassService.getDirectoryClass({
        dataSourceId: dataSourceId
      })
      var ret = this.$validResponse(result)
      if (ret) {
        this.allDirectoryData = ret.data
      }
    },
    /**
     * @description 加载类别属性数据，右侧列表
     */
    async loadClassAttributeData() {
      if (!this.queryClassAttributeInfo.classId) {
        // this.$message.error('请选择一个类别')
        this.classAttributeDatas = []
        return false
      }
      var result = await ClassAttributeService.getList(
        this.queryClassAttributeInfo
      )
      var ret = this.$validResponse(result)
      if (ret) {
        this.classAttributeDatas = ret.data
      }
    },
    getParentValue(data, row) {
      console.log(data)
      row.referenceId = data.id
    },
    /**
     * @description 新增类别属性按钮单击事件
     */
    async createClassAttributeClick() {
      console.log(this.queryClassAttributeInfo)
      console.log(this.data)
      console.log(this.updateInsertClassAttributeDialog.formData)
      this.updateInsertClassAttributeDialog.formData = this.$options.data().updateInsertClassAttributeDialog.formData
      this.updateInsertClassAttributeDialog.formData.classId = this.queryClassAttributeInfo.classId
      this.updateInsertClassAttributeDialog.formData.className = this.queryClassAttributeInfo.className
      this.updateInsertClassAttributeDialog.formData.dataSourceId = this.queryClassAttributeInfo.dataSourceId
      if (!this.updateInsertClassAttributeDialog.formData.classId) {
        this.$message.error('请选择类别')
        return false
      }

      this.updateInsertClassAttributeDialog.visible = true
      this.updateInsertClassAttributeDialog.title = '新增属性'
      this.updateInsertClassAttributeDialog.type = 'add'
    },
    async editorClassAttributeClick(idx, rowData) {
      this.updateInsertClassAttributeDialog.visible = true
      this.updateInsertClassAttributeDialog.title = '编辑属性'
      this.updateInsertClassAttributeDialog.type = 'editor'
      // eslint-disable-next-line no-undef
      this.updateInsertClassAttributeDialog.formData = rowData
      this.updateInsertClassAttributeDialog.formData.dataSourceId = this.queryClassAttributeInfo.dataSourceId
      this.updateInsertClassAttributeDialog.formData.classId = this.queryClassAttributeInfo.classId
      this.updateInsertClassAttributeDialog.formData.className = this.queryClassAttributeInfo.className
    },

    // 更新或新增属性确认按钮单击事件
    async updateInsertClassAttributeOkClick() {
      this.updateInsertClassAttributeDialog.visible = false
      await this.loadClassAttributeData()
    },
    async delClassAttributeData(idx, rowData) {
      var ret = await this.$delConfirm(() =>
        ClassAttributeService.delete(rowData.id)
      )
      if (ret) {
        await this.loadClassAttributeData()
      }
    },
    // 删除属性
    async delAttribute(id) {
      await this.$delConfirm(() => ClassAttributeService.delete(id))
    },
    // 保存属性数据
    async saveAllData() {
      console.log(this.classAttributeDatas)
      var result = await ClassAttributeService.save(this.classAttributeDatas)
      var ret = this.$validResponse(result)
      if (!ret) {
        return false
      }
      this.$message('保存成功')
      this.$emit('ok', ret.data)
    },
    addRow() {
      this.classAttributeDatas.push({
        classId: this.queryClassAttributeInfo.classId,
        className: this.queryClassAttributeInfo.className,
        code: '',
        columnName: '',
        defaultValue: '',
        description: '',
        dataSourceId: this.queryClassAttributeInfo.dataSourceId,
        referenceId: '',
        isNullable: true,
        isPrimary: false,
        length: 0,
        name: '',
        remark: '',
        inheritId: '',
        seqNo: this.classAttributeDatas.length + 1,
        valueType: 0
      })
    },
    codeChange(row, newval) {
      row.columnName = newval
    }
  },
  created: async function() {
    // eslint-disable-next-line no-undef
    await this.initData(_.cloneDeep(this.data))
  }
}
</script>

<style lang="less" scoped></style>
