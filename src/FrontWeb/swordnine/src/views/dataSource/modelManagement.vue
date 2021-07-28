<template>
  <div style="height: 100%">
    <el-row>
      <el-col :span="1.5"><span>数据源：</span></el-col>

      <el-col :span="4">
        <el-select
          size="small"
          filterable
          default-first-option
          v-model="queryDirectoryInfo.dataSourceId"
          @change="dataSourceChange"
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
      <el-col :span="isCollapse ? 0.1 : 3">
        <div class="moduleTreeDiv">
          <div style="margin-bottom: 12px;">
            <i class="iconfont icon-menu" @click="collapseClick"></i>
          </div>
          <el-row v-show="!isCollapse">
            <el-col :span="20" :offset="1">
              <el-input
                placeholder="输入关键字进行过滤"
                size="small"
                v-model="queryDirectoryInfo.keyword"
                @keydown.native="directoryClassSearch"
                clearable
              >
              </el-input>
            </el-col>
          </el-row>
          <el-row class="treeTitle" v-show="!isCollapse">
            <el-col :span="8" :offset="1">
              <span>目录</span>
            </el-col>
            <el-col :span="4" :offset="9" v-show="!isCollapse">
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
          </el-row>
          <el-tree
            v-show="!isCollapse"
            ref="directoryTree"
            icon-class="el-icon-folder"
            :filter-node-method="filterNode"
            :data="allDirectoryData"
            :props="defaultProps"
            @node-click="handleDirectoryClassNodeClick"
            :render-content="renderContent"
            check-strictly
            :draggable="true"
            :allow-drag="node => node.data.type === 1"
            :allow-drop="() => false"
            @node-drag-start="handleDragStart"
            :expand-on-click-node="false"
            @node-drag-end="
              () => {
                this.showModal = false
              }
            "
            highlight-current
            :current-node-key="selDirectoryId"
          ></el-tree>
        </div>
      </el-col>
      <el-col
        :span="isCollapse ? 23 : 20"
        style="margin-left:24px;height: 85vh;"
      >
        <div
          @drop="handleTargetDrop"
          @dragover="handleTargetDragOver"
          v-if="showModal"
          id="dragModal"
          class="mask"
        ></div>
        <iframe
          @load="iframeLoad"
          src="graphEditor/index.html"
          ref="graphframe"
          id="graphframe"
          style="width: 100%; height: 100%"
          frameborder="0"
        ></iframe>
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
    <!-- 新增编辑类别及类别属性对话框 -->
    <el-dialog
      :close-on-click-modal="false"
      :title="updateInsertClassDialog.title"
      :visible.sync="updateInsertClassDialog.visible"
      width="70%"
      top="5vh"
    >
      <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
        <el-tab-pane label="类别信息" name="first">
          <classUpdateAddFrom
            :data="updateInsertClassDialog.classInfo.formData"
            :type="updateInsertClassDialog.classInfo.type"
            @ok="updateInsertClassOkClick"
            @cancel="updateInsertClassDialog.visible = false"
          ></classUpdateAddFrom>
        </el-tab-pane>
        <el-tab-pane label="属性信息" name="second">
          <attributeList
            :data="updateInsertClassDialog.attributeData"
            @ok="saveAttributeClick"
          ></attributeList>
        </el-tab-pane>
      </el-tabs>
    </el-dialog>
  </div>
</template>
<script>
import attributeList from './components/attributeList.vue'
import classUpdateAddFrom from './components/classUpdateAddFrom.vue'
// eslint-disable-next-line no-unused-vars
import ClassDirectoryService from '@/services/dataSourceService/classDirectoryService'
// 数据源服务
import DataSourceService from '@/services/dataSourceService/dataSourceService'
// 类别
import ClassService from '@/services/dataSourceService/classService'
import classDirectoryUpdateAddFrom from './components/classDirectoryUpdateAddFrom.vue'
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
      queryDirectoryInfo: {
        dataSourceId: null,
        keyword: null
      },
      allDataSourceDatas: [],
      drawData: {
        dataId: '',
        leftValue: '',
        value: '',
        children: []
      },
      selDirectoryId: '',
      selDirectoryName: '',
      allDirectoryData: [],
      defaultProps: {
        value: 'id', // ID字段名
        label: (data, node) => {
          if (data.type === 1) {
            return `${data.name}(${data.code})`
          }
          return data.name
        }, // 显示名称
        children: 'childrens', // 子级字段名
        disabled: (data, node) => data.type === 0
      },
      // 修改或更新目录数据弹窗
      updateInsertClassDialog: {
        title: '类别信息',
        visible: false,
        classInfo: {
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
        attributeData: {
          classId: '',
          className: '',
          dataSourceId: ''
        }
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
          dataSourceName: '',
          content: ''
        }
      },
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
      activeName: 'first',
      showModal: false,
      isCollapse: false
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
      this.queryDirectoryInfo.dataSourceId = ret.data[0].id
    },
    // 目录类别树单击事件
    async handleDirectoryClassNodeClick(data, node) {
      if (data.type === 0) {
        var result = await this.getDrawEditorStatus()
        if (result) {
          return false
        }
        this.selDirectoryId = data.id
        this.selDirectoryName = data.name
        await this.renderDrawView()
      }
    },
    // 重新渲染整个绘图
    async renderDrawView() {
      if (!this.selDirectoryId) {
        this.$message.info('请选择目录')
        return false
      }
      var result = await ClassDirectoryService.getId({
        id: this.selDirectoryId
      })
      var ret = this.$validResponse(result)
      if (ret && ret.data) {
        this.$nextTick(() => {
          var graphframe = document.getElementById('graphframe')
          // 通过ifrmae的contentWindow获取到iframe的window对象，通过postmessage向其发送数据消息
          if (graphframe.contentWindow.reRenderDrawing) {
            graphframe.contentWindow.reRenderDrawing(ret.data.content)
          }
        })
      }
    },
    // 加载目录类别树
    async loadDirectoryData() {
      var dataSourceId = this.queryDirectoryInfo.dataSourceId
      if (!dataSourceId) {
        console.error('数据源id不能为空')
        this.$message.error('数据源id不能为空')
        return false
      }
      var result = await ClassService.getDirectoryClass(this.queryDirectoryInfo)
      var ret = this.$validResponse(result)
      if (ret) {
        this.allDirectoryData = ret.data
        if (ret.data && ret.data.length > 0 && !this.selDirectoryId) {
          if (await this.getDrawEditorStatus()) {
            return false
          }
          this.selDirectoryId = ret.data[0].id
          this.selDirectoryName = ret.data[0].name
        }
      }
    },
    // 弹窗tab切换
    handleClick(tab, event) {
      console.log(tab, event)
    },
    // 新增修改类别
    async updateInsertClassOkClick(data) {
      this.drawData.dataId = data.id
      this.drawData.value = `${data.name}(${data.code})`
      await this.updateClassAttrInfo(data.id)
      var graphframe = document.getElementById('graphframe')
      // 通过ifrmae的contentWindow获取到iframe的window对象，通过postmessage向其发送数据消息
      graphframe.contentWindow.updateClassCell(this.drawData)
      this.loadDirectoryData()
      this.$message.success('操作成功')
    },
    // 保存属性
    saveAttributeClick(data) {
      data = data || []
      var list = data.map(e => {
        var leftval = ''
        if (e.isPrimary) {
          leftval = 'pk'
        } else if (e.valueType === 4) {
          leftval = 'fk'
        }
        var valueTypeObj = this.valueTypeDataList.find(
          x => x.value === e.valueType
        )
        var valueTypeName = '未知的值类型'
        if (valueTypeObj) {
          valueTypeName = valueTypeObj.name
        }
        return {
          dataId: e.id,
          valueList: [
            e.seqNo,
            `${e.name}(${e.code})`,
            `${valueTypeName}${e.length > 0 ? `(${e.length})` : ''}`,
            leftval
          ]
        }
      })
      var graphframe = document.getElementById('graphframe')
      // 通过ifrmae的contentWindow获取到iframe的window对象，通过postmessage向其发送数据消息
      graphframe.contentWindow.updateAttributeCell(list)
      // this.updateInsertClassDialog.visible = false
    },
    // 数据源下拉列表变化
    async dataSourceChange() {
      await this.loadDirectoryData()
      if (this.allDirectoryData.length < 1) {
        this.$message.error('请先添加目录')
        return false
      }
      if (await this.getDrawEditorStatus()) {
        return false
      }
      this.selDirectoryId = this.allDirectoryData[0].id
      this.selDirectoryName = this.allDirectoryData[0].name
      await this.renderDrawView()
    },
    // 图元中cell的单击事件
    async cellClick(data) {
      // 绘图中的cell单击事件
      // console.log(data)
      if (data) {
        await this.updateClassAttrInfo(data.dataId)
        this.updateInsertClassDialog.visible = true
      }
    },
    async updateClassAttrInfo(classId) {
      this.updateInsertClassDialog.classInfo.type = 'add'
      this.updateInsertClassDialog.classInfo.formData = this.$options.data().updateInsertClassDialog.classInfo.formData
      this.updateInsertClassDialog.attributeData.classId = ''
      this.updateInsertClassDialog.attributeData.className = ''
      if (classId) {
        var result = await ClassService.getId({ id: classId, status: 0 })
        var ret = this.$validResponse(result)
        if (!ret) {
          return false
        }
        this.updateInsertClassDialog.attributeData.classId = ret.data.id
        this.updateInsertClassDialog.attributeData.className = ret.data.name
        this.updateInsertClassDialog.classInfo.formData = ret.data
        this.updateInsertClassDialog.classInfo.type = 'editor'
      }
      this.updateInsertClassDialog.attributeData.dataSourceId = this.queryDirectoryInfo.dataSourceId

      this.updateInsertClassDialog.classInfo.formData.dataSourceId = this.queryDirectoryInfo.dataSourceId
      this.updateInsertClassDialog.classInfo.formData.dataSourceName = this.allDataSourceDatas[
        this.allDataSourceDatas.findIndex(
          e => e.id === this.queryDirectoryInfo.dataSourceId
        )
      ]?.name
      this.updateInsertClassDialog.classInfo.formData.classDirectoryId =
        this.updateInsertClassDialog.classInfo.formData.classDirectoryId ||
        this.selDirectoryId
      this.updateInsertClassDialog.classInfo.formData.directoryName =
        this.updateInsertClassDialog.classInfo.formData.directoryName ||
        this.selDirectoryName
    },
    // 保存绘图
    async saveDrawing(data) {
      if (!this.selDirectoryId) {
        this.$message.error('请选择目录')
        return false
      }
      data = data || ''
      // 保存整个绘图的内容
      var result = await ClassDirectoryService.saveDrawing({
        directoryId: this.selDirectoryId,
        content: data
      })
      var ret = this.$validResponse(result)
      if (!ret) {
        return false
      }
      var graphframe = document.getElementById('graphframe')
      graphframe.contentWindow.updateEditorStatus(false)
      this.$message('保存成功')
    },
    // 处理类别拖拽
    async handleTargetDrop(ent) {
      this.showModal = false
      var dt = ent.dataTransfer
      var classId = dt.getData('classId')
      console.log(ent, classId)
      var result = await ClassService.getInfo(classId)
      var ret = this.$validResponse(result)
      if (!ret) {
        return false
      }
      var classInfo = ret.data

      var list = classInfo.classAttributeList.map(e => {
        var leftval = ''
        if (e.isPrimary) {
          leftval = 'pk'
        } else if (e.valueType === 4) {
          leftval = 'fk'
        }
        var valueTypeObj = this.valueTypeDataList.find(
          x => x.value === e.valueType
        )
        var valueTypeName = '未知的值类型'
        if (valueTypeObj) {
          valueTypeName = valueTypeObj.name
        }
        return {
          dataId: e.id,
          valueList: [
            e.seqNo,
            `${e.name}(${e.code})`,
            `${valueTypeName}${e.length > 0 ? `(${e.length})` : ''}`,
            leftval
          ]
        }
      })

      var graphframe = document.getElementById('graphframe')
      // 通过ifrmae的contentWindow获取到iframe的window对象，通过postmessage向其发送数据消息
      graphframe.contentWindow.quicklyAddClass(
        {
          dataId: classInfo.id,
          value: `${classInfo.name}(${classInfo.code})`
        },
        list
      )
    },
    // 允许某区域拖拽
    handleTargetDragOver(e) {
      e.preventDefault() // 使该区域允许释放
    },
    // 类别拖拽开始
    handleDragStart(node, ev) {
      var dt = ev.dataTransfer
      ev.dataTransfer.effectAllowed = 'copy'
      dt.setData('classId', node.data.id)
      this.showModal = true
      console.log(this.$refs.graphframe)
      // var dragModal = document.getElementById('dragModal')
      // dragModal.style.width = '1077px'
    },
    // mxgraph窗口加载
    iframeLoad() {
      this.$nextTick(async () => {
        await this.renderDrawView()
      })
    },
    // 获取绘图编辑状态
    async getDrawEditorStatus() {
      var graphframe = document.getElementById('graphframe')
      if (graphframe.contentWindow.IsEditored) {
        var result = await this.$ConfirmMsg(
          '你有操作未保存，确认切换视图吗？将会导致您所做的操作无效'
        )
        return !result
      }
      return false
    },
    // 目录类别树自定义渲染--左侧树
    renderContent(h, { node, data, store }) {
      if (data.type === 1) {
        return (
          <span
            class="custom-tree-node"
            style="text-align: justify;font-size: 14px;"
          >
            <span icon="el-icon-document">{node.label}</span>
          </span>
        )
      }
      data.content = ''
      var editorFunc = this.createdDirectoryClick
      var delFunc = this.delDirectory
      return (
        <span
          class="custom-tree-node"
          style="text-align: justify;font-size: 12px;"
          v-show="!isCollapse"
        >
          <span icon="el-icon-document">{node.label}({data.childrens.length})</span>
          <span style="position: absolute;right:14px;">
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
    // 删除目录
    async delDirectory(data) {
      var ret = await this.$delConfirm(() =>
        ClassDirectoryService.delete(data.id)
      )
      if (ret) {
        await this.loadDirectoryClassData()
      }
    },
    // 创建目录单机事件
    createdDirectoryClick(data) {
      debugger
      this.updateInsertDirectoryDialog.title = '新增目录'
      this.updateInsertDirectoryDialog.type = 'add'
      if (data) {
        this.updateInsertDirectoryDialog.type = 'editor'
        this.updateInsertDirectoryDialog.formData = data
        this.updateInsertDirectoryDialog.title = '修改目录'
      } else {
        this.updateInsertDirectoryDialog.formData = this.$options.data().updateInsertDirectoryDialog.formData
      }
      this.updateInsertDirectoryDialog.formData.dataSourceId = this.queryDirectoryInfo.dataSourceId
      this.updateInsertDirectoryDialog.formData.dataSourceName = this.allDataSourceDatas[
        this.allDataSourceDatas.findIndex(
          e => e.id === this.queryDirectoryInfo.dataSourceId
        )
      ]?.name
      this.updateInsertDirectoryDialog.visible = true
    },
    // 更新或新增目录确认按钮单击事件
    async updateInsertDirectoryOkClick() {
      this.updateInsertDirectoryDialog.visible = false
      await this.loadDirectoryData()
    },
    // 删除类别
    async delClass(ids) {
      var result = await this.$msgbox
        .confirm('是否同时删除类别', '提示', {
          confirmButtonText: '不删除类别',
          cancelButtonText: '删除类别',
          type: 'warning'
        })
        .catch(error => error)
      if (result === 'confirm') {
        return false
      }
      var res = await ClassService.deleteIds(ids)
      var ret = this.$validResponse(res)
      if (ret && ret.code === 0) {
        await this.loadDirectoryData()
      }
      return true
    },
    collapseClick() {
      this.isCollapse = !this.isCollapse
    },
    directoryClassSearch(evt) {
      if (evt.keyCode === 13) {
        this.$refs.directoryTree.filter(this.queryDirectoryInfo.keyword)
      }
    },
    filterNode(value, data) {
      if (!value) return true
      if (data.type === 0) {
        return true
      }
      return data.name.indexOf(value) !== -1 || data.code.indexOf(value) !== -1
    }
  },
  mounted: async function() {
    var that = this
    // window.addEventListener('message', this.listenerIframeMsg)
    window.handleCellClick = params => {
      that.cellClick(params)
    }
    window.handleSaveData = params => {
      that.saveDrawing(params)
    }
    window.receiveErrorMsg = params => {
      that.$message.error(params)
    }
    window.delClass = async params => {
      await that.delClass(params)
    }
    await this.loadDataSourceData()
    await this.loadDirectoryData()
  },
  destroy: async function() {
    var result = await this.getDrawEditorStatus()
    if (result) {
      return false
    }
  }
}
</script>
<style lang="less" scoped>
.moduleTreeDiv {
  height: 82vh;
  border-radius: 3px;
  border: 1px solid gainsboro;
  margin-bottom: 12px;
  background-color: white;
}
.directoryClassFilterInput {
  width: 68% !important;
}
.treeTitle {
  background-color: #f2f2f2;
  height: 39px;
  line-height: 36px;
  color: black !important;
}
.mask {
  background-color: rgb(0, 0, 0);
  opacity: 0.3;
  position: fixed;
  top: 120;
  left: 250;
  width: 100%;
  height: 100%;
  z-index: 1;
}
.el-tables {
  height: 60vh;
  overflow: scroll;
}
</style>
