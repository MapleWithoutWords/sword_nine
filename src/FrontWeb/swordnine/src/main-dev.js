import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './plugins/element.js'
import './assets/css/global.css'
import './assets/myicon/iconfont.css'
import axios from 'axios'
import nprogress from 'nprogress'
import './plugins/querydata.js'
import JSEncrypt from 'jsencrypt'
import md5 from 'md5'
import 'lodash'

var encrypt = new JSEncrypt()

axios.defaults.baseURL = 'https://localhost:5001/'
var appKey = 'fe2c0892-ebc4-42f8-bc90-c4d4a15a2fef'
Vue.prototype.$appKey = appKey
Vue.prototype.$adminId = '24e2f91e-b145-49ef-99f7-cf433a4c5525'

// 添加请求拦截
axios.interceptors.request.use(config => {
  nprogress.start()
  var tokenRes = JSON.parse(window.sessionStorage.getItem('token'))
  if (tokenRes) {
    config.headers.Authorization = 'Bearer ' + tokenRes.access_token
  }
  return config
})

// 添加响应拦截器
axios.interceptors.response.use(
  function(response) {
    nprogress.done()
    console.log(response)
    // debugger
    // 如果出现401则刷新token，当刷新token超过3次则刷新页面
    if (response.status === 401) {
      window.sessionStorage.clear()
      router.push('/login')
    }

    // 对响应数据做点什么
    var data = response.data
    return data
  },
  async function(error) {
    nprogress.done()
    debugger
    console.log(error.response)
    if (!error.response) {
      var json = { code: 1, msg: error }
      return json
    }
    if (error.response.status === 401) {
      window.sessionStorage.clear()
      router.push('/login')
      return null
    }
    // 对响应错误做点什么
    // eslint-disable-next-line prefer-promise-reject-errors
    return null
  }
)
// 添加md5加密
Vue.prototype.$md5 = md5
// 添加rsa加密
Vue.prototype.$encrypt = encrypt

Vue.prototype.$http = axios

// 添加axios
const apiInstance = axios.create()

// eslint-disable-next-line no-undef
_.assign(window, { axios, apiInstance })

// eslint-disable-next-line no-undef
_.assign(Vue.prototype, {
  // eslint-disable-next-line no-undef
  _
})
// window.apiInstance = apiInstance

Vue.config.productionTip = false

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
