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
          ></el-tree>
        </div>
      </el-col>

      <el-col :span="18" style="margin-left:12px">
        <attributeRuleSetting :data="selectClassData"></attributeRuleSetting>
      </el-col>
    </el-row>
  </div>
</template>

<script>
// 数据源服务
import DataSourceService from '@/services/dataSourceService/dataSourceService'
// 类别
import ClassService from '@/services/dataSourceService/classService'

import attributeRuleSetting from './components/attributeRuleSetting.vue'

export default {
  // eslint-disable-next-line vue/no-unused-components
  components: {
    // eslint-disable-next-line vue/no-unused-components
    attributeRuleSetting: attributeRuleSetting
  },
  data() {
    return {
      // 查询目录类别对象
      queryDirectoryClassInfo: {
        dataSourceId: null,
        dataSourceName: null,
        keyword: null
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
            return `${data.name}(${data.childrens.length})`
          }
        }
      },
      // 数据源数据
      allDataSourceDatas: [],
      activeName: 'first',
      selectClassData: {
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
      this.selectClassData.dataSourceId = ret.data[0].id
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
        this.selectClassData.classId = null
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
      this.selectClassData.classId = classId
      this.selectClassData.className = className
      this.selectClassData.dataSourceId = this.queryDirectoryClassInfo.dataSourceId
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
