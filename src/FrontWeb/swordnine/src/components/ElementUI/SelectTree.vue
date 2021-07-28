<template>
  <el-select :value="data.name" :clearable="clearable" @clear="clearHandle">
    <el-input
      class="selectInput"
      :placeholder="placeholder"
      v-model="filterText"
    >
    </el-input>

    <el-option :value="data.id" :label="data.name" class="options">
      <el-tree
        id="tree-option"
        ref="selectTree"
        default-expand-all
        :check-strictly="true"
        :accordion="accordion"
        :expand-on-click-node="false"
        :data="options"
        :props="props"
        :node-key="props.value"
        :default-expanded-keys="defaultExpandedKey"
        :filter-node-method="filterNode"
        @node-click="handleNodeClick"
      >
      </el-tree>
    </el-option>
  </el-select>
</template>

<script>
export default {
  name: 'el-tree-select',
  props: {
    /* 配置项 */
    props: {
      type: Object,
      default: () => {
        return {
          value: 'id', // ID字段名
          label: 'name', // 显示名称
          children: 'childrens' // 子级字段名
        }
      }
    },
    /* 选项列表数据(树形结构的对象数组) */
    options: {
      type: Array,
      default: () => {
        return []
      }
    },
    /* 初始值 */
    data: {
      type: Object,
      default: () => {
        return { id: '', name: '' }
      }
    },
    /* 可清空选项 */
    clearable: {
      type: Boolean,
      default: () => {
        return true
      }
    },
    /* 自动收起 */
    accordion: {
      type: Boolean,
      default: () => {
        return false
      }
    },
    placeholder: {
      type: String,
      default: () => {
        return '检索关键字'
      }
    }
  },
  data() {
    return {
      filterText: '',
      valueId: this.data.id, // 初始值
      valueTitle: this.data.name,
      defaultExpandedKey: []
    }
  },
  mounted() {
    this.$nextTick(function() {
      this.initHandle()
    })
  },
  methods: {
    // 初始化值
    initHandle() {
      if (this.data.id) {
        var treeData = this.$refs.selectTree.getNode(this.data.paridentId)
        if (treeData) {
          this.data.name = treeData.data[this.props.label] // 初始化显示
        }
        this.$refs.selectTree.setCurrentKey(this.data.id) // 设置默认选中
        this.defaultExpandedKey = [this.data.id] // 设置默认展开
      }
      this.initScroll()
    },
    // 初始化滚动条
    initScroll() {
      this.$nextTick(() => {
        var scrollWrap = document.querySelectorAll(
          '.el-scrollbar .el-select-dropdown__wrap'
        )[0]
        var scrollBar = document.querySelectorAll(
          '.el-scrollbar .el-scrollbar__bar'
        )
        scrollWrap.style.cssText =
          'margin: 0px; max-height: none; overflow: hidden;'
        scrollBar.forEach(ele => (ele.style.width = 0))
      })
    },
    // 切换选项
    handleNodeClick(data, node) {
      this.data.name = data.name
      this.data.id = data.id
      this.$emit('getValue', this.data)
      this.defaultExpandedKey = []
    },
    // 清除选中
    clearHandle() {
      this.data.name = ''
      this.data.id = ''
      this.defaultExpandedKey = []
      this.clearSelected()
      this.$emit('getValue', this.data)
    },
    /* 清空选中样式 */
    clearSelected() {
      var allNode = document.querySelectorAll('#tree-option .el-tree-node')
      allNode.forEach(element => element.classList.remove('is-current'))
    },
    filterNode(value, data) {
      if (!value) return true
      return data.name.indexOf(value) !== -1
    }
  },
  watch: {
    // value() {
    //   console.log(this.value)
    //   this.valueId = this.value
    //   // this.valueTitle = this.title
    //   this.initHandle()
    // },
    filterText(val) {
      this.$refs.selectTree.filter(val)
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.el-scrollbar .el-scrollbar__view .el-select-dropdown__item {
  height: auto;
  max-height: 274px;
  padding: 0;
  overflow: hidden;
  overflow-y: auto;
}
.el-select-dropdown__item.selected {
  font-weight: normal;
}
ul li >>> .el-tree .el-tree-node__content {
  height: auto;
  padding: 0 20px;
}
.el-tree-node__label {
  font-weight: normal;
}
.el-tree >>> .is-current .el-tree-node__label {
  color: #409eff;
  font-weight: 700;
}
.el-tree >>> .is-current .el-tree-node__children .el-tree-node__label {
  color: #606266;
  font-weight: normal;
}
.selectInput {
  padding: 0 5px;
  box-sizing: border-box;
}
</style>
