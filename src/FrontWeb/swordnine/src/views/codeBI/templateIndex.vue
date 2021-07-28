<template>
  <div>
    <el-card>
      <!--搜索-->
      <el-row :gutter="15">
        <el-col :span="4">
          <el-select
            size="small"
            v-model="queryInfo.type"
            clearable
            placeholder="请选择模板类型"
          >
            <el-option
              v-for="item in typeDataList"
              :key="item.value"
              :label="item.name"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </el-col>
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
        <el-col :span="2" :offset="12">
          <el-button
            size="small"
            type="primary"
            @click="createClick()"
            icon="el-icon-circle-plus-outline"
            >添加</el-button
          >
        </el-col>
      </el-row>

      <!-- 表格区域-->
      <el-table :data="dataList" :stripe="true" :border="true">
        <el-table-column type="selection"></el-table-column>
        <el-table-column prop="seqNo" label="序号"></el-table-column>
        <el-table-column label="类型">
          <template slot-scope="scope">
            <div>
              {{
                !typeDataList.find(e => e.value === scope.row.type)
                  ? ''
                  : typeDataList.find(e => e.value === scope.row.type).name
              }}
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="name" label="名称"></el-table-column>
        <el-table-column prop="code" label="编码"></el-table-column>
        <el-table-column
          prop="startFileName"
          label="启动文件名称"
        ></el-table-column>
        <el-table-column prop="description" label="描述"></el-table-column>
        <!--操作列-->
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-tooltip
              class="item"
              effect="dark"
              content="编辑"
              placement="top"
            >
              <el-button
                class="operator-buttom"
                type="primary"
                size="mini"
                icon="el-icon-edit"
                @click="editorData(scope.$index, scope.row)"
              ></el-button>
            </el-tooltip>

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
                @click="delData(scope.$index, scope.row)"
                icon="el-icon-delete"
              ></el-button>
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <!-- 分页 -->
    <el-pagination
      @size-change="pageSizeChange"
      @current-change="pageCurrentChange"
      :current-page="queryInfo.pageIndex"
      :page-sizes="[1, 10, 20, 50, 100]"
      :page-size="queryInfo.pageDataCount"
      layout="total, sizes, prev, pager, next, jumper"
      :total="queryInfo.recordCount"
    ></el-pagination>

    <!-- 新增编辑对话框 -->
    <el-dialog
      :close-on-click-modal="false"
      :title="updateOrInsertForm.title"
      :visible.sync="updateOrInsertForm.visible"
    >
      <el-form
        ref="dialogForm"
        :model="updateOrInsertForm.formData"
        width="50%"
        label-width="130px"
        label-position="right"
        :hide-required-asterisk="false"
        label-suffix="："
        :rules="updateOrInsertForm.formRule"
      >
        <el-form-item label="类型" :required="true" prop="type">
          <el-select
            size="small"
            v-model="updateOrInsertForm.formData.type"
            clearable
            placeholder="请选择"
          >
            <el-option
              v-for="item in typeDataList"
              :key="item.value"
              :label="item.name"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="名称" :required="true" prop="name">
          <el-input
            size="small"
            v-model="updateOrInsertForm.formData.name"
          ></el-input>
        </el-form-item>
        <el-form-item label="编码" :required="true" prop="code">
          <el-input
            size="small"
            v-model="updateOrInsertForm.formData.code"
          ></el-input>
        </el-form-item>
        <el-form-item label="序号" :required="true" prop="seqno">
          <el-input
            size="small"
            v-model.number="updateOrInsertForm.formData.seqno"
          ></el-input>
        </el-form-item>
        <el-form-item
          label="启动文件名称"
          :required="true"
          prop="startFileName"
        >
          <el-input
            size="small"
            v-model="updateOrInsertForm.formData.startFileName"
          ></el-input>
        </el-form-item>
        <el-form-item>
          <el-upload
            class="upload-demo"
            drag
            :action="uploadFileAction"
            :headers="uploadRequestHeaders"
            :auto-upload="false"
            :on-remove="removeFileList"
            :file-list="uploadFileList"
            :on-change="uploadFileChange"
          >
            <i class="el-icon-upload"></i>
            <div class="el-upload__text">
              将文件拖到此处，或<em>点击上传</em>
            </div>
          </el-upload>
            <el-button type="primary" @click="uploadFile">上传</el-button>
            <el-button @click="removeFileList">清空</el-button>
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input
            size="small"
            type="textarea"
            v-model="updateOrInsertForm.formData.description"
          ></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="formSubmit">确认</el-button>
          <el-button @click="updateOrInsertForm.visible = false"
            >取消</el-button
          >
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import TemplateService from '@/services/codeBI/templateService'
export default {
  data() {
    return {
      // 查询对象
      queryInfo: {
        name: null,
        type: null,
        status: null,
        pageIndex: 1,
        pageDataCount: 10,
        recordCount: 0
      },
      dataList: [],
      updateOrInsertForm: {
        title: '新增模板',
        visible: false,
        /**
         * @description 表单类型：【add:新增】、【editor:修改】
         */
        type: '',
        formData: {
          assemblyDirectory: '',
          code: '',
          description: '',
          name: '',
          seqNo: 0,
          startFileName: '',
          type: 0
        },
        formRule: {
          name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
          code: [{ required: true, message: '请输入编码', trigger: 'blur' }]
        }
      },
      typeDataList: [
        { value: 0, name: '后端' },
        { value: 1, name: '前端' }
      ],
      uploadRequestHeaders: {
        Authorization: ''
      },
      uploadFileAction: '',
      uploadFileList: []
    }
  },
  methods: {
    /**
     * @description 初始化列表数据
     */
    async init() {
      var res = await TemplateService.getList(this.queryInfo)
      console.log(res)
      var ret = this.$validResponse(res)
      if (ret) {
        this.dataList = ret.data
        this.queryInfo.recordCount = ret.page.count
      }
    },
    /**
     * @description 新增按钮单击事件
     */
    createClick() {
      this.updateOrInsertForm.visible = true
      this.updateOrInsertForm.title = '新增模板'
      this.updateOrInsertForm.type = 'add'
      Object.assign(
        this.updateOrInsertForm.formData,
        this.$options.data().updateOrInsertForm.formData
      )
    },
    editorData(idx, rowData) {
      this.updateOrInsertForm.visible = true
      this.updateOrInsertForm.title = '编辑模板'
      this.updateOrInsertForm.type = 'editor'
      this.updateOrInsertForm.formData = rowData
      this.$nextTick(() => {
        document.getElementsByClassName(
          'el-upload__input'
        )[0].webkitdirectory = true
      })
    },
    async formSubmit() {
      const that = this
      this.$refs.dialogForm.validate(async valid => {
        if (!valid) {
          return false
        }
        var res
        if (that.updateOrInsertForm.type === 'add') {
          // 新增
          res = await TemplateService.addNew(that.updateOrInsertForm.formData)
        } else if (that.updateOrInsertForm.type === 'editor') {
          // 编辑
          res = await TemplateService.update(that.updateOrInsertForm.formData)
        } else {
          that.$message.error('未知的type')
        }
        var ret = that.$validResponse(res)
        if (!ret) {
          return false
        }
        await this.init()
        that.updateOrInsertForm.visible = false
        return true
      })
    },
    async delData(idx, rowData) {
      await this.$delConfirm(() => TemplateService.delete(rowData.id))
    },
    pageSizeChange(val) {
      this.queryInfo.pageDataCount = val
      this.init()
    },
    pageCurrentChange(val) {
      this.queryInfo.pageIndex = val
      this.init()
    },
    removeFileList() {
      this.uploadFileList = []
    },
    uploadFileChange(file, fileList) {
      this.uploadFileList = fileList
    },
    async uploadFile() {
      console.log(this.uploadFileList)
      var formData = new FormData()
      for (let index = 0; index < this.uploadFileList.length; index++) {
        var element = this.uploadFileList[index]
        formData.append(`file${index}`, element.raw)
      }
      var result = await TemplateService.upload(formData)
      var ret = this.$validResponse(result)
      if (ret) {
        this.updateOrInsertForm.formData.assemblyDirectory = ret.data
        this.$message.success('上传成功')
      }
    }
  },
  mounted: async function() {
    var retToken = JSON.parse(window.sessionStorage.getItem('token'))
    if (retToken) {
      this.uploadRequestHeaders.Authorization = `Bearer ${retToken.access_token}`
    }
    this.uploadFileAction =
      window.apiInstance.defaults.baseURL + 'api/sntemplate/upload_template'
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
