export default class BaseCommonHttp {
  static controllerName = ''
  /**
   * @description 获取列表数据
   * @param {Object} playload
   */
  static async getList(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/getbyurl?${this.getQuery(playload)}`,
      method: 'get'
    })
    return { status, data }
  }

  /**
   * @description 根据id获取数据
   * @param {String} playload
   */
  static async getId(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/getbyid/${playload.id}?${(!playload.status ? '' : 'status=' + playload.status)}`,
      method: 'get'
    })
    return res
  }

  /**
   * @description 新增数据
   * @param {Object} playload
   */
  static async addNew(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/insert`,
      method: 'put',
      data: playload
    })
    return res
  }

  /**
   * @description 更新数据
   * @param {Object} playload
   */
  static async update(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/update`,
      method: 'post',
      data: playload
    })
    return res
  }

  static async delete(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/delete/${playload}`,
      method: 'delete'
    })
    return res
  }

  static async deleteIds(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/deleteids`,
      method: 'delete',
      data: playload
    })
    return res
  }

  static getQuery(jsonData) {
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
}
