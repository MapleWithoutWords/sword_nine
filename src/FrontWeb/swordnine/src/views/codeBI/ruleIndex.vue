<template>
  <div>
    <el-card>
      <!--搜索-->
      <el-row :gutter="15">
        <el-col :span="4">
          <el-input
            size="small"
            v-model="queryInfo.name"
            placeholder="请输入名称或编码"
          ></el-input>
        </el-col>
        <el-col :span="2">
          <el-button
            size="small"
            type="primary"
            @click="init()"
            icon="el-icon-search"
            >搜索</el-button
          >
        </el-col>
        <el-col :span="2" :offset="10">
          <el-button
            size="small"
            type="primary"
            @click="addRow()"
            icon="el-icon-circle-plus-outline"
            >添加一行</el-button
          >
        </el-col>
        <el-col :span="2">
          <el-button
            size="small"
            type="primary"
            @click="saveAllData()"
            icon="el-icon-circle-plus-outline"
            >保存</el-button
          >
        </el-col>
      </el-row>

      <!-- 表格区域-->
      <el-table :data="dataList" :stripe="true" :border="true">
        <el-table-column type="selection"></el-table-column>
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
              <el-input v-model="scope.row.name"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="编码">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.code"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="枚举值">
          <template slot-scope="scope">
            <div>
              <el-input v-model.number="scope.row.enumValue"></el-input>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="类型">
          <template slot-scope="scope">
            <div>
              <el-select v-model="scope.row.valueType" placeholder="请选择">
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
        <el-table-column label="描述">
          <template slot-scope="scope">
            <div>
              <el-input v-model="scope.row.description"></el-input>
            </div>
          </template>
        </el-table-column>
        <!--操作列-->
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-tooltip
              class="item"
              effect="dark"
              content="删除"
              placement="top"
            >
              <el-button
                class="operator-buttom"
                type="danger"
                size="mini"
                @click="() => dataList.filter(e => e.id != scope.row.id)"
                icon="el-icon-delete"
              ></el-button>
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import RuleService from '@/services/codeBI/ruleService'
export default {
  data() {
    return {
      // 查询对象
      queryInfo: {
        name: null,
        status: null,
        sort: 'seqno asc'
      },
      dataList: [],
      // 值类型数据
      valueTypeDataList: [
        { value: 0, name: '布尔' },
        { value: 1, name: '字符串' }
      ]
    }
  },
  methods: {
    /**
     * @description 初始化列表数据
     */
    async init() {
      var res = await RuleService.getList(this.queryInfo)
      var ret = this.$validResponse(res)
      if (ret) {
        this.dataList = ret.data
      }
    },
    // 保存属性数据
    async saveAllData() {
      var result = await RuleService.save(this.dataList)
      var ret = this.$validResponse(result)
      if (!ret) {
        return false
      }
      this.$message('保存成功')
      this.$emit('ok', ret.data)
    },
    async addRow() {
      var len = this.dataList.length
      this.dataList.push({
        id: '',
        code: '',
        description: '',
        enumValue: 0,
        name: '',
        seqNo: len + 1,
        valueType: 0
      })
    },
    saveList() {}
  },
  mounted: async function() {
    await this.init()
  }
}
</script>

<style lang="less" scoped>
.el-card {
  margin: 24px;
}
.operator-buttom {
  position: relative;
}
</style>
