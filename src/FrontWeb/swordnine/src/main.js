import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './plugins/element.js'
import './assets/css/global.css'
import axios from 'axios'
import nprogress from 'nprogress'
import './plugins/querydata.js'
import JSEncrypt from 'jsencrypt'
import md5 from 'md5'

var encrypt = new JSEncrypt()

axios.defaults.baseURL = 'http://127.0.0.1:5000'
var appKey = 'fe2c0892-ebc4-42f8-bc90-c4d4a15a2fef'
Vue.prototype.$appKey = appKey

// 添加请求拦截
axios.interceptors.request.use(config => {
  nprogress.start()
  var tokenRes = JSON.parse(window.sessionStorage.getItem('token'))
  if (tokenRes) {
    config.headers.Authorization = 'Bearer ' + tokenRes.access_token
  }
  return config
})
// eslint-disable-next-line no-unused-vars
var count = 0

// 添加响应拦截器
axios.interceptors.response.use(
  async function(response) {
    nprogress.done()
    console.log(response)
    // 如果出现401则刷新token，当刷新token超过3次则刷新页面
    if (response.status === 401) {
      if (count > 3) {
        window.location.reload()
      }
      count++
      var tokenRes = JSON.parse(window.sessionStorage.getItem('token'))
      var tokenData = await axios({
        url: '/api/refresh_token',
        method: 'post',
        data: { appId: appKey, refreshToken: tokenRes.refresh_token }
      })
      window.sessionStorage.setItem('token', JSON.stringify(tokenData))
      var res = await axios(response.config)
      return res
    }

    // 对响应数据做点什么
    var data = response.data
    return data
  },
  function(error) {
    nprogress.done()
    console.debug(error)
    // 对响应错误做点什么
    // eslint-disable-next-line prefer-promise-reject-errors
    return null
  }
)
// 添加md5加密
Vue.prototype.$md5 = md5
// 添加rsa加密
Vue.prototype.$encrypt = encrypt
// 添加axios
Vue.prototype.$http = axios

Vue.config.productionTip = false

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
