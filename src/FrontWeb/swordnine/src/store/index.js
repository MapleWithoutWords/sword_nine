import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    StatusList: [
      {
        key: 0,
        value: '正常'
      },
      {
        key: 1,
        value: '禁用'
      }
    ],
    AppTypeList: [
      {
        key: 0,
        value: 'Web项目'
      },
      {
        key: 1,
        value: 'windows项目'
      },
      {
        key: 2,
        value: 'Web和windows项目'
      }
    ]
  },
  getters: {},
  mutations: {
  },
  actions: {},
  modules: {}
})
