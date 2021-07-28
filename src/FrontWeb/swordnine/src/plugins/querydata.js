import Vue from 'vue'

Vue.prototype.$getQuery = function (jsonData) {
  // eslint-disable-next-line no-unused-vars
  var sb = ''
  for (const key in jsonData) {
    // eslint-disable-next-line no-prototype-builtins
    if (jsonData.hasOwnProperty(key)) {
      const element = jsonData[key]
      if (element) {
        sb += key + '=' + element + '&'
      }
    }
  }
  sb = sb.substr(0, sb.length - 1)
  return sb
}
/**
 * @description 验证请求返回的结果
 * @param {Object} res http请求返回的对象
 */
Vue.prototype.$validResponse = function (res) {
  if (!res || !res.data) {
    this.$message.error('网络错误')
    return null
  }
  if (res.status !== 200) {
    this.$message.error(`请求失败，状态码为：${res.status}`)
    return null
  }
  if (res.data.code !== 0) {
    this.$message.error(res.data.msg)
    return null
  }
  return res.data
}
/**
 * @description 删除数据确认弹窗
 * @param {Function} confirmFunc 确认执行的方法
 * @param {Object} paramObj 方法所需的参数
 */
Vue.prototype.$delConfirm = async function (confirmFunc, msg = '此操作将永久删除该数据, 是否继续?') {
  var result = await this.$msgbox
    .confirm(msg, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    .catch(error => error)
  if (result !== 'confirm') {
    return false
  }
  var res = await confirmFunc()
  var resData = this.$validResponse(res)
  if (resData === null) {
    return false
  }
  return true
}
/**
 * @description 数据确认弹窗
 * @param {Function} confirmFunc 确认执行的方法
 * @param {Object} paramObj 方法所需的参数
 */
Vue.prototype.$ConfirmMsg = async function (msg = '视图未保存，确认切换吗？') {
  var result = await this.$msgbox
    .confirm(msg, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    .catch(error => error)
  if (result !== 'confirm') {
    return false
  }
  return true
}
