<template>
  <div>
    <el-row>
      <el-col :span="1.5"><span>数据源：</span></el-col>

      <el-col :span="4">
        <el-select
          size="small"
          filterable
          default-first-option
          v-model="queryDirectoryClassInfo.dataSourceId"
          @change="
            () => {
              loadDirectoryClassData()
              loadClassAttributeData()
            }
          "
          placeholder="请选择数据源"
        >
          <el-option
            v-for="item in allDataSourceDatas"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          >
            <span style="float: left">{{ item.name }}</span>
            <span style="float: right; color: #8492a6; font-size: 13px">{{
              item.code
            }}</span>
          </el-option>
        </el-select>
      </el-col>
    </el-row>
    <el-row>
      <el-col :span="5">
        <div class="moduleTreeDiv">
          <el-row>
            <el-col :span="15" :offset="1">
              <el-input
                placeholder="输入关键字进行过滤"
                size="small"
                v-model="queryDirectoryClassInfo.keyword"
              >
              </el-input>
            </el-col>
            <el-col :span="6" :offset="2">
              <el-button
                type="primary"
                size="small"
                icon="el-icon-search"
                @click="loadDirectoryClassData"
              ></el-button>
            </el-col>
          </el-row>
          <el-row class="treeTitle">
            <el-col :span="8" :offset="1">
              <span>目录类别</span>
            </el-col>
            <el-col :span="3" :offset="9">
              <el-tooltip
                class="item"
                effect="dark"
                content="新增目录"
                placement="top"
              >
                <el-button
                  type="primary"
                  size="mini"
                  icon="el-icon-folder-add"
                  @click="createdDirectoryClick(null)"
                ></el-button>
              </el-tooltip>
            </el-col>
            <el-col :span="2" >
              <el-tooltip
                class="item"
                effect="dark"
                content="新增类别"
                placement="top"
              >
                <el-button
                  type="primary"
                  size="mini"
                  icon="el-icon-circle-plus-outline"
                  @click="createdUpdateClassClick(null)"
                ></el-button>
              </el-tooltip>
            </el-col>
          </el-row>

          <el-tree
            ref="directoryClassTree"
            icon-class="el-icon-circle-plus-outline"
            :data="directoryClassData.treeData"
            :props="directoryClassData.defaultProps"
            @node-click="handleDirectoryClassNodeClick"
            default-expand-all
            check-strictly
            :expand-on-click-node="false"
            :render-content="renderContent"
          ></el-tree>
        </div>
      </el-col>

      <el-col :span="18" style="margin-left:12px">
        <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
          <el-tab-pane label="属性管理" name="first">
            <attributeList :data="attributeData"></attributeList>
          </el-tab-pane>
        </el-tabs>
      </el-col>
    </el-row>

    <!-- 新增编辑目录对话框 -->
    <el-dialog
      :close-on-click-modal="false"
      :title="updateInsertDirectoryDialog.title"
      :visible.sync="updateInsertDirectoryDialog.visible"
    >
      <classDirectoryUpdateAddFrom
        :data="updateInsertDirectoryDialog.formData"
        :type="updateInsertDirectoryDialog.type"
        @ok="updateInsertDirectoryOkClick"
        @cancel="updateInsertDirectoryDialog.visible = false"
      ></classDirectoryUpdateAddFrom>
    </el-dialog>

    <!-- 新增编辑类别对话框 -->
    <el-dialog
      :close-on-click-modal="false"
      :title="updateInsertClassDialog.title"
      :visible.sync="updateInsertClassDialog.visible"
    >
      <classUpdateAddFrom
        :data="updateInsertClassDialog.formData"
        :type="updateInsertClassDialog.type"
        @ok="updateInsertClassOkClick"
        @cancel="updateInsertClassDialog.visible = false"
      ></classUpdateAddFrom>
    </el-dialog>
  </div>
</template>

<script>
// 类别属性服务
// eslint-disable-next-line no-unused-vars
import ClassAttributeService from '@/services/dataSourceService/classAttributeService'
// 数据源服务
import DataSourceService from '@/services/dataSourceService/dataSourceService'
// 目录
import ClassDirectoryService from '@/services/dataSourceService/classDirectoryService'
// 类别
import ClassService from '@/services/dataSourceService/classService'

import classDirectoryUpdateAddFrom from './components/classDirectoryUpdateAddFrom.vue'
import classUpdateAddFrom from './components/classUpdateAddFrom.vue'
import attributeList from './components/attributeList.vue'

export default {
  // eslint-disable-next-line vue/no-unused-components
  components: {
    classDirectoryUpdateAddFrom: classDirectoryUpdateAddFrom,
    // eslint-disable-next-line vue/no-unused-components
    classUpdateAddFrom: classUpdateAddFrom,
    // eslint-disable-next-line vue/no-unused-components
    attributeList: attributeList
  },
  data() {
    return {
      // 查询目录类别对象
      queryDirectoryClassInfo: {
        dataSourceId: null,
        dataSourceName: null,
        keyword: null
      },
      // 修改或更新目录数据弹窗
      updateInsertDirectoryDialog: {
        title: '新增目录',
        visible: false,
        type: 'add',
        formData: {
          name: '',
          code: '',
          dataSourceId: '',
          parentId: '',
          seqNo: 0,
          dataSourceName: ''
        }
      },
      // 修改或更新目录数据弹窗
      updateInsertClassDialog: {
        title: '新增类别',
        visible: false,
        type: 'add',
        formData: {
          classDirectoryId: '',
          code: '',
          dataSourceId: '',
          dataSourceName: '',
          name: '',
          seqNo: 0,
          tableName: '',
          type: 0,
          parentId: ''
        }
      },
      // 目录类别数据-左侧树
      directoryClassData: {
        treeData: [],
        defaultProps: {
          children: 'childrens',
          label: (data, node) => {
            if (data.type === 1) {
              return `${data.name}(${data.code})`
            }
            return data.name
          }
        }
      },
      // 数据源数据
      allDataSourceDatas: [],
      activeName: 'first',
      attributeData: {
        classId: '',
        className: '',
        dataSourceId: ''
      }
    }
  },
  methods: {
    /**
     * @description 加载数据源数据，左上下拉框
     */
    async loadDataSourceData() {
      var result = await DataSourceService.getList()
      var ret = this.$validResponse(result)
      if (!ret) {
        return false
      }
      if (ret.data.length < 1) {
        this.$message.error('数据源为空，请先新增数据源')
        // 跳转到数据源管理
        this.router.push('/dataSourceIndex')
        return false
      }
      this.allDataSourceDatas = ret.data
      this.queryDirectoryClassInfo.dataSourceId = ret.data[0].id
    },
    /**
     * @description 加载目录及类别，左侧树
     */
    async loadDirectoryClassData() {
      var result = await ClassService.getDirectoryClass(
        this.queryDirectoryClassInfo
      )
      var ret = this.$validResponse(result)
      if (ret) {
        this.attributeData.classId = null
        this.directoryClassData.treeData = ret.data
      }
    },
    // 目录类别树单击事件
    async handleDirectoryClassNodeClick(data, node) {
      console.log(data, 'handleDirectoryClassNodeClick')
      var classId = ''
      var className = ''
      if (data.type === 1) {
        classId = data.id
        className = data.name
      }
      this.attributeData = {
        classId: classId,
        className: className,
        dataSourceId: this.queryDirectoryClassInfo.dataSourceId
      }
    },
    // 目录类别树自定义渲染--左侧树
    renderContent(h, { node, data, store }) {
      var iconhtml
      if (node.data.type === 0 && node.data.childrens.length < 1) {
        iconhtml = (
          <i style="margin-right:6px" class="el-icon-remove-outline"></i>
        )
      }
      var editorFunc =
        data.type === 0
          ? this.createdDirectoryClick
          : this.createdUpdateClassClick
      var delFunc = data.type === 0 ? this.delDirectory : this.delClass

      return (
        <span class="custom-tree-node" style="text-align: justify;">
          <span icon="el-icon-document">
            {iconhtml}
            {node.label}
          </span>
          <span style="position: absolute;right:12px;">
            <el-button
              style="padding: 6px;"
              type="primary"
              size="mini"
              icon="el-icon-edit"
              on-click={() => {
                editorFunc(data)
              }}
            ></el-button>
            <el-button
              style="padding: 6px;"
              size="mini"
              type="danger"
              icon="el-icon-delete"
              on-click={() => delFunc(data)}
            ></el-button>
          </span>
        </span>
      )
    },
    // 创建目录单机事件
    createdDirectoryClick(data) {
      this.updateInsertDirectoryDialog.title = '新增目录'
      this.updateInsertDirectoryDialog.type = 'add'
      if (data) {
        this.updateInsertDirectoryDialog.type = 'editor'
        this.updateInsertDirectoryDialog.formData = data
        this.updateInsertDirectoryDialog.title = '修改目录'
      } else {
        this.updateInsertDirectoryDialog.formData = this.$options.data().updateInsertDirectoryDialog.formData
      }
      this.updateInsertDirectoryDialog.formData.dataSourceId = this.queryDirectoryClassInfo.dataSourceId
      this.updateInsertDirectoryDialog.formData.dataSourceName = this.allDataSourceDatas[
        this.allDataSourceDatas.findIndex(
          e => e.id === this.queryDirectoryClassInfo.dataSourceId
        )
      ]?.name
      this.updateInsertDirectoryDialog.visible = true
    },
    // 创建修改类别单机事件
    async createdUpdateClassClick(data) {
      this.updateInsertClassDialog.title = '新增类别'
      this.updateInsertClassDialog.type = 'add'
      this.updateInsertClassDialog.formData = this.$options.data().updateInsertClassDialog.formData
      if (data) {
        var result = await ClassService.getId({ id: data.id, status: 0 })
        var ret = this.$validResponse(result)
        if (!ret) {
          return false
        }
        this.updateInsertClassDialog.type = 'editor'
        this.updateInsertClassDialog.formData = ret.data
        this.updateInsertClassDialog.title = '修改类别'
      }
      this.updateInsertClassDialog.formData.dataSourceId = this.queryDirectoryClassInfo.dataSourceId
      this.updateInsertClassDialog.formData.dataSourceName = this.allDataSourceDatas[
        this.allDataSourceDatas.findIndex(
          e => e.id === this.queryDirectoryClassInfo.dataSourceId
        )
      ]?.name
      this.updateInsertClassDialog.visible = true
    },
    // 更新或新增目录确认按钮单击事件
    async updateInsertDirectoryOkClick() {
      this.updateInsertDirectoryDialog.visible = false
      await this.loadDirectoryClassData()
    },
    // 更新或新增类别确认按钮单击事件
    async updateInsertClassOkClick() {
      this.updateInsertClassDialog.visible = false
      await this.loadDirectoryClassData()
    },
    // 删除目录
    async delDirectory(data) {
      var ret = await this.$delConfirm(() =>
        ClassDirectoryService.delete(data.id)
      )
      if (ret) {
        await this.loadDirectoryClassData()
      }
    },
    // 删除类别
    async delClass(data) {
      var ret = await this.$delConfirm(() => ClassService.delete(data.id))
      if (ret) {
        await this.loadDirectoryClassData()
      }
    },
    handleClick(tab, event) {
      console.log(tab, event)
    }
  },
  mounted: async function() {
    await this.loadDataSourceData()
    await this.loadDirectoryClassData()
  }
}
</script>

<style lang="less" scoped>
.moduleTreeDiv {
  height: 80vh;
  border-radius: 3px;
  border: 1px solid gainsboro;
  padding-top: 24px;
  margin-bottom: 12px;
  background-color: white;
}
.directoryClassFilterInput {
  width: 68% !important;
}
.directorySearchButton {
  margin-left: 12px;
}
.el-tree {
  font-size: 12px;
}
.treeTitle {
  background-color: #f2f2f2;
  height: 39px;
  line-height: 36px;
  color: black !important;
}
.el-card {
  height: 75vh;
}
.el-pagination {
  margin-top: 12px;
}
.el-button {
  padding: 7px 7px;
}
</style>
